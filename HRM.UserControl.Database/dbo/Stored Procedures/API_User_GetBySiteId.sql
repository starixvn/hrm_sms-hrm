
CREATE PROCEDURE [dbo].[API_User_GetBySiteId] @SiteId SMALLINT
	,@Keyword NVARCHAR(50) = NULL
	,@StatusId TINYINT = NULL
	,@PageIndex INT = NULL
	,@PageSize INT = NULL
	,@TotalRows INT = 0 OUTPUT
AS
/*
Created: 20220107@Tim.Trinh
Task: get all user by site
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
		,u.Email
		,u.IsActive
		,u.IsDeleted
	INTO #tblUser
	FROM dbo.[User] AS u WITH (NOLOCK)
		INNER JOIN dbo.UserSite AS us WITH (NOLOCK) ON us.UserId = u.UserId
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON us.SiteId = s.SiteId
		AND s.SiteId = @SiteId
	WHERE (
			ISNULL(u.Username, '') LIKE @Keyword
			OR ISNULL(u.Code, '') LIKE @Keyword
			OR ISNULL(u.Nickname, '') LIKE @Keyword
			OR ISNULL(u.FullName, '') LIKE @Keyword
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

	SELECT *
	FROM #tblUser WITH (NOLOCK)

	DROP TABLE #tblUser
END