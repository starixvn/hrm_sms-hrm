
CREATE PROCEDURE [dbo].[UserOTP_UpdateSecretKey] @UserId INT
	,@SiteId SMALLINT
	,@SecretKey VARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20230926@Brian.Nguen
Task: Update user's secret key to verify with OTP
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT

	IF EXISTS (
			SELECT 1
			FROM [dbo].[UserOTP] WITH (NOLOCK)
			WHERE UserId = @UserId
				AND SiteId = @SiteId
			)
	BEGIN
		SET @xml = (
				SELECT UserId AS '@UserId'
					,SiteId AS '@SiteId'
					,[Enabled] AS '@Enabled'
					,SecretKey AS '@SecretKey'
					,IsRegistered AS '@IsRegistered'
				FROM [dbo].[UserOTP] WITH (NOLOCK)
				WHERE UserId = @UserId
					AND SiteId = @SiteId
				FOR XML PATH('r')
					,TYPE
				)
		SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

		UPDATE [dbo].[UserOTP] WITH (ROWLOCK, UPDLOCK)
		SET SecretKey = @SecretKey
		WHERE UserId = @UserId
			AND SiteId = @SiteId
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[UserOTP] (
			UserId
			,SiteId
			,[Enabled]
			,SecretKey
			,IsRegistered
			)
		VALUES (
			@UserId
			,@SiteId
			,0
			,@SecretKey
			,0
			)
	END

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,SiteId AS '@SiteId'
				,[Enabled] AS '@Enabled'
				,SecretKey AS '@SecretKey'
				,IsRegistered AS '@IsRegistered'
			FROM [dbo].[UserOTP] WITH (NOLOCK)
			WHERE UserId = @UserId
				AND SiteId = @SiteId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @UserId
		,@SiteId = @SiteId
		,@SpName = 'UserOTP_UpdateSecretKey'
		,@ObjectId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END