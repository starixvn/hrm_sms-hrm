CREATE PROCEDURE [dbo].[Function_GetFunctionsByUserId] @UserId INT
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
         SELECT f.FunctionId,
                f.FunctionKey,
                f.Name,
                CAST(MAX(IsRead) AS BIT) AS IsRead,
                CAST(MAX(IsHost) AS BIT) AS IsHost
         FROM
         (
             SELECT RoleId,
                    FunctionId,
                    CAST(IsRead AS INT) AS IsRead,
                    CAST(IsHost AS INT) AS IsHost
             FROM dbo.RoleFunction WITH (NOLOCK)
         ) AS rf
         INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON f.FunctionId = rf.FunctionId
         INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON rf.RoleId = er.RoleId
         WHERE er.EmployeeId = @UserId
         GROUP BY f.FunctionId,
                  f.FunctionKey,
                  f.Name
     END;
