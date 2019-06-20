
CREATE PROCEDURE [dbo].[LeaveRequest_DeleteWeekendLeaveTrans] @RequestId BIGINT
AS
/*
Created: 20190411@Quinn.Vo
Task: Delete weekend leave transaction.
DB: HRM
Original: 

Revisions: 
*/
BEGIN
	SET NOCOUNT ON;

	DELETE
	FROM dbo.WeekendLeaveTrans
	WHERE Requestid = @RequestId
END