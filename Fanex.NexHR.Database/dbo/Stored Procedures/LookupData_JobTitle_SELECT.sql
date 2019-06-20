/****** Object:  Stored Procedure dbo.LookupData_JobTitle_SELECT    Script Date: 7/20/2016 10:19:11 AM ******/
/****** Object:  Stored Procedure dbo.LookupData_JobTitle_SELECT    Script Date: 7/19/2016 5:22:17 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[LookupData_JobTitle_SELECT]
	@JobTitle NVARCHAR(100)	= ''
AS
/*
Created: Tom.Vo@2013-10-29
Task: Get Job Title
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @lookupId INT
	SELECT @lookupId = Id FROM dbo.[Lookup] WITH (NOLOCK) WHERE Code ='JT'

	IF @JobTitle IS NULL
		SET @JobTitle = ''
	
	IF @JobTitle <> ''
		SELECT l.Id,
				l.LookupId,
				l.Code,
				l.Name,
				SUM(CASE WHEN esum.IsLocal IS NOT NULL THEN 1 ELSE 0 END) AS Value,
				SUM(CASE WHEN esum.IsLocal = 1 THEN 1 ELSE 0 END) TotalLocal,
				SUM(CASE WHEN esum.IsLocal = 0 THEN 1 ELSE 0 END) TotalExpat,
				CAST(ISNULL(l.Value, 0) AS INT) AS CategoryId
		FROM dbo.LookupData AS l WITH(NOLOCK)
		LEFT JOIN dbo.Employee AS e WITH(NOLOCK) 
			ON l.Id=e.JobTitleId
				AND (e.isDeleted IS NULL OR e.isDeleted = 0)
				AND e.Enabled = 1
		LEFT JOIN dbo.EmployeeSummary esum WITH (NOLOCK) ON esum.EmpId = e.EmpId
		WHERE LookupId = @lookupId
			AND l.Name LIKE '%'+ @JobTitle +'%'			
		GROUP BY l.LookupId,l.id,l.Code,l.Name,l.Value
		ORDER BY l.[Name]
	ELSE
		SELECT l.Id,
				l.LookupId,
				l.Code,
				l.Name,
				SUM(CASE WHEN esum.IsLocal IS NOT NULL THEN 1 ELSE 0 END) AS Value,
				SUM(CASE WHEN esum.IsLocal = 1 THEN 1 ELSE 0 END) TotalLocal,
				SUM(CASE WHEN esum.IsLocal = 0 THEN 1 ELSE 0 END) TotalExpat,
				CAST(ISNULL(l.Value, 0) AS INT) AS CategoryId
		FROM dbo.LookupData AS l WITH(NOLOCK)
		LEFT JOIN dbo.Employee AS e WITH(NOLOCK) 
			ON l.Id=e.JobTitleId
				AND (e.isDeleted IS NULL OR e.isDeleted = 0)
				AND e.Enabled = 1
		
		LEFT JOIN dbo.EmployeeSummary esum WITH (NOLOCK) ON esum.EmpId = e.EmpId
		WHERE LookupId = @lookupId
		GROUP BY l.LookupId,l.id,l.Code,l.Name,l.Value
		ORDER BY l.[Name]

	 --SELECT * FROM dbo.Employee AS e WITH(NOLOCK) 
END


