
CREATE PROCEDURE [dbo].[Report_EmployeeCalendarByDateRange] @EmployeeId INT = NULL
	,@FromDate DATE
	,@ToDate DATE
	,@GroupIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
AS
/*
Created: 20140711@Quinn.Vo
Task: Get employee calendar by date range.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblGroupIds (GroupId INT)

	IF ISNULL(@GroupIds, '') <> ''
		INSERT INTO #tblGroupIds (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	ELSE
	BEGIN
		INSERT INTO #tblGroupIds (GroupId)
		SELECT GroupId
		FROM dbo.[Group] WITH (NOLOCK)
		WHERE IsDeleted = 0
	END

	SELECT emp.EmpId AS EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,g.GroupId
		,g.Code AS GroupCode
		,g.[Name] AS GroupName
		,empca.WorkDate
		,(CASE 
			WHEN empca.WorkDate IS NULL THEN ''
			WHEN empca.IsHoliday = 1 THEN 'PH'
			WHEN empca.IsOffDay = 1 THEN 'OFF'
			WHEN empca.IsLeaveDay = 1 THEN (
					lv.Code + CASE 
					WHEN lv2.LeaveTypeId IS NOT NULL THEN '/' + lv2.Code
					WHEN empca.LeaveDay = 1 THEN ''
					WHEN empca.LeaveDay = 0.5 THEN '½'
					ELSE '' END
					)
			WHEN s.Code IS NOT NULL THEN s.Code
			ELSE '' END
			) AS Shift
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON emp.GroupId = g.GroupId
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON empca.EmpId = emp.EmpId
		AND empca.WorkDate BETWEEN @FromDate
			AND @ToDate
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON empca.ShiftId = s.ShiftID
		LEFT JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON empca.LeaveTypeId = lv.LeaveTypeId
		LEFT JOIN dbo.LeaveType AS lv2 WITH (NOLOCK) ON empca.LeaveTypeId2 = lv2.LeaveTypeId
	WHERE emp.[Enabled] = 1
		AND emp.isDeleted = 0
		AND (
			@EmployeeId IS NULL
			OR emp.EmpId = @EmployeeId
			)
		AND EXISTS (
			SELECT 1
			FROM #tblGroupIds AS tblgr
			WHERE tblgr.GroupId = emp.GroupId
			)
		AND (
			@IsLocal IS NULL
			OR empsum.IsLocal = @IsLocal
			)
	ORDER BY emp.NickName
		,empca.WorkDate

	DROP TABLE #tblGroupIds
END
