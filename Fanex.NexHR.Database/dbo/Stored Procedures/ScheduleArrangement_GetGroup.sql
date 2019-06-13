
CREATE PROCEDURE [dbo].[ScheduleArrangement_GetGroup] @ActorId INT
	,@IsIFrame BIT = 0
AS
/*
Created: Tim.Trinh@2019-05-02
Task: get group
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ShiftManagerRoleId TINYINT = 4
		,@EditScheduleUnlimited VARCHAR(100) = 'EditScheduleUnlimited'

	SELECT gr.GroupId
		,gr.Code
		,gr.[Name]
	FROM dbo.[Group] AS gr WITH (NOLOCK)
	WHERE gr.IsDeleted = 0
		AND (
			@IsIFrame = 1
			OR EXISTS (
				SELECT 1
				FROM dbo.RoleFunction AS rf WITH (NOLOCK)
					INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
					AND f.FunctionKey = @EditScheduleUnlimited
					AND f.IsActive = 1
					INNER JOIN dbo.EmployeeRole AS erole WITH (NOLOCK) ON rf.RoleId = erole.RoleId
					AND erole.EmployeeId = @ActorId
				WHERE rf.IsRead = 1
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
					INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.OwnerId = e.EmpId
					AND e.[Enabled] = 1
					AND e.isDeleted = 0
				WHERE es.EmpId = @ActorId
					AND ISNULL(es.IsDeleted, 0) = 0
					AND es.ActionType = @ShiftManagerRoleId
					AND gr.GroupId = e.GroupId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
				WHERE gs.OwnerId = @ActorId
					AND ISNULL(gs.IsDeleted, 0) = 0
					AND gs.ActionType = @ShiftManagerRoleId
					AND gr.GroupId = gs.GroupId
				)
			)
	ORDER BY gr.Code
END
