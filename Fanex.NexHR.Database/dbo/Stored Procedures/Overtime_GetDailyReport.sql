/****** Object:  Stored Procedure dbo.Overtime_GetDailyReport    Script Date: 7/20/2016 10:19:14 AM ******/
/****** Object:  Stored Procedure dbo.Overtime_GetDailyReport    Script Date: 7/19/2016 5:22:18 PM ******/

CREATE PROCEDURE [dbo].[Overtime_GetDailyReport] @EmpID INT
	,@Month DATETIME
	,@IsNormal BIT = 0
AS
/*
Created: 20160614@Quinn.Vo
Task: Get overtime daily report.
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

	IF OBJECT_ID('tempdb..#Calendar') IS NOT NULL
		DROP TABLE #Calendar

	CREATE TABLE #Calendar (WorkDate DATETIME)

	IF (@IsNormal = 0)
	BEGIN
		SELECT @toDate = @Month
			,@fromDate = DATEADD(month, - 1, @Month)

		SELECT @fromDate = CONVERT(VARCHAR(10), YEAR(@fromDate)) + '/' + CONVERT(VARCHAR(10), MONTH(@fromDate)) + '/10 00:00:00'
			,@toDate = CONVERT(VARCHAR(10), YEAR(@toDate)) + '/' + CONVERT(VARCHAR(10), MONTH(@toDate)) + + '/09 00:00:00'

		SELECT @tempDate = @fromDate

		WHILE (DATEDIFF(DD, @tempDate, @toDate) >= 0)
		BEGIN
			INSERT INTO #Calendar (WorkDate)
			VALUES (@tempDate)

			SET @tempDate = DATEADD(DD, 1, @tempDate)
		END
	END
	ELSE
	BEGIN
		SELECT @fromDate = CONVERT(VARCHAR(10), YEAR(@Month)) + '/' + CONVERT(VARCHAR(10), MONTH(@Month)) + '/01 00:00:00'

		SELECT @toDate = DATEADD(DAY, - 1, DATEADD(Month, 1, @fromDate))

		SELECT @tempDate = @fromDate

		WHILE (DATEDIFF(DD, @tempDate, @toDate) >= 0)
		BEGIN
			INSERT INTO #Calendar (WorkDate)
			VALUES (@tempDate)

			SET @tempDate = DATEADD(DD, 1, @tempDate)
		END
	END

	SELECT CAST(0 AS BIGINT) RequestId
		,@Month [Month]
		,CAST(0 AS DECIMAL(18, 2)) AS NormalHours
		,CAST(0 AS DECIMAL(18, 2)) AS OffHours
		,CAST(0 AS DECIMAL(18, 2)) AS PublicHours
		,CAST(0 AS TINYINT) AS CurrentStatus
		,CAST(NULL AS DATETIME) AS VerifiedDate
		,CAST(NULL AS DATETIME) AS ApprovedDate
		,CAST(NULL AS DATETIME) AS HRDate
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

	SELECT tmp.WorkDate AS [OvertimeDate]
		,ISNULL(ot.[EmpId], @EmpID) AS [EmpId]
		,ot.[RequestId]
		,ISNULL(s.ShiftId, sc.ShiftId) AS [ShiftId]
		,ISNULL(s.TotalHours, sc.TotalHours) AS [TotalShiftHours]
		,ISNULL(s.Code, sc.Code) AS [Schedule]
		,(CASE 
			WHEN ot.[TimeIn] IS NULL
				OR s.ShiftId IS NULL THEN CASE 
					WHEN ec.InFixed IS NULL
						OR sc.ShiftId IS NULL THEN NULL
					ELSE CONVERT(VARCHAR(5), ec.InFixed, 108)
					END
			ELSE CONVERT(VARCHAR(5), ot.[TimeIn], 108) END
			) AS [TimeIn]
		,(CASE 
			WHEN ot.[TimeOut] IS NULL
				OR s.ShiftId IS NULL THEN CASE 
					WHEN ec.OutFixed IS NULL
						OR sc.ShiftId IS NULL THEN NULL
					ELSE CONVERT(VARCHAR(5), ec.OutFixed, 108)
					END
			ELSE CONVERT(VARCHAR(5), ot.[TimeOut], 108) END
			) AS [TimeOut]
		,ISNULL(ot.[TotalHours], CASE 
				WHEN sc.ShiftId IS NULL THEN NULL
				ELSE ec.TotalWorkHours
				END) AS [TotalWorkHours]
		,ot.[NormalHours]
		,ot.[OffHours]
		,ot.[PublicHours]
		,ot.[Remark]
		,ot.[Status]
		,ot.[IsUpdated]
		,ecl.IsHoliday
		,ecl.IsOffDay
		,ecl.IsKeepInView
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #Calendar AS tmp ON 1 = 1
		LEFT JOIN [dbo].[OTDailyRequest] AS ot WITH (NOLOCK) ON tmp.WorkDate = ot.OvertimeDate
		AND ot.EmpID = @EmpID
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
		LEFT JOIN dbo.EmployeeAttendance AS ec WITH (NOLOCK) ON ec.AttendanceDate = tmp.WorkDate
		AND ec.EmpId = @EmpID
		LEFT JOIN dbo.EmployeeCalendar AS ecl WITH (NOLOCK) ON ecl.WorkDate = tmp.WorkDate
		AND ecl.EmpId = @EmpId
		LEFT JOIN dbo.[Shift] AS sc WITH (NOLOCK) ON sc.ShiftId = ec.ShiftId
	WHERE e.EmpId = @EmpId
	ORDER BY tmp.WorkDate

	DROP TABLE #Calendar
END