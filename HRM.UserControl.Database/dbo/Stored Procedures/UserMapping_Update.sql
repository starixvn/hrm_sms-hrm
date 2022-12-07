
CREATE PROCEDURE [dbo].[UserMapping_Update] @UserId INT
	,@SiteId SMALLINT
	,@NewUserId INT = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20221027@Tim.Trinh
Task: update user mapping
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
				,SiteId AS '@SiteId'
				,RefId AS '@RefId'
				,NewUserId AS '@NewUserId'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.UserMapping WITH (NOLOCK)
			WHERE SiteId = @SiteId
				AND (
					UserId = @UserId
					OR NewUserId = @NewUserId
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE dbo.UserMapping WITH (ROWLOCK, UPDLOCK)
	SET NewUserId = NULL
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE NewUserId = @NewUserId
		AND SiteId = @SiteId
		AND UserId != @UserId

	UPDATE dbo.UserMapping WITH (ROWLOCK, UPDLOCK)
	SET NewUserId = @NewUserId
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE UserId = @UserId
		AND SiteId = @SiteId

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,SiteId AS '@SiteId'
				,RefId AS '@RefId'
				,NewUserId AS '@NewUserId'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.UserMapping WITH (NOLOCK)
			WHERE SiteId = @SiteId
				AND (
					UserId = @UserId
					OR NewUserId = @NewUserId
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'UserMapping_Update'
		,@ObjectId = @UserId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END