CREATE PROCEDURE [dbo].[Function_AssignFunctionToRole] @RoleId     INT,
                                                       @Functions  VARCHAR(5000) = '',
                                                       @ModifiedBy INT           = NULL,
                                                       @ErrorCode    INT           = 0 OUTPUT,
                                                       @ErrorMessage     VARCHAR(200)  = '' OUTPUT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
	@Functions = <ROOT><Function FunctionId='{0}' IsRead='{1}' IsEdit='{2}' IsCreate='{3}' IsDelete='{4}'></Function></ROOT>
*/

     BEGIN
         SET NOCOUNT ON;
         CREATE TABLE #Functions
         (FunctionId INT,
          IsRead     BIT,
          IsEdit     BIT,
		IsCreate     BIT,
		IsDelete     BIT
         );
         BEGIN TRY
             DECLARE @DocHandler INT;
             EXEC sp_xml_preparedocument
                  @DocHandler OUTPUT,
                  @Functions;
                  
             INSERT INTO #Functions
             (FunctionId,
              IsRead,
              IsEdit,
		    IsCreate,
		    IsDelete
             )
                    SELECT FunctionId,
                           IsRead,
                           IsEdit,
					  IsCreate,
					  IsDelete
                    FROM OPENXML(@DocHandler, '/ROOT/Function', 1) WITH(FunctionId INT, IsRead BIT, IsEdit BIT, IsCreate BIT, IsDelete BIT) AS Data;

             EXEC sp_xml_removedocument
                  @DocHandler;

         END TRY
         BEGIN CATCH
             SELECT @ErrorCode = 1,
                    @ErrorMessage = N'Evaluate failed. Paramter invalid.';
             RETURN;
         END CATCH;
         DECLARE @ModifiedDate DATETIME;
         SET @ModifiedDate = GETDATE();
         BEGIN TRY
             BEGIN TRANSACTION;
             UPDATE rf
               SET
                   rf.IsRead = f.IsRead,
                   rf.IsEdit = f.IsEdit,
			    rf.IsCreate = f.IsCreate,
			    rf.IsDelete = f.IsDelete
             FROM dbo.[RoleFunction] AS rf WITH (ROWLOCK, UPDLOCK)
                  INNER JOIN #Functions AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
             WHERE rf.RoleId = @RoleId;
             DELETE f
             FROM #Functions AS f
             WHERE EXISTS
             (
                 SELECT 1
                 FROM dbo.RoleFunction AS rf WITH (NOLOCK)
                 WHERE rf.RoleId = @RoleId
                       AND rf.FunctionId = f.FunctionId
             );
             INSERT INTO dbo.[RoleFunction]
             ([RoleId],
              [FunctionId],
              [IsRead],
              [IsEdit],
		    [IsCreate],
		    [IsDelete],
              [ModifiedBy],
              [ModifiedDate]
             )
                    SELECT @RoleId,
                           FunctionId,
                           IsRead,
                           IsEdit,
					  IsCreate,
					  IsDelete,
                           @ModifiedBy,
                           @ModifiedDate
                    FROM #Functions;
             COMMIT TRANSACTION;
         END TRY
         BEGIN CATCH
             ROLLBACK TRANSACTION;
             SELECT @ErrorCode = 10,
                    @ErrorMessage = N'Assign function to role unsuccessfully.';
         END CATCH;
     END;

