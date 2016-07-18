--select * from [dbo].[fnc_GetEmployeeByActorId](421, 1)
CREATE FUNCTION [dbo].[fnc_GetEmployeeByActorId] (
	@EmpId INT = NULL
	,@ExcludeUserDelete INT = 1
	,@IsShowUserInTeam BIT = 1
	)
RETURNS @tbl_EmpIds TABLE (EmpID INT)
AS
/*
Created: Quinn.Vo@20151203
Task: Get employee Id by actor id
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	IF (
			@EmpId IS NULL
			OR EXISTS (
				SELECT 1
				FROM dbo.Employee AS e WITH (NOLOCK)
					INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON e.EmpId = er.EmployeeId
				WHERE EmpId = @EmpId
					AND er.RoleId IN (6, 7)
				)
			)
	BEGIN
		INSERT INTO @tbl_EmpIds (EmpID)
		SELECT DISTINCT e.EmpId
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(e.isDeleted, 0) = 0
					AND ISNULL(e.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
	END
	ELSE
	BEGIN
		INSERT INTO @tbl_EmpIds (EmpId)
		SELECT DISTINCT es.EmpId
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

		INSERT INTO @tbl_EmpIds (EmpId)
		SELECT DISTINCT es.EmpId
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

		INSERT INTO @tbl_EmpIds (EmpId)
		SELECT DISTINCT e.EmpId
		FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
			LEFT JOIN @tbl_EmpIds tmpe ON e.EmpId = tmpe.EmpId
		WHERE OwnerId = @EmpId
			AND (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(e.isDeleted, 0) = 0
					AND ISNULL(e.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
			AND tmpe.EmpId IS NULL
			AND ISNULL(gs.IsDeleted, 0) = 0
			AND gs.ActionType IN (2, 3)
			AND (
				NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor WITH (NOLOCK)
					WHERE EmpId = e.EmpId
						AND ActionType IN (2, 3)
						AND ISNULL(IsDeleted, 0) = 0
					)
				)

		INSERT INTO @tbl_EmpIds (EmpId)
		SELECT DISTINCT e.EmpId
		FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
			LEFT JOIN @tbl_EmpIds tmpe ON e.EmpId = tmpe.EmpId
		WHERE OwnerId = @EmpId
			AND (
				(
					@ExcludeUserDelete = 1
					AND ISNULL(e.isDeleted, 0) = 0
					AND ISNULL(e.[Enabled], 0) = 1
					)
				OR @ExcludeUserDelete = 0
				)
			AND tmpe.EmpId IS NULL
			AND ISNULL(gs.IsDeleted, 0) = 0
			AND gs.ActionType IN (4, 5)

		--IF(NOT EXISTS(SELECT 1 FROM @tbl_EmpIds))
		--BEGIN
		--	INSERT INTO @tbl_EmpIds(EmpId)
		--	SELECT DISTINCT e.EmpId 
		--	FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
		--	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
		--	LEFT JOIN @tbl_EmpIds tmpe ON e.EmpId = tmpe.EmpId
		--	WHERE OwnerId = @EmpId
		--		AND ((ISNULL(e.isDeleted, 0) = 0 AND ISNULL(e.[Enabled], 0) = 1) OR @ExcludeUserDelete = 0)
		--		AND tmpe.EmpId IS NULL
		--END
		IF (@IsShowUserInTeam = 1)
		BEGIN
			INSERT INTO @tbl_EmpIds (EmpId)
			SELECT DISTINCT e.EmpId
			FROM dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON ee.GroupId = e.GroupId
				LEFT JOIN @tbl_EmpIds tmpe ON ee.EmpId = tmpe.EmpId
			WHERE e.EmpId = @EmpId
				AND (
					(
						@ExcludeUserDelete = 1
						AND ISNULL(ee.isDeleted, 0) = 0
						AND ISNULL(ee.[Enabled], 0) = 1
						)
					OR @ExcludeUserDelete = 0
					)
				AND tmpe.EmpId IS NULL
		END
	END

	RETURN
END