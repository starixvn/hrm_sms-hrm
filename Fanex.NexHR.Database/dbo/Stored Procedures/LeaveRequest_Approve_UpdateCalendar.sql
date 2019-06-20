
CREATE PROCEDURE [dbo].[LeaveRequest_Approve_UpdateCalendar]
	@RequestId BIGINT,
	@EmpId INT,
	@ProcessedBy INT
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
		,@TotalDays NUMERIC(5,1)
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@ProcessedDate DATETIME
		,@IsSpecialLeave BIT

	SELECT @FromDate = FromDate, 
		@ToDate = ToDate, 
		@LeaveTypeID = lr.LeaveType, 
		@TotalDays = TotalDays,
		@IsSpecialLeave = CASE WHEN lr.LeaveType = 0 THEN 1 ELSE 0 END,
		@ProcessedDate = GETDATE()
	FROM dbo.LeaveRequest lr WITH(NOLOCK)
	WHERE lr.RequestID = @RequestId

	-- update the working calendar to record the check-in time
	IF(@TotalDays = 0)
	BEGIN
		IF(@LeaveTypeID = 13) --13: holiday
		BEGIN
			UPDATE dbo.EmployeeCalendar WITH(ROWLOCK,UPDLOCK)
			SET ShiftId = NULL,
				IsLeaveDay = 1,
				LeaveTypeId = 13,
				IsPublic = 1,
				ModifiedDate = @ProcessedDate,
				ModifiedBy = @ProcessedBy
			WHERE EmpID = @EmpId
				AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111) AND CONVERT(VARCHAR(10), @ToDate, 111)
		END 
	END
    ELSE
	BEGIN
		IF (@IsSpecialLeave = 0)
		BEGIN
			UPDATE dbo.EmployeeCalendar WITH(ROWLOCK,UPDLOCK)
			SET IsLeaveDay = CASE WHEN @LeaveTypeID IS NULL OR @LeaveTypeID = 0 THEN 0 ELSE 1 END,
				LeaveTypeID = CASE WHEN LeaveTypeID IS NULL THEN @LeaveTypeID ELSE LeaveTypeID END,
				LeaveTypeID2 = CASE WHEN LeaveTypeID IS NOT NULL THEN @LeaveTypeID ELSE NULL END,
				LeaveDay = 	CASE WHEN @LeaveTypeID IS NULL OR @LeaveTypeID = 0 THEN 0
								WHEN  (LeaveDay IS NULL OR LeaveDay = 0) THEN 
									CASE WHEN @TotalDays = 0.5 THEN 0.5
									ELSE 1 END 
							ELSE 1 END,
				ModifiedDate = @ProcessedDate
			WHERE EmpID = @EmpId 
				AND IsOffDay = 0 AND IsHoliday = 0 AND IsKeepInView = 0
				AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111) AND CONVERT(VARCHAR(10), @ToDate, 111) -- cut off time before compare

			UPDATE dbo.EmployeeCalendar WITH(ROWLOCK,UPDLOCK)
			SET IsPublic = 1
				,ModifiedDate = @ProcessedDate
				,ShiftId = CASE WHEN (IsOffDay = 1 OR IsHoliday = 1) THEN NULL ELSE ShiftId END
			WHERE EmpID = @EmpId 
				AND WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111) AND CONVERT(VARCHAR(10), @ToDate, 111) -- cut off time before compare
		END
		ELSE
		BEGIN
			UPDATE EC
			SET EC.IsLeaveDay = CASE WHEN lrd.LeaveType = 0 THEN 0 ELSE 1 END,
				EC.LeaveTypeID = CASE WHEN EC.LeaveTypeID IS NULL THEN lrd.LeaveType ELSE EC.LeaveTypeID END,
				EC.LeaveTypeID2 = CASE WHEN EC.LeaveTypeID IS NOT NULL THEN lrd.LeaveType ELSE NULL END,
				EC.LeaveDay = 	CASE WHEN lrd.LeaveType = 0 THEN 0
									WHEN (EC.LeaveDay IS NULL OR EC.LeaveDay = 0) THEN 
										CASE WHEN lrd.IsHalfDay = 1 THEN 0.5
										ELSE 1 END 
							ELSE 1 END
			FROM dbo.EmployeeCalendar EC WITH(ROWLOCK,UPDLOCK)
				INNER JOIN dbo.LeaveRequestDetail lrd WITH(NOLOCK)
					ON EC.WorkDate = lrd.OnDate
			WHERE EC.EmpID = @EmpId 
				AND lrd.RequestId = @RequestID
				AND EC.IsOffDay = 0 and EC.IsHoliday = 0 and EC.IsKeepInView = 0
				AND EC.WorkDate BETWEEN CONVERT(VARCHAR(10), @FromDate, 111) and CONVERT(VARCHAR(10), @ToDate, 111) -- cut off time before compare

			UPDATE EC
				SET EC.IsLeaveDay =  CASE WHEN EC.LeaveTypeID IS NULL OR EC.LeaveTypeID = 0 THEN 0 ELSE 1 END,
					EC.LeaveTypeID = CASE WHEN EC.LeaveTypeID IS NULL THEN lrd.LeaveType ELSE EC.LeaveTypeID END,
					EC.LeaveTypeID2 = CASE WHEN EC.LeaveTypeID IS NOT NULL THEN lrd.LeaveType ELSE NULL END,
					EC.LeaveDay = 	CASE WHEN (EC.LeaveDay IS NULL OR EC.LeaveDay = 0) THEN 
									CASE WHEN lrd.IsHalfDay = 1 THEN 0.5
									ELSE 1 END 
								ELSE 1 END
				FROM dbo.EmployeeCalendar EC WITH(ROWLOCK,UPDLOCK)
					INNER JOIN dbo.LeaveRequestDetail lrd WITH(NOLOCK)
						ON EC.WorkDate = lrd.OnDate AND EC.LeaveTypeId <> lrd.LeaveType
				WHERE EC.EmpID = @EmpId 
					AND lrd.RequestId = @RequestID
					AND lrd.IsHalfDay = 1
					AND EC.IsOffDay = 0 AND EC.IsHoliday = 0 AND EC.IsKeepInView = 0
					AND EC.WorkDate BETWEEN convert(varchar(10),@FromDate,111) AND convert(varchar(10),@ToDate,111) -- cut off time before compare

			UPDATE EC
			SET EC.IsOffDay = 1,
				EC.LeaveTypeId = NULL,
				EC.LeaveTypeId2 = NULL,
				EC.ShiftId = NULl
			FROM dbo.EmployeeCalendar EC WITH(ROWLOCK,UPDLOCK)
				INNER JOIN dbo.LeaveRequestDetail lrd WITH(NOLOCK)
					ON EC.WorkDate = lrd.OnDate 
			WHERE EC.EmpID = @EmpId 
				AND lrd.WorkType = 1
				AND lrd.RequestId = @RequestID

			UPDATE EC
			SET 
				EC.LeaveTypeId = NULL,
				EC.LeaveTypeId2 = NULL
			FROM dbo.EmployeeCalendar EC WITH(ROWLOCK,UPDLOCK)
				INNER JOIN dbo.LeaveRequestDetail lrd WITH(NOLOCK)
					ON EC.WorkDate = lrd.OnDate 
			WHERE EC.EmpID = @EmpId 
				AND lrd.WorkType = 3
				AND lrd.RequestId = @RequestID

			UPDATE EC
			SET EC.IsPublic = 1
				,EC.ModifiedDate = @ProcessedDate
				,EC.ShiftId = CASE WHEN IsHoliday = 1 THEN NULL ELSE EC.ShiftId END
			FROM dbo.EmployeeCalendar EC WITH(ROWLOCK,UPDLOCK)
				INNER JOIN dbo.LeaveRequestDetail lrd WITH(NOLOCK)
					ON EC.WorkDate = lrd.OnDate AND lrd.RequestId = @RequestID
			WHERE EC.EmpID = @EmpId 
		END
	END
END