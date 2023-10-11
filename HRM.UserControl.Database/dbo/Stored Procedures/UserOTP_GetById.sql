
CREATE PROCEDURE [dbo].[UserOTP_GetById] @UserId INT
	,@SiteId SMALLINT
AS
/*
Created: 20230927@Brian.Nguyen
Task: get user otp by userid and siteid
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT UserId
		,SiteId
		,SecretKey
		,[Enabled]
		,IsRegistered
	FROM dbo.[UserOTP] WITH (NOLOCK)
	WHERE UserId = @UserId
		AND SiteId = @SiteId
END