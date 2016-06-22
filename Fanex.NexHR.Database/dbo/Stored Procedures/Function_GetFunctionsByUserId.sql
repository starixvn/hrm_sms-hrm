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
                CAST(MAX(IsRead) AS BIT) AS IsRead,
                CAST(MAX(IsEdit) AS BIT) AS IsEdit,
			 CAST(MAX(IsCreate) AS BIT) AS IsCreate,
			 CAST(MAX(IsDelete) AS BIT) AS IsDelete
         FROM
         (
             SELECT RoleId,
                    FunctionId,
                    CAST(IsRead AS INT) AS IsRead,
                    CAST(IsEdit AS INT) AS IsEdit,
				CAST(IsCreate AS INT) AS IsCreate,
				CAST(IsDelete AS INT) AS IsDelete
             FROM dbo.RoleFunction WITH (NOLOCK)
         ) AS rf
         INNER JOIN dbo.FunctionKey AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
         INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON rf.RoleId = er.RoleId
         WHERE er.EmployeeId = @UserId
         GROUP BY f.FunctionId,
                  FunctionKey;
     END;
