


CREATE PROCEDURE [dbo].[Report_GetWeekendLeaveSummary] 
	 @FromDate DATE
	,@ToDate DATE
	,@GroupId VARCHAR(MAX) = ''
	,@EmployeeId INT = NULL
	,@IsLocal BIT = NULL
	,@MinTotalLeave INT = NULL
	,@SupervisorId INT = NULL
AS

BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblGroupIds (TmpGroupId INT)

	IF ISNULL(@GroupId, '') <> ''
	BEGIN
		INSERT INTO #tblGroupIds (TmpGroupId)
		SELECT DISTINCT val
		FROM fnc_strSplit(@GroupId, ',')
	END

	CREATE TABLE #tblEmployeeId (EmpId INT)

	IF (@EmployeeId IS NULL)
	BEGIN
		INSERT INTO #tblEmployeeId (EmpId)
		SELECT DISTINCT EmpId
		FROM [dbo].[fnc_GetEmployeeByActorId](@SupervisorId, 1, 0)
	END
	ELSE
	BEGIN
		INSERT INTO #tblEmployeeId (EmpId)
		VALUES (@EmployeeId)
	END

	CREATE TABLE #tblWeekendLeaveSummary (
			EmpId INT
			, TotalWeekendLeaveDays FLOAT
			)

	INSERT INTO #tblWeekendLeaveSummary (
		EmpId
		, TotalWeekendLeaveDays)
	SELECT c.EmpId 
		,SUM(c.LeaveDay)
	FROM [dbo].[EmployeeCalendar] as c WITH (NOLOCK)
	INNER JOIN #tblEmployeeId AS emp ON c.EmpId = emp.EmpId
	WHERE (DATENAME(DW, c.WorkDate) = 'Saturday' OR DATENAME(DW, c.WorkDate) = 'Sunday')
		and c.IsLeaveDay != 0
		and (c.LeaveTypeId =2 or c.LeaveTypeId = 25 or c.LeaveTypeId2 = 2 or c.LeaveTypeId2 = 25)
		and (c.WorkDate between @FromDate and @ToDate)

	GROUP BY c.EmpId

	SELECT 
		emp.EmpId AS EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,empSum.IsLocal
		,wl.TotalWeekendLeaveDays as TotalWeekendLeave
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode	
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblWeekendLeaveSummary AS wl WITH (NOLOCK) ON wl.EmpId = emp.EmpId
		INNER JOIN dbo.EmployeeSummary AS empSum WITH (NOLOCK) ON empSum.EmpId = emp.EmpId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE emp.[Enabled] = 1
		AND wl.TotalWeekendLeaveDays >= @MinTotalLeave
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND EXISTS (
			SELECT 1
			FROM #tblGroupIds AS tbl
			WHERE emp.GroupId = tbl.TmpGroupId
			)
		AND ISNULL(@EmployeeId, emp.EmpId) = emp.EmpId
		AND ISNULL(@IsLocal, empSum.IsLocal) = empSum.IsLocal

	ORDER BY gr.[Name] ASC
		,emp.Code ASC

	DROP TABLE #tblWeekendLeaveSummary

	DROP TABLE #tblGroupIds
END