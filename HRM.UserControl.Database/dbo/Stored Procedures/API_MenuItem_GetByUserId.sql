
CREATE PROCEDURE [dbo].[API_MenuItem_GetByUserId] @UserId INT
	,@SiteId SMALLINT
AS
/*
Created: 20211027@Tim.Trinh
Task: get menu by userid
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT menu.MenuItemId
		,menu.[MenuItemName]
		,menu.URL
		,menu.ParentId
		,menu.DisplayOrder
		,menu.IsActive
	FROM dbo.MenuItem AS menu WITH (NOLOCK)
	WHERE menu.SiteId = @SiteId
		AND menu.IsActive = 1
		AND (
			menu.FunctionId IS NULL
			OR EXISTS (
				SELECT urole.UserId
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
	ORDER BY menu.DisplayOrder
END