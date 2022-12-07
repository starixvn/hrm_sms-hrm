
CREATE PROCEDURE [dbo].[User_AssignSite] @SiteId SMALLINT
	,@UserId INT
	,@RoleIds VARCHAR(MAX) = NULL
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211024@Tim.Trinh
Task: assign site to user
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
					SELECT tbl.RoleId AS '@RoleId'
						,@CreatedBy AS '@CreatedBy'
						,@startRun AS '@CreatedDate'
					FROM @tblRole AS tbl
					WHERE NOT EXISTS (
							SELECT ur.UserId
							FROM dbo.UserRole AS ur WITH (NOLOCK)
							WHERE ur.UserId = @UserId
								AND ur.SiteId = @SiteId
								AND ur.RoleId = tbl.RoleId
							)
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	INSERT INTO dbo.UserSite (
		UserId
		,SiteId
		,IsActive
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@UserId
		,@SiteId
		,1
		,@CreatedBy
		,@startRun
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
		,@CreatedBy
		,@startRun
	FROM @tblRole AS tbl

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.UserMapping WITH (NOLOCK)
			WHERE UserId = @UserId
				AND SiteId = @SiteId
			)
	BEGIN
		INSERT INTO dbo.UserMapping (
			UserId
			,SiteId
			)
		VALUES (
			@UserId
			,@SiteId
			)
	END

	-- write log
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @CreatedBy
		,@SiteId = @SiteId
		,@SpName = 'User_AssignSite'
		,@ObjectId = @UserId
		,@Data = @logData
		,@OldData = NULL
		,@LogDate = @startRun
		,@Duration = @duration
END