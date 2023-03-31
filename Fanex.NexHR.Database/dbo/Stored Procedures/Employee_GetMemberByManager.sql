

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

	CREATE TABLE #tblEmployeeIds (EmpId INT)

	INSERT INTO #tblEmployeeIds (EmpId)
	SELECT EmpId
	FROM dbo.fnc_GetEmployeeByActorId(@SupervisorId, 1, 1)

	SELECT e.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #tblEmployeeIds AS tblemp WITH (NOLOCK) ON e.EmpId = tblemp.EmpId
	WHERE (
			e.NickName LIKE @query + '%'
			OR e.FullName LIKE @query + '%'
			)
		AND e.[Enabled] = 1
		AND e.isDeleted = 0

	DROP TABLE #tblEmployeeIds
END
