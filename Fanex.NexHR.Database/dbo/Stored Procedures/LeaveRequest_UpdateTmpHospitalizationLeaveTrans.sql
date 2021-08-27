
CREATE PROCEDURE [dbo].[LeaveRequest_UpdateTmpHospitalizationLeaveTrans] @RequestId BIGINT
	,@Status INT = 1
AS
/*
Created: Tim.Trinh@2021-08-24
Task: update temp hospitalization leave balance.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.tmpHospitalizationLeaveBalance WITH (ROWLOCK, UPDLOCK)
	SET [Status] = @Status
	WHERE RequestId = @RequestId
END