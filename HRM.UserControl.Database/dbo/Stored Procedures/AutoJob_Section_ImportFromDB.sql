
CREATE PROCEDURE [dbo].[AutoJob_Section_ImportFromDB] @SiteId SMALLINT
	,@DbName VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230509@Tim.Trinh
Task: import section from DB
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
	FROM dbo.HRM_Section WITH (NOLOCK)
	WHERE SiteId = @SiteId

	SET @LastModifiedDate = ISNULL(@LastModifiedDate, '2010-01-01')	

	CREATE TABLE #tmpSection (
		SectionId INT
		,Code NVARCHAR(50)
		,[Name] NVARCHAR(200)
		,DepartmentId INT
		,IsActive BIT
		,IsDeleted BIT
		,ModifiedDate DATETIME
		)

	SELECT @sqlExecute = 'INSERT INTO #tmpSection (
		SectionId
		,Code
		,[Name]
		,DepartmentId
		,IsActive
		,ModifiedDate
		)
	SELECT DISTINCT section.DepartmentId
		,section.Code
		,section.[Name]
		,section.ParentId
		,section.IsActive
		,section.ModifiedDate 
	FROM [' + @DbName + '].[dbo].[Department] AS section WITH (NOLOCK)
		INNER JOIN [' + @DbName + '].[dbo].[Group] AS team WITH (NOLOCK) ON section.DepartmentId = team.DepartmentId
	WHERE ISNULL(section.ModifiedDate, ''2010-01-01'') > CAST(''' + CONVERT(VARCHAR, @LastModifiedDate, 121) + ''' AS DATETIME)'

	EXEC (@sqlExecute)

	UPDATE section
	SET section.[Name] = tmp.[Name]
		,section.Code = tmp.Code
		,section.SectionId = tmp.SectionId
		,section.IsActive = ISNULL(tmp.IsActive, 0)
		,section.IsDeleted = ISNULL(tmp.IsDeleted, 0)
		,section.ModifiedDate = tmp.ModifiedDate
		,section.SyncDate = @SyncDate
	FROM dbo.HRM_Section AS section WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpSection AS tmp WITH (NOLOCK) ON section.SectionId = tmp.SectionId
	WHERE section.SiteId = @SiteId

	INSERT INTO dbo.HRM_Section (
		SiteId
		,SectionId
		,[Name]
		,Code
		,DepartmentId
		,IsActive
		,IsDeleted
		,ModifiedDate
		,SyncDate
		)
	SELECT @SiteId
		,tmp.SectionId
		,tmp.[Name]
		,tmp.Code
		,tmp.DepartmentId
		,ISNULL(tmp.IsActive, 0)
		,ISNULL(tmp.IsDeleted, 0)
		,tmp.ModifiedDate
		,@SyncDate
	FROM #tmpSection AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.HRM_Section AS section WITH (NOLOCK)
			WHERE section.SiteId = @SiteId
				AND section.SectionId = tmp.SectionId
			)

	DROP TABLE #tmpSection
END