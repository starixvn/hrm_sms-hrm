
CREATE PROCEDURE [dbo].[UserOTP_UpdateStatus] @UserId INT
	,@SiteId SMALLINT
	,@IsRegistered BIT
	,@Enabled BIT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
 Created: 20230927@Brian.Nguyen
 Task : Update User OTP Registered status
 DB: 
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

	UPDATE dbo.[UserOTP] WITH (ROWLOCK, UPDLOCK)
	SET IsRegistered = @IsRegistered
		,[Enabled] = @Enabled
	WHERE UserId = @UserId
		AND SiteId = @SiteId

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
		,@SpName = 'UserOTP_UpdateStatus'
		,@ObjectId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END