
CREATE PROCEDURE [dbo].[Auto_AnnualLeaveBalance_Get] @year INT
AS
/*
Created: Tim.Trinh@25-07-2022
Task: annual leave balance
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromdate DATETIME = CAST(@year AS VARCHAR(10)) + '/01/01'
		,@todate DATETIME

	SET @todate = DATEADD(yy, 1, @fromdate)

	SELECT e.EmpId
		,ISNULL(CreditDays, 0) AS CreditDays
		,SUM(ISNULL(DeductDays, 0)) AS DeductDays
	INTO #tmpCurrenBalance
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN (
		SELECT EmpId
			,DeductDays
			,FromDate
		FROM dbo.LeaveTrans WITH (NOLOCK)
		WHERE YEAR(FromDate) = @year
			AND ISNULL(Deleted, 0) = 0
		) AS ded ON e.EmpId = ded.EmpId
		LEFT JOIN (
		SELECT lt.EmpId
			,SUM(CreditDays) CreditDays
		FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
		WHERE lt.FromDate >= @fromdate
			AND lt.FromDate < @todate
			AND ISNULL(lt.Deleted, 0) = 0
			AND lt.CreditDays > 0
			AND ISNULL(lt.Deleted, 0) = 0
		GROUP BY lt.EmpId
		) cred ON e.EmpId = cred.Empid
	WHERE e.[Enabled] = 1
		AND (
			e.isDeleted = 0
			OR YEAR(e.ResignedDate) >= year(@todate)
			)
	GROUP BY e.EmpId
		,ISNULL(cred.CreditDays, 0)

	SELECT ISNULL(CreditDays, 0) AS AnnualLeaveCurrentYear
		,ISNULL(CreditDays, 0) - ISNULL(cb.DeductDays, 0) AS LeaveBalance
		,cb.EmpId
		,e.Code
		,Nickname
		,FullName
		,Email
		,sec.SectionId
		,dept.DepartmentId
		,sec.[Name] AS Section
		,dept.[Name] AS Department
		,e.LetsTalk
		,esm.IsLocal
	INTO #tmpEmployeeLeaveBalance
	FROM #tmpCurrenBalance AS cb WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = cb.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON g.SectionId = sec.SectionId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON dept.DepartmentId = sec.DepartmentId
		INNER JOIN dbo.EmployeeSummary AS esm WITH (NOLOCK) ON esm.EmpId = e.EmpId
	WHERE e.[Enabled] = 1
		AND e.isDeleted = 0
		AND ISNULL(CreditDays, 0) > 0
		AND ISNULL(CreditDays, 0) - ISNULL(cb.DeductDays, 0) > 0
		AND esm.IsTopManager = 0
	ORDER BY e.Nickname

	SELECT EmpId
		,Code
		,Nickname
		,FullName
		,Email
		,AnnualLeaveCurrentYear
		,LeaveBalance
		,SectionId
		,DepartmentId
		,Section
		,Department
		,LetsTalk
		,IsLocal
	FROM #tmpEmployeeLeaveBalance WITH (NOLOCK)

	SELECT DISTINCT tmpemp.EmpId
		,su.OwnerId
		,suemp.Code
		,suemp.FullName
		,suemp.Nickname
		,suemp.Email
		,sec.SectionId
		,dept.DepartmentId
		,sec.[Name] AS Section
		,dept.[Name] AS Department
		,suemp.LetsTalk
	FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
		INNER JOIN #tmpEmployeeLeaveBalance AS tmpemp WITH (NOLOCK) ON su.EmpId = tmpemp.EmpId
		INNER JOIN dbo.Employee AS suemp WITH (NOLOCK) ON su.OwnerId = suemp.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = suemp.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON g.SectionId = sec.SectionId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON dept.DepartmentId = sec.DepartmentId
		AND suemp.[Enabled] = 1
		AND suemp.isDeleted = 0
	WHERE su.RequestType = 6
		AND su.ActionType IN (2, 3)
		AND su.IsDeleted = 0
	
	UNION ALL
	SELECT DISTINCT tmpemp.EmpId
		,su.OwnerId
		,suemp.Code
		,suemp.FullName
		,suemp.Nickname
		,suemp.Email
		,sec.SectionId
		,dept.DepartmentId
		,sec.[Name] AS Section
		,dept.[Name] AS Department
		,suemp.LetsTalk
	FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.GroupId = e.GroupId
		INNER JOIN #tmpEmployeeLeaveBalance AS tmpemp WITH (NOLOCK) ON e.EmpId = tmpemp.EmpId
		INNER JOIN dbo.Employee AS suemp WITH (NOLOCK) ON su.OwnerId = suemp.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON g.SectionId = sec.SectionId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON dept.DepartmentId = sec.DepartmentId
	WHERE su.RequestType = 6
		AND su.ActionType IN (2, 3)
		AND su.IsDeleted = 0
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeSupervisor AS empsu WITH (NOLOCK)
			WHERE e.EmpId = empsu.EmpId
				AND empsu.IsDeleted = 0
			)

	SELECT sec.SectionId
		,dep.DepartmentId
		,sec.[Name] AS Section
		,dep.[Name] AS Department
		,COUNT(*) AS TotalEmployee
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON g.SectionId = sec.SectionId
		INNER JOIN dbo.Department AS dep WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId
	WHERE e.[Enabled] = 1
		AND e.isDeleted = 0
	GROUP BY sec.SectionId
		,dep.DepartmentId
		,sec.[Name]
		,dep.[Name]
	ORDER BY dep.[Name]
		,sec.[Name]

	DROP TABLE #tmpCurrenBalance

	DROP TABLE #tmpEmployeeLeaveBalance
END