
CREATE PROCEDURE [dbo].[ReminderAttendance_GetMissingCheckinOut]
AS

/*
Created: 20151115@Quinn.Vo
Task: get missing checkin today and checkout yesterday to remind via email
DB: HRM
Original: 

Revisions:
Param's Explanation:

*/

BEGIN
CREATE TABLE #EmployeeAttendance(
	EmpId INT
	,WorkDate DATETIME
	,ShiftId INT
	,MissingType TINYINT
	,CheckIn DATETIME
	,CheckOut DATETIME)

DECLARE @today DATETIME, @yesterday DATETIME, @now DATETIME
SET @now = DATEADD(hh,7, GETUTCDATE())
SET @today = CONVERT(VARCHAR(10), @now, 111) --use UTC date because the server time is GMT -4, not GMT+7
SELECT @yesterday = DATEADD(d, -1, @today)

-- check to make sure checkin/out data has been impoted from Bioadmin
IF (EXISTS(SELECT 1 FROM dbo.TimeRecord WITH (NOLOCK) WHERE DATEDIFF(DD, RecordTime, @today) = 0))
BEGIN
	INSERT INTO #EmployeeAttendance(EmpId, WorkDate, ShiftId, MissingType, CheckOut)
	SELECT a.EmpId, AttendanceDate, sh.ShiftId, 1, a.OutFixed
	FROM dbo.EmployeeAttendance a WITH (NOLOCK) 
	 INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID= a.EmpID AND c.WorkDate = AttendanceDate
	INNER JOIN dbo.[Shift] sh WITH (NOLOCK) ON a.ShiftId = sh.ShiftID
	WHERE 
		(
			(DATEDIFF(DD, AttendanceDate, @yesterday) = 0 AND sh.IsInAfterDay = 0)
			OR (DATEDIFF(DD, AttendanceDate, @yesterday) = 1 AND sh.IsInAfterDay = 1)
		)
		AND InFixed IS NULL
		AND OutFixed IS NOT NULL
		AND (ISNULL(IsLeaveDay, 0) = 0 OR (IsLeaveDay = 1 AND a.LeaveDay = 0.5))
		AND NOT EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
						INNER JOIN dbo.InoutMissRequest inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
						WHERE r.empid = a.empid
							AND r.RequestType = 1
							AND DATEDIFF(DD, inout.OnDate, AttendanceDate) = 0
							AND r.CurrentStatus IN (1, 2, 4, 6)
				)
		AND NOT EXISTS (
			SELECT 1 
			FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest lv WITH (NOLOCK) ON r.RequestId = lv.RequestId
			WHERE r.empid=a.empid 
				AND r.RequestType = 6 
				AND lv.TotalDays >= 1 
				AND DATEDIFF(DD, lv.FromDate, AttendanceDate) >= 0
				AND DATEDIFF(DD, AttendanceDate, lv.ToDate) >= 0
				AND r.CurrentStatus IN (1, 2, 4, 6)
		)

	INSERT INTO #EmployeeAttendance(EmpId, WorkDate, ShiftId, MissingType, CheckIn)
	SELECT a.EmpId, AttendanceDate, sh.ShiftId, 2, a.InFixed
	FROM dbo.EmployeeAttendance a WITH (NOLOCK) 
	INNER JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empID= a.EmpID AND c.WorkDate = AttendanceDate
	INNER JOIN dbo.[Shift] sh WITH (NOLOCK) ON a.ShiftId = sh.ShiftID
	WHERE 
		(
			(DATEDIFF(DD, AttendanceDate, @yesterday) = 0 AND sh.IsInAfterDay = 0)
			OR (DATEDIFF(DD, AttendanceDate, @yesterday) = 1 AND sh.IsInAfterDay = 1)
		)
		AND OutFixed IS NULL
		AND InFixed IS NOT NULL
		AND (ISNULL(IsLeaveDay, 0) = 0 OR (IsLeaveDay = 1 AND a.LeaveDay = 0.5))
		AND NOT EXISTS (
						SELECT 1
						FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
						INNER JOIN dbo.InoutMissRequest inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
						WHERE r.empid = a.empid
							AND r.RequestType = 2
							AND DATEDIFF(DD, inout.OnDate, AttendanceDate) = 0
							AND r.CurrentStatus IN (1, 2, 4, 6)
				)
		AND NOT EXISTS (
			SELECT 1 
			FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest lv WITH (NOLOCK) ON r.RequestId = lv.RequestId
			WHERE r.empid=a.empid 
				AND r.RequestType = 6 
				AND lv.TotalDays >= 1 
				AND DATEDIFF(DD, lv.FromDate, AttendanceDate) >= 0
				AND DATEDIFF(DD, AttendanceDate, lv.ToDate) >= 0
				AND r.CurrentStatus IN (1, 2, 4, 6)
		)
END

SELECT
		a.EmpId,
		FullName,
		NickName,
		g.Name [Team], 
		g.Code [TeamCode],
		a.WorkDate,
		s.Code ShiftCode,
		e.Email,
		a.MissingType AS CheckinOutType,
		a.CheckIn,
		a.CheckOut
FROM #EmployeeAttendance a
INNER JOIN Employee e WITH (NOLOCK) ON a.Empid = e.Empid
INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON e.groupid = g.groupid
LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ShiftID = a.ShiftId
WHERE ISNULL(es.IsTopManager , 0) = 0
ORDER BY a.WorkDate DESC, e.NickName

DROP TABLE #EmployeeAttendance
END