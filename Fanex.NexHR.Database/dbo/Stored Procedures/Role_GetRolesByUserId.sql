
CREATE PROCEDURE [dbo].[Role_GetRolesByUserId] @UserId INT
AS
/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SMS-HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT emRo.RoleId
		,loDa.Code
		,loDa.[Name]
	FROM dbo.EmployeeRole AS emRo WITH (NOLOCK)
		INNER JOIN dbo.LookupData loDa WITH (NOLOCK) ON emRo.RoleId = loDa.Value
		INNER JOIN dbo.[Lookup] look WITH (NOLOCK) ON look.Id = loDa.LookupId
	WHERE emRO.EmployeeId = @UserId
		AND look.Code = 'SR' -- System Role
END
