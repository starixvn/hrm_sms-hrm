
CREATE PROCEDURE [dbo].[AutoJob_JobTitle_Import] @SiteId SMALLINT
	,@Value NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230508@Tim.Trinh
Task: import job title
DB:
Original:

Revisions:
Param's Explanation:
	- @Value: "<Root><Row JobTitleId='' Name='' IsDeleted='' /></Root>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = GETUTCDATE()

	CREATE TABLE #tmpJobTitle (
		JobTitleId INT
		,[Name] NVARCHAR(200)
		,IsDeleted BIT
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@Value

	INSERT INTO #tmpJobTitle (
		JobTitleId
		,[Name]
		,IsDeleted
		)
	SELECT JobTitleId
		,[Name]
		,ISNULL(IsDeleted, 0)
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (JobTitleId INT, [Name] NVARCHAR(200), IsDeleted BIT) AS Data

	EXEC sp_xml_removedocument @DocHandler

	IF EXISTS (
			SELECT 1
			FROM #tmpJobTitle WITH (NOLOCK)
			)
	BEGIN
		INSERT INTO dbo.HRM_JobTitle (
			SiteId
			,JobTitleId
			,[Name]
			,IsDeleted
			,SyncDate
			)
		SELECT @SiteId
			,tmp.JobTitleId
			,tmp.[Name]
			,tmp.IsDeleted
			,@SyncDate
		FROM #tmpJobTitle AS tmp WITH (NOLOCK)
		WHERE NOT EXISTS (
				SELECT 1
				FROM dbo.HRM_JobTitle AS jt WITH (NOLOCK)
				WHERE tmp.JobTitleId = jt.JobTitleId
					AND jt.SiteId = @SiteId
				)

		UPDATE jt
		SET jt.[Name] = tmp.[Name]
			,jt.IsDeleted = tmp.IsDeleted
			,jt.SyncDate = @SyncDate
		FROM dbo.HRM_JobTitle AS jt WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tmpJobTitle AS tmp WITH (NOLOCK) ON jt.JobTitleId = tmp.JobTitleId
		WHERE jt.SiteId = @SiteId
			AND (jt.[Name] != tmp.[Name] OR jt.IsDeleted != tmp.IsDeleted)
	END

	DROP TABLE #tmpJobTitle
END