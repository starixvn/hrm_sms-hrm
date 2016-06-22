
CREATE PROCEDURE [dbo].[Overtime_GeByMonth] @EmpId INT
	,@Month DATETIME
AS
/*
Created: 20160614@Quinn.Vo
Task: Get Overtime request by month.
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromDate DATETIME
		,@toDate DATETIME
		,@tempDate DATETIME

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

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.OTDailyRequest WITH (NOLOCK)
				WHERE (
						OvertimeDate BETWEEN @fromDate
							AND @toDate
						)
					AND EmpID = @EmpID
				)
			)
	BEGIN
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
		WHERE ot.EmpId = @EmpId
			AND ot.[Month] = @Month

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
	ELSE
	BEGIN
		SELECT CAST (0 AS BIGINT) RequestId
			, @Month [Month]
			,CAST(0 AS DECIMAL(18, 2)) AS NormalHours
			,CAST(0 AS DECIMAL(18, 2)) AS OffHours
			,CAST(0 AS DECIMAL(18, 2)) AS PublicHours
			,CAST(0 AS TINYINT) AS CurrentStatus
			,CAST(NULL AS DATETIME) VerifiedDate
			,CAST(NULL AS DATETIME) ApprovedDate
			,CAST(NULL AS DATETIME) HRDate
			,CAST(0 AS TINYINT) AS VerifiedStatus
			,CAST(0 AS TINYINT) AS ApprovedStatus
			,CAST(0 AS TINYINT) AS HRStatus
			,e.[EmpId]
			,e.NickName
			,e.FullName
			,es.WorkingHours
			,CAST(0 AS INT) AS VerifierId
			,'' AS NickName
			,'' AS FullName
			,CAST(0 AS INT) AS ApproverId
			,'' AS NickName
			,'' AS FullName
			,CAST(0 AS INT) AS HRId
			,'' AS NickName
			,'' AS FullName
		FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		WHERE e.EmpId = @EmpId

		SELECT tmp.WorkDate [OvertimeDate]
			,@EmpID AS [EmpId]
			,CAST (0 AS BIGINT) [RequestId]
			,s.ShiftId
			,s.TotalHours [TotalShiftHours]
			,s.Code [Schedule]
			,CASE 
				WHEN ec.InFixed IS NULL THEN NULL
				ELSE CONVERT(VARCHAR(5), ec.InFixed, 108)
				END [TimeIn]
			,CASE 
				WHEN ec.OutFixed IS NULL THEN NULL
				ELSE CONVERT(VARCHAR(5), ec.OutFixed, 108)
				END [TimeOut]
			,CASE 
				WHEN ec.TotalWorkHours IS NULL THEN NULL
				ELSE CONVERT(NVARCHAR(10), convert(INT, ec.TotalWorkHours)) + ':' + REPLICATE('0', 2 - LEN(CONVERT(NVARCHAR(10), convert(INT, round((ec.TotalWorkHours % 1) * 60, 0))))) + CONVERT(NVARCHAR(10), convert(INT, round((ec.TotalWorkHours % 1) * 60, 0)))
				END [TotalWorkHoursTime]
			,CASE 
				WHEN ec.TotalWorkHours IS NULL THEN NULL
				ELSE ec.TotalWorkHours
				END TotalWorkHours
			,NULL NormalHours
			,NULL OffHours
			,NULL PublicHours
			,NULL Remark
			,1 STATUS
			,ecl.IsHoliday
			,ecl.IsOffDay
			,ecl.IsKeepInView
			,CASE 
				WHEN ecl.ChangeShiftId IS NULL THEN 0
				ELSE 1
				END IsChangeShift
			,esum.WorkingHours WorkingHoursContract
		FROM dbo.Employee e WITH (NOLOCK)
			INNER JOIN #Calendar tmp ON e.[Enabled] = tmp.[Enabled]
			AND e.[IsDeleted] = tmp.[IsDeleted]
			LEFT JOIN dbo.EmployeeAttendance ec WITH (NOLOCK) ON ec.AttendanceDate = tmp.WorkDate
			AND e.EmpId = ec.EmpId
			LEFT JOIN dbo.EmployeeCalendar ecl WITH (NOLOCK) ON ecl.WorkDate = tmp.WorkDate
			AND ecl.EmpId = ec.EmpId
			LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ShiftId = ec.ShiftId
			LEFT JOIN EmployeeSummary esum WITH (NOLOCK) ON esum.empID = @EmpID
		WHERE e.EmpId = @EmpId
		ORDER BY tmp.WorkDate
	END
END
