
CREATE PROCEDURE [dbo].[Request_GetMemberByActorId] @EmpId INT = NULL
	,@ExcludeUserDelete INT = 1
	,@IsShowUserInTeam BIT = 1
AS
/*
Created: Otis.Vu@20230309
Task: Get member by actor id
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	CREATE TABLE #tblEmployee (EmpId INT PRIMARY KEY)

	INSERT INTO #tblEmployee (EmpId)
	SELECT es.EmpId
	FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpId = e.EmpId
	WHERE es.OwnerId = @EmpId
		AND (
			(
				@ExcludeUserDelete = 1
				AND e.isDeleted = 0
				AND e.[Enabled] = 1
				)
			OR @ExcludeUserDelete = 0
			)
		AND es.IsDeleted = 0
		AND es.ActionType IN (2, 3) -- verifier, approver
	
	UNION
	SELECT es.EmpId
	FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpId = e.EmpId
	WHERE es.EmpId = @EmpId
		AND (
			(
				@ExcludeUserDelete = 1
				AND e.isDeleted = 0
				AND e.[Enabled] = 1
				)
			OR @ExcludeUserDelete = 0
			)
		AND es.IsDeleted = 0
		AND es.ActionType IN (4, 5) -- shift manager, on behalf request
	
	UNION
	SELECT e.EmpId
	FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
	WHERE OwnerId = @EmpId
		AND (
			(
				@ExcludeUserDelete = 1
				AND e.isDeleted = 0
				AND e.[Enabled] = 1
				)
			OR @ExcludeUserDelete = 0
			)
		AND gs.IsDeleted = 0
		AND gs.ActionType IN (2, 3)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			WHERE es.EmpId = e.EmpId
				AND es.ActionType = gs.ActionType
				AND es.IsDeleted = 0
			)
	
	UNION
	SELECT e.EmpId
	FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
	WHERE OwnerId = @EmpId
		AND (
			(
				@ExcludeUserDelete = 1
				AND e.isDeleted = 0
				AND e.[Enabled] = 1
				)
			OR @ExcludeUserDelete = 0
			)
		AND gs.IsDeleted = 0
		AND gs.ActionType IN (4, 5)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			WHERE es.EmpId = e.EmpId
				AND es.ActionType = gs.ActionType
				AND es.IsDeleted = 0
			)
	
	UNION
	SELECT e.EmpId
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON ee.GroupId = e.GroupId
	WHERE e.EmpId = @EmpId
		AND (
			(
				@ExcludeUserDelete = 1
				AND ee.isDeleted = 0
				AND ee.[Enabled] = 1
				)
			OR @ExcludeUserDelete = 0
			)
		AND @IsShowUserInTeam = 1
	
	UNION
	SELECT @EmpId

	SELECT EmpId
	FROM #tblEmployee WITH (NOLOCK)

	DROP TABLE #tblEmployee
END