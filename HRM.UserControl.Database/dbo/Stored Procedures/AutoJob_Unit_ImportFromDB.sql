
CREATE PROCEDURE [dbo].[AutoJob_Unit_ImportFromDB] @SiteId SMALLINT
	,@DbName VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230509@Tim.Trinh
Task: import unit from DB
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
	FROM dbo.HRM_Unit WITH (NOLOCK)
	WHERE SiteId = @SiteId

	SET @LastModifiedDate = ISNULL(@LastModifiedDate, '2010-01-01')		

	CREATE TABLE #tmpUnit (
		UnitId INT
		,Code NVARCHAR(50)
		,[Name] NVARCHAR(200)
		,IsActive BIT
		,IsDeleted BIT
		,ModifiedDate DATETIME
		)

	SELECT @sqlExecute = 'INSERT INTO #tmpUnit (
		UnitId
		,Code
		,[Name]
		,IsActive
		,ModifiedDate
		)
	SELECT DISTINCT unit.DepartmentId
		,unit.Code
		,unit.[Name]
		,unit.IsActive
		,unit.ModifiedDate 
	FROM [' + @DbName + '].[dbo].[Department] AS unit WITH (NOLOCK)
		INNER JOIN [' + @DbName + '].[dbo].[Department] AS dept WITH (NOLOCK) ON unit.DepartmentId = dept.ParentId
		INNER JOIN [' + @DbName + '].[dbo].[Department] AS section WITH (NOLOCK) ON dept.DepartmentId = section.ParentId
		INNER JOIN [' + @DbName + '].[dbo].[Group] AS team WITH (NOLOCK) ON section.DepartmentId = team.DepartmentId
	WHERE ISNULL(dept.ModifiedDate, ''2010-01-01'') > CAST(''' + CONVERT(VARCHAR, @LastModifiedDate, 121) + ''' AS DATETIME)'

	EXEC (@sqlExecute)

	UPDATE unit
	SET unit.[Name] = tmp.[Name]
		,unit.Code = tmp.Code
		,unit.IsActive = ISNULL(tmp.IsActive, 0)
		,unit.IsDeleted = ISNULL(tmp.IsDeleted, 0)
		,unit.ModifiedDate = tmp.ModifiedDate
		,unit.SyncDate = @SyncDate
	FROM dbo.HRM_Unit AS unit WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpUnit AS tmp WITH (NOLOCK) ON unit.UnitId = tmp.UnitId
	WHERE unit.SiteId = @SiteId

	INSERT INTO dbo.HRM_Unit (
		SiteId
		,UnitId
		,[Name]
		,Code
		,IsActive
		,IsDeleted
		,ModifiedDate
		,SyncDate
		)
	SELECT @SiteId
		,tmp.UnitId
		,tmp.[Name]
		,tmp.Code
		,ISNULL(tmp.IsActive, 0)
		,ISNULL(tmp.IsDeleted, 0)
		,tmp.ModifiedDate
		,@SyncDate
	FROM #tmpUnit AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.HRM_Unit AS unit WITH (NOLOCK)
			WHERE unit.SiteId = @SiteId
				AND unit.UnitId = tmp.UnitId
			)

	DROP TABLE #tmpUnit
END