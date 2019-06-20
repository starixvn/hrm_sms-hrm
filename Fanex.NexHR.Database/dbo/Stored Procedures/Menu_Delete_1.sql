/****** Object:  Stored Procedure dbo.Menu_Delete    Script Date: 7/20/2016 10:19:12 AM ******/
/****** Object:  Stored Procedure dbo.Menu_Delete    Script Date: 7/19/2016 5:22:17 PM ******/
CREATE PROCEDURE [dbo].[Menu_Delete] @MenuId       INT,
                                     @ErrorCode    INT          = 0 OUTPUT,
                                     @ErrorMessage VARCHAR(200) = '' OUTPUT
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
         DELETE FROM [dbo].[Menu]
         WHERE [MenuId] = @MenuId;
     END;