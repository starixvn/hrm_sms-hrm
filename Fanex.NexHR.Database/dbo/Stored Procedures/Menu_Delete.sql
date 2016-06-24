CREATE PROCEDURE [dbo].[Menu_Delete] @MenuId INT
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
         DELETE FROM [dbo].[Menu] WHERE [MenuId] = @MenuId
     END;