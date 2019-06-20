

CREATE PROCEDURE [dbo].[EmployeeCalendar_GetByDateRange]
	@SupervisorID INT = 0,
	@empId int = null,
	@groupIds varchar(max) = '',
	@fromDate datetime,
	@toDate datetime
AS
/*
Created: Quinn.Vo@2013-11-05
Task: GET calendar of Employee between fromdate and todate
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	CREATE TABLE #GroupsList (GroupId INT)

	INSERT INTO #GroupsList (GroupId)
	SELECT val FROM fnc_strSplit (@groupids, ',')

	SELECT 
		ca.WorkDate,
		DATEPART(dw,ca.WorkDate) AS [Weekday],
		ca.IsHoliday,
		ca.IsOffDay,
		ca.IsLeaveDay,
		ca.IsKeepInView,
		e.EmpId,
		e.NickName,
		e.FullName,
		g.GroupId,
		g.Code,
		g.Name,
		s.ShiftId,
		s.Name,
		lv.LeaveTypeId,
		lv.Name
	FROM dbo.EmployeeCalendar AS ca WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ca.EmpId = e.EmpId
		LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON ca.ShiftId = s.ShiftID
		LEFT JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON ca.LeaveTypeId = lv.LeaveTypeId
	WHERE (@empId IS NULL OR ca.EmpId = @empId)
		AND ca.WorkDate BETWEEN @fromDate AND @toDate
		AND (ISNULL(@groupids,'') = '' OR e.GroupId IN (SELECT GroupId FROM #GroupsList) )
	ORDER BY e.EmpId, ca.WorkDate

DROP TABLE #GroupsList
END



