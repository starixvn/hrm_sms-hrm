
CREATE PROCEDURE [dbo].[KeepInViewRequest_DeleteTrans] @RequestId BIGINT
AS
/*
Created: 20190522@Quinn.Vo
Task: delte KIV transaction.
DB: HRM
Original: 

Revisions: 
*/
BEGIN
	SET NOCOUNT ON;

	DELETE
	FROM dbo.KeepInViewByHourTrans WITH (ROWLOCK, UPDLOCK)
	WHERE RequestId = @RequestId

	DELETE
	FROM dbo.KeepInViewByDayTrans WITH (ROWLOCK, UPDLOCK)
	WHERE RequestId = @RequestId
END