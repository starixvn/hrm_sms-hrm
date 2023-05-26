
CREATE PROCEDURE [dbo].[AutoJob_UserSite_CopyToAnotherSite] @FromSiteId SMALLINT
	,@ToSiteId SMALLINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211024@Tim.Trinh
Task: copy user site to another
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.UserSite (
		SiteId
		,UserId
		,IsActive
		)
	SELECT @ToSiteId
		,fromus.UserId
		,fromus.IsActive
	FROM dbo.UserSite AS fromus WITH (NOLOCK)
	WHERE fromus.SiteId = @FromSiteId
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.UserSite AS tous WITH (NOLOCK)
			WHERE tous.SiteId = @ToSiteId
				AND fromus.UserId = tous.UserId
			)

	UPDATE tous
	SET tous.IsActive = fromus.IsActive
	FROM dbo.UserSite AS tous WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.UserSite AS fromus WITH (NOLOCK) ON tous.UserId = fromus.UserId
	WHERE tous.SiteId = @ToSiteId
		AND fromus.SiteId = @FromSiteId
		AND tous.IsActive != fromus.IsActive
END