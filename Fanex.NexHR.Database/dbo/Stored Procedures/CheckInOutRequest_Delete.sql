
CREATE PROCEDURE [dbo].[CheckInOutRequest_Delete] @RequestId BIGINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190611@Quinn.Vo
Task: Delete check in/out request.
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

	DELETE dbo.WorkingTimeRequest WITH (ROWLOCK)
	WHERE RequestId = @RequestId

	DELETE dbo.CheckInOutRequest WITH (ROWLOCK)
	WHERE RequestId = @RequestId

	DELETE dbo.CheckInOutRequestHistory WITH (ROWLOCK)
	WHERE RequestId = @RequestId
END
