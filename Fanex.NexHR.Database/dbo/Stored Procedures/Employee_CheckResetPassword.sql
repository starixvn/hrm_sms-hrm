
CREATE PROCEDURE [dbo].[Employee_CheckResetPassword] @EmployeeId INT
AS
/*
Created: Tim.Trinh@2019-06-18
Task: check reset password
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsReset BIT = 0

	SELECT @IsReset = PasswordReset
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmpId = @EmployeeId
	
	SELECT @IsReset
END