
CREATE PROCEDURE [dbo].[User_GetUserSiteByUserId] @UserId INT
AS
/*
Created: 20220803@Tim.Trinh
Task: get site by userid
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT s.SiteId
		,s.SiteName
		,s.SiteURL
		,s.SSOUrl
		,CASE 
			WHEN newuser.UserId IS NOT NULL THEN newuser.UserId
			ELSE u.UserId
			END AS UserId
		,CASE 
			WHEN newuser.UserId IS NOT NULL THEN newuser.Username
			ELSE u.Username
			END AS Username
		,CASE 
			WHEN newuser.UserId IS NOT NULL THEN newuser.Code
			ELSE u.Code
			END AS Code
	FROM dbo.UserSite AS us WITH (NOLOCK)
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON us.SiteId = s.SiteId
		INNER JOIN dbo.[User] AS u WITH (NOLOCK) ON us.UserId = u.UserId
		LEFT JOIN dbo.UserMapping AS um WITH (NOLOCK) ON us.UserId = um.UserId
		AND us.SiteId = um.SiteId
		LEFT JOIN dbo.[User] AS newuser WITH (NOLOCK) ON um.NewUserId = newuser.UserId
	WHERE us.UserId = @UserId
		AND us.IsActive = 1
		AND s.IsActive = 1
END