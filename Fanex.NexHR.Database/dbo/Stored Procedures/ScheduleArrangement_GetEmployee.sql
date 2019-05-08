
CREATE PROCEDURE [dbo].[ScheduleArrangement_GetEmployee] @ActorId INT
	,@GroupIds VARCHAR(MAX) = ''
	,@Query NVARCHAR(256) = ''
AS
/*
Created: Tim.Trinh@2019-05-02
Task: get employee
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ShiftManagerRoleId TINYINT = 4
		,@ViewFullDataFunctionKey VARCHAR(100) = 'ViewFullScheduleData'
		,@q NVARCHAR(258) = '%' + ISNULL(@Query, '') + '%'

	CREATE TABLE #tblGroupIds (GroupId INT)

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO #tblGroupIds (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	END

	SELECT e.EmpId AS EmployeeId
		,e.FullName
		,e.NickName
	FROM dbo.Employee AS e WITH (NOLOCK)
	WHERE e.[Enabled] = 1
		AND e.isDeleted = 0
		AND (
			e.NickName LIKE @q
			OR e.FullName LIKE @q
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary AS esum WITH (NOLOCK)
			WHERE esum.EmpId = e.EmpId
				AND esum.IsTopManager = 1
			)
		AND EXISTS (
			SELECT 1
			FROM #tblGroupIds AS tblgr
			WHERE e.GroupId = tblgr.GroupId
			)
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.RoleFunction AS rf WITH (NOLOCK)
					INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
					AND f.FunctionKey = @ViewFullDataFunctionKey
					AND f.IsActive = 1
					INNER JOIN dbo.EmployeeRole AS erole WITH (NOLOCK) ON rf.RoleId = erole.RoleId
					AND erole.EmployeeId = @ActorId
				WHERE rf.IsRead = 1
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
				WHERE es.EmpId = @ActorId
					AND ISNULL(es.IsDeleted, 0) = 0
					AND es.ActionType = @ShiftManagerRoleId
					AND es.OwnerId = e.EmpId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
				WHERE gs.OwnerId = @ActorId
					AND ISNULL(gs.IsDeleted, 0) = 0
					AND gs.ActionType = @ShiftManagerRoleId
					AND e.GroupId = gs.GroupId
				)
			)
	ORDER BY e.NickName

	DROP TABLE #tblGroupIds
END
