/****** Object:  Stored Procedure dbo.Menu_GetMenus    Script Date: 7/20/2016 10:19:12 AM ******/
/****** Object:  Stored Procedure dbo.Menu_GetMenus    Script Date: 7/19/2016 5:22:17 PM ******/
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
                me.DisplayOrder,
                me.IsActive,
                f.FunctionId,
                f.FunctionKey,
                f.Name,
                meP.MenuId,
                meP.Name,
			 meP.[Text]
         FROM dbo.Menu AS me WITH (NOLOCK)
         LEFT JOIN dbo.[Function] AS f WITH(NOLOCK) ON me.FunctionId = f.FunctionId
         LEFT JOIN dbo.Menu AS meP WITH (NOLOCK) ON meP.MenuId = me.ParentId
     END;