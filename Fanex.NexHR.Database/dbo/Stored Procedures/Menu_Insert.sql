CREATE PROCEDURE [dbo].[Menu_Insert] @Name         NVARCHAR(50),
                                     @Text         NVARCHAR(100),
                                     @Link         NVARCHAR(100),
                                     @ParentId     INT,
                                     @FunctionId   INT,
                                     @DisplayOrder INT,
                                     @ErrorCode    INT           = 0 OUTPUT,
                                     @ErrorMessage VARCHAR(200)  = '' OUTPUT
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
         INSERT INTO [dbo].[Menu]
         VALUES
         (@Name,
          @Text,
          @Link,
          @ParentId,
          @FunctionId,
          @DisplayOrder,
          1
         );
     END;