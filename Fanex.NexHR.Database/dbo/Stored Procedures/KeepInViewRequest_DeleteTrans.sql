
CREATE PROCEDURE [dbo].[KeepInViewRequest_DeleteTrans] 
	@RequestId BIGINT
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

	DELETE FROM dbo.KeepInViewByHourTrans WHERE RequestId = @RequestId

	DELETE FROM dbo.KeepInViewByDayTrans WHERE RequestId = @RequestId
END