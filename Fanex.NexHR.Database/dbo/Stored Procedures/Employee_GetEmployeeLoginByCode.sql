
CREATE PROCEDURE [dbo].[Employee_GetEmployeeLoginByCode] @Code VARCHAR(50)
AS
/*
Created: Tim.Trinh@2019-06-05
Task: get employee login by code
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT emp.EmpId AS Id
		,Code
		,FullName
		,NickName
		,[Password]
		,[Enabled]
		,empsum.IsTopManager
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
	WHERE Code = @code
END
