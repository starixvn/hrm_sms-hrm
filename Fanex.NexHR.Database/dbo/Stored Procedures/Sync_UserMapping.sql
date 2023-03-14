
CREATE PROCEDURE [dbo].[Sync_UserMapping] @UserMappingXml NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20221124@Otis.Vu
Task: update userId and syncdate
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:
-@UUserMappingXml
	<ROOT>
		<Row UserId="5" RefId="527" NewUserId="19" ModifiedDate="2022-11-22 02:12:29.857" />
		<Row UserId="9" RefId="1247" NewUserId="97" ModifiedDate="2022-11-20 02:12:29.857" />
		<Row UserId="4" RefId="452" NewUserId="0" ModifiedDate="2022-11-21 02:12:29.857" />
	</ROOT>

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @idoc INT
		,@syncDate DATETIME = GETDATE()
		,@xml NVARCHAR(MAX)

	CREATE TABLE #tblUserMapping (
		UserId INT
		,RefId INT
		,NewUserId INT NULL
		,ModifiedDate DATETIME
		)

	CREATE TABLE #tblUpdatedEmployee (
		EmpId INT
		,OldUserId INT
		,NewUserId INT
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@UserMappingXml

	INSERT INTO #tblUserMapping (
		UserId
		,RefId
		,NewUserId
		,ModifiedDate
		)
	SELECT UserId
		,RefId
		,NewUserId
		,ModifiedDate
	FROM OPENXML(@idoc, '/ROOT/Row', 1) WITH (UserId INT, RefId INT, NewUserId INT, ModifiedDate DATETIME) AS Data

	UPDATE emp
	SET emp.UserId = ISNULL(us.NewUserId, us.UserId)
		,emp.ModifiedDate = @syncDate
		,emp.UC_ModifiedDate = us.ModifiedDate
		,emp.UC_SyncDate = @syncDate
	OUTPUT INSERTED.EmpId
		,DELETED.UserId
		,INSERTED.UserId
	INTO #tblUpdatedEmployee(EmpId, OldUserId, NewUserId)
	FROM dbo.Employee AS emp WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblUserMapping AS us WITH (NOLOCK) ON emp.EmpId = us.RefId
	WHERE emp.UserId != ISNULL(us.NewUserId, us.UserId)

	SET @xml = CAST((
				SELECT EmpId AS '@EmpId'
					,OldUserId AS '@OldUserId'
					,NewUserId AS '@NewUserId'
				FROM #tblUpdatedEmployee WITH (NOLOCK)
				FOR XML PATH('Row')
					,ROOT('Root')
				) AS NVARCHAR(MAX))

	EXEC dbo.Log_WriteLog @LogActionId = 1004
		,@LogDate = @SyncDate
		,@Description = 'Sync user mapping'
		,@TrackingValue = @xml
		,@UserId = - 1

	DROP TABLE #tblUserMapping

	DROP TABLE #tblUpdatedEmployee
END