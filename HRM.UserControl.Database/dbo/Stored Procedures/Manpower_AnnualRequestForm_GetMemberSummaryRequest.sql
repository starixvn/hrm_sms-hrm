
CREATE PROCEDURE [dbo].[Manpower_AnnualRequestForm_GetMemberSummaryRequest] @PeriodId SMALLINT
	,@UserId INT
	,@UnitType NVARCHAR(100) = NULL
AS
/*  
Created: 20230717@Brian.Nguyen
Task: get member summary request
DB:
Original:
Param's Explanation:
	- @UnitType: 'BU,PU'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @VerifyProcessId TINYINT = 2
		,@ApproveProcessId TINYINT = 3
		,@CurrentProcessId TINYINT = NULL
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@ApprovedStatusId TINYINT = 3
		,@Today DATE = GETDATE()
		,@IsOverProcess BIT = 0

	SELECT @CurrentProcessId = ProcessId
	FROM dbo.Manpower_PeriodProcessSetting WITH (NOLOCK)
	WHERE @Today BETWEEN FromDate
			AND ToDate
		AND PeriodId = @PeriodId

	SELECT @IsOverProcess = 1
	FROM dbo.Manpower_PeriodProcessSetting WITH (NOLOCK)
	WHERE @Today > ToDate
		AND PeriodId = @PeriodId
		AND ProcessId = @ApproveProcessId

	CREATE TABLE #tmpUnit (UnitName NVARCHAR(50))

	CREATE TABLE #tmpUnitType (UnitType NVARCHAR(50))

	INSERT INTO #tmpUnitType (UnitType)
	SELECT val
	FROM dbo.fnc_strSplit(@UnitType, ',')

	INSERT INTO #tmpUnit (UnitName)
	SELECT DISTINCT unit.[Name]
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON flow.UnitName = unit.[Name]
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
	WHERE (
			(
				flow.ProcessId IN (@VerifyProcessId, @ApproveProcessId)
				AND flow.ProcessId = @CurrentProcessId
				)
			OR @IsOverProcess = 1
			)
		AND EXISTS (
			SELECT 1
			FROM #tmpUnitType AS unittype WITH (NOLOCK)
			WHERE SUBSTRING(unit.[Name], 1, 2) LIKE N'%' + UnitType + '%'
			)
		AND EXISTS (
			SELECT 1
			FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
			WHERE emp.UserId = @UserId
				AND emp.SiteId = flow.SiteId
				AND emp.EmpId = flow.EmpId
			)

	SELECT DISTINCT rq.RequestId
		,rq.UnitName
		,rq.StatusId
		,@CurrentProcessId AS CurrentProcessId
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.Manpower_Period AS p WITH (NOLOCK) ON rq.PeriodId = p.PeriodId
		INNER JOIN #tmpUnit AS unit WITH (NOLOCK) ON rq.UnitName = unit.UnitName
	WHERE rq.PeriodId = @PeriodId
	ORDER BY rq.UnitName ASC

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
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON rq.UnitName = unit.[Name]
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
		LEFT JOIN dbo.Manpower_AnnualRequestFormDetail AS rqdt WITH (NOLOCK) ON sec.SiteId = rqdt.SiteId
		AND sec.SectionId = rqdt.SectionId
		INNER JOIN #tmpUnit AS tem WITH (NOLOCK) ON rq.UnitName = tem.UnitName
	WHERE rq.PeriodId = @PeriodId
	GROUP BY rq.RequestId
		,s.SiteId
		,s.SiteName
		,c.CompanyId
		,c.[Name]
		,unit.UnitId
		,unit.[Name]
		,dept.DepartmentId
		,dept.[Name]
		,sec.SectionId
		,sec.[Name]
	ORDER BY unit.[Name] ASC
		,dept.[Name] ASC
		,sec.[Name] ASC

	DROP TABLE #tmpUnit

	DROP TABLE #tmpUnitType
END