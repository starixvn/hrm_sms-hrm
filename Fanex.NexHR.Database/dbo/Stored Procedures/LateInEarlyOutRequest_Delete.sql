
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

	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.WorkingTimeRequest WITH (NOLOCK)
				WHERE RequestID = @RequestId
					AND CurrentStatus = 1
				)
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'This Status Request have been changed. You cannot delete. '

		RETURN;
	END

	DELETE dbo.WorkingTimeRequest
	WHERE RequestID = @RequestID

	DELETE dbo.LateInEarlyOutRequest
	WHERE RequestID = @RequestId

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END