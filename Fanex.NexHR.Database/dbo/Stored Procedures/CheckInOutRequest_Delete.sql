
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

	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.WorkingTimeRequest WITH (NOLOCK)
				WHERE RequestID = @RequestID
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

	DELETE dbo.CheckInOutRequest
	WHERE RequestID = @RequestID

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END