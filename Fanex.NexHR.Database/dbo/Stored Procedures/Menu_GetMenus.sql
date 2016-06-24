CREATE PROCEDURE [dbo].[Menu_GetMenus]
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
         SELECT DISTINCT
                me.MenuId,
                me.[Name],
                me.[Text],
                me.Link,
                me.ParentId,
                me.FunctionId,
                me.DisplayOrder,
                me.IsActive
         FROM dbo.Menu AS me WITH (NOLOCK);
     END;