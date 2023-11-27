
CREATE PROCEDURE [dbo].[Employee_GetByUserId] @UserId INT
AS
/*
Created: 20220111@Tim.Trinh
Task: get employee by userid
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.EmpId
		,Code
		,FullName
		,NickName
		,[Password]
		,[Enabled]
		,empsum.IsTopManager
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
	WHERE emp.UserId = @UserId
END