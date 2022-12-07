
CREATE PROCEDURE [dbo].[Log_UserLogin_Insert] @UserId INT
	,@SiteId SMALLINT
	,@IP VARCHAR(50) = ''
	,@Browser NVARCHAR(250) = ''
	,@Domain NVARCHAR(250) = ''
	,@Platform NVARCHAR(250) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(100) = '' OUTPUT
AS
/*
Created: 20210621@Oliver.Dang
Task: log user login
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.Log_UserLogin (
		UserId
		,SiteId
		,IP
		,LogDate
		,Browser
		,Domain
		,[Platform]
		)
	VALUES (
		@UserId
		,@SiteId
		,@IP
		,GETUTCDATE()
		,@Browser
		,@Domain
		,@Platform
		)

	UPDATE dbo.UserSite WITH (ROWLOCK, UPDLOCK)
	SET LastLoginDate = GETUTCDATE()
	WHERE SiteId = @SiteId
		AND UserId = @UserId
END