USE [HRM_Verify]
GO
/****** Object:  StoredProcedure [dbo].[Auto_AnnualLeaveBalance_Get]    Script Date: 10/28/2022 9:15:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Auto_AnnualLeaveBalance_Get] @year INT
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
		,sec.SectionId
		,dep.DepartmentId
		,sec.Code AS SectionCode
		,dep.Code AS DepartmentCode
	INTO #tmpEmployeeLeaveBalance
	FROM #tmpCurrenBalance AS cb
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = cb.Empid
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON g.SectionId = sec.SectionId
		INNER JOIN dbo.Department AS dep WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId
		INNER JOIN dbo.EmployeeSummary AS esm WITH (NOLOCK) ON esm.EmpId = e.EmpId
	WHERE e.[Enabled] = 1
		AND ISNULL(CreditDays, 0) > 0
		AND ISNULL(CreditDays, 0) - ISNULL(cb.DeductDays, 0) > 0
		AND esm.IsTopManager = 0
	ORDER BY e.Nickname

	SELECT EmpId
		,Code
		,FullName
		,Email
		,Nickname
		,AnnualLeaveCurrentYear
		,LeaveBalance
		,SectionId
		,DepartmentId
		,SectionCode
		,DepartmentCode
	FROM #tmpEmployeeLeaveBalance WITH (NOLOCK)

	SELECT DISTINCT tmpemp.EmpId
		,su.OwnerId
		,suemp.Code
		,suemp.FullName
		,suemp.Nickname
		,suemp.Email
		,sec.SectionId
		,dep.DepartmentId
		,sec.Code AS SectionCode
		,dep.Code AS DepartmentCode
	FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
		INNER JOIN #tmpEmployeeLeaveBalance AS tmpemp WITH (NOLOCK) ON su.EmpId = tmpemp.EmpId
		INNER JOIN dbo.Employee AS suemp WITH (NOLOCK) ON su.OwnerId = suemp.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = suemp.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON g.SectionId = sec.SectionId
		INNER JOIN dbo.Department AS dep WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId
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
		,sec.SectionId
		,dep.DepartmentId
		,sec.Code AS SectionCode
		,dep.Code AS DepartmentCode
	FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.GroupId = e.GroupId
		INNER JOIN #tmpEmployeeLeaveBalance AS tmpemp WITH (NOLOCK) ON e.EmpId = tmpemp.EmpId
		INNER JOIN dbo.Employee AS suemp WITH (NOLOCK) ON su.OwnerId = suemp.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON g.SectionId = sec.SectionId
		INNER JOIN dbo.Department AS dep WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId
	WHERE su.RequestType = 6
		AND su.ActionType IN (2, 3)
		AND ISNULL(su.IsDeleted, 0) = 0
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeSupervisor AS empsu WITH (NOLOCK)
			WHERE e.EmpId = empsu.EmpId
			)

	SELECT sec.SectionId, dep.DepartmentId, sec.Code AS SectionCode, dep.Code AS DepartmentCode,
	COUNT(*) AS TotalEmployee
	FROM dbo.Employee AS e
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON g.SectionId = sec.SectionId
		INNER JOIN dbo.Department AS dep WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId
	WHERE e.[Enabled] = 1
	GROUP BY sec.SectionId, dep.DepartmentId, sec.Code, dep.Code
	Order By sec.Code, dep.Code

	DROP TABLE #tmpCurrenBalance
END
