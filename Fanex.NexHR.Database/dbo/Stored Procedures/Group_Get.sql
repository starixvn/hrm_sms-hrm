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
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;


IF @groupId IS NULL
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
		de.Name,
		leaderID, 
		leader.NickName AS LeaderNickName,  
		leader.FullName AS LeaderFullName,  
		AssistantLeaderId,
		astleader.NickName AS AssistantLeaderNickName,  
		astleader.FullName AS AssistantLeaderFullName
	FROM dbo.[Group] g WITH (NOLOCK)
		INNER JOIN dbo.Department AS de WITH (NOLOCK) ON g.DepartmentId = de.DepartmentId
		LEFT JOIN dbo.Employee ee WITH (NOLOCK) ON ee.groupId = g.groupId			
				AND (ee.isDeleted IS NULL OR ee.isDeleted = 0)
				AND ee.Enabled = 1
		LEFT JOIN dbo.EmployeeSummary esum WITH (NOLOCK) ON esum.EmpId = ee.EmpId
		LEFT JOIN dbo.Employee leader WITH (NOLOCK) ON leader.EmpId = g.leaderID
		LEFT JOIN dbo.Employee astLeader WITH (NOLOCK) ON astLeader.EmpId = g.AssistantLeaderId
		--LEFT JOIN dbo.EmployeeGroup eg WITH (NOLOCK) ON eg.groupId=g.groupId
	WHERE g.isDeleted = @isDeleted
	GROUP BY g.groupId, 
			g.Name, 
			g.Code, 
			g.MailGroup,
			g.[Rank],
			de.DepartmentId,
			de.Code,
			de.Name,
			leaderID, 
			AssistantLeaderId,
			leader.NickName, 
			leader.FullName, 
			astleader.NickName, 
			astleader.FullName
	ORDER BY g.[Code]
ELSE
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
		de.Name,
		leaderID, 
		leader.NickName AS LeaderNickName,  
		leader.FullName AS LeaderFullName,  
		AssistantLeaderId,
		astleader.NickName AS AssistantLeaderNickName,  
		astleader.FullName AS AssistantLeaderFullName
	FROM dbo.[Group] g WITH (NOLOCK)
		INNER JOIN dbo.Department AS de WITH (NOLOCK) ON g.DepartmentId = de.DepartmentId
		LEFT JOIN dbo.Employee ee WITH (NOLOCK) ON ee.groupId = g.groupId			
				AND (ee.isDeleted IS NULL OR ee.isDeleted = 0)
				AND ee.Enabled = 1
		LEFT JOIN dbo.EmployeeSummary esum WITH (NOLOCK) ON esum.EmpId = ee.EmpId
		LEFT JOIN dbo.Employee leader WITH (NOLOCK) ON leader.EmpId = g.leaderID
		LEFT JOIN dbo.Employee astLeader WITH (NOLOCK) ON astLeader.EmpId = g.AssistantLeaderId
		--LEFT JOIN dbo.EmployeeGroup eg WITH (NOLOCK) ON eg.groupId=g.groupId
	WHERE  g.groupId = @groupId
	GROUP BY g.groupId, 
			g.Name, 
			g.Code, 
			g.MailGroup,
			g.[Rank],
			de.DepartmentId,
			de.Code,
			de.Name,
			leaderID, 
			AssistantLeaderId,
			leader.NickName, 
			leader.FullName, 
			astleader.NickName, 
			astleader.FullName
	ORDER BY g.[Code]
END