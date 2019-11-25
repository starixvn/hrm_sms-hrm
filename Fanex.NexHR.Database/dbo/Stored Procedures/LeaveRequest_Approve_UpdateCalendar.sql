
CREATE PROCEDURE [dbo].[LeaveRequest_Approve_UpdateCalendar] @RequestId BIGINT
	,@EmpId INT
	,@ProcessedBy INT
AS
/*
Created: Quinn.Vo@20150928
Task: update calendar when approved request.
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @LeaveTypeId INT
		,@TotalDays NUMERIC(5, 1)
		,@FromDate DATE
		,@ToDate DATE
		,@ProcessedDate DATETIME = GETDATE()
		-- leave type
		,@GeneralLeaveTypeId TINYINT = 0
		,@HolidayLeaveTypeId TINYINT = 13

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
		,@LeaveTypeId = LeaveType
		,@TotalDays = TotalDays
	FROM dbo.LeaveRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF @LeaveTypeId = @HolidayLeaveTypeId
	BEGIN
		UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
		SET ShiftId = NULL
			,IsLeaveDay = 1
			,LeaveTypeId = @HolidayLeaveTypeId
			,IsPublic = 1
			,ModifiedDate = @ProcessedDate
			,ModifiedBy = @ProcessedBy
		WHERE EmpId = @EmpId
			AND WorkDate BETWEEN @FromDate
				AND @ToDate
	END
	ELSE IF NOT EXISTS (
			SELECT 1
			FROM dbo.LeaveRequestDetail WITH (NOLOCK)
			WHERE RequestId = @RequestId
			)
	BEGIN
		UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
		SET IsLeaveDay = CASE 
				WHEN @LeaveTypeId IS NULL
					OR @LeaveTypeId = @GeneralLeaveTypeId THEN 0
				ELSE 1
				END
			,LeaveTypeId = CASE 
				WHEN LeaveTypeId IS NULL THEN @LeaveTypeId
				ELSE LeaveTypeId
				END
			,LeaveTypeId2 = CASE 
				WHEN LeaveTypeId IS NOT NULL THEN @LeaveTypeId
				ELSE NULL
				END
			,LeaveDay = CASE 
				WHEN @LeaveTypeId IS NULL
					OR @LeaveTypeId = @GeneralLeaveTypeId THEN 0
				WHEN LeaveDay IS NULL
					OR LeaveDay = 0 THEN CASE 
						WHEN @TotalDays = 0.5 THEN 0.5
						ELSE 1
						END
				ELSE 1
				END
			,ModifiedBy = @ProcessedBy
			,ModifiedDate = @ProcessedDate
		WHERE EmpId = @EmpId
			AND IsOffDay = 0
			AND IsHoliday = 0
			AND IsKeepInView = 0
			AND WorkDate BETWEEN @FromDate
				AND @ToDate

		UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
		SET IsPublic = 1
			,ShiftId = CASE 
				WHEN IsOffDay = 1
					OR IsHoliday = 1 THEN NULL
				ELSE ShiftId
				END
			,ModifiedBy = @ProcessedBy
			,ModifiedDate = @ProcessedDate
		WHERE EmpId = @EmpId
			AND WorkDate BETWEEN @FromDate
				AND @ToDate
	END
	ELSE
	BEGIN
		UPDATE empca
		SET empca.IsLeaveDay = CASE 
				WHEN lrd.LeaveType = 0 THEN 0
				ELSE 1
				END
			,empca.LeaveTypeId = CASE 
				WHEN empca.LeaveTypeId IS NULL THEN lrd.LeaveType
				ELSE empca.LeaveTypeId
				END
			,empca.LeaveTypeId2 = CASE 
				WHEN empca.LeaveTypeId IS NOT NULL THEN lrd.LeaveType
				ELSE NULL
				END
			,empca.LeaveDay = CASE 
				WHEN lrd.LeaveType = 0 THEN 0
				WHEN empca.LeaveDay IS NULL
					OR empca.LeaveDay = 0 THEN CASE 
						WHEN lrd.IsHalfDay = 1 THEN 0.5
						ELSE 1
						END
				ELSE 1
				END
			,empca.ModifiedBy = @ProcessedBy
			,empca.ModifiedDate = @ProcessedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON empca.WorkDate = lrd.OnDate
			AND lrd.RequestId = @RequestId
		WHERE empca.EmpId = @EmpId
			AND empca.IsOffDay = 0
			AND empca.IsHoliday = 0
			AND empca.IsKeepInView = 0
			AND empca.WorkDate BETWEEN @FromDate
				AND @ToDate

		UPDATE empca
		SET empca.IsLeaveDay = CASE 
				WHEN empca.LeaveTypeId IS NULL
					OR empca.LeaveTypeId = 0 THEN 0
				ELSE 1
				END
			,empca.LeaveTypeId = CASE 
				WHEN empca.LeaveTypeId IS NULL THEN lrd.LeaveType
				ELSE empca.LeaveTypeId
				END
			,empca.LeaveTypeId2 = CASE 
				WHEN empca.LeaveTypeId IS NOT NULL THEN lrd.LeaveType
				ELSE NULL
				END
			,empca.LeaveDay = CASE 
				WHEN empca.LeaveDay IS NULL
					OR empca.LeaveDay = 0 THEN CASE 
						WHEN lrd.IsHalfDay = 1 THEN 0.5
						ELSE 1
						END
				ELSE 1
				END
			,empca.ModifiedBy = @ProcessedBy
			,empca.ModifiedDate = @ProcessedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON empca.WorkDate = lrd.OnDate
			AND empca.LeaveTypeId <> lrd.LeaveType
			AND lrd.RequestId = @RequestId
			AND lrd.IsHalfDay = 1
		WHERE empca.EmpId = @EmpId
			AND empca.IsOffDay = 0
			AND empca.IsHoliday = 0
			AND empca.IsKeepInView = 0
			AND empca.WorkDate BETWEEN @FromDate
				AND @ToDate

		UPDATE empca
		SET empca.IsOffDay = 1
			,empca.LeaveTypeId = NULL
			,empca.LeaveTypeId2 = NULL
			,empca.ShiftId = NULL
			,empca.ModifiedBy = @ProcessedBy
			,empca.ModifiedDate = @ProcessedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON empca.WorkDate = lrd.OnDate
			AND lrd.WorkType = 1 -- off
			AND lrd.RequestId = @RequestId
		WHERE empca.EmpId = @EmpId

		UPDATE empca
		SET empca.LeaveTypeId = NULL
			,empca.LeaveTypeId2 = NULL
			,empca.ModifiedBy = @ProcessedBy
			,empca.ModifiedDate = @ProcessedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON empca.WorkDate = lrd.OnDate
			AND lrd.WorkType = 3 -- work
			AND lrd.RequestId = @RequestId
		WHERE empca.EmpId = @EmpId

		UPDATE empca
		SET empca.IsPublic = 1
			,empca.ShiftId = CASE 
				WHEN empca.IsHoliday = 1 THEN NULL
				ELSE empca.ShiftId
				END
			,empca.ModifiedBy = @ProcessedBy
			,empca.ModifiedDate = @ProcessedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON empca.WorkDate = lrd.OnDate
			AND lrd.RequestId = @RequestId
		WHERE empca.EmpId = @EmpId
	END
END
