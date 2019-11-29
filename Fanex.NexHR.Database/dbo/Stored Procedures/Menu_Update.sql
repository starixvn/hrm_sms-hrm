CREATE PROCEDURE [dbo].[Menu_Update] @MenuId       INT,
                                    @Name         NVARCHAR(50),
                                    @Text         NVARCHAR(100) = NULL,
                                    @Link         NVARCHAR(100) = NULL,
                                    @ParentId     INT = NULL,
                                    @FunctionId   INT = NULL,
                                    @DisplayOrder INT = NULL,
                                    @IsActive     BIT,
                                    @ErrCode    INT           = 0 OUTPUT,
                                    @ErrMsg VARCHAR(200)  = '' OUTPUT
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
         UPDATE [dbo].[Menu]
           SET
               [Name] = @Name,
               [Text] = @Text,
               [Link] = @Link,
               [ParentId] = @ParentId,
               [FunctionId] = @FunctionId,
               [DisplayOrder] = @DisplayOrder,
               [IsActive] = @IsActive
         WHERE MenuId = @MenuId;
     END;