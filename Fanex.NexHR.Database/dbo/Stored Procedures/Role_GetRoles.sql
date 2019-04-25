
CREATE PROCEDURE [dbo].[Role_GetRoles]
AS
/*
Created: 20160609@Razor.Nguyen
Task: get system roles
DB: HRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT CAST(loDa.Value AS INT) AS RoleId
		,loDa.[Name]
		,loDa.Code
	FROM dbo.[Lookup] AS look WITH (NOLOCK)
		INNER JOIN dbo.LookupData AS loDa WITH (NOLOCK) ON loDa.LookupId = look.Id
	WHERE look.Code = 'SR' -- System Role
END
