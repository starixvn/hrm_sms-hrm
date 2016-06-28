
CREATE PROCEDURE [dbo].[Menu_GetMenusByUserId] @UserId INT
AS
/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
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
		,me.Policy
	FROM dbo.EmployeeRole AS empRo WITH (NOLOCK)
		INNER JOIN dbo.RoleFunction AS roFunc WITH (NOLOCK) ON empRo.RoleId = roFunc.RoleId
		INNER JOIN dbo.Menu AS me WITH (NOLOCK) ON roFunc.FunctionId = me.FunctionId
		AND me.IsActive = 1
	WHERE empRo.EmployeeId = @UserId
		AND (
			(
				me.Policy = 1
				AND roFunc.IsRead = 1
				)
			OR (
				me.Policy = 2
				AND roFunc.IsEdit = 1
				)
			OR (
				me.Policy = 3
				AND roFunc.IsCreate = 1
				)
			OR (
				me.Policy = 4
				AND roFunc.IsDelete = 1
				)
			)
	
	UNION ALL
	SELECT DISTINCT me.MenuId
		,me.[Name]
		,me.[Text]
		,me.Link
		,me.ParentId
		,me.FunctionId
		,me.DisplayOrder
		,me.IsActive
		,me.Policy
	FROM dbo.Menu AS me WITH (NOLOCK)
	WHERE FunctionId IS NULL
		AND IsActive = 1
END;
