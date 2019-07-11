
CREATE PROCEDURE [dbo].[Overtime_GetEmployeeToRemindClaim]
AS
/*
Created: Tim.Trinh@2019-07-11
Task: Get employee to remind claim OT
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATETIME = GETDATE()
		,@OvertimeRequestTypeId TINYINT = 8
		,@VerifierRoleId TINYINT = 2
		,@ApproverRoleId TINYINT = 3

	SELECT emp.EmpId AS EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Email
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
	WHERE emp.isDeleted = 0
		AND emp.[Enabled] = 1
		AND ISNULL(emp.ResignedDate, @Today + 30) > @Today
		AND (
			empsum.CanOT = 1
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
				WHERE emp.EmpId = su.OwnerId
					AND ISNULL(su.IsDeleted, 0) = 0
					AND su.RequestType = @OvertimeRequestTypeId
					AND su.ActionType IN (@VerifierRoleId, @ApproverRoleId)
				)
			)
END
