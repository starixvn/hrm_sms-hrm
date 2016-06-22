CREATE PROCEDURE [dbo].[Function_GetFunctionsByRoleId] @RoleId INT
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
         SELECT fk.[FunctionId],
                fk.[FunctionKey],
                fk.[Name] AS FunctionName,
                fk.[ParentFunctionId],
                rf.[IsRead],
                rf.[IsEdit],
			 rf.[IsCreate],
			 rf.[IsDelete]
         FROM dbo.[FunctionKey] AS fk WITH (NOLOCK)
              LEFT JOIN
         (
             SELECT [FunctionId],
                    [IsRead],
                    [IsEdit],
				[IsCreate],
				[IsDelete]
             FROM dbo.[RoleFunction] WITH (NOLOCK)
             WHERE [RoleId] = @RoleId
         ) AS rf ON fk.[FunctionId] = rf.[FunctionId]
         WHERE fk.[IsActive] = 1;
     END;
