
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

	DECLARE @CurrentStatus INT

	SELECT TOP 1 @CurrentStatus = CurrentStatus
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestID = @RequestID

	IF (@CurrentStatus <> 1)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'This Status Request have been changed. You cannot delete. '

		RETURN;
	END

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE dbo.WorkingTimeRequest
		WHERE RequestID = @RequestID

		DELETE dbo.LeaveRequest
		WHERE RequestID = @RequestID

		DELETE dbo.LeaveRequestDetail
		WHERE RequestID = @RequestID

		DELETE
		FROM dbo.tmpLeaveBalance
		WHERE RequestID = @RequestID

		DELETE
		FROM dbo.KeepInViewByDayTrans
		WHERE RequestID = @RequestID

		DELETE
		FROM dbo.KeepInViewByHourTrans
		WHERE RequestID = @RequestID

		DELETE
		FROM dbo.WeekendLeaveTrans
		WHERE RequestID = @RequestID

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 101
			,@ErrMsg = N'Delete request failed.'
	END CATCH;

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END