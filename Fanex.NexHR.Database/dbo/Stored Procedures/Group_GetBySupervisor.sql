
CREATE PROCEDURE [dbo].[Group_GetBySupervisor] @SupervisorId INT
AS
/*
Created: 20151027@Quinn.Vo
Task: Get all group by supervisor
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	CREATE TABLE #Groups (GroupId INT)

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH(NOLOCK) ON er.EmployeeId= e.EmpId
				WHERE e.EmpId = @SupervisorId
					AND er.RoleId IN (6,7)
				)
			)
	BEGIN
		INSERT INTO #Groups (GroupId)
		SELECT GroupId
		FROM [Group] AS g WITH (NOLOCK)
	END
	ELSE
	BEGIN
		INSERT INTO #Groups (GroupId)
		SELECT DISTINCT GroupID
		FROM (
			SELECT GroupID
			FROM dbo.GroupSupervisor WITH (NOLOCK)
			WHERE OwnerId = @SupervisorID
				AND ISNULL(IsDeleted, 0) = 0
			
			UNION
			SELECT GroupID
			FROM dbo.Employee WITH (NOLOCK)
			WHERE EmpId = @SupervisorID
			
			UNION
			SELECT g.GroupId
			FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
				INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpId = e.EmpId
				INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
			WHERE es.OwnerId = @SupervisorID
				AND es.ActionType IN (2, 3)
				AND e.[Enabled] = 1
				AND ISNULL(e.isDeleted, 0) = 0
				AND ISNULL(g.isDeleted, 0) = 0
			GROUP BY g.GroupId
			
			UNION
			SELECT g.GroupId
			FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
				INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpId = e.EmpId
				INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
			WHERE es.EmpId = @SupervisorID
				AND es.ActionType IN (4, 5)
				AND e.[Enabled] = 1
				AND ISNULL(e.isDeleted, 0) = 0
				AND ISNULL(g.isDeleted, 0) = 0
			GROUP BY g.GroupId
			) AS a
	END

	SELECT g.groupId AS Id
		,g.[Name]
		,g.Code
	FROM dbo.[Group] AS g WITH (NOLOCK)
		INNER JOIN #Groups AS gr WITH (NOLOCK) ON g.GroupId = gr.GroupId
	WHERE ISNULL(g.isDeleted, 0) = 0
	ORDER BY g.[Rank]
		,g.Code

	DROP TABLE #Groups
END
