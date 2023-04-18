
CREATE PROCEDURE [dbo].[Sync_GO_UnitHead_GetInfo]
AS
/*
Created: 20230227@Tim.Trinh
Task: get unit head info
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.EmpId
		,emp.UserId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,emp.[Enabled] AS IsActive
		,emp.IsDeleted
		,section.Code AS SectionCode
		,section.[Name] AS SectionName
		,dept.Code AS DepartmentCode
		,dept.[Name] AS DepartmentName
		,unit.Code AS UnitCode
		,unit.[Name] AS UnitName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.Section AS section WITH (NOLOCK) ON gr.SectionId = section.SectionId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON section.DepartmentId = dept.DepartmentId
		INNER JOIN dbo.Unit AS unit WITH (NOLOCK) ON dept.UnitId = unit.UnitId
	WHERE emp.Code IN ('00031', '00036', '00053', '00067', '00090')
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0
END