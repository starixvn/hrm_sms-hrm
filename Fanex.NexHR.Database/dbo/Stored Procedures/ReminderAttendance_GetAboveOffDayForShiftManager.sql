/****** Object:  Stored Procedure dbo.ReminderAttendance_GetAboveOffDayForShiftManager    Script Date: 7/20/2016 10:19:16 AM ******/
/****** Object:  Stored Procedure dbo.ReminderAttendance_GetAboveOffDayForShiftManager    Script Date: 7/19/2016 5:22:19 PM ******/

CREATE PROCEDURE [dbo].[ReminderAttendance_GetAboveOffDayForShiftManager]
AS

/*
Created: 20151116@Quinn.Vo
Task: Get empoyee have above off day to remind via email.
DB: HRM
Original: 

Revisions:
Param's Explanation:

*/

BEGIN

	DECLARE @today DATETIME, @fromDate DATETIME, @toDate DATETIME
	SET @today = GETDATE()
	-- SELECT @today = '2015-11-16'
	SELECT @fromDate = DATEADD(d, -7, @today), @toDate = DATEADD(d, -1, @today)

	IF (DATEPART(WEEKDAY, @today) <> 2)
	BEGIN
		RETURN
	END

	SELECT e.EmpId,
		e.FullName,
		e.NickName,
		e.Email,
		g.GroupId,
		g.Name [Team], 
		g.Code [TeamCode]
	FROM 
		(SELECT cl.EmpId, COUNT(1) AS NumberOffDay, SUM(1) AS OffDay, SUM (1) AS OffDayOT
		FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON cl.EmpId = es.EmpId
		WHERE DATEDIFF(DD, @fromDate, cl.WorkDate) >= 0
			AND DATEDIFF(DD, cl.WorkDate, @toDate) >= 0
			AND ISNULL(cl.IsOffDay, 0) = 1
		GROUP BY cl.EmpId, es.OffDaysbyWeek
		HAVING COUNT(1) > ISNULL(es.OffDaysbyWeek, 0)
		) AS od
		INNER JOIN Employee e WITH (NOLOCK) ON od.Empid = e.Empid
		INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.groupid = g.groupid


	SELECT e.EmpId,
		e.FullName,
		e.NickName,
		e.Email,
		gs.GroupId
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.GroupSupervisor AS gs WITH (NOLOCK) ON e.EmpId = gs.OwnerId
	WHERE ActionType = 4
END