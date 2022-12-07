
CREATE PROCEDURE [dbo].[Menu_GetBySiteId] @SiteId SmallInt
AS
/*
Created: 20211125@Teddy.le
Task: Get menu by siteid
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT	
		 menu.MenuItemId
		,menu.MenuItemName
		,menu.SiteId
		,menu.URL
		,menu.ParentId
		,fnc.FunctionName
		,fnc.FunctionId
		,menu.DisplayOrder
		,menu.Description
		,menu.ModifiedDate
		,u.Nickname as ModifiedBy
		,menu.IsActive
	FROM [dbo].[Site] AS s WITH (NOLOCK)
		INNER JOIN [dbo].[MenuItem] AS menu WITH (NOLOCK) ON s.SiteId = menu.SiteId
		LEFT JOIN [dbo].[Function] AS fnc WITH (NOLOCK) ON fnc.FunctionId = menu.FunctionId AND fnc.SiteId = s.SiteId
		LEFT JOIN [dbo].[User] AS u WITH (NOLOCK) ON menu.ModifiedBy = u.UserId
	WHERE s.SiteId = @SiteId
END