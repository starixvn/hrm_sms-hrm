
CREATE PROCEDURE [dbo].[AutoJob_JobLevel_ImportFromDB] @SiteId SMALLINT
	,@DbName VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230509@Tim.Trinh
Task: import job level from DB
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @LastModifiedDate DATETIME = '2010-01-01'
		,@SyncDate DATETIME = GETUTCDATE()
		,@sqlExecute NVARCHAR(MAX)

	SELECT @LastModifiedDate = MAX(ModifiedDate)
	FROM dbo.HRM_JobLevel WITH (NOLOCK)
	WHERE SiteId = @SiteId

	SET @LastModifiedDate = ISNULL(@LastModifiedDate, '2010-01-01')

	CREATE TABLE #tmpJobLevel (
		JobLevelId SMALLINT
		,[Name] NVARCHAR(150)
		,IsDeleted BIT
		,ModifiedDate DATETIME
		)

	SELECT @sqlExecute = 'INSERT INTO #tmpJobLevel (
		JobLevelId
		,Name
		,IsDeleted
		,ModifiedDate
		)
	SELECT JobLevelId
		,Name
		,IsDeleted
		,ModifiedDate
	FROM [' + @DbName + '].[dbo].[JobLevel] WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, ''2010-01-01'') > CAST(''' + CONVERT(VARCHAR, @LastModifiedDate, 121) + ''' AS DATETIME)'

	EXEC (@sqlExecute)

	UPDATE jl
	SET jl.[Name] = tmp.[Name]
		,jl.IsDeleted = tmp.IsDeleted
		,jl.ModifiedDate = tmp.ModifiedDate
		,jl.SyncDate = @SyncDate
	FROM dbo.HRM_JobLevel AS jl WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpJobLevel AS tmp WITH (NOLOCK) ON tmp.JobLevelId = jl.JobLevelId
	WHERE jl.SiteId = @SiteId

	INSERT INTO dbo.HRM_JobLevel (
		SiteId
		,JobLevelId
		,[Name]
		,IsDeleted
		,ModifiedDate
		,SyncDate
		)
	SELECT @SiteId
		,tmp.JobLevelId
		,tmp.[Name]
		,tmp.IsDeleted
		,tmp.ModifiedDate
		,@SyncDate
	FROM #tmpJobLevel AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.HRM_JobLevel AS jl WITH (NOLOCK)
			WHERE jl.SiteId = @SiteId
				AND tmp.JobLevelId = jl.JobLevelId
			)

	DROP TABLE #tmpJobLevel
END