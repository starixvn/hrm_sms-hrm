
CREATE PROCEDURE [dbo].[User_GetSiteAndMenuItemByUserId] @UserId INT
AS
/*
Created: 20220803@Tim.Trinh
Task: get site and menu by userid
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tempMenuItem (
		MenuItemId INT
		,SiteId SMALLINT
		,ParentId INT
		,MenuItemName VARCHAR(100)
		,URL VARCHAR(100)
		,DisplayOrder SMALLINT
		)

	SELECT s.SiteId
		,s.SiteName
		,s.SiteURL
	FROM dbo.UserSite AS us WITH (NOLOCK)
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON us.SiteId = s.SiteId
	WHERE us.UserId = @UserId
		AND us.IsActive = 1
		AND s.IsActive = 1

	INSERT INTO #tempMenuItem (
		MenuItemId
		,SiteId
		,MenuItemName
		,URL
		,ParentId
		,DisplayOrder
		)
	SELECT DISTINCT menu.MenuItemId
		,menu.SiteId
		,menu.MenuItemName
		,menu.URL
		,menu.ParentId
		,menu.DisplayOrder
	FROM dbo.MenuItem AS menu WITH (NOLOCK)
	WHERE menu.IsActive = 1
		AND (
			(
				menu.FunctionId IS NULL
				AND ISNULL(menu.URL, '') != ''
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.UserRole AS urole WITH (NOLOCK)
					INNER JOIN dbo.RoleFunction AS rolefnc WITH (NOLOCK) ON urole.SiteId = rolefnc.SiteId
					AND urole.RoleId = rolefnc.RoleId
					AND menu.FunctionId = rolefnc.FunctionId
					AND menu.SiteId = rolefnc.SiteId
					AND (
						rolefnc.IsRead = 1
						OR rolefnc.IsHost = 1
						)
				WHERE urole.UserId = @UserId
				)
			)
		AND EXISTS (
			SELECT 1
			FROM dbo.UserSite AS us WITH (NOLOCK)
			WHERE us.UserId = @UserId
				AND menu.SiteId = us.SiteId
				AND us.IsActive = 1
			)

	INSERT INTO #tempMenuItem (
		MenuItemId
		,SiteId
		,MenuItemName
		,URL
		,ParentId
		,DisplayOrder
		)
	SELECT DISTINCT menu.MenuItemId
		,menu.SiteId
		,menu.MenuItemName
		,menu.URL
		,menu.ParentId
		,menu.DisplayOrder
	FROM dbo.MenuItem AS menu WITH (NOLOCK)
	WHERE menu.FunctionId IS NULL
		AND ISNULL(menu.URL, '') = ''
		AND EXISTS (
			SELECT 1
			FROM #tempMenuItem AS tbl WITH (NOLOCK)
			WHERE menu.MenuItemId = tbl.ParentId
			)

	SELECT MenuItemId
		,SiteId
		,MenuItemName
		,URL
		,ParentId
		,DisplayOrder
	FROM #tempMenuItem
	ORDER BY SiteId
		,DisplayOrder

	DROP TABLE #tempMenuItem
END