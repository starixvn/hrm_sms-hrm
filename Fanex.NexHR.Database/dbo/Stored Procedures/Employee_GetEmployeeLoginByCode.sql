
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

	SELECT EmpId AS Id
		,Code
		,FullName
		,NickName
		,[Password]
		,[Enabled]
	FROM dbo.Employee WITH (NOLOCK)
	WHERE Code = @code
END
