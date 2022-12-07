
CREATE PROCEDURE [dbo].[Site_GetAll] @IsActive BIT = NULL
AS
/*
Created: 20211019@Tim.Trinh
Task: get all site
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT s.SiteId
		,s.SiteName
		,s.SiteURL
		,s.IsActive
		,s.AccessToken
		,COUNT(us.UserId) AS NumOfUser
		,c.CompanyId
		,c.[Name] AS CompanyName
	FROM dbo.[Site] AS s WITH (NOLOCK)
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
		LEFT JOIN dbo.UserSite AS us WITH (NOLOCK) ON s.SiteId = us.SiteId
	WHERE @IsActive IS NULL
		OR s.IsActive = @IsActive
	GROUP BY s.SiteId
		,s.SiteName
		,s.SiteURL
		,s.IsActive
		,s.AccessToken
		,c.CompanyId
		,c.[Name]
	ORDER BY s.SiteId
END