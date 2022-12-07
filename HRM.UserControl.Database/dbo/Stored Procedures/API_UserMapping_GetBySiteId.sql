
CREATE PROCEDURE [dbo].[API_UserMapping_GetBySiteId] @SiteId SMALLINT
	,@lastModifiedDate DATETIME = NULL
AS
/*
Created: 20221122@Otis.vu
Task: get user mapping
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT UserId
		,RefId
		,NewUserId
		,ModifiedDate
	FROM dbo.UserMapping WITH (NOLOCK)
	WHERE SiteId = @SiteId
		AND ModifiedDate > ISNULL(@lastModifiedDate, '')
END