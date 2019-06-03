
CREATE PROCEDURE [dbo].[Function_AssignFunctionToRole] @ActorId INT
	,@RoleId INT
	,@PermissionXml VARCHAR(5000) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160609@Razor.Nguyen
Task: assign function permission to role
DB: HRM
Original: 

Revisions:  
Param's Explanation:
	@Functions = <Root><Row FunctionId='{0}' IsRead='{1}' IsHost='{2}' /></Root>
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	CREATE TABLE #tblFunctionPermission (
		FunctionId INT
		,IsRead BIT
		,IsHost BIT
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@PermissionXml

		INSERT INTO #tblFunctionPermission (
			FunctionId
			,IsRead
			,IsHost
			)
		SELECT FunctionId
			,IsRead
			,IsHost
		FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (FunctionId INT, IsRead BIT, IsHost BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH

	DECLARE @ModifiedDate DATETIME = GETDATE()

	BEGIN TRY
		UPDATE rf
		SET rf.IsRead = f.IsRead
			,rf.IsHost = f.IsHost
			,rf.ModifiedBy = @ActorId
			,rf.ModifiedDate = @ModifiedDate
		FROM dbo.RoleFunction AS rf WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblFunctionPermission AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
		WHERE rf.RoleId = @RoleId

		DELETE f
		FROM #tblFunctionPermission AS f
		WHERE EXISTS (
				SELECT 1
				FROM dbo.RoleFunction AS rf WITH (NOLOCK)
				WHERE rf.RoleId = @RoleId
					AND rf.FunctionId = f.FunctionId
				)

		INSERT INTO dbo.RoleFunction (
			RoleId
			,FunctionId
			,IsRead
			,IsHost
			,ModifiedBy
			,ModifiedDate
			)
		SELECT @RoleId
			,FunctionId
			,IsRead
			,IsHost
			,@ActorId
			,@ModifiedDate
		FROM #tblFunctionPermission
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Assign function to role unsuccessfully.'
	END CATCH
END
