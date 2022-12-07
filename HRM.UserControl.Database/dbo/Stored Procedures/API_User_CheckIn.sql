
CREATE PROCEDURE [dbo].[API_User_CheckIn] @UserId INT
	,@SiteId SMALLINT
	,@NetSessionId VARCHAR(50) = ''
	,@IP VARCHAR(50) = ''
	,@OnUserId BIGINT = 0 OUTPUT
AS
/*
Created: 202220728@Tim.Trinh
Task: user check in
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.OnUser WITH (ROWLOCK, UPDLOCK)
	SET IsKickedOut = 1
	WHERE UserId = @UserId
		AND SiteId = @SiteId

	INSERT INTO dbo.OnUser (
		UserId
		,SiteId
		,IsKickedOut
		,IP
		,NetSessionId
		)
	VALUES (
		@UserId
		,@SiteId
		,0
		,@IP
		,@NetSessionId
		)

	SET @OnUserId = SCOPE_IDENTITY()

	UPDATE dbo.UserSite WITH (ROWLOCK, UPDLOCK)
	SET LastLoginDate = GETUTCDATE()
	WHERE SiteId = @SiteId
		AND UserId = @UserId
END