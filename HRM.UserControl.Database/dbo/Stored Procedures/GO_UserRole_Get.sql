
CREATE PROCEDURE [dbo].[GO_UserRole_Get] @Keyword NVARCHAR(50) = NULL
	,@SiteId SMALLINT
	,@StatusId TINYINT = NULL
	,@PageIndex INT = NULL
	,@PageSize INT = NULL
	,@TotalRows INT = 0 OUTPUT
AS
/*
Created: 20230808@Tim.Trinh
Task: get user role
DB:
Original:

Revisions:
Param's Explanation:
	- StatusId: 1 - active, 2 - inactive, 3 - deleted
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
			ORDER BY u.UserId
				,u.Username DESC
			) AS RowNum
		,u.UserId
		,u.Username
		,u.Code
		,u.Nickname
		,u.FullName
		,u.IsActive
		,u.IsDeleted
		,s.SiteName
		,c.[Name] AS CompanyName
	INTO #tblUser
	FROM dbo.[User] AS u WITH (NOLOCK)
		LEFT JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON u.UserId = emp.UserId
		LEFT JOIN dbo.[Site] AS s WITH (NOLOCK) ON emp.SiteId = s.SiteId
		LEFT JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
	WHERE (
			ISNULL(u.Username, '') LIKE @Keyword
			OR ISNULL(u.Code, '') LIKE @Keyword
			OR ISNULL(u.Nickname, '') LIKE @Keyword
			OR ISNULL(u.FullName, '') LIKE @Keyword
			)
		AND EXISTS (
			SELECT 1
			FROM dbo.UserSite AS us WITH (NOLOCK)
				INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON us.SiteId = s.SiteId
			WHERE us.UserId = u.UserId
				AND s.SiteId = @SiteId
			)
		AND (
			emp.EmpId IS NULL
			OR EXISTS (
				SELECT 1
				FROM dbo.UserMapping AS um WITH (NOLOCK)
				WHERE emp.SiteId = um.SiteId
					AND emp.UserId = um.UserId
					AND emp.EmpId = um.RefId
					AND um.UserId = ISNULL(um.NewUserId, um.UserId)
				)
			)
		AND (
			@StatusId IS NULL
			OR (
				@StatusId = 1
				AND u.IsActive = 1
				)
			OR (
				@StatusId = 2
				AND u.IsActive = 0
				AND u.IsDeleted = 0
				)
			OR (
				@StatusId = 3
				AND u.IsDeleted = 1
				)
			)

	SELECT @TotalRows = COUNT(1)
	FROM #tblUser WITH (NOLOCK)

	IF @PageIndex >= 0
		AND @PageSize > 0
	BEGIN
		DELETE
		FROM #tblUser WITH (ROWLOCK, UPDLOCK)
		WHERE RowNum < @RowStart
			OR RowNum > @RowEnd
	END

	SELECT RowNum
		,UserId
		,Username
		,Code
		,Nickname
		,FullName
		,IsActive
		,IsDeleted
		,SiteName
		,CompanyName
	FROM #tblUser WITH (NOLOCK)

	SELECT ur.UserId
		,ur.RoleId
		,r.Code
		,r.[Name]
	FROM dbo.UserRole AS ur WITH (NOLOCK)
		INNER JOIN dbo.[Role] AS r WITH (NOLOCK) ON ur.RoleId = r.RoleId
		AND ur.SiteId = r.SiteId
		AND r.IsActive = 1
		INNER JOIN #tblUser AS tbl WITH (NOLOCK) ON ur.UserId = tbl.UserId
	WHERE ur.SiteId = @SiteId
	ORDER BY ur.UserId
		,r.[Name]

	DROP TABLE #tblUser
END