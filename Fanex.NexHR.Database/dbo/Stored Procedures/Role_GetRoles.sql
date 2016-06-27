CREATE PROCEDURE [dbo].[Role_GetRoles]
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         SELECT CAST(loDa.Value AS INT) AS RoleId,
                loDa.NAME AS RoleName,
                loDa.Code AS RoleCode
         FROM dbo.[Lookup] AS look WITH (NOLOCK)
              INNER JOIN dbo.LookupData loDa WITH (NOLOCK) ON loDa.LookupId = look.Id
         WHERE look.Code = 'SR'; -- System Role
     END;
