
CREATE PROCEDURE [dbo].[Overtime_GetMonthlyRequestDetailsByEmpId] @EmployeeId INT
	,@Month DATE
AS
/*
Created: Tim.Trinh@2019-07-02
Task: Get monthly overtime request details.
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromDate DATE = DATEADD(month, - 1, @Month)
		,@toDate DATE
		,@tempDate DATE

	SELECT @fromDate = CONVERT(VARCHAR(5), YEAR(@fromDate)) + '/' + CONVERT(VARCHAR(2), MONTH(@fromDate)) + '/10'
		,@toDate = CONVERT(VARCHAR(5), YEAR(@Month)) + '/' + CONVERT(VARCHAR(2), MONTH(@Month)) + + '/09'

	CREATE TABLE #tblDate (WorkDate DATE)

	SELECT @tempDate = @fromDate

	WHILE @tempDate <= @toDate
	BEGIN
		INSERT INTO #tblDate (WorkDate)
		VALUES (@tempDate)

		SET @tempDate = DATEADD(DD, 1, @tempDate)
	END

	SELECT TOP 1 ot.RequestId
		,@Month AS [Month]
		,ot.NormalHours
		,ot.OffHours
		,ot.PublicHours
		,ot.SpecialHours
		,ot.CurrentStatus AS StatusId
		,emp.[EmpId] AS EmployeeId
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
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		LEFT JOIN dbo.OTMonthlyRequest AS ot WITH (NOLOCK) ON ot.EmpId = emp.EmpId
		AND ot.[Month] = @Month
		LEFT JOIN dbo.Employee AS verifier WITH (NOLOCK) ON ot.VerifierId = verifier.EmpId
		LEFT JOIN dbo.Employee AS approver WITH (NOLOCK) ON ot.ApproverId = approver.EmpId
	WHERE emp.EmpId = @EmployeeId

	SELECT tmp.WorkDate
		,COALESCE(sc.ShiftId, scl.ShiftId, s.ShiftId) ShiftId
		,COALESCE(sc.Code, scl.Code, s.Code) AS ShiftCode
		,CONVERT(VARCHAR(5), empatt.InFixed, 108) AS [TimeIn]
		,CONVERT(VARCHAR(5), empatt.OutFixed, 108) [TimeOut]
		,empatt.TotalWorkHours AS ActualWorkingHours
		,ot.NormalHours
		,ot.OffHours
		,ot.PublicHours
		,ot.SpecialHours
		,ot.[Remark]
		,empca.IsHoliday
		,empca.IsOffDay
		,empca.IsKeepInView
	FROM #tblDate AS tmp
		LEFT JOIN [dbo].OTDailyRequest AS ot WITH (NOLOCK) ON tmp.WorkDate = ot.OvertimeDate
		AND ot.EmpId = @EmployeeId
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
		LEFT JOIN dbo.EmployeeAttendance AS empatt WITH (NOLOCK) ON empatt.AttendanceDate = tmp.WorkDate
		AND empatt.EmpId = @EmployeeId
		LEFT JOIN dbo.[Shift] AS sc WITH (NOLOCK) ON sc.ShiftId = empatt.ShiftId
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON empca.WorkDate = tmp.WorkDate
		AND empca.EmpId = @EmployeeId
		LEFT JOIN dbo.[Shift] AS scl WITH (NOLOCK) ON scl.ShiftId = empca.ShiftId
	ORDER BY tmp.WorkDate
END
