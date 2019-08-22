
CREATE PROCEDURE [dbo].[Overtime_GetDetailsByRequestIds] @RequestIds VARCHAR(MAX) = ''
AS
/*
Created: Tim.Trinh@2019-07-11
Task: get details by requestids
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblRequestIds (RequestId BIGINT)

	INSERT INTO #tblRequestIds (RequestId)
	SELECT val
	FROM dbo.fnc_strSplit(@RequestIds, ',')

	SELECT ot.RequestId
		,ot.OvertimeDate AS WorkDate
		,emp.EmpId AS EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,s.ShiftId
		,s.Code AS ShiftCode
		,s.TotalHours AS TotalShiftHours
		,CONVERT(VARCHAR(5), empatt.InFixed, 108) TimeIn
		,CONVERT(VARCHAR(5), empatt.OutFixed, 108) [TimeOut]
		,ot.TotalHours AS ActualWorkingHours
		,ot.NormalHours
		,ot.OffHours
		,ot.PublicHours
		,ot.SpecialHours
		,ot.Remark
		,ot.[Status] AS StatusId
		,empca.IsHoliday
		,empca.IsOffDay
		,empca.IsKeepInView
	FROM dbo.OTDailyRequest AS ot WITH (NOLOCK)
		INNER JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = ot.EmpId
		INNER JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON empca.WorkDate = ot.OvertimeDate
		AND empca.EmpId = ot.EmpId
		LEFT JOIN dbo.EmployeeAttendance AS empatt WITH (NOLOCK) ON empatt.AttendanceDate = ot.OvertimeDate
		AND empatt.EmpId = ot.EmpId
	WHERE ot.RequestID IN (
			SELECT RequestID
			FROM #tblRequestIds
			)
	ORDER BY emp.NickName
		,ot.OvertimeDate

	DROP TABLE #tblRequestIds
END
