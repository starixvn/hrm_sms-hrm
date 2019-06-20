
CREATE PROCEDURE [dbo].[DisciplineRequestTrans_Delete] @RequestId BIGINT
AS
/*
Created: 20190426@Quinn.Vo
Task: delete discipline transaction.
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DELETE 
	FROM dbo.DisciplineRequestTrans 
	WHERE RequestId = @RequestId

END