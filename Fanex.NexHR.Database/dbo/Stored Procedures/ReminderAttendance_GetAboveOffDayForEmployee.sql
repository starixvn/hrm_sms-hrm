
CREATE PROCEDURE [dbo].[ReminderAttendance_GetAboveOffDayForEmployee]
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
	SELECT @fromDate = DATEADD(d, -7, @today), @toDate = DATEADD(d, -1, @today)

	SELECT e.EmpId,
		e.FullName,
		e.NickName,
		e.Email,
		g.Name [Team], 
		g.Code [TeamCode],
		@fromDate AS FromDate,
		@toDate AS ToDate
	FROM 
		(SELECT cl.EmpId, COUNT(1) AS NumberOffDay
		FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON cl.EmpId = es.EmpId
		WHERE
			DATEDIFF(DD, @fromDate, cl.WorkDate) >= 0
			AND DATEDIFF(DD, cl.WorkDate, @toDate) >= 0
			AND ISNULL(cl.IsOffDay, 0) = 1
		GROUP BY cl.EmpId, es.OffDaysbyWeek
		HAVING COUNT(1) > ISNULL(es.OffDaysbyWeek, 0)
		) AS od
		INNER JOIN Employee e WITH (NOLOCK) ON od.Empid = e.Empid
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.groupid = g.groupid
	WHERE DATEPART(WEEKDAY, @today) = 2
		AND ISNULL(es.IsTopManager , 0) = 0
END
