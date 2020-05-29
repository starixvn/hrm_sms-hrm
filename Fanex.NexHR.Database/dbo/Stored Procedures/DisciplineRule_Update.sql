
CREATE PROCEDURE [dbo].[DisciplineRule_Update] @ActorId INT
	,@GroupId INT
	,@DisciplineRuleXml VARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-11-05
Task: apply discipline rule for section
DB: HRM
Original: 

Revisions:  
Param's Explanation:
	@Functions = <Root><Row DisciplineTypeId='{0}' IsApplyForLocal='{1}' IsApplyForExpat='{2}' /></Root>
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @tblDisciplineRule TABLE (
		DisciplineTypeId SMALLINT
		,IsApplyForLocal BIT
		,IsApplyForExpat BIT
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@DisciplineRuleXml

		INSERT INTO @tblDisciplineRule (
			DisciplineTypeId
			,IsApplyForLocal
			,IsApplyForExpat
			)
		SELECT DisciplineTypeId
			,IsApplyForLocal
			,IsApplyForExpat
		FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (DisciplineTypeId SMALLINT, IsApplyForLocal BIT, IsApplyForExpat BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		DELETE
		FROM dbo.DisciplineGroupRule WITH (ROWLOCK)
		WHERE GroupId = @GroupId

		-- local
		INSERT INTO dbo.DisciplineGroupRule (
			GroupId
			,DisciplineTypeId
			,IsLocal
			,Value
			)
		SELECT @GroupId
			,DisciplineTypeId
			,1
			,IsApplyForLocal
		FROM @tblDisciplineRule

		-- expat
		INSERT INTO dbo.DisciplineGroupRule (
			GroupId
			,DisciplineTypeId
			,IsLocal
			,Value
			)
		SELECT @GroupId
			,DisciplineTypeId
			,0
			,IsApplyForExpat
		FROM @tblDisciplineRule

		DECLARE @logdate DATETIME = GETDATE()

		EXEC dbo.Log_WriteLog @LogActionId = 1003
			,@LogDate = @logdate
			,@Description = 'Update discipline rule'
			,@TrackingValue = @DisciplineRuleXml
			,@UserId = @ActorId
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Updated unsuccessfully.'
	END CATCH
END
