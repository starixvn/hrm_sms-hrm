
CREATE PROCEDURE [dbo].[Manpower_Company_Get]
AS
/*  
Created: 20230628@Tim.Trinh
Task: get company
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT c.CompanyId
		,c.[Name]
	FROM dbo.[Site] AS s WITH (NOLOCK)
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
	WHERE s.IsActive = 1
		AND c.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
			WHERE s.SiteId = emp.SiteId
			)
END