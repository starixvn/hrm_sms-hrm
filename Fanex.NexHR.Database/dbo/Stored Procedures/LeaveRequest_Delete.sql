
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

	SELECT @TotalDaysPaternity = TotalDays
		,@FromDate = FromDate
		,@ToDate = ToDate
		,@EmpId = EmpId
	FROM dbo.LeaveRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId
		AND LeaveType = @PaternityTypeId

	IF ISNULL(@TotalDaysPaternity, 0) = 0
	BEGIN
		SELECT @TotalDaysPaternity = COUNT(1)
			,@FromDate = MIN(OnDate)
			,@ToDate = MAX(OnDate)
			,@EmpId = MIN(EmpId)
		FROM dbo.LeaveRequestDetail WITH (NOLOCK)
		WHERE RequestId = @RequestId
			AND LeaveType = @PaternityTypeId
	END

	IF @TotalDaysPaternity > 0
	BEGIN
		UPDATE dbo.EmployeePaternity WITH (ROWLOCK, UPDLOCK)
		SET BalanceDay = BalanceDay + @TotalDaysPaternity
		WHERE EmpId = @EmpId
			AND @FromDate >= ChildBornDate
			AND @ToDate <= DATEADD(M, 1, ChildBornDate)
	END

	SELECT @EmpId = EmpId
		,@FromDate = FromDate
		,@ToDate = ToDate
	FROM dbo.LeaveRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

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
