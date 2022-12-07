
CREATE PROCEDURE [dbo].[API_Role_GetBySiteId] @SiteId SMALLINT
	,@IsActive BIT = NULL
AS
/*
Created: 20211101@Tim.Trinh
Task: get role by siteid
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT r.RoleId
		,r.Code
		,r.[Name]
		,r.IsActive
		,r.IsDefault
		,s.SiteId
		,s.SiteName
	FROM dbo.[Role] AS r WITH (NOLOCK)
		LEFT JOIN dbo.[Site] AS s WITH (NOLOCK) ON r.SiteId = s.SiteId
	WHERE r.SiteId = @SiteId
		AND (
			@IsActive IS NULL
			OR r.IsActive = @IsActive
			)
END