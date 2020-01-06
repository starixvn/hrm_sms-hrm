CREATE PROCEDURE [dbo].[Department_GetByParentId] @ParentId INT = NULL
	,@IsActive BIT = NULL
AS
/*
Created: 20151222@Quinn.Vo
Task: Get departments by parent id.
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT dep.[DepartmentId]
		,dep.[Code]
		,dep.[Name]
		,dep.IsActive
		,dep.[ModifiedDate]
		,dep.[CreatedDate]
		,dep2.DepartmentId AS [ParentId]
		,dep2.[Name]
		,dep.[ManagerId]
		,mg.FullName
		,mg.NickName
	FROM dbo.Department AS dep WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS mg WITH (NOLOCK) ON dep.[ManagerId] = mg.EmpId
		LEFT JOIN dbo.Department AS dep2 WITH (NOLOCK) ON dep.ParentId = dep2.DepartmentId
	WHERE (
			@ParentId IS NULL
			OR dep.ParentId = @ParentId
			)
		AND (
			@IsActive IS NULL
			OR dep.IsActive = @IsActive
			)
	ORDER BY dep.Code
END
