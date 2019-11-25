
CREATE PROCEDURE [dbo].[LeaveRequest_Reject_RollbackCalendar] @RequestId BIGINT
	,@EmpId INT
	,@ProcessedBy INT
AS
/*
Created: Quinn.Vo@20150928
Task: update calendar when reject request.
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
		,@IsSpecialLeave BIT
		,@KIVLeave NUMERIC(18, 2)
		,@LeaveTransId BIGINT
		,@ProcessedDate DATETIME = GETDATE()
		-- leave type
		,@GeneralLeaveTypeId TINYINT = 0
		,@HolidayLeaveTypeId TINYINT = 13
	DECLARE @LeaveTypeId_Old INT
		,@LeaveTypeId2_Old INT
		,@LeaveDay_Old INT

	SELECT @KIVLeave = SUM(TotalDays)
	FROM dbo.tmpKIVLeaveBalance WITH (NOLOCK)
	WHERE Requestid = @RequestId

	SELECT @FromDate = lr.FromDate
		,@ToDate = lr.ToDate
		,@LeaveTypeId = lr.LeaveType
		,@TotalDays = lr.TotalDays
	FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON rq.RequestId = lr.RequestId
	WHERE rq.RequestId = @RequestId

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.LeaveRequestDetail WITH (NOLOCK)
			WHERE RequestId = @RequestId
			)
	BEGIN
		SELECT TOP 1 @LeaveTypeId_Old = LeaveTypeID
			,@LeaveTypeId2_Old = LeaveTypeID2
			,@LeaveDay_Old = LeaveDay
		FROM dbo.EmployeeCalendar WITH (NOLOCK)
		WHERE EmpId = @EmpId
			AND DATEDIFF(D, @FromDate, WorkDate) >= 0
			AND DATEDIFF(D, WorkDate, @ToDate) >= 0

		IF @LeaveTypeId2_Old IS NULL
		BEGIN
			UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
			SET IsLeaveDay = 0
				,LeaveTypeId = NULL
				,LeaveDay = 0
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE EmpId = @EmpId
				AND DATEDIFF(D, @FromDate, WorkDate) >= 0
				AND DATEDIFF(D, WorkDate, @ToDate) >= 0
		END
		ELSE IF @LeaveTypeId2_Old IS NOT NULL
		BEGIN
			UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
			SET IsLeaveDay = 1
				,LeaveTypeId = CASE 
					WHEN @LeaveTypeId_Old = @LeaveTypeId THEN @LeaveTypeId2_Old
					ELSE @LeaveTypeId_Old
					END
				,LeaveTypeId2 = NULL
				,LeaveDay = 0.5
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE EmpId = @EmpId
				AND DATEDIFF(D, @FromDate, WorkDate) >= 0
				AND DATEDIFF(D, WorkDate, @ToDate) >= 0
		END
	END
	ELSE
	BEGIN
		UPDATE empca
		SET empca.IsLeaveDay = CASE 
				WHEN empca.LeaveTypeId2 IS NOT NULL THEN 1
				ELSE 0
				END
			,empca.LeaveTypeId = CASE 
				WHEN empca.LeaveTypeId2 IS NOT NULL THEN empca.LeaveTypeId2
				ELSE NULL
				END
			,empca.LeaveTypeId2 = NULL
			,empca.LeaveDay = CASE 
				WHEN empca.LeaveDay = 1 THEN CASE 
						WHEN lrd.IsHalfDay = 1 THEN 0.5
						ELSE 0
						END
				ELSE 0
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
				WHEN empca.LeaveTypeId2 IS NOT NULL
					AND empca.LeaveTypeId2 > 0 THEN 1
				ELSE 0
				END
			,empca.LeaveTypeId = CASE 
				WHEN empca.LeaveTypeId2 IS NOT NULL THEN empca.LeaveTypeId2
				ELSE NULL
				END
			,empca.LeaveTypeId2 = NULL
			,empca.LeaveDay = CASE 
				WHEN empca.LeaveDay = 1 THEN CASE 
						WHEN lrd.IsHalfDay = 1 THEN 0.5
						ELSE 0
						END
				ELSE 0
				END
			,empca.ModifiedBy = @ProcessedBy
			,empca.ModifiedDate = @ProcessedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON empca.WorkDate = lrd.OnDate
			AND lrd.RequestId = @RequestId
			AND lrd.IsHalfDay = 1
			AND empca.LeaveTypeId <> lrd.LeaveType
		WHERE empca.EmpId = @EmpId
			AND empca.IsOffDay = 0
			AND empca.IsHoliday = 0
			AND empca.IsKeepInView = 0
			AND empca.WorkDate BETWEEN @FromDate
				AND @ToDate

		UPDATE empca
		SET empca.IsOffDay = 0
			,empca.IsPublic = 1
			,empca.ModifiedBy = @ProcessedBy
			,empca.ModifiedDate = @ProcessedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON empca.WorkDate = lrd.OnDate
			AND lrd.WorkType = 1 -- off
			AND lrd.RequestId = @RequestId
			AND empca.EmpId = lrd.EmpId
		WHERE empca.EmpId = @EmpId
			AND empca.ShiftId IS NULL
	END
END
