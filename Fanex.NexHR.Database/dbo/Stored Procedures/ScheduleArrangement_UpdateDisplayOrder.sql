
CREATE PROCEDURE [dbo].[ScheduleArrangement_UpdateDisplayOrder] @ActorId INT
	,@DisplayOrderXml VARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-04-17
Task: Update display order
DB: HRM
Original:

Revisions:
Param's Explanation:
    @DisplayOrderXml=N'<Root><Row EmployeeId="1" DisplayOrder="1" /></Root>

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
		,@DocHandler INT

	CREATE TABLE #tblDisplayOrder (
		EmployeeId INT
		,DisplayOrder INT
		)

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@DisplayOrderXml

		INSERT INTO #tblDisplayOrder (
			EmployeeId
			,DisplayOrder
			)
		SELECT EmployeeId
			,DisplayOrder
		FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (EmployeeId INT, DisplayOrder INT) AS Data

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
	FROM dbo.ScheduleArrangement AS sa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblDisplayOrder AS tbl ON sa.EmployeeId = tbl.EmployeeId
		AND ISNULL(sa.DisplayOrder, 0) <> tbl.DisplayOrder

	DELETE tbl
	FROM #tblDisplayOrder AS tbl
	WHERE EXISTS (
			SELECT 1
			FROM dbo.ScheduleArrangement AS sa WITH (NOLOCK)
			WHERE sa.EmployeeId = tbl.EmployeeId
				AND sa.DisplayOrder = tbl.DisplayOrder
			)

	INSERT INTO dbo.ScheduleArrangement (
		EmployeeId
		,DisplayOrder
		,ModifiedBy
		,ModifiedDate
		)
	SELECT EmployeeId
		,DisplayOrder
		,@ActorId
		,@ModifiedDate
	FROM #tblDisplayOrder

	DROP TABLE #tblDisplayOrder
END