
CREATE PROCEDURE [dbo].[Manpower_AnnualRequestForm_GetByRequestId] @RequestId INT
AS
/*  
Created: 20230616@Tim.Trinh
Task: get annual request by id
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PeriodId SMALLINT
		,@Year INT
		,@UserId INT
		,@VerifierId INT
		,@ApproverId INT
		,@UnitName NVARCHAR(50)
		,@VerifyProcessId TINYINT = 2
		,@ApproveProcessId TINYINT = 3
		,@CurrentProcessId TINYINT = NULL
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@ApprovedStatusId TINYINT = 3
		,@Today DATE = GETDATE()

	CREATE TABLE #tmpJobLevel (JobLevel NVARCHAR(50))

	SELECT @UserId = p.CreatedBy
		,@Year = p.[Year]
		,@UnitName = UnitName
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.Manpower_Period AS p WITH (NOLOCK) ON rq.PeriodId = p.PeriodId
	WHERE rq.RequestId = @RequestId

	SELECT @VerifierId = emp.UserId
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON emp.SiteId = flow.SiteId
		AND emp.EmpId = flow.EmpId
	WHERE flow.UnitName = @UnitName
		AND flow.ProcessId = @VerifyProcessId

	SELECT @ApproverId = emp.UserId
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON emp.SiteId = flow.SiteId
		AND emp.EmpId = flow.EmpId
	WHERE flow.UnitName = @UnitName
		AND flow.ProcessId = @ApproveProcessId

	SELECT @CurrentProcessId = ProcessId
	FROM dbo.Manpower_PeriodProcessSetting WITH (NOLOCK)
	WHERE @Today BETWEEN FromDate
			AND ToDate

	INSERT INTO #tmpJobLevel (JobLevel)
	SELECT DISTINCT Parent
	FROM dbo.Manpower_JobLevelMapping WITH (NOLOCK)

	SELECT DISTINCT rq.RequestId
		,rq.UnitName
		,rq.StatusId
		,@CurrentProcessId AS CurrentProcessId
		,rq.PeriodId
		,p.[Name]
		,p.[Year]
		,creator.UserId AS CreatorId
		,creator.Code
		,creator.Nickname
		,creator.FullName
		,verifier.UserId AS VerifierId
		,verifier.Code
		,verifier.Nickname
		,verifier.FullName
		,approver.UserId AS ApproverId
		,approver.Code
		,approver.Nickname
		,approver.FullName
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.Manpower_Period AS p WITH (NOLOCK) ON rq.PeriodId = p.PeriodId
		INNER JOIN dbo.[User] AS creator WITH (NOLOCK) ON rq.CreatedBy = creator.UserId
		INNER JOIN dbo.[User] AS verifier WITH (NOLOCK) ON (
			rq.StatusId IN (@VerifiedStatusId, @ApprovedStatusId)
			AND rq.VerifiedBy = verifier.UserId
			)
		OR (
			rq.StatusId = @PendingStatusId
			AND verifier.UserId = @VerifierId
			)
		INNER JOIN dbo.[User] AS approver WITH (NOLOCK) ON (
			rq.StatusId = @ApprovedStatusId
			AND rq.ApprovedBy = approver.UserId
			)
		OR (
			rq.StatusId IN (@PendingStatusId, @VerifiedStatusId)
			AND approver.UserId = @ApproverId
			)
	WHERE rq.RequestId = @RequestId

	SELECT s.SiteId
		,s.SiteName
		,c.CompanyId
		,c.[Name] AS CompanyName
		,unit.UnitId
		,unit.[Name] AS UnitName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,sec.SectionId
		,sec.[Name] AS SectionName
		,tmpjl.JobLevel
		,rqdt.LocalAddition
		,rqdt.ExpatAddition
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
		CROSS APPLY #tmpJobLevel AS tmpjl WITH (NOLOCK)
		LEFT JOIN dbo.Manpower_AnnualRequestFormDetail AS rqdt WITH (NOLOCK) ON sec.SiteId = rqdt.SiteId
		AND sec.SectionId = rqdt.SectionId
		AND rqdt.JobLevel = tmpjl.JobLevel
	WHERE rq.RequestId = @RequestId
	ORDER BY unit.[Name] ASC
		,dept.[Name] ASC
		,sec.[Name] ASC
		,tmpjl.JobLevel DESC

	SELECT s.SiteId
		,s.SiteName
		,c.CompanyId
		,c.[Name] AS CompanyName
		,unit.UnitId
		,unit.[Name] AS UnitName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,sec.SectionId
		,sec.[Name] AS SectionName
		,rqdt.JobLevel
		,rqdt.LocalAddition
		,rqdt.ExpatAddition
		,rqdt.ModifiedDate
		,rqdt.ModifiedBy
		,modifier.Code
		,modifier.Nickname
		,modifier.FullName
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
		INNER JOIN dbo.Manpower_AnnualRequestFormDetail_Log AS rqdt WITH (NOLOCK) ON sec.SiteId = rqdt.SiteId
		AND sec.SectionId = rqdt.SectionId
		INNER JOIN dbo.[User] AS modifier WITH (NOLOCK) ON rqdt.ModifiedBy = modifier.UserId
	WHERE rq.RequestId = @RequestId
	ORDER BY unit.[Name] ASC
		,dept.[Name] ASC
		,sec.[Name] ASC
		,rqdt.JobLevel DESC
		,rqdt.ModifiedDate DESC

	DROP TABLE #tmpJobLevel
END