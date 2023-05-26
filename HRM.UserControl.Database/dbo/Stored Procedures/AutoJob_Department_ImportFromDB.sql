
CREATE PROCEDURE [dbo].[AutoJob_Department_ImportFromDB] @SiteId SMALLINT
	,@DbName VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230509@Tim.Trinh
Task: import department from DB
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
	FROM dbo.HRM_Department WITH (NOLOCK)
	WHERE SiteId = @SiteId

	SET @LastModifiedDate = ISNULL(@LastModifiedDate, '2010-01-01')	

	CREATE TABLE #tmpDepartment (
		DepartmentId INT
		,Code NVARCHAR(50)
		,[Name] NVARCHAR(200)
		,UnitId INT
		,IsActive BIT
		,IsDeleted BIT
		,ModifiedDate DATETIME
		)

	SELECT @sqlExecute = 'INSERT INTO #tmpDepartment (
		DepartmentId
		,Code
		,[Name]
		,UnitId
		,IsActive
		,ModifiedDate
		)
	SELECT DISTINCT dept.DepartmentId
		,dept.Code
		,dept.[Name]
		,dept.ParentId
		,dept.IsActive
		,dept.ModifiedDate 
	FROM [' + @DbName + '].[dbo].[Department] AS dept WITH (NOLOCK)
		INNER JOIN [' + @DbName + '].[dbo].[Department] AS section WITH (NOLOCK) ON dept.DepartmentId = section.ParentId
		INNER JOIN [' + @DbName + '].[dbo].[Group] AS team WITH (NOLOCK) ON section.DepartmentId = team.DepartmentId
	WHERE ISNULL(dept.ModifiedDate, ''2010-01-01'') > CAST(''' + CONVERT(VARCHAR, @LastModifiedDate, 121) + ''' AS DATETIME)'

	EXEC (@sqlExecute)

	UPDATE dept
	SET dept.[Name] = tmp.[Name]
		,dept.Code = tmp.Code
		,dept.UnitId = tmp.UnitId
		,dept.IsActive = ISNULL(tmp.IsActive, 0)
		,dept.IsDeleted = ISNULL(tmp.IsDeleted, 0)
		,dept.ModifiedDate = tmp.ModifiedDate
		,dept.SyncDate = @SyncDate
	FROM dbo.HRM_Department AS dept WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpDepartment AS tmp WITH (NOLOCK) ON dept.DepartmentId = tmp.DepartmentId
	WHERE dept.SiteId = @SiteId

	INSERT INTO dbo.HRM_Department (
		SiteId
		,DepartmentId
		,[Name]
		,Code
		,UnitId
		,IsActive
		,IsDeleted
		,ModifiedDate
		,SyncDate
		)
	SELECT @SiteId
		,tmp.DepartmentId
		,tmp.[Name]
		,tmp.Code
		,tmp.UnitId
		,ISNULL(tmp.IsActive, 0)
		,ISNULL(tmp.IsDeleted, 0)
		,tmp.ModifiedDate
		,@SyncDate
	FROM #tmpDepartment AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.HRM_Department AS dept WITH (NOLOCK)
			WHERE dept.SiteId = @SiteId
				AND dept.DepartmentId = tmp.DepartmentId
			)

	DROP TABLE #tmpDepartment
END