CREATE PROCEDURE [dbo].[Report_MonthlyWorkingHours] @FromDate DATE
	,@ToDate DATE
	,@GroupIds VARCHAR(MAX) = ''
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

	DECLARE @fromDatetmp DATE
		,@Today DATE = GETDATE()
		,@GeneralLeaveId TINYINT = 0
		-- employee status
		,@Enable TINYINT = 1
		,@Disabled TINYINT = 2
		,@Deleted TINYINT = 3
		-- work type
		,@OffDay TINYINT = 1
		,@LeaveDay TINYINT = 2
		,@WorkDay TINYINT = 3
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7
	DECLARE @tblGroupIds TABLE (GroupId INT)

	CREATE TABLE #tblEmployeeIds (EmployeeId INT)

	CREATE TABLE #tblDate (WorkDate DATE)

	CREATE TABLE #tblHalfDayRequest (
		EmployeeId INT
		,RequestedDate DATE
		,FromTime DATETIME
		,ToTime DATETIME
		,LeaveTypeId INT
		,Code VARCHAR(10)
		,[Name] NVARCHAR(100)
		,WorkType TINYINT
		,TotalDays DECIMAL(2, 1)
		,StatusId TINYINT
		)

	IF ISNULL(@GroupIds, '') <> ''
		INSERT INTO @tblGroupIds (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	ELSE
	BEGIN
		INSERT INTO @tblGroupIds (GroupId)
		SELECT GroupId
		FROM dbo.[Group] WITH (NOLOCK)
		WHERE IsDeleted = 0
	END

	INSERT INTO #tblEmployeeIds (EmployeeId)
	SELECT emp.EmpId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
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

	SET @fromDatetmp = @FromDate

	WHILE (@fromDatetmp <= @ToDate)
	BEGIN
		INSERT INTO #tblDate (WorkDate)
		VALUES (@fromDatetmp)

		SET @fromDatetmp = DATEADD(DAY, 1, @fromDatetmp)
	END

	INSERT INTO #tblHalfDayRequest (
		EmployeeId
		,RequestedDate
		,FromTime
		,ToTime
		,LeaveTypeId
		,Code
		,[Name]
		,WorkType
		,TotalDays
		,StatusId
		)
	SELECT emp.EmployeeId
		,tbldate.WorkDate
		,lrd.FromTime
		,lrd.ToTime
		,lrd.LeaveType
		,lv.Code
		,lv.[Name]
		,lrd.WorkType
		,0.5 AS TotalDays
		,wkr.CurrentStatus
	FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
		INNER JOIN #tblEmployeeIds AS emp ON wkr.EmpId = emp.EmployeeId
		INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON wkr.RequestId = lrd.RequestId
		AND lrd.IsHalfDay = 1
		AND lrd.WorkType = @LeaveDay
		INNER JOIN #tblDate AS tbldate ON lrd.OnDate = tbldate.WorkDate
		INNER JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON lrd.LeaveType = lv.LeaveTypeId
	WHERE wkr.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)

	SELECT emp.EmpId AS EmployeeId
		,tbldate.WorkDate
		,empatt.TotalWorkHours AS TotalWorkingHours
		,(CASE 
			WHEN (
					empsum.IsLocal = 1
					OR (
						empsum.IsLocal = 0
						AND empsum.IsExpatUseHoliday = 1
						)
					)
				AND holiday.Id IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,(CASE 
			WHEN empca.IsOffDay = 1
				OR spwkr.WorkType = @OffDay THEN 1
			ELSE 0 END
			) AS IsOffDay
		,(CASE 
			WHEN spwkr.EmpId IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsLeaveDay
		,(CASE 
			WHEN ISNULL(empca.LeaveDay, 0) = 0 THEN ISNULL(spwkr.TotalDays, 0)
			ELSE empca.LeaveDay END
			) AS LeaveDay
		,(CASE 
			WHEN tbldate.WorkDate < GETDATE() - 1
				AND empatt.ShiftId IS NOT NULL
				AND empatt.InFixed IS NULL
				AND empatt.OutFixed IS NOT NULL
				AND empatt.TotalWorkHours IS NULL THEN 1
			ELSE 0 END
			) AS IsMissingCheckIn
		,(CASE 
			WHEN tbldate.WorkDate < GETDATE() - 1
				AND empatt.ShiftId IS NOT NULL
				AND empatt.OutFixed IS NULL
				AND empatt.InFixed IS NOT NULL
				AND empatt.TotalWorkHours IS NULL THEN 1
			ELSE 0 END
			) AS IsMissingCheckOut
		,(CASE 
			WHEN tbldate.WorkDate < GETDATE() - 1
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
		,spwkr.RequestStatusId
		,lv.LeaveTypeId
		,lv.Code
		,lv.[Name]
		,CAST(NULL AS DATETIME) AS FromTime
		,CAST(NULL AS DATETIME) AS ToTime
		,lv2.LeaveTypeId AS LeaveTypeId2
		,lv2.Code AS LeaveCode
		,lv2.[Name] AS LeaveName
	INTO #tblDailyWorking
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblEmployeeIds AS empid ON emp.EmpId = empid.EmployeeId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		CROSS JOIN #tblDate AS tbldate
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmpId = empca.EmpId
		AND tbldate.WorkDate = empca.WorkDate
		AND empca.IsPublic = 1
		LEFT JOIN dbo.LeaveType AS lv2 WITH (NOLOCK) ON empca.LeaveTypeId2 = lv2.LeaveTypeId
		LEFT JOIN dbo.EmployeeAttendance AS empatt WITH (NOLOCK) ON emp.EmpId = empatt.EmpId
		AND tbldate.WorkDate = empatt.AttendanceDate
		LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON (
			holiday.FromDate BETWEEN @FromDate
				AND @ToDate
			OR holiday.ToDate BETWEEN @FromDate
				AND @ToDate
			)
		AND tbldate.WorkDate BETWEEN holiday.FromDate
			AND holiday.ToDate
		OUTER APPLY (
		SELECT lrd.EmpId
			,lrd.LeaveType
			,lrd.WorkType
			,1 AS TotalDays
			,wkr.CurrentStatus AS RequestStatusId
		FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON wkr.RequestId = lrd.RequestId
			AND lrd.OnDate = tbldate.WorkDate
			AND lrd.IsHalfDay = 0
		WHERE wkr.EmpId = emp.EmpId
			AND lrd.WorkType IN (@OffDay, @LeaveDay)
			AND wkr.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		
		UNION
		SELECT lr.EmpId
			,lr.LeaveType
			,@LeaveDay AS WorkType
			,1 AS TotalDays
			,wkr.CurrentStatus AS RequestStatusId
		FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wkr.RequestId = lr.RequestId
			AND wkr.EmpId = emp.EmpId
			AND lr.LeaveType <> @GeneralLeaveId
			AND tbldate.WorkDate BETWEEN lr.FromDate
				AND lr.ToDate
			AND empca.WorkDate = tbldate.WorkDate
		WHERE wkr.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
			AND empca.IsOffDay = 0
			AND empca.IsHoliday = 0
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.LeaveRequestDetail AS lrd WITH (NOLOCK)
				WHERE lr.RequestId = lrd.RequestId
				)
		) AS spwkr
		LEFT JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON spwkr.LeaveType = lv.LeaveTypeId

	UPDATE tbl
	SET tbl.IsLeaveDay = 1
		,tbl.LeaveDay = rq.TotalDays
		,tbl.LeaveTypeId = rq.LeaveTypeId
		,tbl.Code = rq.Code
		,tbl.[Name] = rq.[Name]
		,tbl.FromTime = rq.FromTime
		,tbl.ToTime = rq.ToTime
		,tbl.RequestStatusId = rq.StatusId
	FROM #tblDailyWorking AS tbl
		INNER JOIN #tblHalfDayRequest AS rq ON tbl.EmployeeId = rq.EmployeeId
		AND tbl.WorkDate = rq.RequestedDate
	WHERE tbl.LeaveTypeId IS NULL

	UPDATE tbl
	SET tbl.LeaveDay = tbl.LeaveDay + rq.TotalDays
		,tbl.LeaveTypeId2 = rq.LeaveTypeId
		,tbl.LeaveCode = rq.Code
		,tbl.[LeaveName] = rq.[Name]
		,tbl.FromTime = rq.FromTime
		,tbl.ToTime = rq.ToTime
	FROM #tblDailyWorking AS tbl
		INNER JOIN #tblHalfDayRequest AS rq ON tbl.EmployeeId = rq.EmployeeId
		AND tbl.WorkDate = rq.RequestedDate
	WHERE tbl.FromTime <> rq.FromTime
		AND tbl.ToTime <> rq.ToTime

	SELECT emp.EmpId AS EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,empsum.WorkingDays
		,gr.GroupId
		,gr.Code
		,gr.[Name]
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblEmployeeIds AS empid ON emp.EmpId = empid.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
	ORDER BY gr.[Name]
		,emp.NickName

	SELECT *
	FROM #tblDailyWorking
	ORDER BY EmployeeId
		,WorkDate

	DROP TABLE #tblDailyWorking

	DROP TABLE #tblEmployeeIds

	DROP TABLE #tblDate

	DROP TABLE #tblHalfDayRequest
END
