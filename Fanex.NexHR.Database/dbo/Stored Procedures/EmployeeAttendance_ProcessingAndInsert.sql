
CREATE PROCEDURE [dbo].[EmployeeAttendance_ProcessingAndInsert] --null, '2015-12-09', '2015-12-09'
	@empId INT = NULL, 
	@fromTime DATETIME , 
	@toTime DATETIME ,
	@onlyUpdate BIT = 0
AS
BEGIN
DECLARE @i INT, @TempEmpId INT
DECLARE @Employee TABLE (Id INT PRIMARY KEY IDENTITY(1,1), EmpId INT)

EXEC [dbo].[EmployeeAttendance_ProcessingAndInsert_ForWorkDay] @empId, @fromTime, @toTime

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '#TimeRecord'))
BEGIN
    DROP TABLE #TimeRecord
END
CREATE TABLE #TimeRecord (Id INT, EmpID INT, RecordTime DATETIME, InOutType TINYINT)

-- Get all in/out data into temp table to calculate
INSERT INTO #TimeRecord	(Id, EmpID, RecordTime, InOutType)
SELECT Id, EmpID, RecordTime, InOutType 
FROM dbo.TimeRecord WITH (NOLOCK) 
WHERE RecordTime BETWEEN CONVERT(VARCHAR(10), DATEADD(HH, -2, @fromTime), 111) 
	AND CONVERT(VARCHAR(10), DATEADD(d, 2, @toTime), 111)
	AND (@empId IS NULL OR EmpID = @empId)

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '#LeaveRequestHalfDay'))
BEGIN
    DROP TABLE #LeaveRequestHalfDay
END
CREATE TABLE #LeaveRequestHalfDay (EmpID INT, OnDate DATETIME, FromTime DATETIME, ToTime DATETIME)

-- Get all leave request half day
INSERT INTO #LeaveRequestHalfDay (EmpID, OnDate, FromTime, ToTime)
SELECT lv.EmpId, lv.OnDate, FromTime, ToTime FROM LeaveRequestDetail lv INNER JOIN
(
	SELECT ldt.empId, OnDate FROM LeaveRequestDetail AS ldt 
	INNER JOIN dbo.WorkingTimeRequest AS wrq ON ldt.RequestId = wrq.RequestId
		WHERE WorkType = 2 AND IsHalfDay = 1 -- 2: leave, 1: Half day
		AND OnDate BETWEEN CONVERT(VARCHAR(10), @fromTime, 111) 
		AND CONVERT(VARCHAR(10), DATEADD(d, 2, @toTime), 111)
		AND (@empId IS NULL OR ldt.EmpID = @empId)
		AND wrq.CurrentStatus IN (4,6)
	GROUP BY ldt.empId, OnDate
	HAVING COUNT(*) = 1
) a ON (a.EmpId = lv.EmpId and a.OnDate = lv.OnDate)
INNER JOIN dbo.WorkingTimeRequest AS rq WITH (NOLOCK) ON lv.RequestId = rq.RequestId
WHERE lv.IsHalfDay = 1 AND (@empId IS NULL OR lv.EmpID = @empId)
AND rq.CurrentStatus IN (4, 6)
--UNION ALL
--SELECT EmpId, CONVERT(VARCHAR(10), FromDate, 111), FromDate, ToDate 
--FROM LeaveRequest 
--WHERE LeaveType != 0 AND TotalDays = 0.5 AND (@empId IS NULL OR EmpID = @empId)
--	AND CONVERT(VARCHAR(10), FromDate, 111) BETWEEN CONVERT(VARCHAR(10), @fromTime, 111) AND CONVERT(VARCHAR(10), DATEADD(d, 2, @toTime), 111)


-- Get all calendars of employees into temp table
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '#EmpCalendar'))
BEGIN
    DROP TABLE #EmpCalendar
END
CREATE TABLE #EmpCalendar (
		Id INT PRIMARY KEY IDENTITY(1,1), 
		EmpId INT,
		WorkDate DATETIME ,
		ShiftId INT,
		IsHoliday BIT,
		IsOffDay BIT,
		IsLeaveDay BIT,
		LeaveTypeId INT NULL,
		IsKeepInView BIT,
		LateIn INT,
		EarlyOut INT,
		RequestedCheckInTime DATETIME ,
		RequestedCheckOutTime DATETIME ,
		BeginCheckInTime DATETIME,
		EndCheckInTime DATETIME,
		BeginCheckOutTime DATETIME,
		EndCheckOutTime DATETIME,
		BreakTimeMins INT,
		LeaveDay DECIMAL(18,1))

INSERT INTO #EmpCalendar (
		EmpId,
		WorkDate,
		ShiftId,
		IsHoliday,
		IsOffDay,
		IsLeaveDay,
		LeaveTypeId,
		IsKeepInView,
		LateIn,
		EarlyOut,		
		RequestedCheckInTime,
		RequestedCheckOutTime,
		BeginCheckInTime,
		EndCheckInTime,
		BeginCheckOutTime,
		EndCheckOutTime,
		BreakTimeMins,
		LeaveDay)
SELECT 	
		EmpId,
		WorkDate,
		ec.ShiftId,
		IsHoliday,
		IsOffDay,
		IsLeaveDay,
		LeaveTypeId,
		IsKeepInView,
		LateIn,
		EarlyOut,		
		RequestedCheckinTime,
		RequestedCheckoutTime,
		CASE WHEN IsInAfterDay = 1 -- if the range is cross-date
			THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
			ELSE CAST(WorkDate + ' ' + s.EndCheckInTime AS DATETIME)
			END AS [BeginCheckInTime],
		CASE WHEN IsInAfterDay = 1 -- if the range is cross-date
			THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckInTime)
			ELSE CAST(WorkDate + ' ' + s.EndCheckInTime AS DATETIME)
			END AS [EndCheckInTime],

		CASE WHEN IsOutAfterDay = 1 -- if the range is cross-date
			THEN DATEADD(d, 1, WorkDate + ' ' + s.BeginCheckOutTime )
			ELSE CAST(WorkDate + ' ' + s.BeginCheckOutTime AS DATETIME)
			END AS [BeginCheckOutTime],

		CASE WHEN IsOutAfterDay = 1 -- if the range is cross-date
			THEN DATEADD(d, 1, WorkDate + ' ' + s.EndCheckOutTime )
			ELSE CAST(WorkDate + ' ' + s.EndCheckOutTime AS DATETIME)
			END AS [EndCheckOutTime],
		BreakTimeMins,
		LeaveDay
FROM dbo.EmployeeCalendar ec WITH (NOLOCK)
LEFT JOIN dbo.Shift s WITH (NOLOCK)ON s.ShiftID = ec.ShiftId
WHERE (@EmpId IS NULL OR ec.EmpId = @EmpId)
AND ec.WorkDate BETWEEN @FromTime AND @ToTime
AND (ec.ShiftId IS NULL OR ISNULL(ec.LeaveDay, 0) <> 0)
ORDER BY ec.EmpId, ec.WorkDate

IF @empId IS NULL
BEGIN
	-- Get all enable staff to calculate 
	INSERT INTO @Employee (EmpId)
	SELECT e.EmpId FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE isDeleted IS NULL OR isDeleted = 0
	AND NOT EXISTS (SELECT 1 FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
					INNER JOIN dbo.[Shift] AS sh WITH (NOLOCK) ON cl.ShiftId = sh.ShiftID
				WHERE ISNULL(cl.LeaveDay, 0) = 0
					AND DATEDIFF(DD, @fromTime, WorkDate) >= 0
					AND DATEDIFF(DD, WorkDate, @toTime) >= 0
					AND (
						@empId IS NULL
						OR cl.EmpId = @empId
						)
						AND e.EmpId = cl.EmpId
						)

	SET @i = 1
	WHILE EXISTS (SELECT TOP 1 1 FROM @Employee WHERE Id = @i)
	BEGIN
		SELECT @TempEmpId = EmpId FROM @Employee WHERE Id = @i
		EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId @TempEmpId, @fromTime, @toTime, @onlyUpdate
		SET @i = @i + 1
	END
	
END
ELSE
BEGIN
	IF(NOT EXISTS (SELECT 1 FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
					INNER JOIN dbo.[Shift] AS sh WITH (NOLOCK) ON cl.ShiftId = sh.ShiftID
				WHERE ISNULL(cl.LeaveDay, 0) = 0
					AND DATEDIFF(DD, @fromTime, WorkDate) >= 0
					AND DATEDIFF(DD, WorkDate, @toTime) >= 0
					AND (
						@empId IS NULL
						OR cl.EmpId = @empId
						)
						))
	BEGIN 
		EXEC dbo.EmployeeAttendance_ProcessingAndInsert_ForEmpId @EmpId, @fromTime, @toTime, @onlyUpdate
	END
END

DROP TABLE #TimeRecord
DROP TABLE #EmpCalendar
DROP TABLE #LeaveRequestHalfDay
END