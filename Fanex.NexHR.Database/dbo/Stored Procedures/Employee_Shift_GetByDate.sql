
-- [dbo].[Employee_Shift_GetByDate] '2015-10-19'
CREATE PROCEDURE [dbo].[Employee_Shift_GetByDate] @WorkDate DATETIME
	,@ShiftId INT = NULL
	,@EmployeeId INT = NULL
	,@TeamIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
	,@SupervisorID INT = NULL
	,@IsShowWholeCompany BIT = 0
AS
/*
Created: 20151022@Quinn.Vo
Task: Get employees by shift and date.
DB: NexEPA
Original: 

Revisions:  10:12:18
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #GroupIds (GroupId INT)

	IF ISNULL(@TeamIds, '') <> ''
	BEGIN
		INSERT INTO #GroupIds (GroupId)
		SELECT g.val
		FROM fnc_strSplit(@TeamIds, ',') AS g
	END
	ELSE
	BEGIN
		INSERT INTO #GroupIds (GroupId)
		SELECT g.GRoupID
		FROM dbo.[Group] g WITH (NOLOCK)
	END

	CREATE TABLE #tmpEmployee (EmpId INT)

	IF (@isShowWholeCompany = 0)
	BEGIN
		INSERT INTO #tmpEmployee (EmpId)
		SELECT es.OwnerId
		FROM dbo.EmployeeSupervisor es WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.OwnerId = e.EmpId
			INNER JOIN dbo.EmployeeSummary AS esu WITH (NOLOCK) ON e.EmpId = esu.EmpId
			INNER JOIN #GroupIds AS g ON e.GroupId = g.GroupId
		WHERE es.ActionType = 4
			AND es.EmpID = @SupervisorID
			AND e.Enabled = 1
			AND (
				@EmployeeId IS NULL
				OR e.EmpId = @EmployeeId
				)
			AND (
				@IsLocal IS NULL
				OR esu.IsLocal = @IsLocal
				)
			AND ISNULL(es.IsDeleted, 0) = 0
		
		UNION
		SELECT e.EmpId
		FROM dbo.GroupSupervisor AS gsp WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gsp.GroupId = gsp.GroupId
			INNER JOIN dbo.EmployeeSummary AS esu WITH (NOLOCK) ON e.EmpId = esu.EmpId
			INNER JOIN #GroupIds AS g ON e.GroupId = g.GroupId
		WHERE gsp.OwnerId = @SupervisorID
			AND gsp.ActionType = 4
			AND e.Enabled = 1
			AND (
				@EmployeeId IS NULL
				OR e.EmpId = @EmployeeId
				)
			AND (
				@IsLocal IS NULL
				OR esu.IsLocal = @IsLocal
				)
			AND ISNULL(gsp.IsDeleted, 0) = 0
	END
	ELSE
	BEGIN
		INSERT INTO #tmpEmployee (EmpId)
		SELECT e.EmpId
		FROM dbo.Employee AS e WITH (NOLOCK)
			INNER JOIN dbo.EmployeeSummary AS esu WITH (NOLOCK) ON e.EmpId = esu.EmpId
			INNER JOIN #GroupIds AS g ON e.GroupId = g.GroupId
		WHERE e.Enabled = 1
			AND (
				@EmployeeId IS NULL
				OR e.EmpId = @EmployeeId
				)
			AND (
				@IsLocal IS NULL
				OR esu.IsLocal = @IsLocal
				)
	END

	SELECT ec.WorkDate
		,e.EmpId
		,e.Code
		,e.NickName
		,e.FullName
		,g.GroupId
		,g.Code
		,g.NAME
		,sh.ShiftID
		,sh.NAME
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #tmpEmployee AS ee ON e.EmpId = ee.EmpId
		INNER JOIN dbo.EmployeeCalendar AS ec WITH (NOLOCK) ON e.EmpId = ec.EmpId
		INNER JOIN dbo.[Shift] AS sh WITH (NOLOCK) ON ec.ShiftId = sh.ShiftID
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE DATEDIFF(DD, ec.WorkDate, @WorkDate) = 0
		AND ISNULL(ec.ShiftId, 0) <> 0
		AND ISNULL(ec.LeaveDay, 0) <> 1
		AND (
			@ShiftId IS NULL
			OR ISNULL(ec.ShiftId, 0) = @ShiftId
			)
	ORDER BY e.GroupId
		,e.EmpId

	DROP TABLE #tmpEmployee

	DROP TABLE #GroupIds
END
