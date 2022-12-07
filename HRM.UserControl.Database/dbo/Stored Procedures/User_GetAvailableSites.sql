
CREATE PROCEDURE [dbo].[User_GetAvailableSites] @UserId INT
	,@IsActive BIT = NULL
AS
/*
Created: 20211123@Tim.Trinh
Task: get available sites for user
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT s.SiteId
		,s.SiteName
		,s.IsActive
	FROM dbo.[Site] AS s WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT SiteId
			FROM dbo.UserSite AS us WITH (NOLOCK)
			WHERE us.UserId = @UserId
				AND s.SiteId = us.SiteId
			)
		AND (
			@IsActive IS NULL
			OR s.IsActive = @IsActive
			)
END