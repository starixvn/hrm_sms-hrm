
CREATE PROCEDURE [dbo].[Menu_GetMenuById] @MenuId INT
AS
/*
Created: 20160609@Razor.Nguyen
Task: get menu by id
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT me.MenuId
		,me.[Name]
		,me.[Text]
		,me.Link
		,me.ParentId
		,me.FunctionId
		,me.DisplayOrder
		,me.IsActive
	FROM dbo.Menu AS me WITH (NOLOCK)
	WHERE me.MenuId = @MenuId
END