
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
		,@PrevPeriodId SMALLINT = NULL
		,@EndDateOfYear DATE

	SELECT @Year = p.[Year]
	FROM dbo.Manpower_Period AS p WITH (NOLOCK)
	WHERE p.PeriodId = @PeriodId

	SELECT TOP 1 @PrevPeriodId = PeriodId
	FROM dbo.Manpower_Period WITH (NOLOCK)
	WHERE [Year] = @Year - 1
		AND IsActive = 1

	IF ISNULL(@PrevPeriodId, 0) > 0
	BEGIN
		SET @EndDateOfYear = DATEADD(DAY, - 1, CAST((@Year - 1) AS NVARCHAR(10)) + '-01-01')
	END
	ELSE
	BEGIN
		SET @EndDateOfYear = DATEADD(DAY, - 1, CAST(@Year AS NVARCHAR(10)) + '-01-01')
	END

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
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON rqdt.SiteId = s.SiteId
		AND s.IsActive = 1
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
		AND c.IsActive = 1
	WHERE rq.PeriodId = @PrevPeriodId
		AND rq.StatusId = @ApprovedStatusId

	SELECT s.SiteId
		,unit.UnitId
		,unit.[Name] AS UnitName
		,dept.DepartmentId
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
	INTO #tmpPrevCurrentHeadcount
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
		,unit.UnitId
		,unit.[Name]
		,dept.DepartmentId
		,dept.[Name]
		,section.SectionId
		,section.[Name]
		,jlm.Parent
	ORDER BY s.SiteId
		,unit.[Name]
		,dept.[Name]
		,section.SectionId
		,jlm.Parent

	SELECT tmpcur.SiteId
		,tmpcur.UnitName
		,tmpcur.DepartmentName
		,tmpcur.SectionId
		,tmpcur.SectionName
		,tmpcur.JobLevel
		,tmpcur.LocalCurrentHeadcount
		,tmpcur.ExpatCurrentHeadcount
		,tmpprev.LocalAddition
		,tmpprev.ExpatAddition
	FROM #tmpPrevCurrentHeadcount AS tmpcur WITH (NOLOCK)
		LEFT JOIN #tmpPrevAnnualRequest AS tmpprev WITH (NOLOCK) ON tmpcur.SiteId = tmpprev.SiteId
		AND tmpcur.UnitId = tmpprev.UnitId
		AND tmpcur.DepartmentId = tmpprev.DepartmentId
		AND tmpcur.SectionId = tmpprev.SectionId
		AND tmpcur.JobLevel = tmpprev.JobLevel

	UNION
	SELECT tmpprev.SiteId
		,tmpprev.UnitName
		,tmpprev.DepartmentName
		,tmpprev.SectionId
		,tmpprev.SectionName
		,tmpprev.JobLevel
		,0 AS LocalCurrentHeadcount
		,0 AS ExpatCurrentHeadcount
		,tmpprev.LocalAddition
		,tmpprev.ExpatAddition
	FROM #tmpPrevAnnualRequest AS tmpprev WITH (NOLOCK)
	WHERE NOT EXISTS (
		SELECT 1
		FROM #tmpPrevCurrentHeadcount AS tmpcur WITH (NOLOCK)
		WHERE tmpcur.SiteId = tmpprev.SiteId
			AND tmpcur.UnitId = tmpprev.UnitId
			AND tmpcur.DepartmentId = tmpprev.DepartmentId
			AND tmpcur.SectionId = tmpprev.SectionId
			AND tmpcur.JobLevel = tmpprev.JobLevel
		)

	DROP TABLE #tmpPrevAnnualRequest

	DROP TABLE #tmpPrevCurrentHeadcount
END