
CREATE PROCEDURE [dbo].[EmployeeAttendance_ForWaitingCase]
AS
BEGIN
	SET XACT_ABORT ON;

	DECLARE @i INT
		,@TempEmpId INT
		,@TempWorkDate DATE
		,@TempShiftId INT
		,@fromDate DATE
		,@toDate DATE
	DECLARE @tblEmployee TABLE (
		Id INT PRIMARY KEY IDENTITY(1, 1)
		,EmpId INT
		,WorkDate DATE
		)

	SELECT @fromDate = MIN(tmpAtt.WorkDate)
		,@toDate = MAX(tmpAtt.WorkDate)
	FROM dbo.tmpEmployeeAttendace AS tmpAtt WITH (NOLOCK)

	INSERT INTO @tblEmployee (
		EmpId
		,WorkDate
		)
	SELECT TOP 100 EmpId
		,WorkDate
	FROM dbo.tmpEmployeeAttendace AS tmpAtt WITH (NOLOCK)

	DELETE tmp
	FROM dbo.tmpEmployeeAttendace AS tmp
		INNER JOIN @tblEmployee AS e ON tmp.EmpId = e.EmpId
		AND tmp.WorkDate = e.WorkDate

	IF EXISTS (
			SELECT *
			FROM INFORMATION_SCHEMA.TABLES
			WHERE TABLE_NAME = '#TimeRecord'
			)
	BEGIN
		DROP TABLE #TimeRecord
	END

	CREATE TABLE #TimeRecord (
		Id INT
		,EmpId INT
		,RecordTime DATETIME
		,InOutType TINYINT
		)

	-- Get all in/out data into temp table to calculate
	INSERT INTO #TimeRecord (
		Id
		,EmpId
		,RecordTime
		,InOutType
		)
	SELECT Id
		,EmpId
		,RecordTime
		,InOutType
	FROM dbo.TimeRecord AS ti WITH (NOLOCK)
	WHERE RecordTime BETWEEN DATEADD(HH, - 2, CONVERT(VARCHAR(10), @fromDate, 111))
			AND DATEADD(d, 2, CONVERT(VARCHAR(10), @toDate, 111))
		AND EXISTS (
			SELECT 1
			FROM @tblEmployee
			WHERE EmpId = ti.EmpId
			)

	IF EXISTS (
			SELECT *
			FROM INFORMATION_SCHEMA.TABLES
			WHERE TABLE_NAME = '#LeaveRequestHalfDay'
			)
	BEGIN
		DROP TABLE #LeaveRequestHalfDay
	END

	CREATE TABLE #LeaveRequestHalfDay (
		EmpId INT
		,OnDate DATETIME
		,FromTime DATETIME
		,ToTime DATETIME
		)

	-- Get all leave request half day
	INSERT INTO #LeaveRequestHalfDay (
		EmpId
		,OnDate
		,FromTime
		,ToTime
		)
	SELECT lv.EmpId
		,lv.OnDate
		,FromTime
		,ToTime
	FROM LeaveRequestDetail lv
		INNER JOIN (
		SELECT ldt.empId
			,OnDate
		FROM LeaveRequestDetail AS ldt
			INNER JOIN dbo.WorkingTimeRequest AS wrq ON ldt.RequestId = wrq.RequestId
		WHERE WorkType = 2
			AND IsHalfDay = 1 -- 2: leave, 1: Half day
			AND DATEDIFF(DD, @fromDate, OnDate) >= 0
			AND DATEDIFF(DD, OnDate, DATEADD(DD, 2, @toDate)) >= 0
			AND EXISTS (
				SELECT 1
				FROM @tblEmployee
				WHERE EmpId = ldt.EmpId
				)
			AND wrq.CurrentStatus IN (4, 6)
		GROUP BY ldt.empId
			,OnDate
		HAVING COUNT(1) = 1
		) a ON (
			a.EmpId = lv.EmpId
			AND a.OnDate = lv.OnDate
			)
		INNER JOIN dbo.WorkingTimeRequest AS rq WITH (NOLOCK) ON lv.RequestId = rq.RequestId
	WHERE lv.IsHalfDay = 1
		AND EXISTS (
			SELECT 1
			FROM @tblEmployee
			WHERE EmpId = lv.EmpId
			)
		AND rq.CurrentStatus IN (4, 6)

	-- Get all calendars of employees into temp table
	IF EXISTS (
			SELECT *
			FROM INFORMATION_SCHEMA.TABLES
			WHERE TABLE_NAME = '#EmpCalendar'
			)
	BEGIN
		DROP TABLE #EmpCalendar
	END

	CREATE TABLE #EmpCalendar (
		Id INT PRIMARY KEY IDENTITY(1, 1)
		,EmpId INT
		,WorkDate DATETIME
		,ShiftId INT
		,IsHoliday BIT
		,IsOffDay BIT
		,IsLeaveDay BIT
		,LeaveTypeId INT NULL
		,IsKeepInView BIT
		,LateIn INT
		,EarlyOut INT
		,RequestedCheckInTime DATETIME
		,RequestedCheckOutTime DATETIME
		,BeginCheckInTime DATETIME
		,EndCheckInTime DATETIME
		,BeginCheckOutTime DATETIME
		,EndCheckOutTime DATETIME
		,BreakTimeMins INT
		,LeaveDay DECIMAL(18, 1)
		)

	INSERT INTO #EmpCalendar (
		EmpId
		,WorkDate
		,ShiftId
		,IsHoliday
		,IsOffDay
		,IsLeaveDay
		,LeaveTypeId
		,IsKeepInView
		,LateIn
		,EarlyOut
		,RequestedCheckInTime
		,RequestedCheckOutTime
		,BeginCheckInTime
		,EndCheckInTime
		,BeginCheckOutTime
		,EndCheckOutTime
		,BreakTimeMins
		,LeaveDay
		)
	SELECT e.EmpId
		,e.WorkDate
		,ec.ShiftId
		,IsHoliday
		,IsOffDay
		,IsLeaveDay
		,LeaveTypeId
		,IsKeepInView
		,LateIn
		,EarlyOut
		,RequestedCheckinTime
		,RequestedCheckoutTime
		,CASE 
			WHEN IsInAfterDay = 1 THEN DATEADD(D, 1, ec.WorkDate + ' ' + s.EndCheckInTime)
			ELSE CAST(ec.WorkDate + ' ' + s.EndCheckInTime AS DATETIME)
			END AS BeginCheckInTime
		,CASE 
			WHEN IsInAfterDay = 1 THEN DATEADD(d, 1, ec.WorkDate + ' ' + s.EndCheckInTime)
			ELSE CAST(ec.WorkDate + ' ' + s.EndCheckInTime AS DATETIME)
			END AS EndCheckInTime
		,CASE 
			WHEN IsOutAfterDay = 1 THEN DATEADD(d, 1, ec.WorkDate + ' ' + s.BeginCheckOutTime)
			ELSE CAST(ec.WorkDate + ' ' + s.BeginCheckOutTime AS DATETIME)
			END AS BeginCheckOutTime
		,CASE 
			WHEN IsOutAfterDay = 1 THEN DATEADD(d, 1, ec.WorkDate + ' ' + s.EndCheckOutTime)
			ELSE CAST(ec.WorkDate + ' ' + s.EndCheckOutTime AS DATETIME)
			END AS EndCheckOutTime
		,BreakTimeMins
		,LeaveDay
	FROM @tblEmployee AS e
		LEFT JOIN dbo.EmployeeCalendar AS ec WITH (NOLOCK) ON ec.EmpId = e.EmpId
		AND ec.WorkDate = e.WorkDate
		LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.ShiftId = ec.ShiftId
	ORDER BY e.EmpId
		,e.WorkDate

	IF EXISTS (
			SELECT 1
			FROM #EmpCalendar
			)
	BEGIN
		SET @i = 1

		WHILE EXISTS (
				SELECT 1
				FROM #EmpCalendar
				WHERE Id = @i
				)
		BEGIN
			SELECT @TempEmpId = EmpId
				,@TempWorkDate = WorkDate
				,@TempShiftId = ShiftId
			FROM #EmpCalendar
			WHERE Id = @i

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId @TempEmpId
				,@TempWorkDate
				,@TempWorkDate
				,1

			EXEC dbo.Discipline_Insert @TempWorkDate
				,@TempWorkDate
				,@TempEmpId

			SET @i = @i + 1
		END
	END

	DROP TABLE #TimeRecord

	DROP TABLE #EmpCalendar

	DROP TABLE #LeaveRequestHalfDay
END
