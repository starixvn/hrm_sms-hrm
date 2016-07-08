
CREATE PROCEDURE [dbo].[Overtime_GetDailyReport] --@SupervisorID INT
	@EmpID INT
	,@Month DATETIME
	--,@role VARCHAR(max) = '6'
	,@IsNormal BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
	,@isDebug BIT = 0
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

	CREATE TABLE #Calendar (
		WorkDate DATETIME
		,[Enabled] BIT
		,IsDeleted BIT
		)

	IF (@IsNormal = 0)
	BEGIN
		SELECT @toDate = @Month
			,@fromDate = DATEADD(month, - 1, @Month)

		SELECT @fromDate = CONVERT(VARCHAR(10), YEAR(@fromDate)) + '/' + CONVERT(VARCHAR(10), MONTH(@fromDate)) + '/10 00:00:00'
			,@toDate = CONVERT(VARCHAR(10), YEAR(@toDate)) + '/' + CONVERT(VARCHAR(10), MONTH(@toDate)) + + '/09 00:00:00'

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
	END
	ELSE
	BEGIN
		SELECT @fromDate = CONVERT(VARCHAR(10), YEAR(@Month)) + '/' + CONVERT(VARCHAR(10), MONTH(@Month)) + '/01 00:00:00'

		SELECT @toDate = DATEADD(DAY, - 1, DATEADD(Month, 1, @fromDate))

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
	END

	SELECT CAST(0 AS BIGINT) RequestId
		,@Month [Month]
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
		,ISNULL(ot.[EmpId], @EmpID) AS [EmpId]
		,ot.[RequestId]
		,ISNULL(s.ShiftId, sc.ShiftId) [ShiftId]
		,ISNULL(s.TotalHours, sc.TotalHours) [TotalShiftHours]
		,ISNULL(s.Code, sc.Code) [Schedule]
		,CASE 
			WHEN ot.[TimeIn] IS NULL
				OR s.ShiftId IS NULL THEN CASE 
					WHEN ec.InFixed IS NULL
						OR sc.ShiftId IS NULL THEN NULL
					ELSE CONVERT(VARCHAR(5), ec.InFixed, 108)
					END
			ELSE CONVERT(VARCHAR(5), ot.[TimeIn], 108)
			END [TimeIn]
		,CASE 
			WHEN ot.[TimeOut] IS NULL
				OR s.ShiftId IS NULL THEN CASE 
					WHEN ec.OutFixed IS NULL
						OR sc.ShiftId IS NULL THEN NULL
					ELSE CONVERT(VARCHAR(5), ec.OutFixed, 108)
					END
			ELSE CONVERT(VARCHAR(5), ot.[TimeOut], 108)
			END [TimeOut]
		,ISNULL(ot.[TotalHours], CASE 
				WHEN sc.ShiftId IS NULL THEN NULL
				ELSE ec.TotalWorkHours
				END) [TotalWorkHours]
		,ot.[NormalHours]
		,ot.[OffHours]
		,ot.[PublicHours]
		,ot.[Remark]
		,ot.[Status]
		,ot.[IsUpdated]
		,ecl.IsHoliday
		,ecl.IsOffDay
		,ecl.IsKeepInView
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #Calendar tmp ON e.[Enabled] = tmp.[Enabled]
		AND e.[IsDeleted] = tmp.[IsDeleted]
		LEFT JOIN [dbo].[OTDailyRequest] ot WITH (NOLOCK) ON tmp.WorkDate = ot.OvertimeDate
		AND ot.EmpID = @EmpID
		LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
		LEFT JOIN dbo.EmployeeAttendance ec WITH (NOLOCK) ON ec.AttendanceDate = tmp.WorkDate
		AND ec.EmpId = @EmpID
		LEFT JOIN dbo.EmployeeCalendar ecl WITH (NOLOCK) ON ecl.WorkDate = tmp.WorkDate
		AND ecl.EmpId = @EmpId
		LEFT JOIN dbo.[Shift] sc WITH (NOLOCK) ON sc.ShiftId = ec.ShiftId
	WHERE e.EmpId = @EmpId
	ORDER BY tmp.WorkDate

	DROP TABLE #Calendar

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
