
CREATE PROCEDURE [dbo].[UserMapping_Search] @Keyword NVARCHAR(50) = NULL
	,@SiteId SMALLINT = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20211019@Tim.Trinh
Task: get all user
DB:
Original:

Revisions:
Param's Explanation:
	- StatusId: 1 - active, 2 - inactive, 3 - deleted
*/
BEGIN
	SET NOCOUNT ON;
	SET @Keyword = '%' + ISNULL(@Keyword, '') + '%'

	SELECT DISTINCT TOP 10 u.UserId
		,u.Username
		,u.Code
		,u.Nickname
		,u.FullName
		,u.Email
		,u.IsActive
		,u.IsDeleted
	FROM dbo.[User] AS u WITH (NOLOCK)
		INNER JOIN dbo.UserMapping AS um WITH (NOLOCK) ON u.UserId = um.UserId
	WHERE (
			ISNULL(u.Username, '') LIKE @Keyword
			OR ISNULL(u.Code, '') LIKE @Keyword
			OR ISNULL(u.Nickname, '') LIKE @Keyword
			OR ISNULL(u.FullName, '') LIKE @Keyword
			)
		AND (
			@SiteId IS NULL
			OR um.SiteId = @SiteId
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
END