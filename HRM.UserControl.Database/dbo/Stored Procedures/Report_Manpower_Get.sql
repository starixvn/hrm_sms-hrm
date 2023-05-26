
CREATE PROCEDURE [dbo].[Report_Manpower_Get]
AS
/*  
Created: 20230512@Tim.Trinh
Task: manpower report
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT s.SiteName
		,c.[Name] AS CompanyName
		,c.Country
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,emp.IsLocal
		,jl.[Name] AS JobLevel
		,team.Code AS TeamCode
		,team.[Name] AS TeamName
		,section.Code AS SectionCode
		,section.[Name] AS SectionName
		,dept.Code AS DepartmentCode
		,dept.[Name] AS DepartmentName
		,unit.Code AS Unit
		,unit.[Name] AS UnitName
	FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.HRM_Team AS team WITH (NOLOCK) ON emp.SiteId = team.SiteId
		AND emp.TeamId = team.TeamId
		AND team.IsDeleted = 0
		INNER JOIN dbo.HRM_Section AS section WITH (NOLOCK) ON team.SiteId = section.SiteId
		AND team.SectionId = section.SectionId
		AND section.IsActive = 1
		AND section.IsDeleted = 0
		INNER JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON section.SiteId = dept.SiteId
		AND section.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON dept.SiteId = unit.SiteId
		AND dept.UnitId = unit.UnitId
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON emp.SiteId = s.SiteId
		AND s.IsActive = 1
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
		AND c.IsActive = 1
		INNER JOIN dbo.HRM_JobLevel AS jl WITH (NOLOCK) ON emp.SiteId = jl.SiteId
		AND emp.JobLevelId = jl.JobLevelId
		AND jl.IsDeleted = 0
	WHERE emp.IsActive = 1
		AND emp.IsDeleted = 0
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.UserMapping AS um WITH (NOLOCK)
			WHERE emp.SiteId = um.SiteId
				AND emp.EmpId = um.RefId
				AND emp.UserId = um.UserId
				AND ISNULL(um.NewUserId, um.UserId) != um.UserId
			)
END