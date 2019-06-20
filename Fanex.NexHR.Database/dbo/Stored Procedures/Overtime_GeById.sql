/****** Object:  Stored Procedure dbo.Overtime_GeById    Script Date: 7/20/2016 10:19:14 AM ******/
/****** Object:  Stored Procedure dbo.Overtime_GeById    Script Date: 7/19/2016 5:22:18 PM ******/

CREATE PROCEDURE [dbo].[Overtime_GeById] @RequestId INT
AS
/*
Created: 20160616@Quinn.Vo
Task: Get Overtime request by overtime id.
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Month DATETIME
		,@fromDate DATETIME
		,@toDate DATETIME
		,@tempDate DATETIME
		,@EmpId INT

	SELECT @Month = [Month]
		,@EmpId = EmpId
	FROM dbo.OTMonthlyRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	SELECT @toDate = @Month
		,@fromDate = DATEADD(month, - 1, @Month)

	SELECT @fromDate = CONVERT(VARCHAR(10), YEAR(@fromDate)) + '/' + CONVERT(VARCHAR(10), MONTH(@fromDate)) + '/10'
		,@toDate = CONVERT(VARCHAR(10), YEAR(@toDate)) + '/' + CONVERT(VARCHAR(10), MONTH(@toDate)) + + '/09'

	IF OBJECT_ID('tempdb..#Calendar') IS NOT NULL
		DROP TABLE #Calendar

	CREATE TABLE #Calendar (
		WorkDate DATETIME
		,[Enabled] BIT
		,IsDeleted BIT
		)

	SELECT @tempDate = @fromDate

	WHILE (DATEDIFF(DD, @tempDate, @toDate) >= 0)
	BEGIN
		INSERT INTO #Calendar (
			WorkDate
			,[Enabled]
			,IsDeleted
			)
		VALUES (
			@tempDate
			,1
			,0
			)

		SET @tempDate = DATEADD(DD, 1, @tempDate)
	END

	SELECT ot.RequestId
		,ot.[Month]
		,ot.[NormalHours]
		,ot.[OffHours]
		,ot.[PublicHours]
		,ot.[CurrentStatus]
		,ot.[VerifiedDate]
		,ot.[ApprovedDate]
		,ot.[HRDate]
		,ot.[VerifiedStatus]
		,ot.[ApprovedStatus]
		,ot.[HRStatus]
		,e.[EmpId]
		,e.NickName
		,e.FullName
		,es.WorkingHours
		,ot.[VerifierId]
		,ve.NickName
		,ve.FullName
		,ot.[ApproverId]
		,ap.NickName
		,ap.FullName
		,ot.[HRId]
		,hr.NickName
		,hr.FullName
	FROM dbo.OTMonthlyRequest AS ot WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ot.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.Employee AS ve WITH (NOLOCK) ON ot.[VerifierId] = ve.EmpId
		INNER JOIN dbo.Employee AS ap WITH (NOLOCK) ON ot.[ApproverId] = ap.EmpId
		LEFT JOIN dbo.Employee AS hr WITH (NOLOCK) ON ot.[HRId] = hr.EmpId
	WHERE ot.RequestId = @RequestId

	SELECT tmp.WorkDate [OvertimeDate]
		,ISNULL(ot.[EmpId], @EmpID) AS [EmpId]
		,ot.[RequestId]
		,COALESCE(sc.ShiftId, scl.ShiftId, s.ShiftId) [ShiftId]
		,COALESCE(sc.TotalHours, scl.TotalHours, s.TotalHours) [TotalShiftHours]
		,COALESCE(sc.Code, scl.Code, s.Code) [Schedule]
		,CONVERT(VARCHAR(5), ec.InFixed, 108) [TimeIn]
		,CONVERT(VARCHAR(5), ec.OutFixed, 108) [TimeOut]
		,CASE 
			WHEN ec.TotalWorkHours IS NULL THEN NULL
			ELSE CONVERT(NVARCHAR(10), convert(INT, ec.TotalWorkHours)) + ':' + REPLICATE('0', 2 - LEN(CONVERT(NVARCHAR(10), convert(INT, round((ec.TotalWorkHours % 1) * 60, 0))))) + CONVERT(NVARCHAR(10), convert(INT, round((ec.TotalWorkHours % 1) * 60, 0)))
			END [TotalWorkHoursTime]
		,CASE 
			WHEN ec.TotalWorkHours IS NULL THEN NULL
			ELSE ec.TotalWorkHours
			END TotalWorkHours
		,ot.[NormalHours]
		,ot.[OffHours]
		,ot.[PublicHours]
		,ot.[Remark]
		,ot.[Status]
		,ot.[IsUpdated]
		,ecl.IsHoliday
		,ecl.IsOffDay
		,ecl.IsKeepInView
		,CASE 
			WHEN ecl.ChangeShiftId IS NULL THEN 0
			ELSE 1
			END IsChangeShift
		,esum.WorkingHours WorkingHoursContract
	FROM #Calendar tmp
		LEFT JOIN [dbo].[OTDailyRequest] ot WITH (NOLOCK) ON tmp.WorkDate = ot.OvertimeDate
		AND ot.EmpID = @EmpID
		LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
		LEFT JOIN dbo.EmployeeAttendance ec WITH (NOLOCK) ON ec.AttendanceDate = tmp.WorkDate
		AND ec.EmpId = @EmpID
		LEFT JOIN dbo.EmployeeCalendar ecl WITH (NOLOCK) ON ecl.WorkDate = tmp.WorkDate
		AND ecl.EmpId = @EmpId
		LEFT JOIN dbo.[Shift] sc WITH (NOLOCK) ON sc.ShiftId = ec.ShiftId
		LEFT JOIN dbo.[Shift] scl WITH (NOLOCK) ON scl.ShiftId = ecl.ShiftId
		LEFT JOIN EmployeeSummary esum WITH (NOLOCK) ON esum.empID = @EmpID
	ORDER BY tmp.WorkDate
END
