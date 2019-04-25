
CREATE PROCEDURE [dbo].[ScheduleArrangement_UpdateGroupOrder] @ActorId INT
	,@DisplayOrderXml VARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-04-22
Task: Update group order
DB: HRM
Original:

Revisions:
Param's Explanation:
    @DisplayOrderXml=N'<Root><Row GroupId="1" DisplayOrder="1" /></Root>

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
		,@DocHandler INT

	CREATE TABLE #tblDisplayOrder (
		GroupId INT
		,DisplayOrder SMALLINT
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@DisplayOrderXml

		INSERT INTO #tblDisplayOrder (
			GroupId
			,DisplayOrder
			)
		SELECT GroupId
			,DisplayOrder
		FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (GroupId INT, DisplayOrder SMALLINT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH;

	UPDATE sa
	SET sa.DisplayOrder = tbl.DisplayOrder
		,sa.ModifiedBy = @ActorId
		,ModifiedDate = @ModifiedDate
	FROM dbo.ScheduleArrangementGroupOrder AS sa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblDisplayOrder AS tbl ON sa.GroupId = tbl.GroupId
		AND ISNULL(sa.DisplayOrder, 0) <> tbl.DisplayOrder

	DELETE tbl
	FROM #tblDisplayOrder AS tbl
	WHERE EXISTS (
			SELECT 1
			FROM dbo.ScheduleArrangementGroupOrder AS sa WITH (NOLOCK)
			WHERE sa.GroupId = tbl.GroupId
				AND sa.DisplayOrder = tbl.DisplayOrder
			)

	INSERT INTO dbo.ScheduleArrangementGroupOrder (
		GroupId
		,DisplayOrder
		,ModifiedBy
		,ModifiedDate
		)
	SELECT GroupId
		,DisplayOrder
		,@ActorId
		,@ModifiedDate
	FROM #tblDisplayOrder

	DROP TABLE #tblDisplayOrder
END