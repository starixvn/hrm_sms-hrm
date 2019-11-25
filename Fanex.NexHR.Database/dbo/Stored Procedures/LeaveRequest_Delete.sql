
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
END
