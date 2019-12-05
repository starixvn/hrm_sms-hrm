
CREATE PROCEDURE [dbo].[OtherDiscipline_GetMember] @SupervisorId INT
	,@Query NVARCHAR(100) = NULL
AS
/*
Created: 20191205@Tim.Trinh
Task: Get member by Supervisor.
DB: SMSHRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (ISNULL(@Query, '') <> '')
		SET @Query = '%' + @Query + '%'

	SELECT DISTINCT emp.EmpId AS EmployeeId
		,emp.NickName
		,emp.FullName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeSupervisor AS es WITH (NOLOCK) ON emp.EmpId = es.EmpId
	WHERE (
			EXISTS (
				SELECT 1
				FROM EmployeeRole WITH (NOLOCK)
				WHERE EmployeeId = @SupervisorId
					AND RoleId IN (6, 7)
				)
			OR (
				es.OwnerId = @SupervisorId
				AND es.EmpId <> @SupervisorId
				AND es.ActionType = 8
				)
			)
		AND (
			emp.NickName LIKE '%' + @query + '%'
			OR emp.FullName LIKE '%' + @query + '%'
			)
		AND emp.isDeleted = 0
		AND emp.[Enabled] = 1
	
	UNION
	SELECT DISTINCT emp.EmpId AS EmployeeId
		,emp.NickName
		,emp.FullName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON emp.GroupId = gs.GroupId
	WHERE gs.OwnerId = @SupervisorId
		AND emp.EmpId <> @SupervisorId
		AND gs.ActionType = 8
		AND (
			emp.NickName LIKE '%' + @query + '%'
			OR emp.FullName LIKE '%' + @query + '%'
			)
		AND emp.isDeleted = 0
		AND emp.[Enabled] = 1
END
