
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
	INTO #tmpCurrenBalace
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
		WHERE FromDate >= @fromdate
			AND FromDate < @todate
			AND ISNULL(Deleted, 0) <> 1
			AND CreditDays > 0
			AND ISNULL(Deleted, 0) <> 1
		GROUP BY lt.EmpId
		) cred ON e.EmpId = cred.Empid
	WHERE e.[Enabled] = 1
		AND (
			ISNULL(e.isDeleted, 0) = 0
			OR YEAR(ResignedDate) >= year(@todate)
			)
	GROUP BY e.EmpId
		,ISNULL(CreditDays, 0)

	SELECT ISNULL(CreditDays, 0) AS AnnualLeaveCurrentYear
		,ISNULL(CreditDays, 0) - ISNULL(cb.DeductDays, 0) AS LeaveBalance
		,cb.EmpId
		,e.Code
		,FullName
		,Email
		,Nickname
	INTO #tmpEmployeeLeaveBalance
	FROM #tmpCurrenBalace AS cb
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = cb.Empid
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
	WHERE e.[Enabled] = 1
		AND ISNULL(CreditDays, 0) > 0
		AND ISNULL(CreditDays, 0) - ISNULL(cb.DeductDays, 0) > 0
	ORDER BY e.Nickname

	SELECT EmpId
		,Code
		,FullName
		,Email
		,Nickname
		,AnnualLeaveCurrentYear
		,LeaveBalance
	FROM #tmpEmployeeLeaveBalance WITH (NOLOCK)

	SELECT DISTINCT tmpemp.EmpId
		,su.OwnerId
		,suemp.Code
		,suemp.FullName
		,suemp.Nickname
		,suemp.Email
	FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
		INNER JOIN #tmpEmployeeLeaveBalance AS tmpemp WITH (NOLOCK) ON su.EmpId = tmpemp.EmpId
		INNER JOIN dbo.Employee AS suemp WITH (NOLOCK) ON su.OwnerId = suemp.EmpId
		AND suemp.[Enabled] = 1
		AND suemp.isDeleted = 0
	WHERE su.RequestType = 6
		AND su.ActionType IN (2, 3)
		AND ISNULL(su.IsDeleted, 0) = 0
	
	UNION ALL
	SELECT DISTINCT tmpemp.EmpId
		,su.OwnerId
		,suemp.Code
		,suemp.FullName
		,suemp.Nickname
		,suemp.Email
	FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.GroupId = e.GroupId
		INNER JOIN #tmpEmployeeLeaveBalance AS tmpemp WITH (NOLOCK) ON e.EmpId = tmpemp.EmpId
		INNER JOIN dbo.Employee AS suemp WITH (NOLOCK) ON su.OwnerId = suemp.EmpId
	WHERE su.RequestType = 6
		AND su.ActionType IN (2, 3)
		AND ISNULL(su.IsDeleted, 0) = 0
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeSupervisor AS empsu WITH (NOLOCK)
			WHERE e.EmpId = empsu.EmpId
			)

	DROP TABLE #tmpCurrenBalace
END
