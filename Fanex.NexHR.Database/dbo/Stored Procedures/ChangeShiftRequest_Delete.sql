
CREATE PROCEDURE [dbo].[ChangeShiftRequest_Delete] @RequestId BIGINT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-09-17
Task: delete change shift
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @CurrentStatus INT
		-- request status
		,@PendingStatusId TINYINT = 1

	SELECT @CurrentStatus = CurrentStatus
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF @CurrentStatus <> @PendingStatusId
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Status is not pending, you can not delete this request.'

		RETURN
	END

	EXEC dbo.ChangeShiftRequest_ResetCalendar @RequestId = @RequestId
		,@ActorId = @ActorId
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrCode OUTPUT

	DELETE dbo.WorkingTimeRequest WITH (ROWLOCK)
	WHERE RequestId = @RequestId

	DELETE dbo.ChangeShiftRequest WITH (ROWLOCK)
	WHERE RequestId = @RequestId

	DELETE dbo.ChangeShiftRequestHistory WITH (ROWLOCK)
	WHERE RequestId = @RequestId

	DELETE dbo.ChangeShiftRequestDetail WITH (ROWLOCK)
	WHERE RequestId = @RequestId

	DELETE dbo.ChangeShiftRequestDetailHistory WITH (ROWLOCK)
	WHERE RequestId = @RequestId
END
