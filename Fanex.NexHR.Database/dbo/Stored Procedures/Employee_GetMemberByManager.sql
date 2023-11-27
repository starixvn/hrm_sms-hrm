
CREATE PROCEDURE [dbo].[Employee_GetMemberByManager] @supervisorId INT
	,@excludedManager BIT
	,@query NVARCHAR(100)
AS
/*
Created: Quinn.Vo@20160129
Task: Get list of employees by manager
DB: HRM
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SET @query = @query + '%'

	CREATE TABLE #tblEmployeeIds (EmpId INT PRIMARY KEY)

	--INSERT INTO #tblEmployeeIds (EmpId)
	--SELECT DISTINCT EmpId
	--FROM dbo.fnc_GetEmployeeByActorId(@SupervisorId, 1, 1)

	INSERT INTO #tblEmployeeIds (EmpId)
	SELECT emp.EmpId
	FROM dbo.Employee AS emp WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM dbo.EmployeeRole AS erole WITH (NOLOCK)
			WHERE erole.EmployeeId = @supervisorId
				AND erole.RoleId IN (6, 7) -- HR, SA
			)
		--AND emp.isDeleted = 0
		--AND emp.[Enabled] = 1
	
	UNION
	SELECT es.EmpId
	FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON es.EmpId = emp.EmpId
	WHERE es.OwnerId = @supervisorId
		--AND emp.isDeleted = 0
		--AND emp.[Enabled] = 1
		AND es.ActionType IN (2, 3) -- verifier, approver
		AND es.IsDeleted = 0
	
	UNION
	SELECT es.EmpId
	FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.EmpId = e.EmpId
	WHERE es.EmpId = @supervisorId
		--AND e.isDeleted = 0
		--AND e.[Enabled] = 1
		AND es.ActionType IN (4, 5) -- shift manager, on behalf request
		AND es.IsDeleted = 0
	
	UNION
	SELECT e.EmpId
	FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
	WHERE gs.OwnerId = @supervisorId
		--AND e.isDeleted = 0
		--AND e.[Enabled] = 1
		AND gs.ActionType IN (2, 3) -- verifier, approver
		AND gs.IsDeleted = 0
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			WHERE es.EmpId = e.EmpId
				AND es.ActionType = gs.ActionType
				AND es.IsDeleted = 0
			)
	
	UNION
	SELECT emp.EmpId
	FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gs.GroupId = emp.GroupId
	WHERE gs.OwnerId = @supervisorId
		--AND emp.isDeleted = 0
		--AND emp.[Enabled] = 1
		AND gs.ActionType IN (4, 5) -- shift manager, on behalf request
		AND gs.IsDeleted = 0
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			WHERE es.EmpId = emp.EmpId
				AND es.ActionType = gs.ActionType
				AND es.IsDeleted = 0
			)
	
	UNION
	SELECT ee.EmpId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON ee.GroupId = emp.GroupId
	WHERE emp.EmpId = @supervisorId
		--AND ee.isDeleted = 0
		--AND ee.[Enabled] = 1
	
	UNION
	SELECT @supervisorId

	SELECT e.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #tblEmployeeIds AS tblemp WITH (NOLOCK) ON e.EmpId = tblemp.EmpId
	WHERE (
			e.NickName LIKE @query
			OR e.FullName LIKE @query
			)
		AND e.[Enabled] = 1
		AND e.isDeleted = 0

	DROP TABLE #tblEmployeeIds
END
