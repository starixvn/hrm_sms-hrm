CREATE PROCEDURE [dbo].[DisciplineIgnoreDetail_Update] @ActorId INT
	,@DisciplineIgnoreXml NVARCHAR(MAX) = NULL
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

	IF ISNULL(@DisciplineIgnoreXml, '') <> ''
	BEGIN
		DECLARE @DocHandler INT

		CREATE TABLE #tblIgnoredDetail (
			WorkDate DATE
			,EmpId INT
			,DisciplineTypeId INT
			,IgnoreBy INT
			,IgnoreDate DATETIME
			,IsIgnored BIT
			)

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@DisciplineIgnoreXml

		INSERT INTO #tblIgnoredDetail (
			WorkDate
			,EmpId
			,DisciplineTypeId
			,IgnoreBy
			,IgnoreDate
			,IsIgnored
			)
		SELECT WorkDate
			,EmployeeId
			,DisciplineTypeId
			,@ActorId
			,GETDATE()
			,IsIgnored
		FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (WorkDate DATE, EmployeeId INT, DisciplineTypeId INT, IsIgnored BIT) AS Data

		EXEC sp_xml_removedocument @DocHandler

		INSERT INTO dbo.DisciplineIgnoreDetail (
			WorkDate
			,EmpId
			,DisciplineTypeId
			,IgnoreBy
			,IgnoreDate
			)
		SELECT WorkDate
			,EmpId
			,DisciplineTypeId
			,IgnoreBy
			,IgnoreDate
		FROM #tblIgnoredDetail WITH (NOLOCK)
		WHERE IsIgnored = 1

		DELETE d1
		FROM dbo.DisciplineIgnoreDetail AS d1 WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblIgnoredDetail AS d2 WITH (NOLOCK) ON d1.EmpId = d2.EmpId
			AND d1.DisciplineTypeId = d2.DisciplineTypeId
			AND d1.WorkDate = d2.WorkDate
			AND d2.IsIgnored = 0

		DROP TABLE #tblIgnoredDetail
	END
END
