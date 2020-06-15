
CREATE PROCEDURE [dbo].[Report_MonthlyWorkingHours] @ActorId INT
	,@FromDate DATE
	,@ToDate DATE
	,@TeamIds VARCHAR(MAX) = ''
	,@Employee NVARCHAR(200) = NULL
	,@IsLocal BIT = NULL
	,@EmployeeStatusId TINYINT = NULL
AS
/*
Created: Tim.Trinh@2019-06-24
Task: monthly working hours
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@EmployeeIds VARCHAR(MAX) = ''
		-- employee status
		,@Enable TINYINT = 1
		,@Disabled TINYINT = 2
		,@Deleted TINYINT = 3
	DECLARE @tblGroupIds TABLE (GroupId INT)
	CREATE TABLE #tblEmployeeIds (EmployeeId INT)

	IF ISNULL(@TeamIds, '') <> ''
		INSERT INTO @tblGroupIds (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')

	INSERT INTO #tblEmployeeIds (EmployeeId)
	SELECT emp.EmpId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		INNER JOIN (
		SELECT EmpId
		FROM dbo.fnc_GetEmployeeByActorId(@ActorId, 0, 1)
		) AS member ON emp.EmpId = member.EmpId
	WHERE (
			@EmployeeStatusId IS NULL
			OR (
				@EmployeeStatusId = @Enable
				AND emp.[Enabled] = 1
				AND emp.isDeleted = 0
				)
			OR (
				@EmployeeStatusId = @Disabled
				AND emp.[Enabled] = 0
				AND emp.isDeleted = 0
				)
			OR (
				@EmployeeStatusId = @Deleted
				AND emp.isDeleted = 1
				)
			)
		AND (
			emp.NickName LIKE '%' + ISNULL(@Employee, '') + '%'
			OR emp.FullName LIKE '%' + ISNULL(@Employee, '') + '%'
			)
		AND (
			@IsLocal IS NULL
			OR empsum.IsLocal = @IsLocal
			)
		AND EXISTS (
			SELECT 1
			FROM @tblGroupIds AS tbl
			WHERE tbl.GroupId = emp.GroupId
			)

	SELECT @EmployeeIds = COALESCE(@EmployeeIds + ',', '') + CAST(EmployeeId AS VARCHAR(20))
	FROM #tblEmployeeIds WITH (NOLOCK)

	EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @fromDate
		,@toDate = @toDate
		,@empIds = @EmployeeIds

	SELECT emp.EmpId AS EmployeeId
		,empca.WorkDate
		,empatt.TotalWorkHours AS TotalWorkingHours
		,empca.IsHoliday
		,empca.IsOffDay
		,empca.IsLeaveDay
		,empca.LeaveDay
		,(CASE 
			WHEN empca.WorkDate < GETDATE() - 1
				AND empatt.ShiftId IS NOT NULL
				AND empatt.InFixed IS NULL
				AND empatt.OutFixed IS NOT NULL
				AND empatt.TotalWorkHours IS NULL THEN 1
			ELSE 0 END
			) AS IsMissingCheckIn
		,(CASE 
			WHEN empca.WorkDate < GETDATE() - 1
				AND empatt.ShiftId IS NOT NULL
				AND empatt.OutFixed IS NULL
				AND empatt.InFixed IS NOT NULL
				AND empatt.TotalWorkHours IS NULL THEN 1
			ELSE 0 END
			) AS IsMissingCheckOut
		,(CASE 
			WHEN empca.WorkDate < GETDATE() - 1
				AND empca.ShiftId IS NOT NULL
				AND (
					empsum.IsDefaultSchedule = 0
					OR (
						empsum.IsLocal = 1
						AND empsum.IsDefaultSchedule = 1
						AND NOT (
							empca.IsHoliday = 1
							OR empca.IsOffDay = 1
							)
						)
					)
				AND empatt.InFixed IS NULL
				AND empatt.OutFixed IS NULL
				AND empatt.TotalWorkHours IS NULL THEN 1
			ELSE 0 END
			) AS IsAbsent
		,r.CurrentStatus AS RequestStatusId
		,empca.LeaveTypeId
		,lv.Code
		,lv.[Name]
		,empca.FromTime
		,empca.ToTime
		,empca.LeaveTypeId AS LeaveTypeId2
		,lv2.Code AS LeaveCode
		,lv2.[Name] AS LeaveName
	INTO #tblEmployeeCalendar
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblEmployeeIds AS empid WITH (NOLOCK) ON emp.EmpId = empid.EmployeeId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		INNER JOIN dbo.tmpEmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmpId = empca.EmpId
		AND empca.WorkDate BETWEEN @fromDate
			AND @toDate
		LEFT JOIN dbo.EmployeeAttendance AS empatt WITH (NOLOCK) ON emp.EmpId = empatt.EmpId
		AND empatt.AttendanceDate = empca.WorkDate
		LEFT JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON empca.LeaveTypeId = lv.LeaveTypeId
		LEFT JOIN dbo.LeaveType AS lv2 WITH (NOLOCK) ON empca.LeaveTypeId2 = lv2.LeaveTypeId
		LEFT JOIN dbo.WorkingTimeRequest AS r WITH (NOLOCK) ON empca.RequestId = r.RequestId

	SELECT emp.EmpId AS EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,empsum.WorkingDays
		,gr.GroupId AS Id
		,gr.Code
		,gr.[Name]
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblEmployeeIds AS empid WITH (NOLOCK) ON emp.EmpId = empid.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
	ORDER BY gr.[Name]
		,emp.Code

	SELECT *
	FROM #tblEmployeeCalendar WITH (NOLOCK)
	ORDER BY EmployeeId
		,WorkDate

	DROP TABLE #tblEmployeeIds
	DROP TABLE #tblEmployeeCalendar
END
