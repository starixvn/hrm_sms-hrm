
CREATE PROCEDURE [dbo].[Manpower_GetSection]
AS
/*  
Created: 20230908@Brian.Nguyen
Task: get section
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT sec.[Name] AS SectionName
		,dept.[Name] AS DepartmentName
		,dept.DepartmentId
		,sec.SectionId
	FROM dbo.HRM_Section AS sec WITH (NOLOCK)
		INNER JOIN dbo.HRM_Department AS dept ON sec.DepartmentId = dept.DepartmentId
	WHERE sec.IsActive = 1
		AND sec.IsDeleted = 0
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
END