/****** Object:  Stored Procedure dbo.Function_GetFunctions    Script Date: 7/20/2016 10:19:09 AM ******/
/****** Object:  Stored Procedure dbo.Function_GetFunctions    Script Date: 7/19/2016 5:22:15 PM ******/
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
                [Name] AS Name,
                [ParentFunctionId],
                [IsActive]
         FROM dbo.[Function] WITH (NOLOCK)
         WHERE [IsActive] = 1;
     END;
