

CREATE PROCEDURE [dbo].[Employee_GetMemberByManager] --421, 1, 'Yen'
	@supervisorId INT
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

	DECLARE @searchGO NVARCHAR(100)

	DECLARE @tblEmployeeIds TABLE (EmpId INT)

	INSERT INTO @tblEmployeeIds (EmpId)
	SELECT EmpId 
	FROM [dbo].[fnc_GetEmployeeByActorId](@SupervisorId, 1, 1)

	SELECT e.EmpId,
		e.NickName,
		e.FullName
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE 
		(e.NickName LIKE  @query + '%'
		OR e.FullName LIKE @query + '%')
		AND EXISTS (SELECT 1 FROM @tblEmployeeIds WHERE EmpId = e.EmpId)
		AND e.Enabled = 1 
		AND ISNULL(e.isDeleted, 0) = 0
END
