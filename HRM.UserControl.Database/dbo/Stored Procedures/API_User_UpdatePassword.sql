
CREATE PROCEDURE [dbo].[API_User_UpdatePassword] @UserId INT
	,@SiteId SMALLINT
	,@Password NVARCHAR(100)
	,@IsResetPassword BIT = 0
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211015@Tim.Trinh
Task: update user password
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
		,@tmpUserId INT

	SELECT @tmpUserId = UserId
	FROM dbo.UserMapping WITH (NOLOCK)
	WHERE SiteId = @SiteId
		AND NewUserId = @UserId

	SET @tmpUserId = ISNULL(@tmpUserId, @UserId)

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,[Password] AS '@Password'
				,OldPassword AS '@OldPassword'
				,IsResetPassword AS '@IsResetPassword'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @tmpUserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update password
	UPDATE dbo.[User] WITH (ROWLOCK, UPDLOCK)
	SET [Password] = @Password
		--,OldPassword = NULL
		,IsResetPassword = @IsResetPassword
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE UserId = @tmpUserId

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,[Password] AS '@Password'
				,OldPassword AS '@OldPassword'
				,IsResetPassword AS '@IsResetPassword'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @tmpUserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'API_User_UpdatePassword'
		,@ObjectId = @tmpUserId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END