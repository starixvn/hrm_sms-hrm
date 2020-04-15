
CREATE PROCEDURE [dbo].[LeaveRequest_Delete] @RequestId BIGINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190416
Task: delete a leave request
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PendingStatusId TINYINT = 1
		,@PaternityTypeId TINYINT = 14
		,@GeneralTypeId TINYINT = 0
		,@LeaveType TINYINT
		,@TotalDays INT
		,@EmpId INT
		,@FromDate DATE
		,@ToDate DATE
		,@TotalDaysPaternity INT

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest WITH (NOLOCK)
			WHERE RequestId = @RequestId
				AND CurrentStatus = @PendingStatusId
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Status is not pending, you can not delete this request.'

		RETURN;
	END

	SELECT @EmpId = EmpId
		,@FromDate = FromDate
		,@ToDate = ToDate
		,@LeaveType = LeaveType
		,@TotalDays = TotalDays
	FROM dbo.LeaveRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF @LeaveType = @PaternityTypeId
	BEGIN
		UPDATE dbo.EmployeePaternity WITH (ROWLOCK, UPDLOCK)
		SET BalanceDay = BalanceDay + @TotalDays
		WHERE EmpId = @EmpId
			AND DATEADD(M, 1, ChildBornDate) >= @ToDate
			AND @FromDate >= ChildBornDate
	END

	IF @LeaveType = @GeneralTypeId AND (SELECT COUNT(*) FROM dbo.LeaveRequestDetail WHERE LeaveType = @PaternityTypeId) > 0
	BEGIN
		SELECT @TotalDaysPaternity = COUNT(*)
						FROM dbo.LeaveRequestDetail
						WHERE LeaveType = @PaternityTypeId AND RequestId = @RequestId

		UPDATE dbo.EmployeePaternity WITH (ROWLOCK, UPDLOCK)
		SET BalanceDay = BalanceDay + @TotalDaysPaternity
		WHERE EmpId = @EmpId
			AND (DATEADD(M, 1, ChildBornDate) >= @FromDate OR @FromDate >= ChildBornDate)
	END

	DELETE dbo.WorkingTimeRequest
	WHERE RequestId = @RequestId

	DELETE dbo.LeaveRequest
	WHERE RequestId = @RequestId

	DELETE dbo.LeaveRequestDetail
	WHERE RequestId = @RequestId

	DELETE dbo.tmpLeaveBalance
	WHERE RequestId = @RequestId

	DELETE dbo.KeepInViewByDayTrans
	WHERE RequestId = @RequestId

	DELETE dbo.KeepInViewByHourTrans
	WHERE RequestId = @RequestId

	DELETE dbo.WeekendLeaveTrans
	WHERE RequestId = @RequestId
	
	DELETE dbo.DisciplineRequestTrans
	WHERE RequestId = @RequestId

	EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @FromDate
		,@toDate = @ToDate
		,@empIds = @EmpId
		,@isUpdateRequestData = 1
END
