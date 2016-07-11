
CREATE PROCEDURE [dbo].[EmployeeSupervisor_GetEmployeeByShiftManagement] @EmpId INT
AS
/*
Created: 20151224@Quinn.Vo
Task: Get list employees by shift management.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT
		emp.EmpId AS OwnerId
		,emp.FullName
		,emp.NickName
		,emp.OrderCode
		,emp.OrderNo
		,emp.Team
	FROM (
	SELECT ee.EmpId
			,ee.FullName
			,ee.NickName
			,sd.OrderCode
			,sd.OrderNo
			,ISNULL(g.Code, '') AS [Team]
		FROM [dbo].[EmployeeSupervisor] su WITH (NOLOCK)
			INNER JOIN [dbo].Employee ee WITH (NOLOCK) ON su.OwnerId = ee.EmpId
			INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON ee.GroupId = g.GroupId
			LEFT JOIN dbo.ShiftArrangementOrder AS sd ON su.EmpId = sd.EmpId 
				AND su.OwnerId = sd.OwnerId
		WHERE su.EmpId = @EmpId
			AND su.ActionType = 4
			AND ISNULL(su.IsDeleted, 0) = 0
	UNION ALL
	SELECT e.EmpId
		,e.FullName
		,e.NickName
		,sd.OrderCode
		,sd.OrderNo
		,ISNULL(g.NAME, '') AS [Team]
	FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.ActionType = 4 
		AND gs.OwnerId = @EmpId 
		AND gs.GroupId = e.GroupId
	INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	LEFT JOIN dbo.ShiftArrangementOrder AS sd ON e.EmpId = sd.OwnerId
				AND gs.OwnerId = sd.EmpId
	WHERE gs.OwnerId = @EmpId
			AND gs.ActionType = 4
			AND ISNULL(gs.IsDeleted, 0) = 0
			AND e.Enabled = 1
			AND ISNULL(e.isDeleted, 0) = 0
	) AS emp
	ORDER BY OrderCode, OrderNo, NickName
END
