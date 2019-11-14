
CREATE PROCEDURE [dbo].[Overtime_GetMonthlyRequestDetailsById] @RequestId BIGINT
AS
/*
Created: Tim.Trinh@2019-07-08
Task: Get monthly overtime request by id.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Month DATE
		,@fromDate DATE
		,@toDate DATE
		,@EmployeeId INT

	SELECT @Month = [Month]
		,@EmployeeId = EmpId
	FROM dbo.OTMonthlyRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	SELECT @toDate = @Month
		,@fromDate = DATEADD(month, - 1, @Month)

	SELECT @fromDate = CONVERT(VARCHAR(5), YEAR(@fromDate)) + '/' + CONVERT(VARCHAR(2), MONTH(@fromDate)) + '/10'
		,@toDate = CONVERT(VARCHAR(5), YEAR(@toDate)) + '/' + CONVERT(VARCHAR(2), MONTH(@toDate)) + + '/09'

	EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @fromDate
		,@toDate = @toDate
		,@empIds = @EmployeeId

	SELECT ot.RequestId
		,ot.[Month]
		,ot.NormalHours
		,ot.OffHours
		,ot.PublicHours
		,ot.SpecialHours
		,ot.CurrentStatus AS StatusId
		,emp.EmpId AS EmployeeId
		,emp.NickName
		,emp.FullName
		,empsum.WorkingHours
		,empsum.SpecialOT
		,ot.VerifierId
		,verifier.NickName
		,verifier.FullName
		,ot.ApproverId
		,approver.NickName
		,approver.FullName
	FROM dbo.OTMonthlyRequest AS ot WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON ot.EmpId = emp.EmpId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		INNER JOIN dbo.Employee AS verifier WITH (NOLOCK) ON ot.VerifierId = verifier.EmpId
		INNER JOIN dbo.Employee AS approver WITH (NOLOCK) ON ot.ApproverId = approver.EmpId
	WHERE ot.RequestId = @RequestId

	SELECT empca.WorkDate
		,COALESCE(sc.ShiftId, scl.ShiftId, s.ShiftId) AS ShiftId
		,COALESCE(sc.Code, scl.Code, s.Code) AS ShiftCode
		,COALESCE(sc.TotalHours, scl.TotalHours, s.TotalHours) AS TotalShiftHours
		,CONVERT(VARCHAR(5), empatt.InFixed, 108) AS [TimeIn]
		,CONVERT(VARCHAR(5), empatt.OutFixed, 108) AS [TimeOut]
		,empatt.TotalWorkHours AS ActualWorkingHours
		,ot.NormalHours
		,ot.OffHours
		,ot.PublicHours
		,ot.SpecialHours
		,ot.[Remark]
		,empca.IsHoliday
		,empca.IsOffDay
		,empca.IsKeepInView
	FROM dbo.tmpEmployeeCalendar AS empca WITH (NOLOCK)
		LEFT JOIN dbo.OTDailyRequest AS ot WITH (NOLOCK) ON empca.WorkDate = ot.OvertimeDate
		AND ot.EmpId = empca.EmpId
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
		LEFT JOIN dbo.EmployeeAttendance AS empatt WITH (NOLOCK) ON empatt.AttendanceDate = empca.WorkDate
		AND empatt.EmpId = empca.EmpId
		LEFT JOIN dbo.[Shift] AS sc WITH (NOLOCK) ON sc.ShiftId = empatt.ShiftId
		LEFT JOIN dbo.[Shift] AS scl WITH (NOLOCK) ON scl.ShiftId = empca.ShiftId
	WHERE empca.EmpId = @employeeId
		AND empca.WorkDate BETWEEN @fromDate
			AND @toDate
	ORDER BY empca.WorkDate
END
