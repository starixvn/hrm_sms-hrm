CREATE PROCEDURE [dbo].[API_Site_GetById] @SiteId SMALLINT
AS
/*
Created: 20210621@Oliver.Dang
Task: get site by id
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT s.SiteId
		,s.SiteName
		,s.AccessToken
		,s.IsActive
		,s.SiteURL
	FROM dbo.[Site] AS s WITH (NOLOCK)
	WHERE s.SiteId = @SiteId
END