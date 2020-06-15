
CREATE PROCEDURE [dbo].[Report_ShiftStatisticDetail] @WorkDate DATE
	,@ShiftId INT = NULL
	,@EmployeeId INT = NULL
	,@teamIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
	,@SupervisorId INT = NULL
	,@IsShowWholeCompany BIT = 0
AS
/*
Created: Tim.Trinh@2019-11-13
Task: shift statistic detail
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @tblGroupId TABLE (GroupId INT)
	DECLARE @tblEmployeeId TABLE (EmpId INT)

	IF ISNULL(@teamIds, '') <> ''
	BEGIN
		INSERT INTO @tblGroupId (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@teamIds, ',')
	END

	IF @isShowWholeCompany = 0
	BEGIN
		INSERT INTO @tblEmployeeId (EmpId)
		SELECT es.OwnerId
		FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON es.OwnerId = emp.EmpId
		WHERE es.ActionType = 4
			AND es.EmpId = @SupervisorId
			AND emp.[Enabled] = 1
			AND ISNULL(es.IsDeleted, 0) = 0
		
		UNION
		SELECT emp.EmpId
		FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gs.GroupId = emp.GroupId
		WHERE gs.OwnerId = @SupervisorId
			AND gs.ActionType = 4
			AND emp.[Enabled] = 1
			AND ISNULL(gs.IsDeleted, 0) = 0
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
				WHERE emp.EmpId = es.OwnerId
					AND es.ActionType = 4
					AND ISNULL(es.IsDeleted, 0) = 0
				)
	END
	ELSE
	BEGIN
		INSERT INTO @tblEmployeeId (EmpId)
		SELECT EmpId
		FROM dbo.Employee WITH (NOLOCK)
		WHERE [Enabled] = 1
	END

	SELECT empca.WorkDate
		,emp.EmpId
		,emp.NickName
		,emp.FullName
		,gr.[Name] AS Team
		,shift.[Name] AS [Shift]
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN @tblEmployeeId AS tblemp ON emp.EmpId = tblemp.EmpId
		INNER JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmpId = empca.EmpId
		AND empca.WorkDate = @WorkDate
		AND ISNULL(empca.ShiftId, 0) <> 0
		AND empca.IsLeaveDay = 0
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		INNER JOIN dbo.[Shift] AS shift WITH (NOLOCK) ON empca.ShiftId = shift.ShiftId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE (
			@employeeId IS NULL
			OR emp.EmpId = @employeeId
			)
		AND (
			@isLocal IS NULL
			OR empsum.IsLocal = @isLocal
			)
		AND (
			@ShiftId IS NULL
			OR ISNULL(empca.ShiftId, 0) = @ShiftId
			)
END
