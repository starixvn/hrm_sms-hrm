
CREATE PROCEDURE [dbo].[Manpower_Employee_Get] @Keyword NVARCHAR(100) = NULL
	,@CompanyId SMALLINT = NULL
	,@StatusId TINYINT = NULL
	,@PageIndex SMALLINT = NULL
	,@PageSize SMALLINT = NULL
	,@TotalRows INT = 0 OUTPUT
AS
/*  
Created: 20230530@Tim.Trinh
Task: get employee
DB:
Original:
Param's Explanation:
	@StatusId: 1 - include; 2 - excluded
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RowStart INT
		,@RowEnd INT

	SELECT @PageIndex = @PageIndex - 1
		,@RowStart = @PageIndex * @PageSize + 1
		,@RowEnd = @RowStart + @PageSize - 1

	SET @Keyword = '%' + ISNULL(@Keyword, '') + '%'

	SELECT ROW_NUMBER() OVER (
			ORDER BY unit.[Name]
				,dept.[Name]
				,section.[Name]
				,team.[Name]
				,emp.Code
				,emp.Nickname
			) AS RowNum
		,s.SiteId
		,s.SiteName
		,c.CompanyId
		,c.[Name] AS CompanyName
		,c.Country
		,emp.EmpId
		,emp.Username
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,emp.IsLocal
		,emp.IsActive
		,emp.IsDeleted
		,CASE 
			WHEN exclude.EmpId IS NOT NULL THEN 1
			ELSE 0
			END AS IsExcluded
		,jl.[Name] AS JobLevel
		,team.Code AS TeamCode
		,team.[Name] AS TeamName
		,section.Code AS SectionCode
		,section.[Name] AS SectionName
		,dept.Code AS DepartmentCode
		,dept.[Name] AS DepartmentName
		,unit.Code AS UnitCode
		,unit.[Name] AS UnitName
	INTO #tmpEmployee
	FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.HRM_Team AS team WITH (NOLOCK) ON emp.SiteId = team.SiteId
		AND emp.TeamId = team.TeamId
		AND team.IsDeleted = 0
		INNER JOIN dbo.HRM_Section AS section WITH (NOLOCK) ON team.SiteId = section.SiteId
		AND team.SectionId = section.SectionId
		AND section.IsActive = 1
		AND section.IsDeleted = 0
		INNER JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON section.SiteId = dept.SiteId
		AND section.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON dept.SiteId = unit.SiteId
		AND dept.UnitId = unit.UnitId
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON emp.SiteId = s.SiteId
		AND s.IsActive = 1
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
		AND c.IsActive = 1
		INNER JOIN dbo.HRM_JobLevel AS jl WITH (NOLOCK) ON emp.SiteId = jl.SiteId
		AND emp.JobLevelId = jl.JobLevelId
		AND jl.IsDeleted = 0
		LEFT JOIN dbo.Manpower_ExcludedEmployee AS exclude WITH (NOLOCK) ON emp.SiteId = exclude.SiteId
		AND emp.EmpId = exclude.EmpId
	WHERE (
			ISNULL(emp.Nickname, '') LIKE @Keyword
			OR ISNULL(emp.FullName, '') LIKE @Keyword
			)
		AND (
			@CompanyId IS NULL
			OR c.CompanyId = @CompanyId
		)
		AND (
			@StatusId IS NULL
			OR (
				@StatusId = 1
				AND exclude.EmpId IS NULL
				)
			OR (
				@StatusId = 2
				AND exclude.EmpId IS NOT NULL
				)
			)
		AND emp.IsActive = 1
		AND emp.IsDeleted = 0

	SELECT @TotalRows = COUNT(1)
	FROM #tmpEmployee WITH (NOLOCK)

	IF @PageIndex >= 0
		AND @PageSize > 0
	BEGIN
		DELETE
		FROM #tmpEmployee WITH (ROWLOCK, UPDLOCK)
		WHERE RowNum < @RowStart
			OR RowNum > @RowEnd
	END

	SELECT RowNum
		,SiteId
		,SiteName
		,CompanyName
		,Country
		,EmpId
		,Username
		,Code
		,Nickname
		,FullName
		,IsLocal
		,IsExcluded
		,JobLevel
		,TeamCode
		,TeamName
		,SectionCode
		,SectionName
		,DepartmentCode
		,DepartmentName
		,UnitCode
		,UnitName
	FROM #tmpEmployee WITH (NOLOCK)

	DROP TABLE #tmpEmployee
END