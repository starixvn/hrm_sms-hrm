
CREATE PROCEDURE [dbo].[User_UpdateSiteRole] @SiteId SMALLINT
	,@UserId INT
	,@RoleIds VARCHAR(MAX) = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211019@Tim.Trinh
Task: update user site role
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
	DECLARE @tblRole TABLE (RoleId SMALLINT)

	IF ISNULL(@RoleIds, '') <> ''
	BEGIN
		INSERT INTO @tblRole (RoleId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@RoleIds, ',')
	END

	SET @xml = (
			SELECT @SiteId AS '@SiteId'
				,@UserId AS '@UserId'
				,(
					SELECT ur.RoleId AS '@RoleId'
						,ur.CreatedBy AS '@CreatedBy'
						,ur.CreatedDate AS '@CreatedDate'
					FROM dbo.UserRole AS ur WITH (NOLOCK)
					WHERE ur.SiteId = @SiteId
						AND ur.UserId = @UserId
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	DELETE ur
	FROM dbo.UserRole AS ur WITH (ROWLOCK, UPDLOCK)
	WHERE UserId = @UserId
		AND SiteId = @SiteId
		AND NOT EXISTS (
			SELECT tbl.RoleId
			FROM @tblRole AS tbl
			WHERE ur.RoleId = tbl.RoleId
			)

	INSERT INTO dbo.UserRole (
		UserId
		,RoleId
		,SiteId
		,CreatedBy
		,CreatedDate
		)
	SELECT @UserId
		,tbl.RoleId
		,@SiteId
		,@ModifiedBy
		,@startRun
	FROM @tblRole AS tbl
	WHERE NOT EXISTS (
			SELECT ur.UserId
			FROM dbo.UserRole AS ur WITH (NOLOCK)
			WHERE ur.UserId = @UserId
				AND ur.SiteId = @SiteId
				AND ur.RoleId = tbl.RoleId
			)

	-- write log
	SET @xml = (
			SELECT @SiteId AS '@SiteId'
				,@UserId AS '@UserId'
				,(
					SELECT tbl.RoleId AS '@RoleId'
						,@ModifiedBy AS '@CreatedBy'
						,@startRun AS '@CreatedDate'
					FROM @tblRole AS tbl
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'User_UpdateSiteRole'
		,@ObjectId = @UserId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END