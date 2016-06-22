--select * from [dbo].[fnc_GetEmployeeByActorId](421, 1)
CREATE FUNCTION [dbo].[fnc_GetEmployeeByActorId]
(
	@EmpId INT = NULL,
	@ExcludeUserDelete INT = 1,
	@IsShowUserInTeam BIT = 1
) 
RETURNS @tbl_EmpIds TABLE
		(
			EmpID INT
		)
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
	IF (@EmpId IS NULL OR EXISTS (SELECT 1 FROM dbo.Employee WHERE EmpId = @EmpId AND (Roles LIKE '%6%' OR Roles LIKE '%7%')))
	BEGIN
		INSERT INTO @tbl_EmpIds(EmpID)
		SELECT DISTINCT e.EmpId
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE ((ISNULL(e.isDeleted, 0) = 0 AND ISNULL(e.[Enabled], 0) = 1) OR @ExcludeUserDelete = 0)
	END
	ELSE
	BEGIN
		INSERT INTO @tbl_EmpIds(EmpId)
		SELECT DISTINCT es.EmpId
		FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpId = e.EmpId
		WHERE es.OwnerId = @EmpId
			AND ((ISNULL(e.isDeleted, 0) = 0 AND ISNULL(e.[Enabled], 0) = 1) OR @ExcludeUserDelete = 0)
			AND ISNULL(es.IsDeleted, 0) = 0
			AND es.ActionType IN (2,3)

		INSERT INTO @tbl_EmpIds(EmpId)
		SELECT DISTINCT es.EmpId
		FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpId = e.EmpId
		WHERE es.EmpId = @EmpId
			AND ((ISNULL(e.isDeleted, 0) = 0 AND ISNULL(e.[Enabled], 0) = 1) OR @ExcludeUserDelete = 0)
			AND ISNULL(es.IsDeleted, 0) = 0
			AND es.ActionType IN (4,5)
		

		INSERT INTO @tbl_EmpIds(EmpId)
			SELECT DISTINCT e.EmpId 
			FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
			LEFT JOIN @tbl_EmpIds tmpe ON e.EmpId = tmpe.EmpId
			WHERE OwnerId = @EmpId
				AND ((ISNULL(e.isDeleted, 0) = 0 AND ISNULL(e.[Enabled], 0) = 1) OR @ExcludeUserDelete = 0)
				AND tmpe.EmpId IS NULL
				AND ISNULL(gs.IsDeleted, 0) = 0

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

		IF(@IsShowUserInTeam = 1)
		BEGIN
			INSERT INTO @tbl_EmpIds(EmpId)
			SELECT DISTINCT ee.EmpId 
			FROM dbo.Employee AS e WITH (NOLOCK)
			INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON ee.GroupId = e.GroupId
			LEFT JOIN @tbl_EmpIds tmpe ON ee.EmpId = tmpe.EmpId
			WHERE e.EmpId  = @EmpId
				AND ((ISNULL(ee.isDeleted, 0) = 0 AND ISNULL(ee.[Enabled], 0) = 1) OR @ExcludeUserDelete = 0)
				AND tmpe.EmpId IS NULL
		END
	END
	
	RETURN
END

