
CREATE PROCEDURE [dbo].[AutoJob_JobTitle_ImportFromDB] @SiteId SMALLINT
	,@DbName VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230509@Tim.Trinh
Task: import job title from DB
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = GETUTCDATE()
		,@sqlExecute NVARCHAR(MAX)

	CREATE TABLE #tmpJobTitle (
		JobTitleId SMALLINT
		,[Name] NVARCHAR(200)
		)

	SELECT @sqlExecute = 'INSERT INTO #tmpJobTitle (
		JobTitleId
		,[Name]
		)
	SELECT Id
		,[Name]
	FROM [' + @DbName + '].[dbo].[HRMJobTitle] WITH (NOLOCK)'

	EXEC (@sqlExecute)

	UPDATE jt
	SET jt.[Name] = tmp.[Name]
		,jt.SyncDate = @SyncDate
	FROM dbo.HRM_JobTitle AS jt WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpJobTitle AS tmp WITH (NOLOCK) ON tmp.JobTitleId = jt.JobTitleId
	WHERE jt.SiteId = @SiteId

	INSERT INTO dbo.HRM_JobTitle (
		SiteId
		,JobTitleId
		,[Name]
		,SyncDate
		)
	SELECT @SiteId
		,tmp.JobTitleId
		,tmp.[Name]
		,@SyncDate
	FROM #tmpJobTitle AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.HRM_JobTitle AS jt WITH (NOLOCK)
			WHERE jt.SiteId = @SiteId
				AND tmp.JobTitleId = jt.JobTitleId
			)

	DROP TABLE #tmpJobTitle
END