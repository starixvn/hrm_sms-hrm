
CREATE PROCEDURE [dbo].[EmployeeCalendar_Get] 
	@FromDate DATETIME = NULL
AS
/*
Created: 20141002@Quinn.Vo
Task: Get employee calendar from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tmpEmployeeCalendarChange
	(
		EmpId INT,
		WorkDate DATETIME
	)

	INSERT INTO #tmpEmployeeCalendarChange(EmpId, WorkDate)
	SELECT [EmpId]
	   ,[WorkDate]
	FROM dbo.EmployeeCalendar WITH (NOLOCK)
	WHERE @FromDate IS NULL
	OR ModifiedDate > @FromDate
	UNION 
	SELECT ca.[EmpId]
	   ,[WorkDate]
	FROM dbo.tmpEmployeeCalendar AS ca WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ca.EmpId = e.EmpId AND e.Enabled =1
	WHERE @FromDate IS NULL
	OR ca.ModifiedDate > @FromDate
	UNION
	SELECT EmpId, AttendanceDate
	FROM dbo.EmployeeAttendance
	WHERE @FromDate IS NULL
	OR InsertDate > @FromDate
	UNION 
	SELECT wfh.EmpId, [WorkDate]
	FROM dbo.tmpEmployeeCalendar AS tmp WITH (NOLOCK)
	INNER JOIN dbo.EmployeeWorkFromHome AS wfh WITH (NOLOCK) ON tmp.WorkDate BETWEEN wfh.FromDate AND wfh.ToDate
	WHERE @FromDate IS NULL
	OR wfh.ModifiedDate > @FromDate

	SELECT
	  tcl.[WorkDate]
      ,tcl.[EmpId] AS UserId
      ,tcl.[ShiftId]
      ,tcl.[IsHoliday]
      ,CAST(CASE WHEN (tcl.[IsOffDay] = 1 OR wfh.EmployeeWorkFromHomeId IS NOT NULL) THEN 1 ELSE 0 END AS BIT) AS IsOffDay
	   ,CASE WHEN tcl.EmpId IS NULL THEN cl.IsLeaveDay ELSE tcl.[IsLeaveDay] END AS IsLeaveDay
      ,CASE WHEN tcl.EmpId IS NULL THEN cl.[LeaveTypeId] ELSE tcl.[LeaveTypeId] END AS LeaveTypeId
      ,CASE WHEN tcl.EmpId IS NULL THEN cl.[LeaveDay] ELSE tcl.[LeaveDay] END AS LeaveDay
	  ,CASE WHEN cl.ModifiedDate > ISNULL(tcl.ModifiedDate, '2010-01-01') AND cl.ModifiedDate > ISNULL(att.InsertDate, '2010-01-01') THEN cl.ModifiedDate
		WHEN  ISNULL(tcl.ModifiedDate, '2010-01-01') > cl.ModifiedDate AND ISNULL(tcl.ModifiedDate, '2010-01-01')  > ISNULL(att.InsertDate, '2010-01-01') THEN tcl.ModifiedDate
		ELSE att.InsertDate END AS ModifiedDate
	  ,CAST(CASE WHEN att.InFixed IS NULL THEN 0 ELSE 1 END AS BIT) AS HasCheckIn

	FROM #tmpEmployeeCalendarChange AS tmp WITH (NOLOCK)
	INNER JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON tmp.EmpId = cl.EmpId AND tmp.WorkDate = cl.WorkDate
	INNER JOIN dbo.tmpEmployeeCalendar as tcl  ON tcl.EmpId = tmp.EmpId AND tcl.WorkDate = tmp.WorkDate
	LEFT JOIN dbo.EmployeeAttendance AS att ON tmp.EmpId = att.EmpId AND tmp.WorkDate = att.AttendanceDate
	LEFT JOIN dbo.EmployeeWorkFromHome AS wfh ON tmp.EmpId = wfh.EmpId AND tmp.WorkDate BETWEEN wfh.FromDate AND wfh.ToDate

	DROP TABLE #tmpEmployeeCalendarChange

END