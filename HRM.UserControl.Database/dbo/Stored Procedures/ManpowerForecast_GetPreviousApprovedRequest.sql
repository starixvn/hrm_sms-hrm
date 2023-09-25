
CREATE PROCEDURE [dbo].[ManpowerForecast_GetPreviousApprovedRequest] @PeriodId SMALLINT
AS
/*  
Created: 20230622@Tim.Trinh
Task: get previous annual manpower forecast request
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ApprovedStatusId TINYINT = 3
		,@Year INT
		,@PrevPeriodId SMALLINT
		,@EndDateOfYear DATE

	SELECT @Year = p.[Year]
	FROM dbo.Manpower_Period AS p WITH (NOLOCK)
	WHERE p.PeriodId = @PeriodId

	SELECT TOP 1 @PrevPeriodId = PeriodId
	FROM dbo.Manpower_Period WITH (NOLOCK)
	WHERE [Year] = @Year - 1
		AND IsActive = 1

	SET @EndDateOfYear = DATEADD(DAY, - 1, CAST(@Year AS NVARCHAR(10)) + '-01-01')

	SELECT s.SiteId
		,s.SiteName
		,unit.UnitId
		,unit.[Name] AS UnitName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,sec.SectionId
		,sec.[Name] AS SectionName
		,rqdt.JobLevel
		,rqdt.LocalAddition
		,rqdt.ExpatAddition
	INTO #tmpPrevAnnualRequest
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.Manpower_AnnualRequestFormDetail AS rqdt WITH (NOLOCK) ON rq.RequestId = rqdt.RequestId
		INNER JOIN dbo.HRM_Section AS sec WITH (NOLOCK) ON sec.SiteId = rqdt.SiteId
		AND sec.SectionId = rqdt.SectionId
		AND sec.IsActive = 1
		AND sec.IsDeleted = 0
		INNER JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON dept.SiteId = sec.SiteId
		AND dept.DepartmentId = sec.DepartmentId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON unit.SiteId = dept.SiteId
		AND unit.UnitId = dept.UnitId
		AND unit.IsActive = 0
		AND unit.IsDeleted = 0
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON rqdt.SiteId = s.SiteId
		AND s.IsActive = 1
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
		AND c.IsActive = 1
	WHERE rq.PeriodId = @PrevPeriodId
		AND rq.StatusId = @ApprovedStatusId

	SELECT s.SiteId
		,unit.[Name] AS UnitName
		,dept.[Name] AS DepartmentName
		,section.SectionId
		,section.[Name] AS SectionName
		,jlm.Parent AS JobLevel
		,SUM(CASE 
				WHEN emp.IsLocal = 1 THEN 1
				ELSE 0
				END) AS LocalCurrentHeadcount
		,SUM(CASE 
				WHEN emp.IsLocal = 0 THEN 1
				ELSE 0
				END) AS ExpatCurrentHeadcount
		,SUM(CASE 
				WHEN tmpprev.LocalAddition IS NULL THEN 0
				ELSE tmpprev.LocalAddition
				END) AS LocalAddition
		,SUM(CASE 
				WHEN tmpprev.ExpatAddition IS NULL THEN 0
				ELSE tmpprev.ExpatAddition
				END) AS ExpatAddition
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
		INNER JOIN dbo.Manpower_JobLevelMapping AS jlm WITH (NOLOCK) ON jl.[Name] = jlm.Child
		LEFT JOIN #tmpPrevAnnualRequest AS tmpprev WITH (NOLOCK) ON s.SiteId = tmpprev.SiteId
		AND unit.UnitId = tmpprev.UnitId
		AND dept.DepartmentId = tmpprev.DepartmentId
		AND section.SectionId = tmpprev.SectionId
		AND jlm.Parent = tmpprev.JobLevel
	WHERE emp.JoinedDate <= @EndDateOfYear
		AND (
			emp.IsActive = 1
			OR (
				emp.IsActive = 0
				AND emp.ResignedDate > @EndDateOfYear
				)
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.Manpower_ExcludedEmployee AS exclude WITH (NOLOCK)
			WHERE emp.SiteId = exclude.SiteId
				AND emp.EmpId = exclude.EmpId
			)
	GROUP BY s.SiteId
		,unit.[Name] 
		,dept.[Name] 
		,section.SectionId
		,section.[Name]
		,jlm.Parent
	ORDER BY s.SiteId
		,unit.[Name] 
		,dept.[Name] 
		,section.SectionId
		,jlm.Parent
END