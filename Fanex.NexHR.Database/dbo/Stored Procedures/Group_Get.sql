
CREATE PROCEDURE [dbo].[Group_Get] @groupId INT = NULL
	,@isDeleted BIT = 0
AS
/*
Created: Tom.Vo@2013-10-21
Task: get group
DB: HRMS
Original:

Revisions:
    Modified by Oliver Dang on 2019-05-15
*/
BEGIN
	SET NOCOUNT ON;

	SELECT g.groupId
		,g.[Name]
		,g.Code
		,g.[Rank]
		,g.MailGroup
		,SUM(CASE 
				WHEN esum.IsLocal IS NOT NULL THEN 1
				ELSE 0
				END) AS NoOfMembers
		,SUM(CASE 
				WHEN esum.IsLocal = 1 THEN 1
				ELSE 0
				END) TotalLocal
		,SUM(CASE 
				WHEN esum.IsLocal = 0 THEN 1
				ELSE 0
				END) TotalExpat
		,de.DepartmentId
		,de.Code
		,de.[Name]
	FROM dbo.[Group] AS g WITH (NOLOCK)
		INNER JOIN dbo.Department AS de WITH (NOLOCK) ON g.DepartmentId = de.DepartmentId
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.groupId = g.groupId
		AND ISNULL(emp.isDeleted, 0) = 0
		AND emp.[Enabled] = 1
		LEFT JOIN dbo.EmployeeSummary AS esum WITH (NOLOCK) ON esum.EmpId = emp.EmpId
	WHERE (
			@groupId IS NULL
			OR g.GroupId = @groupId
			)
		AND g.isDeleted = @isDeleted
	GROUP BY g.groupId
		,g.[Name]
		,g.Code
		,g.MailGroup
		,g.[Rank]
		,de.DepartmentId
		,de.Code
		,de.[Name]
	ORDER BY g.[Code]

	SELECT gl.GroupId
		,gl.IsLeader
		,emp.EmpId AS EmployeeId
		,emp.NickName
		,emp.FullName
	FROM dbo.GroupLeader AS gl WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON gl.GroupId = g.GroupId
		AND (
			@groupId IS NULL
			OR g.GroupId = @groupId
			)
		AND g.isDeleted = @isDeleted
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gl.EmployeeId = emp.EmpId
		AND ISNULL(emp.isDeleted, 0) = 0
		AND emp.[Enabled] = 1
END