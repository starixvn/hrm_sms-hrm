CREATE PROCEDURE [dbo].[WorkFromHome_MealAllowanceSummaryReport] @FromDate DATE
	,@ToDate DATE
	,@TeamId VARCHAR(MAX) = ''
	,@EmployeeId INT = NULL
	,@IsLocal BIT = NULL
AS
/*
Created: 20211109@Quinn.Vo
Task: Get meal allowance report
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @tmpToDate DATETIME = DATEADD(SECOND, -1, DATEADD(DAY, 2, CONVERT(DATETIME, @ToDate)))

	CREATE TABLE #tblEmpIds (EmpId INT)

	CREATE TABLE #tblGroupIds (TmpGroupId INT)

	IF ISNULL(@TeamId, '') <> ''
	BEGIN
		INSERT INTO #tblGroupIds (TmpGroupId)
		SELECT DISTINCT val
		FROM fnc_strSplit(@TeamId, ',')
	END

	INSERT INTO #tblEmpIds (EmpId)
	SELECT e.EmpId
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId AND es.IsTopManager = 0
	WHERE (
			@EmployeeId IS NULL
			OR e.EmpId = @EmployeeId
			)
		AND (
			ISNULL(@TeamId, '') = ''
			OR EXISTS (
				SELECT 1
				FROM #tblGroupIds AS tbl WITH (NOLOCK)
				WHERE e.GroupId = tbl.TmpGroupId
				)
			)
		AND (
			@IsLocal IS NULL
			OR es.IsLocal = @IsLocal
			)

	CREATE TABLE #tmpInOut (
		EmpId INT
		,RecordTime DATETIME
		)

	CREATE CLUSTERED INDEX ix_tmpInOut ON #tmpInOut (
		EmpId
		,RecordTime
		)

	INSERT INTO #tmpInOut (
		EmpId
		,RecordTime
		)
	SELECT tr.EmpId
		,RecordTime
	FROM dbo.TimeRecord AS tr WITH (NOLOCK)
		INNER JOIN dbo.TimeRecordExt AS te WITH (NOLOCK) ON tr.ID = te.ID
		--INNER JOIN #tblEmpIds AS tmpemp WITH (NOLOCK) ON tr.EmpId = tmpemp.EmpId
	WHERE tr.RecordTime BETWEEN DATEADD(dd, - 1, @FromDate)
			AND @tmpToDate

	CREATE TABLE #tblEmpAttandance (
		EmpId INT
		,TotalDays DECIMAL(18, 2)
		)

	INSERT INTO #tblEmpAttandance (
		EmpId
		,TotalDays
		)
	SELECT att.EmpId
		,CASE 
			WHEN DATEDIFF(HOUR, att.ExpectedInTime, att.ExpectedOutTime) >= 9 THEN CASE 
					WHEN ISNULL(att.TotalWorkHours, 0) >= 8 THEN 1
					ELSE CASE 
							WHEN ISNULL(att.TotalWorkHours, 0) >= 3.5 THEN 0.5
							ELSE 0
							END
					END
			ELSE CASE 
					WHEN ISNULL(att.TotalWorkHours, 0) >= 7 THEN 1
					ELSE CASE 
							WHEN ISNULL(att.TotalWorkHours, 0) >= 3.5 THEN 0.5
							ELSE 0
							END
					END
			END AS TotalDays
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
		INNER JOIN #tblEmpIds AS e WITH (NOLOCK) ON att.EmpId = e.EmpId
		INNER JOIN dbo.tmpEmployeeCalendar AS tcl WITH (NOLOCK) ON att.EmpId = tcl.EmpId
		AND att.AttendanceDate = tcl.WorkDate
	WHERE ISNULL(att.TotalWorkHours, 0) > 3
		AND tcl.LeaveDay <> 1
		AND att.AttendanceDate BETWEEN @FromDate
			AND @ToDate
		AND EXISTS (
			SELECT 1
			FROM #tmpInOut AS tblinout WITH (NOLOCK)
			WHERE att.EmpId = tblinout.EmpId
				AND (
					att.InFixed = tblinout.RecordTime
					OR att.OutFixed = tblinout.RecordTime
					)
			)

	SELECT att.TotalDays
		,att.EmpId
		,e.Code
		,e.FullName
		,e.NickName
		,g.GroupId AS TeamId
		,g.[Name] AS Team
		,sec.Code AS Section
		,dep.Code AS Department
	FROM (
		SELECT EmpId
			,SUM(TotalDays) AS TotalDays
		FROM #tblEmpAttandance WITH (NOLOCK)
		GROUP BY EmpId
		) AS att
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.EmpId
		LEFT JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON sec.SectionId = g.SectionId
		INNER JOIN dbo.Department AS dep WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId
	ORDER BY dep.Code
		,sec.Code
		,g.[Name]
		,e.Code

	DROP TABLE #tblEmpIds

	DROP TABLE #tblGroupIds

	DROP TABLE #tblEmpAttandance

	DROP TABLE #tmpInOut
END
