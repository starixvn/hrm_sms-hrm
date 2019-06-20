
CREATE PROCEDURE [dbo].[LeaveRequest_DeleteKIVLeaveTrans] @RequestId BIGINT
AS
/*
Created: Quinn.Vo@20190613
Task: delete kiv temp leave balance.
DB: HRM
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.KeepInViewByDayTrans WITH (NOLOCK)
			WHERE Requestid = @RequestId
			)
	BEGIN
		DELETE
		FROM dbo.KeepInViewByDayTrans
		WHERE Requestid = @RequestId
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.KeepInViewByHourTrans WITH (NOLOCK)
			WHERE Requestid = @RequestId
			)
	BEGIN
		DELETE
		FROM dbo.KeepInViewByHourTrans
		WHERE RequestId = @RequestId
	END
END