CREATE FUNCTION [dbo].[fnc_GetEmployeeByActorId] (
	@EmpId INT = NULL
	,@ExcludeUserDelete INT = 1
	,@IsShowUserInTeam BIT = 1
	)
RETURNS TABLE
/*
Created: Quinn.Vo@20151203
Task: Get employee Id by actor id
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
AS
RETURN WITH cteEmployee AS (
		SELECT e.EmpId
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(e.isDeleted, 0) = 0
					AND ISNULL(e.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
			AND (
				@EmpId IS NULL
				OR EXISTS (
					SELECT 1
					FROM dbo.Employee AS e1 WITH (NOLOCK)
						INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON e1.EmpId = er.EmployeeId
					WHERE e1.EmpId = @EmpId
						AND er.RoleId IN (6, 7)
					)
				)
		
		UNION ALL
		SELECT es.EmpId
		FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpId = e.EmpId
		WHERE es.OwnerId = @EmpId
			AND (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(e.isDeleted, 0) = 0
					AND ISNULL(e.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
			AND ISNULL(es.IsDeleted, 0) = 0
			AND es.ActionType IN (2, 3)
		
		UNION ALL
		SELECT es.EmpId
		FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpId = e.EmpId
		WHERE es.EmpId = @EmpId
			AND (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(e.isDeleted, 0) = 0
					AND ISNULL(e.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
			AND ISNULL(es.IsDeleted, 0) = 0
			AND es.ActionType IN (4, 5)
		
		UNION ALL
		SELECT e.EmpId
		FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
		WHERE OwnerId = @EmpId
			AND (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(e.isDeleted, 0) = 0
					AND ISNULL(e.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
			AND ISNULL(gs.IsDeleted, 0) = 0
			AND gs.ActionType = 2
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
				WHERE EmpId = e.EmpId
					AND ActionType = 2
					AND ISNULL(IsDeleted, 0) = 0
				)
		
		UNION ALL
		SELECT e.EmpId
		FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
		WHERE OwnerId = @EmpId
			AND (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(e.isDeleted, 0) = 0
					AND ISNULL(e.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
			AND ISNULL(gs.IsDeleted, 0) = 0
			AND gs.ActionType = 3
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor WITH (NOLOCK)
				WHERE EmpId = e.EmpId
					AND ActionType = 3
					AND ISNULL(IsDeleted, 0) = 0
				)
		
		UNION ALL
		SELECT e.EmpId
		FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
		WHERE OwnerId = @EmpId
			AND (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(e.isDeleted, 0) = 0
					AND ISNULL(e.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
			AND ISNULL(gs.IsDeleted, 0) = 0
			AND gs.ActionType = 4
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
					INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON es.EmpId = ee.EmpId
				WHERE OwnerId = e.EmpId
					AND ActionType = 4
					AND ISNULL(es.IsDeleted, 0) = 0
					AND ISNULL(ee.IsDeleted, 0) = 0
					AND ISNULL(ee.[Enabled], 0) = 1
				)
		
		UNION ALL
		SELECT e.EmpId
		FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
		WHERE OwnerId = @EmpId
			AND (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(e.isDeleted, 0) = 0
					AND ISNULL(e.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
			AND ISNULL(gs.IsDeleted, 0) = 0
			AND gs.ActionType = 5
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
					INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON es.EmpId = ee.EmpId
				WHERE OwnerId = e.EmpId
					AND ActionType = 5
					AND ISNULL(es.IsDeleted, 0) = 0
					AND ISNULL(ee.IsDeleted, 0) = 0
					AND ISNULL(ee.[Enabled], 0) = 1
				)
		
		UNION ALL
		SELECT e.EmpId
		FROM dbo.Employee AS e WITH (NOLOCK)
			INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON ee.GroupId = e.GroupId
		WHERE e.EmpId = @EmpId
			AND (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(ee.isDeleted, 0) = 0
					AND ISNULL(ee.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
			AND @IsShowUserInTeam = 1
		)

SELECT DISTINCT EmpId
FROM cteEmployee

UNION
SELECT @EmpId
WHERE @EmpId IS NOT NULL
	AND NOT EXISTS (
		SELECT 1
		FROM cteEmployee
		WHERE EmpId = @EmpId
		)