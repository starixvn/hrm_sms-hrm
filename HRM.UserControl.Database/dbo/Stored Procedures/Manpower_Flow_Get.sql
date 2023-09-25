
CREATE PROCEDURE [dbo].[Manpower_Flow_Get]
AS
/*  
Created: 20230605@Tim.Trinh
Task: get flow
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT flow.UnitName
		,flow.ProcessId
		,s.SiteId
		,s.SiteName
		,c.CompanyId
		,c.[Name] AS CompanyName
		,emp.EmpId
		,emp.UserId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,emp.IsActive
		,emp.IsDeleted
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON flow.SiteId = emp.SiteId
		AND flow.EmpId = emp.EmpId
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON emp.SiteId = s.SiteId
		AND s.IsActive = 1
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
		AND c.IsActive = 1
END