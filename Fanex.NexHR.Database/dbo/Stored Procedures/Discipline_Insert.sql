
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

	DECLARE @SyncFromDate DATETIME

	SET @SyncFromDate = '2015-06-01 00:00:00.000'

	DECLARE @TotalWorkHoursWeekend DECIMAL(18, 2)
		,@TotalWorkHours DECIMAL(18, 2)
		,@LateInMins INT
		,@EarlyOutMins INT
		,@AnnualLeaveType INT
		,@UnpaidLeaveType INT
		,@GeneralLeaveType INT

	SELECT @TotalWorkHoursWeekend = 9.75
		,@TotalWorkHours = 7
		,@EarlyOutMins = 15
		,@LateInMins = 0
		,@AnnualLeaveType = 2
		,@UnpaidLeaveType = 25
		,@GeneralLeaveType = 0

	CREATE TABLE #MissingCheckInOutDiscipline (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[EmployeeComment] NVARCHAR(500)
		,[ApproverComment] NVARCHAR(500)
		)

	INSERT INTO #MissingCheckInOutDiscipline (
		WorkDate
		,EmpId
		,EmployeeComment
		,ApproverComment
		)
	SELECT [AttendanceDate]
		,empA.[EmpId]
		,ISNULL(req.Remark, '')
		,ISNULL(req.ApproverComment, '')
	FROM [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK)
		INNER JOIN [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK) ON empA.[EmpId] = empC.[EmpId]
		AND empA.[AttendanceDate] = empC.[WorkDate]
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empA.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 2
		LEFT JOIN (
		SELECT RequestId
			,EmpId
			,OnDate
			,Remark
			,ApproverComment
		FROM (
			SELECT missR.RequestId
				,missR.EmpId
				,missR.OnDate
				,workR.Remark
				,workR.ApproverComment
				,row_number() OVER (
					PARTITION BY missR.OnDate, missR.EmpId ORDER BY missR.RequestId DESC
					) AS rownum
			FROM dbo.InoutMissRequest AS missR WITH (NOLOCK)
				INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON missR.[RequestId] = workR.[RequestId]
				AND workR.[CurrentStatus] IN (4, 6)
				AND workR.[RequestType] IN (1, 2) -- Check In / Check Out
			WHERE missR.[OnDate] BETWEEN @FromDate
					AND @ToDaTe
				AND (
					@EmpId IS NULL
					OR missR.[EmpId] = @EmpId
					)
			) AS T
		WHERE rownum = 1
		) AS req ON empA.[AttendanceDate] = req.[OnDate]
		AND empA.[EmpId] = req.[EmpId]
	WHERE [AttendanceDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR empA.[EmpId] = @EmpId
			)
		AND (
			empA.[AttendanceDate] BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(empA.[ShiftId], '') <> ''
		AND empC.LeaveDay <> 1
		AND (
			(
				ISNULL([InFixed], '') = ''
				AND ISNULL([OutFixed], '') <> ''
				AND GETDATE() > empA.[ExpectedInTime]
				)
			OR (
				ISNULL([InFixed], '') <> ''
				AND ISNULL([OutFixed], '') = ''
				AND GETDATE() > empA.[ExpectedOutTime]
				)
			OR req.[EmpId] IS NOT NULL
			)
		AND ISNULL(rul.Value, 0) = 1

	CREATE TABLE #LateInDiscipline (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[EmployeeComment] NVARCHAR(500)
		,[ApproverComment] NVARCHAR(500)
		)

	INSERT INTO #LateInDiscipline (
		WorkDate
		,EmpId
		,EmployeeComment
		,ApproverComment
		)
	SELECT [AttendanceDate]
		,empA.[EmpId]
		,ISNULL(req.Remark, '')
		,ISNULL(req.ApproverComment, '')
	FROM [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK)
		INNER JOIN [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK) ON empA.[EmpId] = empC.[EmpId]
		AND empA.[AttendanceDate] = empC.[WorkDate]
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empA.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 3
		LEFT JOIN (
		SELECT inR.FromDate
			,inR.[EmpId]
			,workR.Remark
			,workR.ApproverComment
		FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
			INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
			AND workR.[CurrentStatus] IN (4, 6)
			AND workR.[RequestType] = 3
		WHERE inR.FromDate BETWEEN @FromDate
				AND @ToDaTe
			AND (
				@EmpId IS NULL
				OR inR.[EmpId] = @EmpId
				)
		) AS req ON empA.[AttendanceDate] = req.FromDate
		AND empA.[EmpId] = req.[EmpId]
	WHERE [AttendanceDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR empA.[EmpId] = @EmpId
			)
		AND (
			empA.[AttendanceDate] BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(empA.[ShiftId], '') <> ''
		AND empC.LeaveDay <> 1
		AND (
			empA.[LateIn] > @LateInMins
			OR req.[EmpId] IS NOT NULL
			)
		AND ISNULL(rul.Value, 0) = 1

	CREATE TABLE #EarlyOutDiscipline (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[EmployeeComment] NVARCHAR(500)
		,[ApproverComment] NVARCHAR(500)
		)

	INSERT INTO #EarlyOutDiscipline (
		WorkDate
		,EmpId
		,EmployeeComment
		,ApproverComment
		)
	SELECT [AttendanceDate]
		,empA.[EmpId]
		,ISNULL(req.Remark, '')
		,ISNULL(req.ApproverComment, '')
	FROM [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK)
		INNER JOIN [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK) ON empA.[EmpId] = empC.[EmpId]
		AND empA.[AttendanceDate] = empC.[WorkDate]
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empA.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 4
		LEFT JOIN (
		SELECT inR.FromDate
			,inR.[EmpId]
			,workR.Remark
			,workR.ApproverComment
		FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
			INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
			AND workR.[CurrentStatus] IN (4, 6)
			AND workR.[RequestType] = 4
		WHERE inR.FromDate BETWEEN @FromDate
				AND @ToDaTe
			AND (
				@EmpId IS NULL
				OR inR.[EmpId] = @EmpId
				)
		) AS req ON empA.[AttendanceDate] = req.FromDate
		AND empA.[EmpId] = req.[EmpId]
	WHERE [AttendanceDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR empA.[EmpId] = @EmpId
			)
		AND (
			empA.[AttendanceDate] BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(empA.[ShiftId], '') <> ''
		AND empC.LeaveDay <> 1
		AND (
			empA.EarlyOut > @EarlyOutMins
			OR req.[EmpId] IS NOT NULL
			)
		AND ISNULL(rul.Value, 0) = 1

	CREATE TABLE #ChangeShiftDiscipline (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[EmployeeComment] NVARCHAR(500)
		,[ApproverComment] NVARCHAR(500)
		)

	INSERT INTO #ChangeShiftDiscipline (
		WorkDate
		,EmpId
		,EmployeeComment
		,ApproverComment
		)
	SELECT [AttendanceDate]
		,empA.[EmpId]
		,ISNULL(req.Remark, '')
		,ISNULL(req.ApproverComment, '')
	FROM [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK)
		INNER JOIN [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK) ON empA.[EmpId] = empC.[EmpId]
		AND empA.[AttendanceDate] = empC.[WorkDate]
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empA.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 1
		INNER JOIN (
		SELECT cg.FromDate
			,cg.[EmpId]
			,workR.Remark
			,workR.ApproverComment
		FROM [ChangeShiftRequest] AS cg WITH (NOLOCK)
			INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON cg.[RequestId] = workR.[RequestId]
			AND workR.[CurrentStatus] IN (4, 6)
			AND workR.[RequestType] = 5
		WHERE cg.FromDate BETWEEN @FromDate
				AND @ToDaTe
			AND (
				@EmpId IS NULL
				OR cg.[EmpId] = @EmpId
				)
			--AND workR.CreatedDate >= cg.FromDate
		) AS req ON empA.[AttendanceDate] = req.FromDate
		AND empA.[EmpId] = req.[EmpId]
	WHERE [AttendanceDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR empA.[EmpId] = @EmpId
			)
		AND (
			empA.[AttendanceDate] BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(rul.Value, 0) = 1

	CREATE TABLE #WorkingUnderTimeDiscipline (
		[WorkDate] DATETIME
		,[EmpId] INT
		)

	INSERT INTO #WorkingUnderTimeDiscipline (
		WorkDate
		,EmpId
		)
	SELECT [AttendanceDate]
		,empA.[EmpId]
	FROM [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK)
		INNER JOIN [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK) ON empA.[EmpId] = empC.[EmpId]
		AND empA.[AttendanceDate] = empC.[WorkDate]
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empA.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 7
	WHERE [AttendanceDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR empA.[EmpId] = @EmpId
			)
		AND (
			empA.[AttendanceDate] BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(empA.[ShiftId], '') <> ''
		AND empC.LeaveDay <> 1
		AND (
			(
				DATEPART(DW, empA.[AttendanceDate]) IN (1, 7) -- Sunday or Saturday
				AND [TotalWorkHours] < @TotalWorkHoursWeekend
				)
			OR (
				DATEPART(DW, empA.[AttendanceDate]) NOT IN (1, 7)
				AND [TotalWorkHours] < @TotalWorkHours
				)
			)
		AND ISNULL(rul.Value, 0) = 1

	CREATE TABLE #UnpainLeaveAbsentDiscipline (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[EmployeeComment] NVARCHAR(500)
		,[ApproverComment] NVARCHAR(500)
		)

	INSERT INTO #UnpainLeaveAbsentDiscipline (
		WorkDate
		,EmpId
		,EmployeeComment
		,ApproverComment
		)
	SELECT empC.WorkDate
		,empC.[EmpId]
		,lea.Remark
		,lea.ApproverComment
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 8
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lv.FromDate
			,lv.ToDate
			,wk.Remark
			,wk.ApproverComment
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
		WHERE lv.LeaveType = @UnpaidLeaveType
			AND wk.[CurrentStatus] IN (4, 6)
			AND DATEDIFF(dd, lv.FromDate, wk.[CreatedDate]) >= 0
			AND lv.ToDate >= @SyncFromDate
			AND (
				(
					lv.FromDate BETWEEN @FromDate
						AND @ToDaTe
					)
				OR (
					@FromDate BETWEEN lv.FromDate
						AND lv.ToDate
					)
				)
			AND (
				ISNULL(@EmpId, '') = ''
				OR lv.[EmpId] = @EmpId
				)
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND DATEDIFF(dd, empC.WorkDate, lea.FromDate) = 0
	WHERE CAST(empC.[WorkDate] AS DATE) <= CAST(GETDATE() AS DATE)
		AND empC.[WorkDate] >= @SyncFromDate
		AND (
			empC.WorkDate BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(rul.Value, 0) = 1
	
	UNION ALL
	SELECT empC.WorkDate
		,empC.[EmpId]
		,lea.Remark
		,lea.ApproverComment
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 8
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lvt.OnDate
			,wk.Remark
			,wk.ApproverComment
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
			INNER JOIN dbo.LeaveRequestDetail lvt WITH (NOLOCK) ON lv.RequestId = lvt.RequestId
		WHERE lv.LeaveType = @GeneralLeaveType
			AND lvt.LeaveType = @UnpaidLeaveType
			AND wk.[CurrentStatus] IN (4, 6)
			AND lvt.OnDate = (
				SELECT MIN(OnDate)
				FROM dbo.LeaveRequestDetail
				WHERE LeaveType = @UnpaidLeaveType
					AND RequestId = lv.RequestId
				GROUP BY RequestId
				)
			AND DATEDIFF(dd, lv.FromDate, wk.[CreatedDate]) >= 0
			AND lv.ToDate >= @SyncFromDate
			AND lvt.OnDate BETWEEN @FromDate
				AND @ToDaTe
			AND (
				ISNULL(@EmpId, '') = ''
				OR lv.[EmpId] = @EmpId
				)
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND DATEDIFF(dd, empC.WorkDate, lea.OnDate) = 0
	WHERE CAST(empC.[WorkDate] AS DATE) <= CAST(GETDATE() AS DATE)
		AND empC.[WorkDate] >= @SyncFromDate
		AND (
			empC.WorkDate BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(rul.Value, 0) = 1

	CREATE TABLE #AbsentDiscipline (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[EmployeeComment] NVARCHAR(500)
		,[ApproverComment] NVARCHAR(500)
		)

	INSERT INTO #AbsentDiscipline (
		WorkDate
		,EmpId
		,EmployeeComment
		,ApproverComment
		)
	SELECT empC.WorkDate
		,empC.[EmpId]
		,lea.Remark
		,lea.ApproverComment
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 5
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lv.FromDate
			,lv.ToDate
			,wk.Remark
			,wk.ApproverComment
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
		WHERE lv.LeaveType = 2
			AND wk.[CurrentStatus] IN (4, 6)
			AND DATEDIFF(dd, lv.FromDate, wk.[CreatedDate]) >= 0
			AND lv.ToDate >= @SyncFromDate
			AND (
				(
					lv.FromDate BETWEEN @FromDate
						AND @ToDaTe
					)
				OR (
					@FromDate BETWEEN lv.FromDate
						AND lv.ToDate
					)
				)
			AND (
				ISNULL(@EmpId, '') = ''
				OR lv.[EmpId] = @EmpId
				)
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND DATEDIFF(dd, empC.WorkDate, lea.FromDate) = 0
	WHERE CAST(empC.[WorkDate] AS DATE) <= CAST(GETDATE() AS DATE)
		AND empC.[WorkDate] >= @SyncFromDate
		AND (
			empC.WorkDate BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(rul.Value, 0) = 1
	
	UNION ALL
	SELECT empC.WorkDate
		,empC.[EmpId]
		,lea.Remark
		,lea.ApproverComment
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 5
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lvt.OnDate
			,wk.Remark
			,wk.ApproverComment
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
			INNER JOIN dbo.LeaveRequestDetail lvt WITH (NOLOCK) ON lv.RequestId = lvt.RequestId
		WHERE lv.LeaveType = @GeneralLeaveType
			AND lvt.LeaveType = @AnnualLeaveType
			AND wk.[CurrentStatus] IN (4, 6)
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.LeaveRequestDetail WITH (NOLOCK)
				WHERE RequestId = lv.RequestId
					AND LeaveType = @UnpaidLeaveType
				)
			AND lvt.OnDate = (
				SELECT MIN(OnDate)
				FROM dbo.LeaveRequestDetail
				WHERE LeaveType = @AnnualLeaveType
					AND RequestId = lv.RequestId
				GROUP BY RequestId
				)
			AND DATEDIFF(dd, lv.FromDate, wk.[CreatedDate]) >= 0
			AND lv.ToDate >= @SyncFromDate
			AND (
				(
					lv.FromDate BETWEEN @FromDate
						AND @ToDaTe
					)
				OR (
					@FromDate BETWEEN lv.FromDate
						AND lv.ToDate
					)
				)
			AND (
				ISNULL(@EmpId, '') = ''
				OR lv.[EmpId] = @EmpId
				)
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND DATEDIFF(dd, empC.WorkDate, lea.OnDate) = 0
	WHERE CAST(empC.[WorkDate] AS DATE) <= CAST(GETDATE() AS DATE)
		AND empC.[WorkDate] >= @SyncFromDate
		AND (
			empC.WorkDate BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(rul.Value, 0) = 1
	
	UNION
	SELECT att.AttendanceDate
		,att.EmpId
		,''
		,''
	FROM [dbo].[EmployeeAttendance] AS att WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 5
	WHERE (
			att.AttendanceDate BETWEEN @FromDate
				AND @ToDate
			)
		AND att.[EmpId] = att.[EmpId]
		AND ISNULL(att.[ShiftId], '') <> ''
		AND ISNULL(att.[InFixed], '') = ''
		AND ISNULL(att.[OutFixed], '') = ''
		AND GETDATE() > att.[ExpectedOutTime]
		AND att.LeaveDay <> 1
		AND ISNULL(rul.Value, 0) = 1

	CREATE TABLE #UnpaidLeaveDiscipline (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[EmployeeComment] NVARCHAR(500)
		,[ApproverComment] NVARCHAR(500)
		)

	INSERT INTO #UnpaidLeaveDiscipline (
		WorkDate
		,EmpId
		,EmployeeComment
		,ApproverComment
		)
	SELECT empC.WorkDate
		,empC.[EmpId]
		,lea.Remark
		,lea.ApproverComment
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 6
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lv.FromDate
			,lv.ToDate
			,wk.Remark
			,wk.ApproverComment
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
		WHERE lv.LeaveType = @UnpaidLeaveType
			AND wk.[CurrentStatus] IN (4, 6)
			AND lv.ToDate >= @SyncFromDate
			AND (
				(
					lv.FromDate BETWEEN @FromDate
						AND @ToDaTe
					)
				OR (
					@FromDate BETWEEN lv.FromDate
						AND lv.ToDate
					)
				)
			AND (
				ISNULL(@EmpId, '') = ''
				OR lv.[EmpId] = @EmpId
				)
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND CAST(empC.[WorkDate] AS DATE) >= CAST(lea.[FromDate] AS DATE)
		AND CAST(empC.[WorkDate] AS DATE) <= CAST(lea.[ToDate] AS DATE)
	WHERE CAST(empC.[WorkDate] AS DATE) <= CAST(GETDATE() AS DATE)
		AND empC.[WorkDate] >= @SyncFromDate
		AND (
			empC.WorkDate BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(rul.Value, 0) = 1
	
	UNION ALL
	SELECT empC.WorkDate
		,empC.[EmpId]
		,lea.Remark
		,lea.ApproverComment
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 6
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lvt.OnDate
			,wk.Remark
			,wk.ApproverComment
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
			INNER JOIN dbo.LeaveRequestDetail lvt WITH (NOLOCK) ON lv.RequestId = lvt.RequestId
		WHERE lv.LeaveType = @GeneralLeaveType
			AND lvt.LeaveType = @UnpaidLeaveType
			AND wk.[CurrentStatus] IN (4, 6)
			AND lv.ToDate >= @SyncFromDate
			AND (
				lvt.OnDate BETWEEN @FromDate
					AND @ToDaTe
				)
			AND (
				ISNULL(@EmpId, '') = ''
				OR lv.[EmpId] = @EmpId
				)
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND empC.[WorkDate] = lea.OnDate
	WHERE CAST(empC.[WorkDate] AS DATE) <= CAST(GETDATE() AS DATE)
		AND empC.[WorkDate] >= @SyncFromDate
		AND (
			empC.WorkDate BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(rul.Value, 0) = 1

	CREATE TABLE #EmployeeDiscipline (
		[EmpId] [int] NOT NULL
		,[WorkDate] [date] NOT NULL
		,[DisciplineType] [int] NOT NULL
		,[ShiftName] [varchar](50) NULL
		,[CheckIn] [datetime] NULL
		,[CheckOut] [datetime] NULL
		,[TotalHours] [decimal](5, 2) NULL
		,[LateIn] [int] NULL
		,[EarlyOut] [int] NULL
		,[DisciplineScore] [decimal](5, 2) NULL
		,[EmployeeComment] [nvarchar](500) NULL
		,[ApproverComment] [nvarchar](500) NULL
		,[IsMissingCheckInOut] [bit] NULL
		,[IsLateIn] [bit] NULL
		,[IsEarlyOut] [bit] NULL
		,[IsWorkingUndertime] [bit] NULL
		,[IsChangeShift] [bit] NULL
		,[IsAbsent] [bit] NULL
		,[IsUnpaidLeave] [bit] NULL
		,[IsUnpaidLeaveAbsent] [bit] NULL
		)

	INSERT #EmployeeDiscipline (
		[EmpId]
		,[WorkDate]
		,[DisciplineType]
		,[ShiftName]
		,[CheckIn]
		,[CheckOut]
		,[TotalHours]
		,[LateIn]
		,[EarlyOut]
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
		)
	SELECT e.EmpId
		,att.AttendanceDate
		,1
		,ISNULL(sh.NAME, '')
		,att.InFixed
		,att.OutFixed
		,att.TotalWorkHours
		,att.LateIn
		,att.EarlyOut
		,(CASE 
			WHEN (upa.EmpId IS NOT NULL AND ISNULL(dis.IsIgnoredUnpaidLeaveAbsent, 0) = 0) THEN 3.5
			ELSE 0 END + CASE 
			WHEN (ab.EmpId IS NOT NULL AND upa.EmpId IS NULL AND ISNULL(dis.IsIgnoredAbsent, 0) = 0) THEN 3.5
			ELSE 0 END + CASE 
			WHEN (up.EmpId IS NOT NULL AND upa.EmpId IS NULL AND ab.EmpId IS NULL AND ISNULL(dis.IsIgnoredUnpaidLeave, 0) = 0) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (la.EmpId IS NOT NULL AND ISNULL(dis.IsIgnoredLateIn, 0) = 0) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (ear.EmpId IS NOT NULL AND ISNULL(dis.IsIgnoredEarlyOut, 0) = 0) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (wud.EmpId IS NOT NULL AND ISNULL(dis.IsIgnoredWorkingUndertime, 0) = 0) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (mis.EmpId IS NOT NULL AND ISNULL(dis.IsIgnoredMissingCheckInOut, 0) = 0) THEN 2.5
			ELSE 0 END + CASE 
			WHEN (chs.EmpId IS NOT NULL AND ISNULL(dis.IsIgnoredChangeShift, 0) = 0) THEN 2.5
			ELSE 0 END
			)
		,CASE 
			WHEN ISNULL(mis.EmployeeComment, '') <> '' THEN 'Missing Check In/Out_ ' + ISNULL(mis.EmployeeComment, '') + ' & '
			ELSE ''
			END + CASE 
			WHEN ISNULL(la.EmployeeComment, '') <> '' THEN + 'Late In_ ' + ISNULL(la.EmployeeComment, '') + ' & '
			ELSE ''
			END + CASE 
			WHEN ISNULL(ear.EmployeeComment, '') <> '' THEN + 'Early Out_ ' + ISNULL(ear.EmployeeComment, '') + ' & '
			ELSE ''
			END + CASE 
			WHEN ISNULL(chs.EmployeeComment, '') <> '' THEN + 'Change Shift_ ' + ISNULL(chs.EmployeeComment, '') + ' & '
			ELSE ''
			END + CASE 
			WHEN ISNULL(upa.EmployeeComment, '') <> ''
				OR ISNULL(ab.EmployeeComment, '') <> ''
				OR ISNULL(up.EmployeeComment, '') <> '' THEN + 'Leave_ ' + (CASE 
					WHEN ISNULL(upa.EmployeeComment, '') <> '' THEN upa.EmployeeComment
					ELSE CASE 
							WHEN ISNULL(ab.EmployeeComment, '') <> '' THEN ab.EmployeeComment
							ELSE CASE 
									WHEN ISNULL(up.EmployeeComment, '') <> '' THEN up.EmployeeComment
									ELSE ''
									END
							END END
					) + ' & '
			ELSE ''
			END
		,CASE 
			WHEN ISNULL(mis.ApproverComment, '') <> '' THEN 'Missing Check In/Out_ ' + ISNULL(mis.ApproverComment, '') + ' & '
			ELSE ''
			END + CASE 
			WHEN ISNULL(la.ApproverComment, '') <> '' THEN + 'Late In_ ' + ISNULL(la.ApproverComment, '') + ' & '
			ELSE ''
			END + CASE 
			WHEN ISNULL(ear.ApproverComment, '') <> '' THEN + 'Early Out_ ' + ISNULL(ear.ApproverComment, '') + ' & '
			ELSE ''
			END + CASE 
			WHEN ISNULL(chs.ApproverComment, '') <> '' THEN + 'Change Shift_ ' + ISNULL(chs.ApproverComment, '') + ' & '
			ELSE ''
			END + CASE 
			WHEN ISNULL(upa.ApproverComment, '') <> ''
				OR ISNULL(ab.ApproverComment, '') <> ''
				OR ISNULL(up.ApproverComment, '') <> '' THEN + 'Leave_ ' + (CASE 
					WHEN ISNULL(upa.ApproverComment, '') <> '' THEN upa.ApproverComment
					ELSE CASE 
							WHEN ISNULL(ab.ApproverComment, '') <> '' THEN ab.ApproverComment
							ELSE CASE 
									WHEN ISNULL(up.ApproverComment, '') <> '' THEN up.ApproverComment
									ELSE ''
									END
							END END
					) + ' & '
			ELSE ''
			END
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
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON e.EmpId = att.EmpId
		LEFT JOIN dbo.[Shift] AS sh WITH (NOLOCK) ON att.ShiftId = sh.ShiftID
		LEFT JOIN #UnpainLeaveAbsentDiscipline AS upa ON e.EmpId = upa.EmpId
		AND att.AttendanceDate = upa.WorkDate
		LEFT JOIN #AbsentDiscipline AS ab ON e.EmpId = ab.EmpId
		AND att.AttendanceDate = ab.WorkDate
		LEFT JOIN #UnpaidLeaveDiscipline AS up ON e.EmpId = up.EmpId
		AND att.AttendanceDate = up.WorkDate
		LEFT JOIN #LateInDiscipline AS la ON e.EmpId = la.EmpId
		AND att.AttendanceDate = la.WorkDate
		LEFT JOIN #EarlyOutDiscipline AS ear ON e.EmpId = ear.EmpId
		AND att.AttendanceDate = ear.WorkDate
		LEFT JOIN #WorkingUnderTimeDiscipline AS wud ON e.EmpId = wud.EmpId
		AND att.AttendanceDate = wud.WorkDate
		LEFT JOIN #MissingCheckInOutDiscipline AS mis ON e.EmpId = mis.EmpId
		AND att.AttendanceDate = mis.WorkDate
		LEFT JOIN #ChangeShiftDiscipline AS chs ON e.EmpId = chs.EmpId
		AND att.AttendanceDate = chs.WorkDate
		LEFT JOIN dbo.EmployeeDiscipline AS dis WITH (NOLOCK) ON e.EmpId = dis.EmpId
		AND att.AttendanceDate = dis.WorkDate AND dis.DisciplineType = 1
	WHERE ISNULL(e.isDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND CAST(att.AttendanceDate AS DATE) <= CAST(GETDATE() AS DATE)
		AND att.AttendanceDate >= @SyncFromDate
		AND (
			att.AttendanceDate BETWEEN @FromDate
				AND @ToDate
			)
		AND (
			upa.EmpId IS NOT NULL
			OR upa.EmpId IS NOT NULL
			OR up.EmpId IS NOT NULL
			OR ab.EmpId IS NOT NULL
			OR la.EmpId IS NOT NULL
			OR ear.EmpId IS NOT NULL
			OR wud.EmpId IS NOT NULL
			OR mis.EmpId IS NOT NULL
			OR chs.EmpId IS NOT NULL
			)

	UPDATE edi
	SET edi.DisciplineType = dis.DisciplineType
		,edi.ShiftName = dis.ShiftName
		,edi.CheckIn = dis.CheckIn
		,edi.CheckOut = dis.CheckOut
		,edi.TotalHours = dis.TotalHours
		,edi.LateIn = dis.LateIn
		,edi.EarlyOut = dis.EarlyOut
		,edi.DisciplineScore = dis.DisciplineScore
		,edi.EmployeeComment = CASE 
			WHEN LEN(dis.EmployeeComment) > 1 THEN LEFT(dis.EmployeeComment, LEN(dis.EmployeeComment) - 2)
			ELSE ''
			END
		,edi.ApproverComment = CASE 
			WHEN LEN(dis.ApproverComment) > 1 THEN LEFT(dis.ApproverComment, LEN(dis.ApproverComment) - 2)
			ELSE ''
			END
		,edi.IsMissingCheckInOut = dis.IsMissingCheckInOut
		,edi.IsLateIn = dis.IsLateIn
		,edi.IsEarlyOut = dis.IsEarlyOut
		,edi.IsWorkingUndertime = dis.IsWorkingUndertime
		,edi.IsChangeShift = dis.IsChangeShift
		,edi.IsAbsent = dis.IsAbsent
		,edi.IsUnpaidLeave = dis.IsUnpaidLeave
		,edi.IsUnpaidLeaveAbsent = dis.IsUnpaidLeaveAbsent
		,edi.IsDeleted = 0
		,edi.ModifiedDate = GETDATE()
	FROM [dbo].[EmployeeDiscipline] AS edi WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #EmployeeDiscipline AS dis ON edi.EmpId = dis.EmpId AND edi.DisciplineType = 1
		AND edi.WorkDate = dis.WorkDate

	UPDATE edi
	SET edi.IsDeleted = 1
		,edi.ModifiedDate = GETDATE()
	FROM [dbo].[EmployeeDiscipline] AS edi WITH (ROWLOCK, UPDLOCK)
	WHERE 
		edi.DisciplineType = 1
		AND NOT EXISTS (
			SELECT 1
			FROM #EmployeeDiscipline
			WHERE edi.EmpId = EmpId
				AND edi.WorkDate = WorkDate
			)
		AND edi.WorkDate BETWEEN @FromDate
				AND @ToDate
		AND (
				@EmpId IS NULL
				OR edi.[EmpId] = @EmpId
				)
	INSERT [dbo].[EmployeeDiscipline] (
		[EmpId]
		,[WorkDate]
		,[DisciplineType]
		,[ShiftName]
		,[CheckIn]
		,[CheckOut]
		,[TotalHours]
		,[LateIn]
		,[EarlyOut]
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
		,[IsIgnoredUnpaidLeaveAbsent]
		,[IsIgnoredAbsent]
		,[IsIgnoredUnpaidLeave]
		,[IsIgnoredLateIn]
		,[IsIgnoredEarlyOut]
		,[IsIgnoredWorkingUnderTime]
		,[IsIgnoredMissingCheckInOut]
		,[IsIgnoredChangeShift]
		,[IsDeleted]
		,[ModifiedBy]
		,[ModifiedDate]
		)
	SELECT dis.EmpId
		,dis.WorkDate
		,dis.DisciplineType
		,dis.ShiftName
		,dis.CheckIn
		,dis.CheckOut
		,dis.TotalHours
		,dis.LateIn
		,dis.EarlyOut
		,dis.DisciplineScore
		,CASE 
			WHEN LEN(dis.EmployeeComment) > 1 THEN LEFT(dis.EmployeeComment, LEN(dis.EmployeeComment) - 2)
			ELSE ''
			END
		,CASE 
			WHEN LEN(dis.ApproverComment) > 1 THEN LEFT(dis.ApproverComment, LEN(dis.ApproverComment) - 2)
			ELSE ''
			END
		,dis.IsMissingCheckInOut
		,dis.IsLateIn
		,dis.IsEarlyOut
		,dis.IsWorkingUndertime
		,dis.IsChangeShift
		,dis.IsAbsent
		,dis.IsUnpaidLeave
		,dis.IsUnpaidLeaveAbsent
		,0
		,0
		,0
		,0
		,0
		,0
		,0
		,0
		,0
		,NULL
		,GETDATE()
	FROM #EmployeeDiscipline AS dis
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeDiscipline WITH (NOLOCK)
			WHERE EmpId = dis.EmpId
				AND WorkDate = dis.WorkDate
				AND DisciplineType = 1
			)

	DROP TABLE #MissingCheckInOutDiscipline

	DROP TABLE #LateInDiscipline

	DROP TABLE #EarlyOutDiscipline

	DROP TABLE #ChangeShiftDiscipline

	DROP TABLE #WorkingUnderTimeDiscipline

	DROP TABLE #UnpainLeaveAbsentDiscipline

	DROP TABLE #AbsentDiscipline

	DROP TABLE #UnpaidLeaveDiscipline

	DROP TABLE #EmployeeDiscipline
END
