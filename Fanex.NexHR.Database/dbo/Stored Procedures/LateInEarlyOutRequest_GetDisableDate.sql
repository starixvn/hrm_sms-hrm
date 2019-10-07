
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_GetDisableDate] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
	,@RequestId BIGINT = NULL
AS
/*
Created: Tim.Trinh@2019-09-24
Task: get disable date for request
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsDefaultSchedule BIT
		,@RequestTypeId TINYINT
		,@GeneralLeaveId TINYINT = 0
		-- change shift type
		,@ChangeShift TINYINT = 1
		,@MoveOffDay TINYINT = 2
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

	CREATE TABLE #tblDate (
		WorkDate DATE
		,ShiftId INT
		)

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

	SELECT @IsDefaultSchedule = IsDefaultSchedule
	FROM dbo.EmployeeSummary WITH (NOLOCK)
	WHERE EmpId = @EmpId

	IF @RequestId IS NOT NULL
	BEGIN
		SELECT @RequestTypeId = r.RequestType
		FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
			INNER JOIN dbo.LateInEarlyOutRequest AS lieo WITH (NOLOCK) ON r.RequestId = lieo.RequestId
			AND lieo.RequestId = @RequestId
	END

	WHILE @FromDate <= @toDate
	BEGIN
		IF @IsDefaultSchedule = 1
		BEGIN
			INSERT INTO #tblDate (
				WorkDate
				,ShiftId
				)
			SELECT @FromDate
				,CASE 
					WHEN DATEPART(dw, @FromDate) = 1 THEN SunShift
					WHEN DATEPART(dw, @FromDate) = 2 THEN MonShift
					WHEN DATEPART(dw, @FromDate) = 3 THEN TueShift
					WHEN DATEPART(dw, @FromDate) = 4 THEN WedShift
					WHEN DATEPART(dw, @FromDate) = 5 THEN ThuShift
					WHEN DATEPART(dw, @FromDate) = 6 THEN FriShift
					WHEN DATEPART(dw, @FromDate) = 7 THEN SatShift
					END
			FROM dbo.EmployeeDefaultShift WITH (NOLOCK)
			WHERE EmpId = @EmpId
		END
		ELSE
		BEGIN
			INSERT INTO #tblDate (
				WorkDate
				,ShiftId
				)
			SELECT TOP 1 @FromDate
				,ShiftId
			FROM dbo.ScheduleArrangementDefaultShift WITH (NOLOCK)
			WHERE EmployeeId = @EmpId
				AND ShiftId > 0
				AND FromDate <= @FromDate
			ORDER BY FromDate DESC
		END

		SET @FromDate = DATEADD(DD, 1, @FromDate)
	END

	INSERT INTO #tblHalfDayRequest (
		RequestedDate
		,FromTime
		,ToTime
		,LeaveTypeId
		,Code
		,[Name]
		,WorkType
		,TotalDays
		,StatusId
		)
	SELECT tbldate.WorkDate
		,lrd.FromTime
		,lrd.ToTime
		,lrd.LeaveType
		,lv.Code
		,lv.[Name]
		,lrd.WorkType
		,0.5 AS TotalDays
		,wkr.CurrentStatus
	FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON wkr.RequestId = lrd.RequestId
		AND wkr.EmpId = lrd.EmpId
		AND lrd.EmpId = @EmpId
		AND lrd.IsHalfDay = 1
		AND lrd.WorkType = @LeaveDay
		INNER JOIN #tblDate AS tbldate ON lrd.OnDate = tbldate.WorkDate
		INNER JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON lrd.LeaveType = lv.LeaveTypeId
	WHERE wkr.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)

	SELECT tbldate.WorkDate
		,(CASE 
			WHEN empca.EmpId IS NOT NULL THEN empca.ShiftId
			ELSE tbldate.ShiftId END
			) AS ShiftId
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
			WHEN empca.EmpId IS NOT NULL THEN empca.IsOffDay
			ELSE 0 END
			) AS IsOffDay
		,(CASE 
			WHEN spwkr.EmpId IS NOT NULL
				AND spwkr.WorkType = @OffDay THEN 1
			ELSE 0 END
			) AS IsLeaveOffday
		,(CASE 
			WHEN spwkr.EmpId IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsLeaveDay
		,(CASE 
			WHEN ISNULL(empca.LeaveDay, 0) = 0 THEN ISNULL(spwkr.TotalDays, 0)
			ELSE empca.LeaveDay END
			) AS LeaveDay
		,spwkr.RequestStatusId
		,spwkr.LeaveTypeId
		,CAST(NULL AS DATETIME) AS FromTime
		,CAST(NULL AS DATETIME) AS ToTime
		,empca.LeaveTypeId2
	INTO #tblDailyWorking
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		AND emp.EmpId = @EmpId
		CROSS JOIN #tblDate AS tbldate
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmpId = empca.EmpId
		AND tbldate.WorkDate = empca.WorkDate
		AND empca.IsPublic = 1
		LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON tbldate.WorkDate BETWEEN holiday.FromDate
			AND holiday.ToDate
		OUTER APPLY (
		SELECT lrd.EmpId
			,lrd.LeaveType AS LeaveTypeId
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
			,lr.LeaveType AS LeaveTypeId
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

	UPDATE tbl
	SET tbl.IsLeaveDay = 1
		,tbl.LeaveDay = rq.TotalDays
		,tbl.LeaveTypeId = rq.LeaveTypeId
		,tbl.FromTime = rq.FromTime
		,tbl.ToTime = rq.ToTime
		,tbl.RequestStatusId = rq.StatusId
	FROM #tblDailyWorking AS tbl
		INNER JOIN #tblHalfDayRequest AS rq ON tbl.WorkDate = rq.RequestedDate
	WHERE tbl.LeaveTypeId IS NULL

	UPDATE tbl
	SET tbl.LeaveDay = tbl.LeaveDay + rq.TotalDays
		,tbl.LeaveTypeId2 = rq.LeaveTypeId
	FROM #tblDailyWorking AS tbl
		INNER JOIN #tblHalfDayRequest AS rq ON tbl.WorkDate = rq.RequestedDate
	WHERE tbl.FromTime <> rq.FromTime
		AND tbl.ToTime <> rq.ToTime

	SELECT WorkDate
	FROM #tblDailyWorking
	WHERE ShiftId IS NULL
		OR (
			IsLeaveDay = 1
			AND LeaveDay = 1
			)
		OR IsLeaveOffday = 1
		OR (
			IsOffDay = 1
			AND ShiftId IS NULL
			)
		OR (
			IsHoliday = 1
			AND ShiftId IS NULL
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
				INNER JOIN dbo.LateInEarlyOutRequest AS lieo WITH (NOLOCK) ON r.RequestId = lieo.RequestId
				AND WorkDate = lieo.OnDate
				AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
			WHERE @RequestId IS NULL
				OR (
					r.RequestId <> @RequestId
					AND r.RequestType = @RequestTypeId
					)
			)

	DROP TABLE #tblDate

	DROP TABLE #tblHalfDayRequest

	DROP TABLE #tblDailyWorking
END
