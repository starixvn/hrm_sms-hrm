
CREATE PROCEDURE [dbo].[API_UnitHead_GetInfo]
AS
/*  
Created: 20230227@Tim.Trinh
Task: get unit head info
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT uhinfo.SiteId
		,s.SiteName
		,c.CompanyId
		,c.[Name] AS CompanyName
		,uhinfo.EmpId
		,uhinfo.Code
		,uhinfo.Nickname
		,uhinfo.FullName
		,uhinfo.DepartmentCode
		,uhinfo.DepartmentName
		,uhinfo.IsActive
		,uhinfo.IsDeleted
	FROM dbo.UnitHeadInfo AS uhinfo WITH (NOLOCK)
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON uhinfo.SiteId = s.SiteId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
END