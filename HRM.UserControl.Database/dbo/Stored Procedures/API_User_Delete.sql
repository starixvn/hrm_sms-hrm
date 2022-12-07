
CREATE PROCEDURE [dbo].[API_User_Delete] @UserId INT
	,@SiteId SMALLINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211109@Tim.Trinh
Task: delete user
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

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,IsDeleted AS '@IsDeleted'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.[User] WITH (ROWLOCK, UPDLOCK)
	SET IsDeleted = 1
		,IsActive = 0
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE UserId = @UserId

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,IsDeleted AS '@IsDeleted'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'API_User_Delete'
		,@ObjectId = @UserId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END