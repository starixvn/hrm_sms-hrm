
CREATE PROCEDURE [dbo].[Manpower_Site_Get]
AS
/*  
Created: 20230530@Tim.Trinh
Task: get site
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT s.SiteId
		,s.SiteName
	FROM dbo.[Site] AS s WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
			WHERE s.SiteId = emp.SiteId
			)
END