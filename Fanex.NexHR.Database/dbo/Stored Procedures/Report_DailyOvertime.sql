
CREATE PROCEDURE [dbo].[Report_DailyOvertime] @fromDate DATE
	,@toDate DATE
	,@EmployeeId INT = NULL
	,@groupIds VARCHAR(MAX) = NULL
	,@isLocal BIT = NULL
AS
/*
Created: Tim.Trinh@2019-07-10
Task: Get daily overtime details report 
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblGroupIds (GroupId INT)

	INSERT INTO #tblGroupIds (GroupId)
	SELECT val
	FROM dbo.fnc_strSplit(@groupids, ',')

	SELECT emp.EmpId AS EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,ot.OvertimeDate AS WorkDate
		,s.ShiftId
		,s.Code AS ShiftCode
		,CONVERT(VARCHAR(5), ot.TimeIn, 108) AS TimeIn
		,CONVERT(VARCHAR(5), ot.[TimeOut], 108) AS [TimeOut]
		,ot.TotalHours AS ActualWorkingHours
		,ot.NormalHours
		,ot.OffHours
		,ot.PublicHours
		,ot.Remark
		,empca.IsHoliday
		,empca.IsOffDay
		,ot.[Status] AS StatusId
	FROM dbo.OTDailyRequest AS ot WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = ot.EmpId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empsum.EmpId = emp.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmpId = empca.EmpId
		AND ot.OvertimeDate = empca.WorkDate
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
	WHERE ot.OvertimeDate BETWEEN @fromDate
			AND @toDate
		AND (
			@EmployeeId IS NULL
			OR ot.EmpId = @EmployeeId
			)
		AND (
			@isLocal IS NULL
			OR empsum.IsLocal = @isLocal
			)
		AND (
			ISNULL(@groupIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM #tblGroupIds AS tbl
				WHERE emp.GroupId = tbl.GroupId
				)
			)
	ORDER BY ot.OvertimeDate
		,emp.NickName

	DROP TABLE #tblGroupIds
END
