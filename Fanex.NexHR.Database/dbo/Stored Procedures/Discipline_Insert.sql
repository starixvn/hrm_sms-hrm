
CREATE PROCEDURE [dbo].[Discipline_Insert] @FromDate DATETIME
	,@ToDate DATETIME
	,@EmpId INT = NULL
AS
/*
Created: 20160418@Quinn.Vo
Task: Insert employee attendance discipline.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncFromDate DATETIME = '2016-01-01 00:00:00.000'
	DECLARE @TotalWorkHoursWeekend DECIMAL(18, 2)
		,@TotalWorkHours DECIMAL(18, 2)
		,@LateInMins INT
		,@EarlyOutMins INT
		,@AnnualLeaveType INT
		,@UnpaidLeaveType INT
		,@GeneralLeaveType INT
		,@KeepInViewLeaveType INT
		,@PublicHolidayLeaveType INT

	SELECT @TotalWorkHoursWeekend = 9.75
		,@TotalWorkHours = 7
		,@EarlyOutMins = 15
		,@LateInMins = 0
		,@AnnualLeaveType = 2
		,@UnpaidLeaveType = 25
		,@GeneralLeaveType = 0
		,@KeepInViewLeaveType = 10
		,@PublicHolidayLeaveType = 13

	CREATE TABLE #MissingCheckInOutDiscipline (
		EmpId INT
		,WorkDate DATE
		,IsIgnored BIT
		,IsOutOfControl BIT
		)

	INSERT INTO #MissingCheckInOutDiscipline (
		EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
		)
	SELECT EmpId
		,WorkDate
		,CASE 
			WHEN SUM(CASE 
						WHEN IsIgnored = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
		,CASE 
			WHEN SUM(CASE 
						WHEN IsOutOfControl = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
	FROM dbo.DisciplineRequestTrans WITH (NOLOCK)
	WHERE (
			@EmpId IS NULL
			OR EmpId = @EmpId
			)
		AND WorkDate >= @SyncFromDate
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND DisciplineTypeId = 2
	GROUP BY EmpId
		,WorkDate
	
	UNION
	SELECT empA.EmpId
		,AttendanceDate
		,0
		,0
	FROM dbo.EmployeeAttendance AS empA WITH (NOLOCK)
		INNER JOIN dbo.EmployeeCalendar AS empC WITH (NOLOCK) ON empA.EmpId = empC.EmpId
		AND empA.AttendanceDate = empC.WorkDate
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empA.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 2
	WHERE AttendanceDate >= @SyncFromDate
		AND ISNULL(@EmpId, empA.EmpId) = empA.EmpId
		AND empA.AttendanceDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(empA.ShiftId, '') <> ''
		AND empC.LeaveDay <> 1
		AND (
			(
				ISNULL(InFixed, '') = ''
				AND ISNULL(OutFixed, '') <> ''
				AND GETDATE() > empA.ExpectedInTime
				)
			OR (
				ISNULL(InFixed, '') <> ''
				AND ISNULL(OutFixed, '') = ''
				AND GETDATE() > empA.ExpectedOutTime
				)
			)
		AND ISNULL(rul.Value, 0) = 1
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.DisciplineRequestTrans WITH (NOLOCK)
			WHERE EmpId = empA.EmpId
				AND WorkDate = empA.AttendanceDate
				AND DisciplineTypeId = 2
			)

	CREATE TABLE #LateInDiscipline (
		EmpId INT
		,WorkDate DATE
		,IsIgnored BIT
		,IsOutOfControl BIT
		)

	INSERT INTO #LateInDiscipline (
		EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
		)
	SELECT EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
	FROM dbo.DisciplineRequestTrans WITH (NOLOCK)
	WHERE (
			@EmpId IS NULL
			OR EmpId = @EmpId
			)
		AND WorkDate >= @SyncFromDate
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND DisciplineTypeId = 3
	
	UNION
	SELECT empA.EmpId
		,AttendanceDate
		,0
		,0
	FROM dbo.EmployeeAttendance AS empA WITH (NOLOCK)
		INNER JOIN dbo.EmployeeCalendar AS empC WITH (NOLOCK) ON empA.[EmpId] = empC.EmpId
		AND empA.AttendanceDate = empC.WorkDate
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empA.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 3
	WHERE AttendanceDate >= @SyncFromDate
		AND ISNULL(@EmpId, empA.EmpId) = empA.EmpId
		AND empA.AttendanceDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(empA.ShiftId, '') <> ''
		AND empC.LeaveDay <> 1
		AND empA.LateIn > @LateInMins
		AND ISNULL(rul.Value, 0) = 1
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.DisciplineRequestTrans WITH (NOLOCK)
			WHERE EmpId = empA.EmpId
				AND WorkDate = empA.AttendanceDate
				AND DisciplineTypeId = 3
			)

	CREATE TABLE #EarlyOutDiscipline (
		EmpId INT
		,WorkDate DATE
		,IsIgnored BIT
		,IsOutOfControl BIT
		)

	INSERT INTO #EarlyOutDiscipline (
		EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
		)
	SELECT EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
	FROM dbo.DisciplineRequestTrans WITH (NOLOCK)
	WHERE (
			@EmpId IS NULL
			OR EmpId = @EmpId
			)
		AND WorkDate >= @SyncFromDate
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND DisciplineTypeId = 4
	
	UNION
	SELECT empA.EmpId
		,AttendanceDate
		,0
		,0
	FROM dbo.EmployeeAttendance AS empA WITH (NOLOCK)
		INNER JOIN dbo.EmployeeCalendar AS empC WITH (NOLOCK) ON empA.EmpId = empC.EmpId
		AND empA.AttendanceDate = empC.WorkDate
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empA.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 4
	WHERE AttendanceDate >= @SyncFromDate
		AND ISNULL(@EmpId, empA.EmpId) = empA.EmpId
		AND empA.AttendanceDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(empA.ShiftId, '') <> ''
		AND empC.LeaveDay <> 1
		AND ISNULL(empC.IsOffDay, 0) = 0
		AND empA.EarlyOut > @EarlyOutMins
		AND ISNULL(rul.Value, 0) = 1
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.DisciplineRequestTrans WITH (NOLOCK)
			WHERE EmpId = empA.EmpId
				AND WorkDate = empA.AttendanceDate
				AND DisciplineTypeId = 4
			)

	CREATE TABLE #ChangeShiftDiscipline (
		EmpId INT
		,WorkDate DATE
		,IsIgnored BIT
		,IsOutOfControl BIT
		)

	INSERT INTO #ChangeShiftDiscipline (
		EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
		)
	SELECT EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
	FROM dbo.DisciplineRequestTrans WITH (NOLOCK)
	WHERE (
			@EmpId IS NULL
			OR EmpId = @EmpId
			)
		AND WorkDate >= @SyncFromDate
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND DisciplineTypeId = 1

	CREATE TABLE #WorkingUnderTimeDiscipline (
		[WorkDate] DATE
		,[EmpId] INT
		,IsIgnored BIT
		,IsOutOfControl BIT
		)

	INSERT INTO #WorkingUnderTimeDiscipline (
		WorkDate
		,EmpId
		,IsIgnored
		,IsOutOfControl
		)
	SELECT AttendanceDate
		,empA.EmpId
		,ISNULL(drt.IsIgnored, 0)
		,ISNULL(drt.IsOutOfControl, 0)
	FROM dbo.EmployeeAttendance AS empA WITH (NOLOCK)
		INNER JOIN dbo.EmployeeCalendar AS empC WITH (NOLOCK) ON empA.EmpId = empC.EmpId
		AND empA.AttendanceDate = empC.WorkDate
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empA.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 7
		LEFT JOIN dbo.DisciplineRequestTrans AS drt  WITH (NOLOCK) ON drt.EmpId = empA.EmpId
			AND drt.WorkDate = empA.AttendanceDate
			AND drt.DisciplineTypeId = 7
	WHERE AttendanceDate >= @SyncFromDate
		AND ISNULL(@EmpId, empA.EmpId) = empA.EmpId
		AND empA.AttendanceDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(empA.ShiftId, '') <> ''
		AND empC.LeaveDay <> 1
		AND (
			(
				DATEPART(DW, empA.AttendanceDate) IN (1, 7) -- Sunday or Saturday
				AND TotalWorkHours < @TotalWorkHoursWeekend
				)
			OR (
				DATEPART(DW, empA.AttendanceDate) NOT IN (1, 7)
				AND TotalWorkHours < @TotalWorkHours
				)
			)
		AND ISNULL(rul.Value, 0) = 1

	CREATE TABLE #UnpaidLeaveAbsentDiscipline (
		EmpId INT
		,WorkDate DATE
		,IsIgnored BIT
		,IsOutOfControl BIT
		)

	INSERT INTO #UnpaidLeaveAbsentDiscipline (
		EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
		)
	SELECT EmpId
		,WorkDate
		,CASE 
			WHEN SUM(CASE 
						WHEN IsIgnored = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
		,CASE 
			WHEN SUM(CASE 
						WHEN IsOutOfControl = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
	FROM dbo.DisciplineRequestTrans WITH (NOLOCK)
	WHERE (
			@EmpId IS NULL
			OR EmpId = @EmpId
			)
		AND WorkDate >= @SyncFromDate
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND DisciplineTypeId = 8
	GROUP BY EmpId
		,WorkDate

	CREATE TABLE #AbsentDiscipline (
		EmpId INT
		,WorkDate DATE
		,IsIgnored BIT
		,IsOutOfControl BIT
		)

	INSERT INTO #AbsentDiscipline (
		EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
		)
	SELECT EmpId
		,WorkDate
		,CASE 
			WHEN SUM(CASE 
						WHEN IsIgnored = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
		,CASE 
			WHEN SUM(CASE 
						WHEN IsOutOfControl = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
	FROM dbo.DisciplineRequestTrans AS drt WITH (NOLOCK)
	WHERE (
			@EmpId IS NULL
			OR EmpId = @EmpId
			)
		AND WorkDate >= @SyncFromDate
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND DisciplineTypeId = 5
		AND NOT EXISTS (
			SELECT 1
			FROM #UnpaidLeaveAbsentDiscipline
			WHERE EmpId = drt.EmpId
				AND WorkDate = drt.WorkDate
			)
	GROUP BY EmpId
		,WorkDate
	
	UNION
	SELECT att.EmpId
		,att.AttendanceDate
		,0
		,0
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 5
	WHERE att.AttendanceDate BETWEEN @FromDate
			AND @ToDate
		AND att.[EmpId] = att.[EmpId]
		AND ISNULL(att.[ShiftId], '') <> ''
		AND ISNULL(att.[InFixed], '') = ''
		AND ISNULL(att.[OutFixed], '') = ''
		AND GETDATE() > att.[ExpectedOutTime]
		AND ISNULL(att.LeaveDay, 0) <> 1
		AND ISNULL(rul.Value, 0) = 1

	CREATE TABLE #UnplannedDiscipline (
		EmpId INT
		,WorkDate DATE
		,IsIgnored BIT
		,IsOutOfControl BIT
		)

	INSERT INTO #UnplannedDiscipline (
		EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
		)
	SELECT EmpId
		,WorkDate
		,CASE 
			WHEN SUM(CASE 
						WHEN IsIgnored = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
		,CASE 
			WHEN SUM(CASE 
						WHEN IsOutOfControl = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
	FROM dbo.DisciplineRequestTrans AS drt WITH (NOLOCK)
	WHERE (
			@EmpId IS NULL
			OR EmpId = @EmpId
			)
		AND WorkDate >= @SyncFromDate
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND DisciplineTypeId = 9
		AND NOT EXISTS (
			SELECT 1
			FROM #UnpaidLeaveAbsentDiscipline
			WHERE EmpId = drt.EmpId
				AND WorkDate = drt.WorkDate
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #AbsentDiscipline
			WHERE EmpId = drt.EmpId
				AND WorkDate = drt.WorkDate
			)
	GROUP BY EmpId
		,WorkDate

	CREATE TABLE #UnpaidLeaveDiscipline (
		EmpId INT
		,WorkDate DATE
		,IsIgnored BIT
		,IsOutOfControl BIT
		)

	INSERT INTO #UnplannedDiscipline (
		EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
		)
	SELECT EmpId
		,WorkDate
		,CASE 
			WHEN SUM(CASE 
						WHEN IsIgnored = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
		,CASE 
			WHEN SUM(CASE 
						WHEN IsOutOfControl = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
	FROM dbo.DisciplineRequestTrans AS drt WITH (NOLOCK)
	WHERE (
			@EmpId IS NULL
			OR EmpId = @EmpId
			)
		AND WorkDate >= @SyncFromDate
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND DisciplineTypeId = 6
		AND NOT EXISTS (
			SELECT 1
			FROM #UnpaidLeaveAbsentDiscipline
			WHERE EmpId = drt.EmpId
				AND WorkDate = drt.WorkDate
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #AbsentDiscipline
			WHERE EmpId = drt.EmpId
				AND WorkDate = drt.WorkDate
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #UnplannedDiscipline
			WHERE EmpId = drt.EmpId
				AND WorkDate = drt.WorkDate
			)
	GROUP BY EmpId
		,WorkDate

	CREATE TABLE #WeekendLeaveDiscipline (
		EmpId INT
		,WorkDate DATE
		,IsIgnored BIT
		,IsOutOfControl BIT
		)

	INSERT INTO #WeekendLeaveDiscipline (
		EmpId
		,WorkDate
		,IsIgnored
		,IsOutOfControl
		)
	SELECT EmpId
		,WorkDate
		,CASE 
			WHEN SUM(CASE 
						WHEN IsIgnored = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
		,CASE 
			WHEN SUM(CASE 
						WHEN IsOutOfControl = 0 THEN 1
						ELSE 0
						END) > 0 THEN 0
			ELSE 1
			END
	FROM dbo.DisciplineRequestTrans AS drt WITH (NOLOCK)
	WHERE (
			@EmpId IS NULL
			OR EmpId = @EmpId
			)
		AND WorkDate >= @SyncFromDate
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND DisciplineTypeId = 10
	GROUP BY EmpId
		,WorkDate
		
	CREATE TABLE #Discipline (
		[EmpId] [int] NOT NULL
		,[WorkDate] [date] NOT NULL
		,[DisciplineScore] [decimal](5, 2) NULL
		,[IsOutOfControlReason] [bit] NULL
		,[EmployeeComment] [nvarchar](MAX) NULL
		,[ApproverComment] [nvarchar](MAX) NULL
		,[IsMissingCheckInOut] [bit] NULL
		,[IsLateIn] [bit] NULL
		,[IsEarlyOut] [bit] NULL
		,[IsWorkingUndertime] [bit] NULL
		,[IsChangeShift] [bit] NULL
		,[IsAbsent] [bit] NULL
		,[IsUnpaidLeave] [bit] NULL
		,[IsUnpaidLeaveAbsent] [bit] NULL
		,[IsUnplannedLeave] [bit] NULL
		,[IsWeekendLeave] [bit] NULL
		,[IsIgnoredMissingCheckInOut] [bit] NULL
		,[IsIgnoredLateIn] [bit] NULL
		,[IsIgnoredEarlyOut] [bit] NULL
		,[IsIgnoredWorkingUndertime] [bit] NULL
		,[IsIgnoredChangeShift] [bit] NULL
		,[IsIgnoredAbsent] [bit] NULL
		,[IsIgnoredUnpaidLeave] [bit] NULL
		,[IsIgnoredUnpaidLeaveAbsent] [bit] NULL
		,[IsIgnoredUnplannedLeave] [bit] NULL
		,[IsIgnoredWeekendLeave] [bit] NULL
		,[IsOutOfControlMissingCheckInOut] [bit] NULL
		,[IsOutOfControlLateIn] [bit] NULL
		,[IsOutOfControlEarlyOut] [bit] NULL
		,[IsOutOfControlWorkingUndertime] [bit] NULL
		,[IsOutOfControlChangeShift] [bit] NULL
		,[IsOutOfControlAbsent] [bit] NULL
		,[IsOutOfControlUnpaidLeave] [bit] NULL
		,[IsOutOfControlUnpaidLeaveAbsent] [bit] NULL
		,[IsOutOfControlUnplannedLeave] [bit] NULL
		,[IsOutOfControlWeekendLeave] [bit] NULL
		)

	INSERT #Discipline (
		[EmpId]
		,[WorkDate]
		,[DisciplineScore]
		,[EmployeeComment]
		,[ApproverComment]
		,[IsMissingCheckInOut]
		,[IsLateIn]
		,[IsEarlyOut]
		,[IsWorkingUndertime]
		,[IsChangeShift]
		,[IsAbsent]
		,[IsUnpaidLeave]
		,[IsUnpaidLeaveAbsent]
		,[IsUnplannedLeave]
		,[IsWeekendLeave]
		,[IsIgnoredMissingCheckInOut]
		,[IsIgnoredLateIn]
		,[IsIgnoredEarlyOut]
		,[IsIgnoredWorkingUndertime]
		,[IsIgnoredChangeShift]
		,[IsIgnoredAbsent]
		,[IsIgnoredUnpaidLeave]
		,[IsIgnoredUnpaidLeaveAbsent]
		,[IsIgnoredUnplannedLeave]
		,[IsIgnoredWeekendLeave]
		,[IsOutOfControlMissingCheckInOut]
		,[IsOutOfControlLateIn]
		,[IsOutOfControlEarlyOut]
		,[IsOutOfControlWorkingUndertime]
		,[IsOutOfControlChangeShift]
		,[IsOutOfControlAbsent]
		,[IsOutOfControlUnpaidLeave]
		,[IsOutOfControlUnpaidLeaveAbsent]
		,[IsOutOfControlUnplannedLeave]
		,[IsOutOfControlWeekendLeave]
		)
	SELECT e.EmpId
		,cal.WorkDate
		,(CASE 
			WHEN (
					upa.EmpId IS NOT NULL
					AND ISNULL(upa.IsIgnored, 0) = 0
					) THEN 3.5
			ELSE 0 END + CASE 
			WHEN (
					upl.EmpId IS NOT NULL
					AND ISNULL(upl.IsIgnored, 0) = 0
					) THEN 3.5
			ELSE 0 END + CASE 
			WHEN (
					ab.EmpId IS NOT NULL
					AND upa.EmpId IS NULL
					AND ISNULL(ab.IsIgnored, 0) = 0
					) THEN 3.5
			ELSE 0 END + CASE 
			WHEN (
					up.EmpId IS NOT NULL
					AND upa.EmpId IS NULL
					AND ab.EmpId IS NULL
					AND ISNULL(up.IsIgnored, 0) = 0
					) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (
					la.EmpId IS NOT NULL
					AND ISNULL(la.IsIgnored, 0) = 0
					) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (
					ear.EmpId IS NOT NULL
					AND ISNULL(ear.IsIgnored, 0) = 0
					) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (
					wud.EmpId IS NOT NULL
					AND ISNULL(dis.IsIgnoredWorkingUndertime, 0) = 0
					) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (
					mis.EmpId IS NOT NULL
					AND ISNULL(mis.IsIgnored, 0) = 0
					) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (
					chs.EmpId IS NOT NULL
					AND ISNULL(chs.IsIgnored, 0) = 0
					) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (
					wld.EmpId IS NOT NULL
					AND ISNULL(wld.IsIgnored, 0) = 0
					) THEN 2.5
			ELSE 0 END
			)
		,(
				SELECT CASE 
						WHEN rq.Remark <> '' THEN ('- ' + dt.NAME + ': ' + rq.Remark + '. ')
						ELSE ''
						END
				FROM dbo.DisciplineRequestTrans AS drt WITH (NOLOCK)
					INNER JOIN dbo.WorkingTimeRequest AS rq WITH (NOLOCK) ON drt.RequestId = rq.RequestId
					INNER JOIN dbo.DisciplineType AS dt WITH (NOLOCK) ON drt.DisciplineTypeId = dt.DisciplineTypeId
				WHERE drt.EmpId = cal.EmpId
					AND drt.WorkDate = cal.WorkDate
				FOR XML PATH('')
				) AS [EmployeeComment]
		,(
				SELECT CASE 
						WHEN rq.ApproverComment <> '' THEN ('- ' + dt.NAME + ': ' + rq.ApproverComment + '. ')
						ELSE ''
						END
				FROM dbo.DisciplineRequestTrans AS drt WITH (NOLOCK)
					INNER JOIN dbo.WorkingTimeRequest AS rq WITH (NOLOCK) ON drt.RequestId = rq.RequestId
					INNER JOIN dbo.DisciplineType AS dt WITH (NOLOCK) ON drt.DisciplineTypeId = dt.DisciplineTypeId
				WHERE drt.EmpId = cal.EmpId
					AND drt.WorkDate = cal.WorkDate
				FOR XML PATH('')
				) AS ApproverComment
		,CASE 
			WHEN mis.EmpId IS NULL THEN 0
			ELSE 1
			END
		,CASE 
			WHEN la.EmpId IS NULL THEN 0
			ELSE 1
			END
		,CASE 
			WHEN ear.EmpId IS NULL THEN 0
			ELSE 1
			END
		,CASE 
			WHEN wud.EmpId IS NULL THEN 0
			ELSE 1
			END
		,CASE 
			WHEN chs.EmpId IS NULL THEN 0
			ELSE 1
			END
		,CASE 
			WHEN (
					upa.EmpId IS NOT NULL
					OR ab.EmpId IS NULL
					) THEN 0
			ELSE 1
			END
		,CASE 
			WHEN (
					upa.EmpId IS NOT NULL
					OR ab.EmpId IS NOT NULL
					OR up.EmpId IS NULL
					) THEN 0
			ELSE 1
			END
		,CASE 
			WHEN upa.EmpId IS NULL THEN 0
			ELSE 1
			END
		,CASE 
			WHEN upl.EmpId IS NULL THEN 0
			ELSE 1
			END
		,CASE 
			WHEN wld.EmpId IS NULL THEN 0
			ELSE 1
			END
		,ISNULL(mis.IsIgnored, 0)
		,ISNULL(la.IsIgnored, 0)
		,ISNULL(ear.IsIgnored, 0)
		,ISNULL(wud.IsIgnored, 0)
		,ISNULL(chs.IsIgnored, 0)
		,ISNULL(ab.IsIgnored, 0)
		,ISNULL(up.IsIgnored, 0)
		,ISNULL(upa.IsIgnored, 0)
		,ISNULL(upl.IsIgnored, 0)
		,ISNULL(wld.IsIgnored, 0)
		,ISNULL(mis.IsOutOfControl, 0)
		,ISNULL(la.IsOutOfControl, 0)
		,ISNULL(ear.IsOutOfControl, 0)
		,ISNULL(wud.IsOutOfControl, 0)
		,ISNULL(chs.IsOutOfControl, 0)
		,ISNULL(ab.IsOutOfControl, 0)
		,ISNULL(up.IsOutOfControl, 0)
		,ISNULL(upa.IsOutOfControl, 0)
		,ISNULL(upl.IsOutOfControl, 0)
		,ISNULL(wld.IsOutOfControl, 0)
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeCalendar AS cal WITH (NOLOCK) ON e.EmpId = cal.EmpId
		LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON e.EmpId = att.EmpId
		AND cal.WorkDate = att.AttendanceDate
		LEFT JOIN dbo.[Shift] AS sh WITH (NOLOCK) ON cal.ShiftId = sh.ShiftID
		LEFT JOIN #UnpaidLeaveAbsentDiscipline AS upa WITH (NOLOCK) ON e.EmpId = upa.EmpId
		AND cal.WorkDate = upa.WorkDate
		LEFT JOIN #AbsentDiscipline AS ab WITH (NOLOCK) ON e.EmpId = ab.EmpId
		AND cal.WorkDate = ab.WorkDate
		LEFT JOIN #UnplannedDiscipline AS upl WITH (NOLOCK) ON e.EmpId = upl.EmpId
		AND cal.WorkDate = upl.WorkDate
		LEFT JOIN #UnpaidLeaveDiscipline AS up WITH (NOLOCK) ON e.EmpId = up.EmpId
		AND cal.WorkDate = up.WorkDate
		LEFT JOIN #WeekendLeaveDiscipline AS wld WITH (NOLOCK) ON e.EmpId = wld.EmpId
		AND cal.WorkDate = wld.WorkDate
		LEFT JOIN #LateInDiscipline AS la WITH (NOLOCK) ON e.EmpId = la.EmpId
		AND cal.WorkDate = la.WorkDate
		LEFT JOIN #EarlyOutDiscipline AS ear WITH (NOLOCK) ON e.EmpId = ear.EmpId
		AND cal.WorkDate = ear.WorkDate
		LEFT JOIN #WorkingUnderTimeDiscipline AS wud WITH (NOLOCK) ON e.EmpId = wud.EmpId
		AND cal.WorkDate = wud.WorkDate
		LEFT JOIN #MissingCheckInOutDiscipline AS mis WITH (NOLOCK) ON e.EmpId = mis.EmpId
		AND cal.WorkDate = mis.WorkDate
		LEFT JOIN #ChangeShiftDiscipline AS chs WITH (NOLOCK) ON e.EmpId = chs.EmpId
		AND cal.WorkDate = chs.WorkDate
		LEFT JOIN dbo.EmployeeDiscipline AS dis WITH (NOLOCK) ON e.EmpId = dis.EmpId
		AND cal.WorkDate = dis.WorkDate
	WHERE ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND ISNULL(@EmpId, e.EmpId) = e.EmpId
		AND cal.WorkDate >= @SyncFromDate
		AND cal.WorkDate BETWEEN @FromDate
			AND @ToDate
		AND (
			upa.EmpId IS NOT NULL
			OR up.EmpId IS NOT NULL
			OR ab.EmpId IS NOT NULL
			OR upl.EmpId IS NOT NULL
			OR wld.EmpId IS NOT NULL
			OR la.EmpId IS NOT NULL
			OR ear.EmpId IS NOT NULL
			OR wud.EmpId IS NOT NULL
			OR mis.EmpId IS NOT NULL
			OR chs.EmpId IS NOT NULL
			)

	UPDATE edi
	SET edi.DisciplineScore = dis.DisciplineScore
		,edi.EmployeeComment = dis.EmployeeComment
		,edi.ApproverComment = dis.ApproverComment
		,edi.IsMissingCheckInOut = dis.IsMissingCheckInOut
		,edi.IsLateIn = dis.IsLateIn
		,edi.IsEarlyOut = dis.IsEarlyOut
		,edi.IsWorkingUndertime = dis.IsWorkingUndertime
		,edi.IsChangeShift = dis.IsChangeShift
		,edi.IsAbsent = dis.IsAbsent
		,edi.IsUnplannedLeave = dis.IsUnplannedLeave
		,edi.IsUnpaidLeave = dis.IsUnpaidLeave
		,edi.IsUnpaidLeaveAbsent = dis.IsUnpaidLeaveAbsent
		,edi.IsWeekendLeave = dis.IsWeekendLeave
		,edi.IsDeleted = 0
		,edi.ModifiedDate = GETDATE()
	FROM dbo.Discipline AS edi WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #Discipline AS dis WITH (NOLOCK) ON edi.EmpId = dis.EmpId
		AND edi.WorkDate = dis.WorkDate

	UPDATE edi
	SET edi.IsMissingCheckInOut = 0
		,edi.IsLateIn = 0
		,edi.IsEarlyOut = 0
		,edi.IsChangeShift = 0
		,edi.IsAbsent = 0
		,edi.IsUnplannedLeave = 0
		,edi.IsUnpaidLeave = 0
		,edi.IsUnpaidLeaveAbsent = 0
		,edi.IsWeekendLeave = 0
		,edi.IsDeleted = 1
		,edi.ModifiedDate = GETDATE()
	FROM dbo.Discipline AS edi WITH (ROWLOCK, UPDLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM #Discipline WITH (NOLOCK)
			WHERE edi.EmpId = EmpId
				AND edi.WorkDate = WorkDate
			)
		AND edi.WorkDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(@EmpId, edi.EmpId) = edi.EmpId
		
	INSERT dbo.Discipline (
		EmpId
		,[WorkDate]
		,[DisciplineScore]
		,[EmployeeComment]
		,[ApproverComment]
		,[IsMissingCheckInOut]
		,[IsLateIn]
		,[IsEarlyOut]
		,[IsWorkingUndertime]
		,[IsChangeShift]
		,[IsAbsent]
		,[IsUnplannedLeave]
		,[IsUnpaidLeave]
		,[IsUnpaidLeaveAbsent]
		,[IsWeekendLeave]
		,[IsDeleted]
		,[ModifiedBy]
		,[ModifiedDate]
		)
	SELECT dis.EmpId
		,dis.WorkDate
		,dis.DisciplineScore
		,dis.EmployeeComment
		,dis.ApproverComment
		,dis.IsMissingCheckInOut
		,dis.IsLateIn
		,dis.IsEarlyOut
		,dis.IsWorkingUndertime
		,dis.IsChangeShift
		,dis.IsAbsent
		,dis.IsUnplannedLeave
		,dis.IsUnpaidLeave
		,dis.IsUnpaidLeaveAbsent
		,dis.IsWeekendLeave
		,0
		,NULL
		,GETDATE()
	FROM #Discipline AS dis WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.Discipline WITH (NOLOCK)
			WHERE EmpId = dis.EmpId
				AND WorkDate = dis.WorkDate
			)

	UPDATE di
	SET di.IsIgnoredMissingCheckInOut = dis.IsIgnoredMissingCheckInOut
		,di.IsIgnoredLateIn = dis.IsIgnoredLateIn
		,di.IsIgnoredEarlyOut = dis.IsIgnoredEarlyOut
		,di.IsIgnoredWorkingUndertime = dis.IsIgnoredWorkingUndertime
		,di.IsIgnoredChangeShift = dis.IsIgnoredChangeShift
		,di.IsIgnoredAbsent = dis.IsIgnoredAbsent
		,di.IsIgnoredUnplannedLeave = dis.IsIgnoredUnplannedLeave
		,di.IsIgnoredUnpaidLeave = dis.IsIgnoredUnpaidLeave
		,di.IsIgnoredUnpaidLeaveAbsent = dis.IsIgnoredUnpaidLeaveAbsent
		,di.IsIgnoredWeekendLeave = dis.IsIgnoredWeekendLeave
	FROM dbo.DisciplineIgnore AS di WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #Discipline AS dis WITH (NOLOCK) ON di.EmpId = dis.EmpId
		AND di.WorkDate = dis.WorkDate

	DELETE di
	FROM dbo.DisciplineIgnore AS di
	WHERE NOT EXISTS (
			SELECT 1
			FROM #Discipline WITH (NOLOCK)
			WHERE di.EmpId = EmpId
				AND di.WorkDate = WorkDate
			)
		AND di.WorkDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(@EmpId, di.EmpId) = di.EmpId

	INSERT INTO dbo.DisciplineIgnore (
		EmpId
		,WorkDate
		,IsIgnoredMissingCheckInOut
		,IsIgnoredLateIn
		,IsIgnoredEarlyOut
		,IsIgnoredWorkingUndertime
		,IsIgnoredChangeShift
		,IsIgnoredAbsent
		,IsIgnoredUnplannedLeave
		,IsIgnoredUnpaidLeave
		,IsIgnoredUnpaidLeaveAbsent
		,IsIgnoredWeekendLeave
		)
	SELECT
		EmpId
		,WorkDate 
		,IsIgnoredMissingCheckInOut
		,IsIgnoredLateIn
		,IsIgnoredEarlyOut
		,IsIgnoredWorkingUndertime
		,IsIgnoredChangeShift
		,IsIgnoredAbsent
		,IsIgnoredUnplannedLeave
		,IsIgnoredUnpaidLeave
		,IsIgnoredUnpaidLeaveAbsent
		,IsIgnoredWeekendLeave
	FROM #Discipline AS dis
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.DisciplineIgnore
			WHERE EmpId = dis.EmpId
				AND WorkDate = dis.WorkDate
			)

	UPDATE doc
	SET doc.IsOutOfControlMissingCheckInOut = dis.IsOutOfControlMissingCheckInOut
		,doc.IsOutOfControlLateIn = dis.IsOutOfControlLateIn
		,doc.IsOutOfControlEarlyOut = dis.IsOutOfControlEarlyOut
		,doc.IsOutOfControlWorkingUndertime = dis.IsOutOfControlWorkingUndertime
		,doc.IsOutOfControlChangeShift = dis.IsOutOfControlChangeShift
		,doc.IsOutOfControlAbsent = dis.IsOutOfControlAbsent
		,doc.IsOutOfControlUnplannedLeave = dis.IsOutOfControlUnplannedLeave
		,doc.IsOutOfControlUnpaidLeave = dis.IsOutOfControlUnpaidLeave
		,doc.IsOutOfControlUnpaidLeaveAbsent = dis.IsOutOfControlUnpaidLeaveAbsent
		,doc.IsOutOfControlWeekendLeave = dis.IsOutOfControlWeekendLeave
	FROM dbo.DisciplineOutOfControl AS doc WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #Discipline AS dis WITH (NOLOCK) ON doc.EmpId = dis.EmpId
		AND doc.WorkDate = dis.WorkDate

	DELETE doc
	FROM dbo.DisciplineOutOfControl AS doc
	WHERE NOT EXISTS (
			SELECT 1
			FROM #Discipline WITH (NOLOCK)
			WHERE doc.EmpId = EmpId
				AND doc.WorkDate = WorkDate
			)
		AND doc.WorkDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(@EmpId, doc.EmpId) = doc.EmpId

	INSERT INTO dbo.DisciplineOutOfControl (
		EmpId
		,WorkDate 
		,IsOutOfControlMissingCheckInOut
		,IsOutOfControlLateIn
		,IsOutOfControlEarlyOut
		,IsOutOfControlWorkingUndertime
		,IsOutOfControlChangeShift
		,IsOutOfControlAbsent
		,IsOutOfControlUnplannedLeave
		,IsOutOfControlUnpaidLeave
		,IsOutOfControlUnpaidLeaveAbsent
		,IsOutOfControlWeekendLeave
		)
	SELECT 
		EmpId
		,WorkDate 
		,IsOutOfControlMissingCheckInOut
		,IsOutOfControlLateIn
		,IsOutOfControlEarlyOut
		,IsOutOfControlWorkingUndertime
		,IsOutOfControlChangeShift
		,IsOutOfControlAbsent
		,IsOutOfControlUnplannedLeave
		,IsOutOfControlUnpaidLeave
		,IsOutOfControlUnpaidLeaveAbsent
		,IsOutOfControlWeekendLeave
	FROM #Discipline AS dis
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.DisciplineOutOfControl
			WHERE EmpId = dis.EmpId
				AND WorkDate = dis.WorkDate
			)

	DROP TABLE #MissingCheckInOutDiscipline

	DROP TABLE #LateInDiscipline

	DROP TABLE #EarlyOutDiscipline

	DROP TABLE #ChangeShiftDiscipline

	DROP TABLE #WorkingUnderTimeDiscipline

	DROP TABLE #UnpaidLeaveAbsentDiscipline

	DROP TABLE #AbsentDiscipline

	DROP TABLE #UnplannedDiscipline

	DROP TABLE #UnpaidLeaveDiscipline

	DROP TABLE #WeekendLeaveDiscipline

	DROP TABLE #Discipline
END
