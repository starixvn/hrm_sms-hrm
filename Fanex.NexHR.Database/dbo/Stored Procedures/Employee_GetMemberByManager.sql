
CREATE PROCEDURE [dbo].[Employee_GetMemberByManager] 
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

	IF (ISNULL(@query, '') <> '')
		SET @query = '%' + @query + '%'

	PRINT @query

	SELECT e.EmpId,
		e.NickName,
		e.FullName
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE 
		e.NickName LIKE '%' + @query + '%'
		AND EXISTS (SELECT 1 FROM [dbo].[fnc_GetEmployeeByActorId](@SupervisorId, 1, 1) WHERE EmpId = e.EmpId)
		AND e.Enabled = 1 
		AND ISNULL(e.isDeleted, 0) = 0
END
