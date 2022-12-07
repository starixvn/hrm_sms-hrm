
CREATE PROCEDURE [dbo].[API_Role_AssignFunction] @RoleId SMALLINT
	,@SiteId SMALLINT
	,@Value VARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*  
Created: 20211102@Tim.Trinh
Task: assign function by roleid
DB:
Original:
Param's Explanation: 
	• @Value: is xml
		<Root>
			<Row FunctionId="1" IsRead="true" IsHost="true" />
		</Root>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData VARCHAR(MAX)
		,@logDataOld VARCHAR(MAX)
		,@xml XML

	CREATE TABLE #tmpRoleFunction (
		FunctionId INT
		,IsRead BIT
		,IsHost BIT
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@Value

	INSERT INTO #tmpRoleFunction (
		FunctionId
		,IsRead
		,IsHost
		)
	SELECT FunctionId
		,IsRead
		,IsHost
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (FunctionId INT, IsRead BIT, IsHost BIT) AS Data

	EXEC sp_xml_removedocument @DocHandler

	-- write log
	SELECT rf.FunctionId
		,rf.IsRead
		,rf.IsHost
		,rf.CreatedBy
		,rf.CreatedDate
		,rf.ModifiedBy
		,rf.ModifiedDate
	INTO #tmpOldRoleFunction
	FROM dbo.RoleFunction AS rf WITH (NOLOCK)
		LEFT JOIN #tmpRoleFunction AS tbl WITH (NOLOCK) ON rf.FunctionId = tbl.FunctionId
	WHERE rf.RoleId = @RoleId
		AND rf.SiteId = @SiteId
		AND (
			tbl.FunctionId IS NULL
			OR (
				tbl.FunctionId IS NOT NULL
				AND (
					rf.IsRead != tbl.IsRead
					OR rf.IsHost != tbl.IsHost
					)
				)
			)

	SET @xml = (
			SELECT @RoleId AS '@RoleId'
				,@SiteId AS '@SiteId'
				,(
					SELECT FunctionId AS '@FunctionId'
						,IsRead AS '@IsRead'
						,IsHost AS '@IsHost'
						,CreatedBy AS '@CreatedBy'
						,CreatedDate AS '@CreatedDate'
						,ModifiedBy AS '@ModifiedBy'
						,ModifiedDate AS '@ModifiedDate'
					FROM #tmpOldRoleFunction WITH (NOLOCK)
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	DELETE rf
	FROM dbo.RoleFunction AS rf WITH (ROWLOCK, UPDLOCK)
	WHERE rf.RoleId = @RoleId
		AND rf.SiteId = @SiteId
		AND NOT EXISTS (
			SELECT FunctionId
			FROM #tmpRoleFunction AS tbl WITH (NOLOCK)
			WHERE rf.FunctionId = tbl.FunctionId
			)

	UPDATE rf
	SET rf.IsRead = tbl.IsRead
		,rf.IsHost = tbl.IsHost
		,rf.ModifiedBy = @ModifiedBy
		,rf.ModifiedDate = @startRun
	FROM dbo.RoleFunction AS rf WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpRoleFunction AS tbl WITH (NOLOCK) ON rf.FunctionId = tbl.FunctionId
	WHERE rf.RoleId = @RoleId
		AND rf.SiteId = @SiteId
		AND (
			rf.IsRead != tbl.IsRead
			OR rf.IsHost != tbl.IsHost
			)

	INSERT INTO dbo.RoleFunction (
		RoleId
		,FunctionId
		,SiteId
		,IsRead
		,IsHost
		,CreatedBy
		,CreatedDate
		)
	SELECT @RoleId
		,tbl.FunctionId
		,@SiteId
		,tbl.IsRead
		,tbl.IsHost
		,@ModifiedBy
		,@startRun
	FROM #tmpRoleFunction AS tbl WITH (NOLOCK)
	WHERE (
			tbl.IsRead = 1
			OR tbl.IsHost = 1
			)
		AND NOT EXISTS (
			SELECT rf.RoleId
			FROM dbo.RoleFunction AS rf WITH (NOLOCK)
			WHERE rf.RoleId = @RoleId
				AND rf.SiteId = @SiteId
				AND rf.FunctionId = tbl.FunctionId
			)

	-- write log
	SET @xml = (
			SELECT @RoleId AS '@RoleId'
				,@SiteId AS '@SiteId'
				,(
					SELECT rf.FunctionId AS '@FunctionId'
						,rf.IsRead AS '@IsRead'
						,rf.IsHost AS '@IsHost'
						,rf.CreatedBy AS '@CreatedBy'
						,rf.CreatedDate AS '@CreatedDate'
						,rf.ModifiedBy AS '@ModifiedBy'
						,rf.ModifiedDate AS '@ModifiedDate'
					FROM dbo.RoleFunction AS rf WITH (NOLOCK)
						LEFT JOIN #tmpOldRoleFunction AS tbl ON rf.FunctionId = tbl.FunctionId
					WHERE rf.RoleId = @RoleId
						AND rf.SiteId = @SiteId
						AND (
							tbl.FunctionId IS NULL
							OR (
								tbl.FunctionId IS NOT NULL
								AND (
									rf.IsRead != tbl.IsRead
									OR rf.IsHost != tbl.IsHost
									)
								)
							)
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'API_Role_AssignFunction'
		,@ObjectId = @RoleId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration

	DROP TABLE #tmpOldRoleFunction

	DROP TABLE #tmpRoleFunction
END