
CREATE PROCEDURE [dbo].[UserMapping_GetByUserId] @UserId INT
AS
/*
Created: 20221027@Tim.Trinh
Task: get user mapping
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT s.SiteId
		,s.SiteName
		,u.UserId
		,u.Username
		,u.Code
		,u.Nickname
		,u.FullName
	FROM dbo.[Site] AS s WITH (NOLOCK)
		LEFT JOIN dbo.UserMapping AS um WITH (NOLOCK) ON s.SiteId = um.SiteId
		AND um.NewUserId = @UserId
		LEFT JOIN dbo.[User] AS u WITH (NOLOCK) ON um.UserId = u.UserId
END