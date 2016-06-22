CREATE PROCEDURE [dbo].[WorkingTimeRequest_LeaveReject_RollbackCalendar]
	@RequestId BIGINT,
	@EmpId INT,
	@ProcessedBy INT
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
		,@TotalDays NUMERIC(5,1)
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@IsSpecialLeave BIT
		,@KIVLeave NUMERIC(18,2)
		,@LeaveTransId BIGINT
		,@CurrentStatus INT
		,@ProcessedDate DATETIME

	DECLARE @_FromDate DATETIME, @_LeaveTypeId SMALLINT,@CursorLRD Cursor
	DECLARE @LeaveTypeId_Old INT, @LeaveTypeId2_Old INT, @LeaveDay_Old INT

	SELECT @KIVLeave = SUM(TotalDays) FROM dbo.tmpKIVLeaveBalance WITH(NOLOCK) 
	WHERE Requestid = @RequestId

	SELECT @FromDate = lr.FromDate, 
		@ToDate = lr.ToDate, 
		@LeaveTypeId = lr.LeaveType, 
		@TotalDays = lr.TotalDays,
		@CurrentStatus = CurrentStatus,
		@IsSpecialLeave = CASE WHEN lr.LeaveType = 0 THEN 1 ELSE 0 END,
		@ProcessedDate = GETDATE()
	FROM dbo.WorkingTimeRequest rq WITH(NOLOCK) 
		INNER JOIN dbo.LeaveRequest lr WITH(NOLOCK) ON rq.RequestId = lr.RequestId
	WHERE rq.RequestID = @RequestId

	IF @IsSpecialLeave = 0
	BEGIN
		SELECT TOP 1 @LeaveTypeId_Old = LeaveTypeID , @LeaveTypeId2_Old = LeaveTypeID2, @LeaveDay_Old = LeaveDay
		FROM dbo.EmployeeCalendar WITH(NOLOCK) 
		WHERE EmpID = @EmpId
		AND DATEDIFF(D, @FromDate, WorkDate) >= 0
		AND DATEDIFF(D, WorkDate, @ToDate) >= 0

		IF @LeaveTypeId2_Old IS NULL 
			BEGIN
					UPDATE dbo.EmployeeCalendar  WITH(ROWLOCK,UPDLOCK)
					SET IsLeaveDay = 0,
						LeaveTypeID = NULL,
						LeaveDay = 0,
						ModifiedDate = @ProcessedDate
					WHERE EmpID = @EmpId
					AND DATEDIFF(D, @FromDate, WorkDate) >= 0
					AND DATEDIFF(D, WorkDate, @ToDate) >= 0
			END
		ELSE IF @LeaveTypeId2_Old IS NOT NULL 
			BEGIN
					UPDATE dbo.EmployeeCalendar  WITH(ROWLOCK,UPDLOCK)
					SET IsLeaveDay = 1,
						LeaveTypeID = CASE WHEN @LeaveTypeId_Old = @LeaveTypeID THEN @LeaveTypeID2_OLD ELSE @LeaveTypeId_Old END,
						LeaveTypeId2 = NULL,
						LeaveDay = 0.5,
						ModifiedDate = @ProcessedDate
					WHERE EmpID = @EmpId
						AND DATEDIFF(D, @FromDate, WorkDate) >= 0
						AND DATEDIFF(D, WorkDate, @ToDate) >= 0
			END
					
				
	END
	ELSE
	BEGIN
		UPDATE EC
					SET EC.IsLeaveDay = CASE WHEN EC.LeaveTypeId2 IS NOT NULL THEN 1 ELSE 0 END,
						EC.LeaveTypeID = CASE WHEN EC.LeaveTypeId2 IS  NOT NULL THEN EC.LeaveTypeID2 ELSE NULL END,
						EC.LeaveTypeID2 = NULL,
						EC.LeaveDay = 	case when (EC.LeaveDay = 1) then 
											case when lrd.IsHalfDay = 1 then 0.5 else 0 end 
										else 0 end,
						EC.ModifiedDate = @ProcessedDate
					FROM dbo.EmployeeCalendar EC WITH(ROWLOCK,UPDLOCK)
						INNER JOIN dbo.LeaveRequestDetail lrd WITH(NOLOCK)
							ON EC.WorkDate = lrd.OnDate
					WHERE EC.EmpID = @EmpId 
						and	lrd.RequestId = @RequestID
						and EC.IsOffDay = 0 and EC.IsHoliday = 0 and EC.IsKeepInView = 0
						and EC.WorkDate between convert(varchar(10),@FromDate,111) and convert(varchar(10),@ToDate,111) -- cut off time before compare

		UPDATE EC
		SET EC.IsLeaveDay = CASE WHEN EC.LeaveTypeId2 IS NOT NULL AND EC.LeaveTypeId2 > 0 THEN 1 ELSE 0 END,
						EC.LeaveTypeID = CASE WHEN EC.LeaveTypeId2 IS  NOT NULL THEN EC.LeaveTypeID2 ELSE NULL END,
						EC.LeaveTypeID2 = NULL,
						EC.LeaveDay = 	case when (EC.LeaveDay = 1) then 
											case when lrd.IsHalfDay = 1 then 0.5 else 0 end 
										else 0 end,
						EC.ModifiedDate = @ProcessedDate
		FROM dbo.EmployeeCalendar EC WITH(ROWLOCK,UPDLOCK)
			INNER JOIN dbo.LeaveRequestDetail lrd WITH(NOLOCK)
				ON EC.WorkDate = lrd.OnDate AND EC.LeaveTypeId <> lrd.LeaveType
		WHERE EC.EmpID = @EmpId 
			and	lrd.RequestId = @RequestID
			and lrd.IsHalfDay = 1
			and EC.IsOffDay = 0 and EC.IsHoliday = 0 and EC.IsKeepInView = 0
			and EC.WorkDate between convert(varchar(10),@FromDate,111) and convert(varchar(10),@ToDate,111) -- cut off time before compare

		UPDATE EC
		SET EC.IsOffDay=0,
			EC.IsPublic = 1,
			EC.ModifiedDate = @ProcessedDate
		FROM dbo.EmployeeCalendar EC WITH(ROWLOCK,UPDLOCK)
			INNER JOIN dbo.LeaveRequestDetail lrd WITH(NOLOCK)
				ON EC.WorkDate = lrd.OnDate AND EC.EmpId = lrd.EmpId
			INNER JOIN dbo.WorkingTimeRequest wkr WITH(NOLOCK)
				ON EC.ModifiedBy = wkr.ApproverId AND lrd.RequestId = wkr.RequestId 
		WHERE EC.EmpID = @EmpId 
			AND	lrd.WorkType = 1
			AND	lrd.RequestId = @RequestID
			AND EC.ShiftId IS NULL
	END
END