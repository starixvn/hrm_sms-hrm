
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_Delete] @RequestId BIGINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190625
Task: Delete late in/early out request.
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest WITH (NOLOCK)
			WHERE RequestId = @RequestId
				AND CurrentStatus = 1
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Status is not pending, you can not delete this request.'

		RETURN;
	END

	DELETE dbo.WorkingTimeRequest WITH (ROWLOCK)
	WHERE RequestId = @RequestId

	DELETE dbo.LateInEarlyOutRequest WITH (ROWLOCK)
	WHERE RequestId = @RequestId
	
	DELETE dbo.LateInEarlyOutRequestHistory WITH (ROWLOCK)
	WHERE RequestId = @RequestId
END
