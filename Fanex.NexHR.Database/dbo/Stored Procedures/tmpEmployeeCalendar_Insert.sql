
CREATE PROCEDURE [dbo].[tmpEmployeeCalendar_Insert] @fromDate DATE
	,@toDate DATE
	,@empIds VARCHAR(MAX) = NULL
	,@isEnable BIT = NULL
	,@isDelete BIT = NULL
AS
/*
Created: Tim.Trinh@2019-10-10
Task: get employee calendar
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	DECLARE @tempDate DATE = @fromDate
		,@GeneralLeaveId TINYINT = 0
		,@OffDayShiftId SMALLINT = - 1
		-- work type
		,@OffDay TINYINT = 1
		,@LeaveDay TINYINT = 2
		,@WorkDay TINYINT = 3
		-- change shift type
		,@ChangeShiftTypeId TINYINT = 1
		,@MoveOfDayTypeId TINYINT = 2
		-- request status
		,@NotVerifiedStatusId TINYINT = 3
		,@RejectedStatusId TINYINT = 5
		,@VoidStatusId TINYINT = 7
	DECLARE @tblEmployeeIds TABLE (EmpId INT)

	CREATE TABLE #tblEmployeeCalendar (
		EmpId INT
		,WorkDate DATE
		,ShiftId INT
		,IsOffDay BIT
		)

	CREATE TABLE #tblHalfDayRequest (
		RequestId BIGINT
		,RequestTypeId SMALLINT
		,EmployeeId INT
		,WorkDate DATE
		,FromTime DATETIME
		,ToTime DATETIME
		,LeaveTypeId INT
		,WorkType TINYINT
		,LeaveDay DECIMAL(2, 1)
		)

	IF ISNULL(@empIds, '') <> ''
	BEGIN
		INSERT INTO @tblEmployeeIds (EmpId)
		SELECT val
		FROM dbo.fnc_strSplit(@empIds, ',')
	END
	ELSE
	BEGIN
		INSERT INTO @tblEmployeeIds (EmpId)
		SELECT EmpId
		FROM dbo.Employee WITH (NOLOCK)
		WHERE (
				@isEnable IS NULL
				OR [Enabled] = @isEnable
				)
			AND (
				@isDelete IS NULL
				OR isDeleted = @isDelete
				)
	END

	INSERT INTO #tblEmployeeCalendar (
		EmpId
		,WorkDate
		,ShiftId
		,IsOffDay
		)
	SELECT empca.EmpId
		,WorkDate
		,ShiftId
		,IsOffDay
	FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
		INNER JOIN @tblEmployeeIds AS emp ON empca.EmpId = emp.EmpId
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate

	WHILE @tempDate <= @toDate
	BEGIN
		INSERT INTO #tblEmployeeCalendar (
			EmpId
			,WorkDate
			,ShiftId
			,IsOffDay
			)
		SELECT emp.EmpId
			,@tempDate
			,CASE 
				WHEN DATEPART(dw, @tempDate) = 1 THEN empshift.SunShift
				WHEN DATEPART(dw, @tempDate) = 2 THEN empshift.MonShift
				WHEN DATEPART(dw, @tempDate) = 3 THEN empshift.TueShift
				WHEN DATEPART(dw, @tempDate) = 4 THEN empshift.WedShift
				WHEN DATEPART(dw, @tempDate) = 5 THEN empshift.ThuShift
				WHEN DATEPART(dw, @tempDate) = 6 THEN empshift.FriShift
				WHEN DATEPART(dw, @tempDate) = 7 THEN empshift.SatShift
				END
			,CASE 
				WHEN DATEPART(dw, @tempDate) = 1
					AND ISNULL(empshift.SunShift, @OffDayShiftId) = @OffDayShiftId THEN 1
				WHEN DATEPART(dw, @tempDate) = 2
					AND ISNULL(empshift.MonShift, @OffDayShiftId) = @OffDayShiftId THEN 1
				WHEN DATEPART(dw, @tempDate) = 3
					AND ISNULL(empshift.TueShift, @OffDayShiftId) = @OffDayShiftId THEN 1
				WHEN DATEPART(dw, @tempDate) = 4
					AND ISNULL(empshift.WedShift, @OffDayShiftId) = @OffDayShiftId THEN 1
				WHEN DATEPART(dw, @tempDate) = 5
					AND ISNULL(empshift.ThuShift, @OffDayShiftId) = @OffDayShiftId THEN 1
				WHEN DATEPART(dw, @tempDate) = 6
					AND ISNULL(empshift.FriShift, @OffDayShiftId) = @OffDayShiftId THEN 1
				WHEN DATEPART(dw, @tempDate) = 7
					AND ISNULL(empshift.SatShift, @OffDayShiftId) = @OffDayShiftId THEN 1
				ELSE 0
				END
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN @tblEmployeeIds AS tblemp ON emp.EmpId = tblemp.EmpId
			INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
			AND empsum.IsDefaultSchedule = 1
			INNER JOIN dbo.EmployeeDefaultShift AS empshift WITH (NOLOCK) ON emp.EmpId = empshift.EmpId
		WHERE NOT EXISTS (
				SELECT 1
				FROM #tblEmployeeCalendar AS tbl
				WHERE tbl.EmpId = emp.EmpId
					AND tbl.WorkDate = @tempDate
				)

		INSERT INTO #tblEmployeeCalendar (
			EmpId
			,WorkDate
			,ShiftId
			,IsOffDay
			)
		SELECT emp.EmpId
			,@tempDate
			,empshift.ShiftId
			,0
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN @tblEmployeeIds AS tblemp ON emp.EmpId = tblemp.EmpId
			INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
			AND empsum.IsDefaultSchedule = 0
			OUTER APPLY (
			SELECT TOP 1 empshift.ShiftId
			FROM dbo.ScheduleArrangementDefaultShift AS empshift WITH (NOLOCK)
			WHERE empshift.EmployeeId = emp.EmpId
				AND empshift.ShiftId > 0
				AND empshift.FromDate <= @tempDate
			ORDER BY empshift.FromDate DESC
			) AS empshift
		WHERE NOT EXISTS (
				SELECT 1
				FROM #tblEmployeeCalendar AS tbl
				WHERE tbl.EmpId = emp.EmpId
					AND tbl.WorkDate = @tempDate
				)

		SET @tempDate = DATEADD(d, 1, @tempDate)
	END

	MERGE dbo.tmpEmployeeCalendar AS [target]
	USING #tblEmployeeCalendar AS [source] ON (
				[target].EmpId = [source].EmpId
				AND [target].WorkDate = [source].WorkDate
				)
	WHEN MATCHED THEN
			UPDATE
			SET [target].ShiftId = [source].ShiftId
				,[target].IsOffDay = [source].IsOffDay
	WHEN NOT MATCHED BY TARGET THEN
			INSERT (
				EmpId
				,WorkDate
				,ShiftId
				,IsOffDay
				)
			VALUES (
				[source].EmpId
				,[source].WorkDate
				,[source].ShiftId
				,[source].IsOffDay
				);

	UPDATE empca
	SET empca.IsHoliday = (CASE 
			WHEN (
					empsum.IsLocal = 1
					OR (
						empsum.IsLocal = 0
						AND empsum.IsExpatUseHoliday = 1
						)
					)
				AND holiday.Id IS NOT NULL THEN 1
			ELSE 0 END
			)
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empca.EmpId = empsum.EmpId
		LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON empca.WorkDate BETWEEN holiday.FromDate
			AND holiday.ToDate
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate

	-- overtime
	UPDATE empca
	SET empca.IsOvertime = (CASE 
			WHEN overtime.EmpId IS NOT NULL THEN 1
			ELSE 0 END
			)
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		LEFT JOIN dbo.OTDailyRequest AS overtime WITH (NOLOCK) ON empca.EmpId = overtime.EmpId
		AND empca.WorkDate = overtime.OvertimeDate
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate
		AND (
			(
				empca.IsOvertime = 1
				AND overtime.EmpId IS NULL
				)
			OR (
				empca.IsOvertime = 0
				AND overtime.EmpId IS NOT NULL
				)
			)

	-- keep in view
	UPDATE empca
	SET empca.RequestId = kiv.RequestId
		,empca.RequestTypeId = kiv.RequestType
		,empca.IsKeepInView = (CASE 
			WHEN kiv.RequestId IS NULL THEN 0
			ELSE 1 END
			)
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		OUTER APPLY (
		SELECT kiv.RequestId
			,r.RequestType
		FROM dbo.KeepInViewRequest AS kiv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS r WITH (NOLOCK) ON r.RequestId = kiv.RequestId
			AND empca.EmpId = kiv.EmpId
			AND empca.WorkDate = kiv.OnDate
			AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		) AS kiv
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate
		AND (
			(
				empca.IsKeepInView = 1
				AND kiv.RequestId IS NULL
				)
			OR (
				empca.IsKeepInView = 0
				AND kiv.RequestId IS NOT NULL
				)
			)

	-- change shift
	UPDATE empca
	SET empca.RequestId = cs.RequestId
		,empca.RequestTypeId = cs.RequestType
		,empca.ShiftId = (CASE 
			WHEN cs.ToShift = @OffDayShiftId THEN NULL
			ELSE cs.ToShift END
			)
		,empca.IsOffDay = (CASE 
			WHEN cs.ToShift = @OffDayShiftId THEN 1
			ELSE empca.IsOffDay END
			)
		,empca.ChangeShiftTypeId = cs.ChangeShiftTypeId
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		OUTER APPLY (
		SELECT r.RequestId
			,r.RequestType
			,@ChangeShiftTypeId AS ChangeShiftTypeId
			,csdt.ToShift
		FROM dbo.ChangeShiftRequest AS cs WITH (NOLOCK)
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
			AND empca.EmpId = cs.EmpId
			AND empca.WorkDate = csdt.OnDate
			AND cs.TypeId = @ChangeShiftTypeId
			INNER JOIN dbo.WorkingTimeRequest AS r WITH (NOLOCK) ON cs.RequestId = r.RequestId
			AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		) AS cs
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate
		AND (
			(
				empca.RequestId IS NOT NULL
				AND cs.RequestId IS NULL
				)
			OR (
				empca.RequestId IS NULL
				AND cs.RequestId IS NOT NULL
				)
			)

	UPDATE empca
	SET empca.RequestId = cs.RequestId
		,empca.RequestTypeId = cs.RequestType
		,empca.IsOffDay = CASE 
			WHEN cs.RequestId IS NOT NULL THEN 0
			ELSE empca.IsOffDay
			END
		,empca.ChangeShiftTypeId = cs.ChangeShiftTypeId
		,empca.MoveOffFromDate = NULL
		,empca.MoveOffToDate = cs.ToDate
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		OUTER APPLY (
		SELECT r.RequestId
			,r.RequestType
			,@MoveOfDayTypeId AS ChangeShiftTypeId
			,csdt.ToDate
		FROM dbo.ChangeShiftRequest AS cs WITH (NOLOCK)
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
			AND empca.EmpId = cs.EmpId
			AND empca.WorkDate = csdt.OnDate
			AND cs.TypeId = @MoveOfDayTypeId
			INNER JOIN dbo.WorkingTimeRequest AS r WITH (NOLOCK) ON cs.RequestId = r.RequestId
			AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		) AS cs
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate
		AND (
			(
				empca.RequestId IS NOT NULL
				AND cs.RequestId IS NULL
				)
			OR (
				empca.RequestId IS NULL
				AND cs.RequestId IS NOT NULL
				)
			)

	UPDATE empca
	SET empca.RequestId = cs.RequestId
		,empca.RequestTypeId = cs.RequestType
		,empca.ShiftId = CASE 
			WHEN cs.RequestId IS NOT NULL THEN NULL
			ELSE empca.ShiftId
			END
		,empca.IsOffDay = CASE 
			WHEN cs.RequestId IS NOT NULL THEN 1
			ELSE empca.IsOffDay
			END
		,empca.ChangeShiftTypeId = cs.ChangeShiftTypeId
		,empca.MoveOffFromDate = cs.OnDate
		,empca.MoveOffToDate = NULL
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		OUTER APPLY (
		SELECT r.RequestId
			,r.RequestType
			,@MoveOfDayTypeId AS ChangeShiftTypeId
			,csdt.OnDate
		FROM dbo.ChangeShiftRequest AS cs WITH (NOLOCK)
			INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON cs.RequestId = csdt.RequestId
			AND empca.EmpId = cs.EmpId
			AND empca.WorkDate = csdt.ToDate
			AND cs.TypeId = @MoveOfDayTypeId
			INNER JOIN dbo.WorkingTimeRequest AS r WITH (NOLOCK) ON cs.RequestId = r.RequestId
			AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		) AS cs
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate
		AND (
			(
				empca.RequestId IS NOT NULL
				AND cs.RequestId IS NULL
				)
			OR (
				empca.RequestId IS NULL
				AND cs.RequestId IS NOT NULL
				)
			)

	-- leave request
	UPDATE empca
	SET empca.RequestId = lr.RequestId
		,empca.RequestTypeId = lr.RequestType
		,empca.LeaveTypeId = lr.LeaveType
		,empca.IsLeaveDay = CASE 
			WHEN lr.RequestId IS NOT NULL THEN 1
			ELSE 0
			END
		,empca.IsLeaveOffDay = CASE 
			WHEN lr.WorkType = @OffDay THEN 1
			ELSE 0
			END
		,empca.LeaveDay = CASE 
			WHEN lr.RequestId IS NOT NULL THEN 1
			ELSE 0
			END
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		OUTER APPLY (
		SELECT r.RequestId
			,r.RequestType
			,lrdt.LeaveType
			,lrdt.WorkType
		FROM dbo.LeaveRequestDetail AS lrdt WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS r WITH (NOLOCK) ON lrdt.RequestId = r.RequestId
			AND empca.EmpId = r.EmpId
			AND lrdt.OnDate = empca.WorkDate
			AND lrdt.WorkType IN (@OffDay, @LeaveDay)
			AND lrdt.IsHalfDay = 0
			AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		) AS lr
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate
		AND (
			(
				empca.RequestId IS NOT NULL
				AND lr.RequestId IS NULL
				)
			OR (
				empca.RequestId IS NULL
				AND lr.RequestId IS NOT NULL
				)
			)

	UPDATE empca
	SET empca.RequestId = lr.RequestId
		,empca.RequestTypeId = lr.RequestType
		,empca.LeaveTypeId = lr.LeaveType
		,empca.IsLeaveDay = CASE 
			WHEN lr.RequestId IS NOT NULL THEN 1
			ELSE 0
			END
		,empca.IsLeaveOffDay = CASE 
			WHEN lr.RequestId IS NOT NULL THEN 1
			ELSE 0
			END
		,empca.LeaveDay = CASE 
			WHEN lr.RequestId IS NOT NULL THEN 1
			ELSE 0
			END
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		OUTER APPLY (
		SELECT r.RequestId
			,r.RequestType
			,lr.LeaveType
		FROM dbo.LeaveRequest AS lr WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS r WITH (NOLOCK) ON lr.RequestId = r.RequestId
			AND lr.LeaveType <> @GeneralLeaveId
			AND empca.EmpId = r.EmpId
			AND empca.WorkDate BETWEEN lr.FromDate
				AND lr.ToDate
			AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.LeaveRequestDetail AS lrdt WITH (NOLOCK)
				WHERE lr.RequestId = lrdt.RequestId
				)
		) AS lr
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate
		AND (
			(
				empca.RequestId IS NOT NULL
				AND lr.RequestId IS NULL
				)
			OR (
				empca.RequestId IS NULL
				AND lr.RequestId IS NOT NULL
				)
			)

	INSERT INTO #tblHalfDayRequest (
		RequestId
		,RequestTypeId
		,EmployeeId
		,WorkDate
		,FromTime
		,ToTime
		,LeaveTypeId
		,WorkType
		,LeaveDay
		)
	SELECT r.RequestId
		,r.RequestType
		,r.EmpId
		,lrdt.OnDate
		,lrdt.FromTime
		,lrdt.ToTime
		,lrdt.LeaveType
		,lrdt.WorkType
		,0.5 AS LeaveDay
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON r.EmpId = emp.EmpId
		INNER JOIN @tblEmployeeIds AS tblemp ON emp.EmpId = tblemp.EmpId
		INNER JOIN dbo.LeaveRequestDetail AS lrdt WITH (NOLOCK) ON r.RequestId = lrdt.RequestId
		AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		AND lrdt.IsHalfDay = 1
		AND lrdt.WorkType = @LeaveDay
		AND lrdt.OnDate BETWEEN @fromDate
			AND @toDate
		INNER JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON lrdt.LeaveType = lv.LeaveTypeId

	UPDATE empca
	SET empca.RequestId = rq.RequestId
		,empca.RequestTypeId = rq.RequestTypeId
		,empca.IsLeaveDay = 1
		,empca.LeaveDay = rq.LeaveDay
		,empca.LeaveTypeId = rq.LeaveTypeId
		,empca.FromTime = rq.FromTime
		,empca.ToTime = rq.ToTime
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblHalfDayRequest AS rq ON empca.EmpId = rq.EmployeeId
		AND empca.WorkDate = rq.WorkDate
	WHERE empca.LeaveTypeId IS NULL
		OR empca.LeaveDay = 0.5

	UPDATE empca
	SET empca.LeaveDay = empca.LeaveDay + rq.LeaveDay
		,empca.IsLeaveDay = 1
		,empca.LeaveTypeId2 = rq.LeaveTypeId
		,empca.FromTime2 = rq.FromTime
		,empca.ToTime2 = rq.ToTime
	FROM dbo.tmpEmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblHalfDayRequest AS rq ON empca.EmpId = rq.EmployeeId
		AND empca.WorkDate = rq.WorkDate
	WHERE empca.FromTime <> rq.FromTime
		AND empca.ToTime <> rq.ToTime

	DROP TABLE #tblEmployeeCalendar

	DROP TABLE #tblHalfDayRequest
END
