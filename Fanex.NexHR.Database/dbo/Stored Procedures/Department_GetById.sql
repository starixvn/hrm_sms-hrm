
CREATE PROCEDURE [dbo].[Department_GetById]
	@DepartmentId INT
AS
/*
Created: 20251222@Quinn.Vo
Task: Get by departmentId.
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
		,dep.[ParentId]
		,dep2.[Name]
		,dep.[ManagerId]
		,mg.FullName
		,mg.NickName
		--,dep.[CreatedBy]
		--,cr.FullName
		--,cr.NickName
		--,dep.[ModifiedBy]
		--,md.FullName
		--,md.NickName
	FROM dbo.Department AS dep WITH (NOLOCK)
	LEFT JOIN dbo.Employee AS cr WITH (NOLOCK) ON dep.CreatedBy = cr.EmpId
	LEFT JOIN dbo.Employee AS mg WITH (NOLOCK) ON dep.[ManagerId] = mg.EmpId
	LEFT JOIN dbo.Employee AS md WITH (NOLOCK) ON dep.[ModifiedBy] = md.EmpId
	LEFT JOIN dbo.Department AS dep2 WITH (NOLOCK) ON dep.ParentId = dep2.DepartmentId
	WHERE dep.DepartmentId = @DepartmentId

END
