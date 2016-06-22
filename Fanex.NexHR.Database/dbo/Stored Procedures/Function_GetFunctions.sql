CREATE PROCEDURE [dbo].[Function_GetFunctions]
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
         SELECT [FunctionId],
                [FunctionKey],
                [Name] AS FunctionName,
                [ParentFunctionId],
                [IsActive]
         FROM dbo.[FunctionKey] WITH (NOLOCK)
         WHERE [IsActive] = 1;
     END;
