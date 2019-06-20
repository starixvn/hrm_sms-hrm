/****** Object:  Stored Procedure dbo.Group_Get    Script Date: 7/20/2016 10:19:09 AM ******/
/****** Object:  Stored Procedure dbo.Group_Get    Script Date: 7/19/2016 5:22:15 PM ******/
CREATE PROCEDURE [dbo].[Group_Get]
	@groupId INT=NULL,
	@isDeleted bit=0
AS
/*
Created: Tom.Vo@2013-10-21
Task: Retrieve data on Group
DB: HRMS
Original: 

Revisions:  
Modified by Oliver Dang on 2019-05-15
*/
BEGIN
	SET NOCOUNT ON;


IF @groupId IS NULL
	BEGIN
		SELECT g.groupId, 
			g.Name, 
			g.Code, 
			g.Rank, 
			g.MailGroup, 
			SUM(CASE WHEN esum.IsLocal IS NOT NULL THEN 1 ELSE 0 END) AS NoOfMembers,
			SUM(CASE WHEN esum.IsLocal = 1 THEN 1 ELSE 0 END) TotalLocal,
			SUM(CASE WHEN esum.IsLocal = 0 THEN 1 ELSE 0 END) TotalExpat,
			de.DepartmentId,
			de.Code,
			de.Name
		FROM dbo.[Group] g WITH (NOLOCK)
			INNER JOIN dbo.Department AS de WITH (NOLOCK) ON g.DepartmentId = de.DepartmentId
			LEFT JOIN dbo.Employee ee WITH (NOLOCK) ON ee.groupId = g.groupId			
					AND (ee.isDeleted IS NULL OR ee.isDeleted = 0)
					AND ee.Enabled = 1
			LEFT JOIN dbo.EmployeeSummary esum WITH (NOLOCK) ON esum.EmpId = ee.EmpId
		WHERE g.isDeleted = @isDeleted
		GROUP BY g.groupId, 
				g.Name, 
				g.Code, 
				g.MailGroup,
				g.[Rank],
				de.DepartmentId,
				de.Code,
				de.Name
		ORDER BY g.[Code]

		SELECT gl.GroupId
			  ,gl.IsLeader
			  ,e.EmpId as EmployeeId
			  ,e.NickName
			  ,e.FullName
		FROM dbo.GroupLeader gl WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON gl.EmployeeId = e.EmpId
		WHERE ISNULL(e.isDeleted, 0) = 0 AND e.[Enabled] = 1
	END

ELSE
BEGIN
	SELECT g.groupId, 
		g.Name, 
		g.Code, 
		g.Rank, 
		g.MailGroup, 
		SUM(CASE WHEN esum.IsLocal IS NOT NULL THEN 1 ELSE 0 END) AS NoOfMembers,
		SUM(CASE WHEN esum.IsLocal = 1 THEN 1 ELSE 0 END) TotalLocal,
		SUM(CASE WHEN esum.IsLocal = 0 THEN 1 ELSE 0 END) TotalExpat,
		de.DepartmentId,
		de.Code,
		de.Name
	FROM dbo.[Group] g WITH (NOLOCK)
		INNER JOIN dbo.Department AS de WITH (NOLOCK) ON g.DepartmentId = de.DepartmentId
		LEFT JOIN dbo.Employee ee WITH (NOLOCK) ON ee.groupId = g.groupId			
				AND (ee.isDeleted IS NULL OR ee.isDeleted = 0)
				AND ee.Enabled = 1
		LEFT JOIN dbo.EmployeeSummary esum WITH (NOLOCK) ON esum.EmpId = ee.EmpId
	WHERE  g.groupId = @groupId
	GROUP BY g.groupId, 
			g.Name, 
			g.Code, 
			g.MailGroup,
			g.[Rank],
			de.DepartmentId,
			de.Code,
			de.Name
	ORDER BY g.[Code]

	SELECT gl.GroupId
			  ,gl.IsLeader
			  ,e.EmpId as EmployeeId
			  ,e.NickName
			  ,e.FullName
		FROM dbo.GroupLeader gl WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON gl.EmployeeId = e.EmpId
		WHERE gl.GroupId = @groupId
	END
END