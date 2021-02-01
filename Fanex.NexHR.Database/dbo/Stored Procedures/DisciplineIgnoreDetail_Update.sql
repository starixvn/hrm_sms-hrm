CREATE PROCEDURE [dbo].[DisciplineIgnoreDetail_Update]
	@ActorId INT
	,@IgnoreDetailXml NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20210127@Lilac.Nguyen
Task: Update Discipline Ignore Detail
DB: HRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	CREATE TABLE #tblIgnoredDisciplineDetail (
		EmpId INT
		,WorkDate DATE
		,DisciplineTypeId INT
		,IgnoreBy INT
		,IgnoreDate DATE
		,IsIgnored BIT
		)

	IF ISNULL(@IgnoreDetailXml, '') <> ''
	BEGIN
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@IgnoreDetailXml

		INSERT INTO #tblIgnoredDisciplineDetail(
			EmpId
			,WorkDate
			,DisciplineTypeId
			,IgnoreBy
			,IgnoreDate
			,IsIgnored
			)
		SELECT
			EmployeeId
			,WorkDate
			,DisciplineTypeId
			,@ActorId
			,GETDATE()
			,IsIgnored
		FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (WorkDate DATE, EmployeeId INT, DisciplineTypeId INT, IsIgnored BIT) AS Data

		INSERT INTO dbo.DisciplineIgnoreDetail(
		WorkDate
		,EmpId
		,DisciplineTypeId
		,IgnoreBy
		,IgnoreDate
		)
		SELECT 
		WorkDate
		,EmpId
		,DisciplineTypeId
		,IgnoreBy
		,IgnoreDate
		FROM #tblIgnoredDisciplineDetail 
		WHERE IsIgnored = 1
			

		DELETE DisciplineIgnoreDetail
		FROM dbo.DisciplineIgnoreDetail d1
		INNER JOIN #tblIgnoredDisciplineDetail d2 ON d1.EmpId = d2.EmpId AND d1.DisciplineTypeId = d2.DisciplineTypeId AND d1.WorkDate = d2.WorkDate
		WHERE d2.IsIgnored = 0;

		EXEC sp_xml_removedocument @DocHandler
	END

	DROP TABLE #tblIgnoredDisciplineDetail

END