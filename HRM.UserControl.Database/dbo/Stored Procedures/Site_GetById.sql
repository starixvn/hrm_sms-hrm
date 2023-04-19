
CREATE PROCEDURE [dbo].[Site_GetById] @SiteId SMALLINT
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
		,s.SSOUrl
		,c.CompanyId
		,c.[Name] AS CompanyName
	FROM dbo.[Site] AS s WITH (NOLOCK)
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
	WHERE s.SiteId = @SiteId
END