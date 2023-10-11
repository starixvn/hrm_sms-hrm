
CREATE PROCEDURE [dbo].[Report_ManpowerForecast_Get] @PeriodId SMALLINT
	,@UserId INT
	,@SiteId SMALLINT
	,@UnitType NVARCHAR(100) = NULL
AS
/*  
Created: 20230912@Brian.Nguyen
Task: get manpower forecast report
DB:
Original:
Param's Explanation:
	- @UnitType: 'BU,PU'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RoleAdminId TINYINT = 2
		,@RoleSAId TINYINT = 1

	CREATE TABLE #tmpUnit (UnitName NVARCHAR(50))

	CREATE TABLE #tmpUnitType (UnitType NVARCHAR(50))

	CREATE TABLE #tmpUserRole (RoleId SMALLINT)

	INSERT INTO #tmpUnitType (UnitType)
	SELECT val
	FROM dbo.fnc_strSplit(@UnitType, ',')

	INSERT INTO #tmpUserRole (RoleId)
	SELECT RoleId
	FROM dbo.UserRole WITH (NOLOCK)
	WHERE UserId = @UserId
		AND SiteId = @SiteId

	INSERT INTO #tmpUnit (UnitName)
	SELECT DISTINCT unit.[Name]
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON flow.UnitName = unit.[Name]
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
	WHERE EXISTS (
			SELECT 1
			FROM #tmpUnitType AS unittype WITH (NOLOCK)
			WHERE SUBSTRING(unit.[Name], 1, 2) LIKE N'%' + UnitType + '%'
			)
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
				WHERE emp.UserId = @UserId
					AND emp.SiteId = flow.SiteId
					AND emp.EmpId = flow.EmpId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.UserRole AS ur WITH (NOLOCK)
				WHERE ur.UserId = @UserId
					AND ur.SiteId = @SiteId
					AND ur.RoleId IN (@RoleSAId, @RoleAdminId)
				)
			)

	SELECT rq.RequestId
		,unit.UnitName
		,rq.StatusId
		,p.PeriodId
	FROM #tmpUnit AS unit WITH (NOLOCK)
		LEFT JOIN dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK) ON rq.UnitName = unit.UnitName
		AND rq.PeriodId = @PeriodId
		LEFT JOIN dbo.Manpower_Period AS p WITH (NOLOCK) ON rq.PeriodId = p.PeriodId
	ORDER BY unit.UnitName ASC

	SELECT rq.RequestId
		,s.SiteId
		,s.SiteName
		,c.CompanyId
		,c.[Name] AS CompanyName
		,unit.UnitId
		,unit.[Name] AS UnitName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,sec.SectionId
		,sec.[Name] AS SectionName
		,SUM(rqdt.LocalAddition) AS LocalAddition
		,SUM(rqdt.ExpatAddition) AS ExpatAddition
	FROM #tmpUnit AS tmpunit WITH (NOLOCK)
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON tmpunit.UnitName = unit.[Name]
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
		LEFT JOIN dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK) ON rq.UnitName = tmpunit.UnitName
		AND rq.PeriodId = @PeriodId
		LEFT JOIN dbo.Manpower_AnnualRequestFormDetail AS rqdt WITH (NOLOCK) ON rq.RequestId = rqdt.RequestId
		AND sec.SiteId = rqdt.SiteId
		AND sec.SectionId = rqdt.SectionId
	GROUP BY s.SiteId
		,s.SiteName
		,c.CompanyId
		,c.[Name]
		,unit.UnitId
		,unit.[Name]
		,dept.DepartmentId
		,dept.[Name]
		,sec.SectionId
		,sec.[Name]
		,rq.RequestId
	ORDER BY unit.[Name] ASC
		,dept.[Name] ASC
		,sec.[Name] ASC

	DROP TABLE #tmpUnit

	DROP TABLE #tmpUnitType

	DROP TABLE #tmpUserRole
END