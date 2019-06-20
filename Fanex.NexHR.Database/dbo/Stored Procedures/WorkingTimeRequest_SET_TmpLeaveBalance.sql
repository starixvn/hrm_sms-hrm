/****** Object:  Stored Procedure dbo.WorkingTimeRequest_SET_TmpLeaveBalance    Script Date: 7/20/2016 10:19:25 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_SET_TmpLeaveBalance    Script Date: 7/19/2016 5:22:23 PM ******/
-- [dbo].[WorkingTimeRequest_SET_TmpLeaveBalance_xtest] 173841, 217 select * from tmpLeaveBalance WHERE InYear = 2015
CREATE PROCEDURE [dbo].[WorkingTimeRequest_SET_TmpLeaveBalance] 
	@Requestid BIGINT
	,@EmpID INT
AS
/*
Created: Quinn.Vo@20150927
Task: insert temp al leave balance
DB: HRM_SMS
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
		,@tmpDate DATETIME
		,@LeaveDay NUMERIC(5, 1)

	SELECT @FromDate = lr.FromDate, 
		@ToDate = lr.ToDate, 
		@LeaveTypeId = lr.LeaveType, 
		@TotalDays = lr.TotalDays
	FROM dbo.WorkingTimeRequest rq WITH(NOLOCK) 
		INNER JOIN dbo.LeaveRequest lr WITH(NOLOCK) ON rq.RequestId = lr.RequestId
	WHERE rq.RequestID = @RequestId

	IF EXISTS (
			SELECT 1
			FROM dbo.tmpLeaveBalance WITH (NOLOCK)
			WHERE Requestid = @Requestid
			)
	BEGIN
		DELETE
		FROM dbo.tmpLeaveBalance
		WHERE Requestid = @Requestid
	END

	IF (
			@LeaveTypeId = 0
			OR @TotalDays = 0.5
			)
	BEGIN
		SET @tmpDate = @FromDate
		WHILE (DATEDIFF(MM, @tmpDate, @ToDate) >= 0)
		BEGIN
			SELECT @LeaveDay = SUM(CASE 
					WHEN (DATEDIFF(MM, OnDate, @tmpDate) = 0) THEN CASE 
							WHEN IsHalfDay = 1 THEN 0.5
							ELSE 1
							END
					ELSE 0
					END)
			FROM dbo.LeaveRequestDetail WITH (NOLOCK)
			WHERE RequestId = @Requestid
				AND WorkType = 2
				AND LeaveType = 2

			IF(@LeaveDay > 0)
			BEGIN
				INSERT INTO dbo.tmpLeaveBalance (
				EmpID
				,Requestid
				,TotalDays
				,InYear
				,[Status]
				,InMonth
				)
			VALUES (
				@EmpID
				,@Requestid
				,@LeaveDay
				,YEAR(@tmpDate)
				,0
				,MONTH(@tmpDate)
				)
			END

			SET @tmpDate = DATEADD(MM, 1, @tmpDate)
		END
	END
	ELSE IF (@LeaveTypeId = 2)
	BEGIN
		SET @tmpDate = @FromDate
		WHILE (DATEDIFF(MM, @tmpDate, @ToDate) >= 0)
		BEGIN
			SELECT @LeaveDay = COUNT(1)
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE DATEDIFF(MM, WorkDate, @tmpDate) = 0
				AND EmpID = @EmpId
				AND IsHoliday = 0
				AND IsOffDay = 0
				AND DATEDIFF(DD, @FromDate, WorkDate) >= 0
				AND DATEDIFF(DD, WorkDate, @ToDate) >= 0

			IF(@LeaveDay > 0)
			BEGIN
				INSERT INTO dbo.tmpLeaveBalance (
					EmpID
					,Requestid
					,TotalDays
					,InYear
					,[Status]
					,InMonth
					)
				VALUES (
					@EmpID
					,@Requestid
					,@LeaveDay
					,YEAR(@tmpDate)
					,0
					,MONTH(@tmpDate)
					)
			END
			SET @tmpDate = DATEADD(MM, 1, @tmpDate)
		END
	END
END
