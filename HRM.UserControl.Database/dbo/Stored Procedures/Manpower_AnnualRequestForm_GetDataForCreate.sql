
CREATE PROCEDURE [dbo].[Manpower_AnnualRequestForm_GetDataForCreate] @UserId INT
	,@UnitName NVARCHAR(50)
AS
/*  
Created: 20230616@Tim.Trinh
Task: get data for create annual request
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PeriodId SMALLINT
		,@UnitHeadProcessId TINYINT = 1
		,@Today DATE = GETDATE()

	CREATE TABLE #tmpUnit (
		SiteId SMALLINT
		,UnitId INT
		,UnitName NVARCHAR(50)
		)

	CREATE TABLE #tmpJobLevel (JobLevel NVARCHAR(50))

	SELECT TOP 1 @PeriodId = p.PeriodId
	FROM dbo.Manpower_Period AS p WITH (NOLOCK)
		INNER JOIN dbo.Manpower_PeriodProcessSetting AS ps WITH (NOLOCK) ON p.PeriodId = ps.PeriodId
	WHERE ps.ProcessId = @UnitHeadProcessId
		AND @Today BETWEEN ps.FromDate
			AND ps.ToDate
	ORDER BY p.[Year] DESC

	INSERT INTO #tmpUnit (
		SiteId
		,UnitId
		,UnitName
		)
	SELECT DISTINCT unit.SiteId
		,unit.UnitId
		,unit.[Name]
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON flow.UnitName = unit.[Name]
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
	WHERE flow.ProcessId = @UnitHeadProcessId
		AND unit.[Name] = @UnitName
		AND EXISTS (
			SELECT 1
			FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
			WHERE emp.UserId = @UserId
				AND emp.SiteId = flow.SiteId
				AND emp.EmpId = flow.EmpId
			)

	INSERT INTO #tmpJobLevel (JobLevel)
	SELECT DISTINCT Parent
	FROM dbo.Manpower_JobLevelMapping WITH (NOLOCK)

	SELECT PeriodId
		,[Name]
		,[Year]
	FROM dbo.Manpower_Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SELECT s.SiteId
		,s.SiteName
		,c.CompanyId
		,c.[Name] AS CompanyName
		,unit.UnitId
		,unit.UnitName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,sec.SectionId
		,sec.[Name] AS SectionName
		,tmpjl.JobLevel
		,rqdt.LocalAddition
		,rqdt.ExpatAddition
	FROM #tmpUnit AS unit WITH (NOLOCK)
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON unit.SiteId = s.SiteId
		AND s.IsActive = 1
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
		AND c.IsActive = 1
		INNER JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON unit.SiteId = dept.SiteId
		AND unit.UnitId = dept.UnitId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
		INNER JOIN dbo.HRM_Section AS sec WITH (NOLOCK) ON dept.SiteId = sec.SiteId
		AND dept.DepartmentId = sec.DepartmentId
		AND sec.IsActive = 1
		AND sec.IsDeleted = 0
		CROSS APPLY #tmpJobLevel AS tmpjl WITH (NOLOCK)
		LEFT JOIN dbo.Manpower_AnnualRequestFormDetail AS rqdt WITH (NOLOCK) ON sec.SiteId = rqdt.SiteId
		AND sec.SectionId = rqdt.SectionId
		AND rqdt.JobLevel = tmpjl.JobLevel
	WHERE @PeriodId > 0
	ORDER BY unit.UnitName ASC
		,dept.[Name] ASC
		,sec.[Name] ASC
		,tmpjl.JobLevel DESC

	DROP TABLE #tmpUnit

	DROP TABLE #tmpJobLevel
END