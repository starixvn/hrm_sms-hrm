-- [dbo].[Report_Manpower] 2015
CREATE PROCEDURE [dbo].[Report_ManpowerCategory] @Year INT
AS
/*
Created: 20151103@Quinn.Vo
Task: Get data for manpower category report.
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME

	SELECT @FromDate = CAST(@Year AS VARCHAR(4)) + '/01/01'
		,@ToDate = CAST(@Year AS VARCHAR(4)) + '/12/31'

	SELECT g.GroupId
		,g.Code AS GroupCode
		,g.[Name] AS GroupName
		,SUM(CASE 
				WHEN DATEDIFF(MM, e.JoinedDate, @ToDate) >= 0
					AND (
						e.ResignedDate IS NULL
						OR DATEDIFF(MM, @ToDate, e.ResignedDate) > 0
						) THEN 1
				ELSE 0
				END) AS Total
		,SUM(CASE 
				WHEN es.IsLocal = 0
					AND DATEDIFF(MM, e.JoinedDate, @ToDate) >= 0
					AND (
						e.ResignedDate IS NULL
						OR DATEDIFF(MM, @ToDate, e.ResignedDate) > 0
						) THEN 1
				ELSE 0
				END) AS TotalExpat
		,SUM(CASE 
				WHEN es.IsLocal = 1
					AND DATEDIFF(MM, e.JoinedDate, @ToDate) >= 0
					AND (
						e.ResignedDate IS NULL
						OR DATEDIFF(MM, @ToDate, e.ResignedDate) > 0
						) THEN 1
				ELSE 0
				END) AS TotalLocal
		,SUM(CASE 
				WHEN job.Value = '5'
					AND DATEDIFF(MM, e.JoinedDate, @ToDate) >= 0
					AND (
						e.ResignedDate IS NULL
						OR DATEDIFF(MM, @ToDate, e.ResignedDate) > 0
						) THEN 1
				ELSE 0
				END) AS Management
		,SUM(CASE 
				WHEN job.Value = '4'
					AND DATEDIFF(MM, e.JoinedDate, @ToDate) >= 0
					AND (
						e.ResignedDate IS NULL
						OR DATEDIFF(MM, @ToDate, e.ResignedDate) > 0
						) THEN 1
				ELSE 0
				END) AS ExecutiveWithPosition
		,SUM(CASE 
				WHEN job.Value = '3'
					AND DATEDIFF(MM, e.JoinedDate, @ToDate) >= 0
					AND (
						e.ResignedDate IS NULL
						OR DATEDIFF(MM, @ToDate, e.ResignedDate) > 0
						) THEN 1
				ELSE 0
				END) AS Executive
		,SUM(CASE 
				WHEN job.Value = '2'
					AND DATEDIFF(MM, e.JoinedDate, @ToDate) >= 0
					AND (
						e.ResignedDate IS NULL
						OR DATEDIFF(MM, @ToDate, e.ResignedDate) > 0
						) THEN 1
				ELSE 0
				END) AS Staff
		,SUM(CASE 
				WHEN job.Value = '1'
					AND DATEDIFF(MM, e.JoinedDate, @ToDate) >= 0
					AND (
						e.ResignedDate IS NULL
						OR DATEDIFF(MM, @ToDate, e.ResignedDate) > 0
						) THEN 1
				ELSE 0
				END) AS Worker
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN dbo.LookupData AS job WITH (NOLOCK) ON e.JobTitleId = job.Id
	WHERE g.IsDeleted = 0
		AND (e.ResignedDate IS NULL 
			OR
			YEAR(e.ResignedDate) >= @Year)
	GROUP BY g.GroupId
		,g.Code
		,g.[Name]
END