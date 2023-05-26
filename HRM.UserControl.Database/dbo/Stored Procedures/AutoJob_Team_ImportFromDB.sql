
CREATE PROCEDURE [dbo].[AutoJob_Team_ImportFromDB] @SiteId SMALLINT
	,@DbName VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230509@Tim.Trinh
Task: import team from DB
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
	FROM dbo.HRM_Team WITH (NOLOCK)
	WHERE SiteId = @SiteId

	SET @LastModifiedDate = ISNULL(@LastModifiedDate, '2010-01-01')

	CREATE TABLE #tmpTeam (
		TeamId INT
		,Code NVARCHAR(50)
		,[Name] NVARCHAR(200)
		,SectionId INT
		,IsDeleted BIT
		,ModifiedDate DATETIME
		)

	SELECT @sqlExecute = 'INSERT INTO #tmpTeam (
		TeamId
		,Code
		,[Name]
		,SectionId
		,IsDeleted
		,ModifiedDate
		)
	SELECT GroupId
		,Code
		,[Name]
		,DepartmentId
		,IsDeleted
		,ModifiedDate 
	FROM [' + @DbName + '].[dbo].[Group] WITH (NOLOCK)
	WHERE ISNULL(ModifiedDate, ''2010-01-01'') > CAST(''' + CONVERT(VARCHAR, @LastModifiedDate, 121) + ''' AS DATETIME)'

	EXEC (@sqlExecute)

	UPDATE team
	SET team.[Name] = tmp.[Name]
		,team.Code = tmp.Code
		,team.SectionId = tmp.SectionId
		,team.IsDeleted = ISNULL(tmp.IsDeleted, 0)
		,team.ModifiedDate = tmp.ModifiedDate
		,SyncDate = @SyncDate
	FROM dbo.HRM_Team AS team WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpTeam AS tmp WITH (NOLOCK) ON team.TeamId = tmp.TeamId
	WHERE team.SiteId = @SiteId

	INSERT INTO dbo.HRM_Team (
		SiteId
		,TeamId
		,[Name]
		,Code
		,SectionId
		,IsDeleted
		,ModifiedDate
		,SyncDate
		)
	SELECT @SiteId
		,tmp.TeamId
		,tmp.[Name]
		,tmp.Code
		,tmp.SectionId
		,ISNULL(tmp.IsDeleted, 0)
		,tmp.ModifiedDate
		,@SyncDate
	FROM #tmpTeam AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.HRM_Team AS team WITH (NOLOCK)
			WHERE team.SiteId = @SiteId
				AND team.TeamId = tmp.TeamId
			)

	DROP TABLE #tmpTeam
END