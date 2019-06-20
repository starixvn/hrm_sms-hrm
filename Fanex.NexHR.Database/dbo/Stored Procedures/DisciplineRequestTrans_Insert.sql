
CREATE PROCEDURE [dbo].[DisciplineRequestTrans_Insert] @RequestId BIGINT
	,@ModifiedBy INT
	,@IgnoredDetails NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190426@Quinn.Vo
Task: insert discipline request transaction.
DB: HRM
Original: 

Revisions:
Param's Explanation:
    @IgnoredDetails: 
        <ROOT>
            <Discipline WorkDate='' EmployeeId='' DisciplineTypeId='' IsIgnored=''></Discipline>
        </ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #IgnoredDisciplineDetail (
		WorkDate DATE
		,EmployeeId INT
		,DisciplineTypeId INT
		,IsIgnored BIT
		)

	IF ISNULL(@IgnoredDetails, '') <> ''
	BEGIN
		BEGIN TRY
			DECLARE @DocHandler INT

			EXEC sp_xml_preparedocument @DocHandler OUTPUT
				,@IgnoredDetails

			INSERT INTO #IgnoredDisciplineDetail
			SELECT DISTINCT WorkDate
				,EmployeeId
				,DisciplineTypeId
				,IsIgnored
			FROM OPENXML(@DocHandler, '/ROOT/Discipline', 1) WITH (WorkDate DATE, EmployeeId INT, DisciplineTypeId INT, IsIgnored BIT) AS Data

			EXEC sp_xml_removedocument @DocHandler
		END TRY

		BEGIN CATCH
			SELECT @ErrCode = 1
				,@ErrMsg = N'Parameter invalid.'

			RETURN
		END CATCH
	END

	IF EXISTS (
			SELECT 1
			FROM #IgnoredDisciplineDetail
			)
	BEGIN
		

		INSERT INTO dbo.DisciplineRequestTrans (
			EmpId
			,WorkDate
			,RequestId
			,DisciplineTypeId
			,IsIgnored
			,IsOutOfControl
			,ModifiedDate
			)
		SELECT EmployeeId
			,WorkDate
			,@RequestId
			,DisciplineTypeId
			,IsIgnored
			,0
			,GETDATE()
		FROM #IgnoredDisciplineDetail
	END

	DROP TABLE #IgnoredDisciplineDetail
END