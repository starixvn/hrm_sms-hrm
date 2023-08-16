
CREATE PROCEDURE [dbo].[HRM_Employee_Search] @Keyword NVARCHAR(100) = NULL
	,@Top TINYINT = 10
AS
/*  
Created: 20230606@Tim.Trinh
Task: get employee
DB:
Original:
Param's Explanation:
	@StatusId: 1 - include; 2 - excluded
*/
BEGIN
	SET NOCOUNT ON;
	SET @Keyword = '%' + ISNULL(@Keyword, '') + '%'

	SELECT TOP (@Top) s.SiteId
		,s.SiteName
		,c.[Name] AS CompanyName
		,emp.EmpId
		,emp.Username
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,emp.IsActive
		,emp.IsDeleted
	FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Site] AS s WITH (NOLOCK) ON emp.SiteId = s.SiteId
		AND s.IsActive = 1
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON s.CompanyId = c.CompanyId
		AND c.IsActive = 1
	WHERE emp.IsActive = 1
		AND emp.IsDeleted = 0
		AND (
			ISNULL(emp.Nickname, '') LIKE @Keyword
			OR ISNULL(emp.FullName, '') LIKE @Keyword
			)
END