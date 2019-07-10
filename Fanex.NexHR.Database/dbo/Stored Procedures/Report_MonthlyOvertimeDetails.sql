
CREATE PROCEDURE [dbo].[Report_MonthlyOvertimeDetails] @EmployeeId INT
	,@FromDate DATE
	,@ToDate DATE
AS
/*
Created: Tim.Trinh@2019-07-02
Task: Get monthly overtime details.
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @tempDate DATE = @fromDate

	CREATE TABLE #tblDate (WorkDate DATE)

	WHILE @tempDate <= @toDate
	BEGIN
		INSERT INTO #tblDate (WorkDate)
		VALUES (@tempDate)

		SET @tempDate = DATEADD(DAY, 1, @tempDate)
	END

	SELECT emp.[EmpId] AS EmployeeId
		,emp.NickName
		,emp.FullName
		,tmp.WorkDate
		,COALESCE(sc.Code, scl.Code, s.Code) AS ShiftCode
		,CONVERT(VARCHAR(5), empatt.InFixed, 108) AS [TimeIn]
		,CONVERT(VARCHAR(5), empatt.OutFixed, 108) [TimeOut]
		,empatt.TotalWorkHours AS ActualWorkingHours
		,ot.NormalHours
		,ot.OffHours
		,ot.PublicHours
		,ot.[Remark]
		,empca.IsHoliday
		,empca.IsOffDay
		,ot.[Status] AS StatusId
	FROM #tblDate AS tmp
		CROSS JOIN dbo.Employee AS emp WITH (NOLOCK)
		LEFT JOIN [dbo].OTDailyRequest AS ot WITH (NOLOCK) ON tmp.WorkDate = ot.OvertimeDate
		AND ot.EmpId = emp.EmpId
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
		LEFT JOIN dbo.EmployeeAttendance AS empatt WITH (NOLOCK) ON empatt.AttendanceDate = tmp.WorkDate
		AND empatt.EmpId = emp.EmpId
		LEFT JOIN dbo.[Shift] AS sc WITH (NOLOCK) ON sc.ShiftId = empatt.ShiftId
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON empca.WorkDate = tmp.WorkDate
		AND empca.EmpId = emp.EmpId
		LEFT JOIN dbo.[Shift] AS scl WITH (NOLOCK) ON scl.ShiftId = empca.ShiftId
	WHERE emp.EmpId = @EmployeeId
	ORDER BY tmp.WorkDate
END
