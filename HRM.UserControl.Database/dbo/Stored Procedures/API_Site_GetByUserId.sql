
CREATE PROCEDURE [dbo].[API_Site_GetByUserId] @UserId INT
	,@IsActive BIT = NULL
AS
/*
Created: 20211015@Tim.Trinh
Task: get site by user id
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT us.SiteId
		,s.SiteName
		,us.IsActive
		,us.LastLoginDate
	FROM dbo.UserSite AS us WITH (NOLOCK)
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON us.SiteId = s.SiteId
	WHERE us.UserId = @UserId
		AND (
			@IsActive IS NULL
			OR us.IsActive = @IsActive
			)
		AND s.IsActive = 1
END