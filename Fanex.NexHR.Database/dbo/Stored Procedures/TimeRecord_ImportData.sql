
CREATE PROCEDURE [dbo].[TimeRecord_ImportData]
AS
-- =============================================
-- Author:		Quinn.Vo
-- Create date: 2013/06/20
-- Description:	Import data replace for Job (in case use SQL Express is can not create job)
-- =============================================
BEGIN
	SET NOCOUNT ON;
	DECLARE @fromdate DATETIME
		, @todate DATETIME

	SET @todate= CONVERT(VARCHAR(10), getdate(), 111)
	
	SET @fromdate = dateadd(d, -2 , @todate)
	EXEC dbo.TimeRecord_ImportFromBioStar @fromdate, @todate
	
	EXEC [dbo].[EmployeeAttendance_ForWaitingCase]

	
	SELECT EmpId
	INTO #tblEmp
	FROM dbo.EmployeeCalendar WITH (NOLOCK) 
	WHERE DATEDIFF(DD, WorkDate, @todate) = 0
	AND (
		ShiftId IS NOT NULL
		OR IsOffDay = 1
		OR IsHoliday = 1
		OR IsLeaveDay = 1
	)
	IF (EXISTS (SELECT 1 FROM dbo.Employee e WITH (NOLOCK)
			INNER JOIN 
			#tblEmp ec ON e.EmpId = ec.EmpId
			WHERE e.Enabled = 1
				AND DATEDIFF(DD, e.JoinedDate, @todate) > 0
				AND (e.ResignedDate IS NULL OR DATEDIFF(DD, @todate, e.ResignedDate) > 0)
				AND e.EmpId NOT IN (SELECT EmpId 
				FROM dbo.EmployeeAttendance WITH (NOLOCK) WHERE DATEDIFF(DD, AttendanceDate, @todate) = 0
				AND EmpId = e.EmpId)))
		BEGIN
			INSERT INTO dbo.tmpEmployeeAttendace(EmpId, WorkDate, InsertedDate)
			SELECT e.EmpId,@todate, GETDATE()
			FROM dbo.Employee e WITH (NOLOCK)
			INNER JOIN #tblEmp ec ON e.EmpId = ec.EmpId 

			WHERE e.Enabled = 1
				AND DATEDIFF(DD, e.JoinedDate, @todate) > 0
				AND (e.ResignedDate IS NULL OR DATEDIFF(DD, @todate, e.ResignedDate) > 0)
				AND e.EmpId NOT IN (SELECT EmpId 
					FROM dbo.EmployeeAttendance WITH (NOLOCK) WHERE DATEDIFF(DD, AttendanceDate, @todate) = 0 AND EmpId = e.EmpId)
		END

	INSERT INTO dbo.tmpEmployeeAttendace(EmpId, WorkDate, InsertedDate)
		SELECT empA.[EmpId]
			,[AttendanceDate]
			, GETDATE()
	FROM [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK)
		INNER JOIN [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK) ON empA.[EmpId] = empC.[EmpId]
		AND empA.[AttendanceDate] = empC.[WorkDate]
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empA.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 2
	WHERE 
			(
			empA.[AttendanceDate] BETWEEN @FromDate
				AND @ToDate
			)
		AND ISNULL(empA.[ShiftId], '') <> ''
		AND empC.LeaveDay <> 1
		AND (
			(
				ISNULL([InFixed], '') = ''
				AND ISNULL([OutFixed], '') <> ''
				AND GETDATE() > empA.[ExpectedInTime]
				AND DATEDIFF(MI, empA.[ExpectedInTime], GETDATE()) < 10
				)
			OR (
				ISNULL([InFixed], '') <> ''
				AND ISNULL([OutFixed], '') = ''
				AND GETDATE() > empA.[ExpectedOutTime]
				AND DATEDIFF(MI, empA.[ExpectedOutTime], GETDATE()) < 10
				)
			)
		AND ISNULL(rul.Value, 0) = 1

	UNION
	SELECT att.EmpId
		,att.AttendanceDate
		, GETDATE()
	FROM [dbo].[EmployeeAttendance] AS att WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 5
	WHERE (
			att.AttendanceDate BETWEEN @FromDate
				AND @ToDate
			)
		AND att.[EmpId] = att.[EmpId]
		AND ISNULL(att.[ShiftId], '') <> ''
		AND ISNULL(att.[InFixed], '') = ''
		AND ISNULL(att.[OutFixed], '') = ''
		AND GETDATE() > att.[ExpectedOutTime]
		AND DATEDIFF(MI, att.[ExpectedOutTime], GETDATE()) < 10
		AND ISNULL(att.LeaveDay, 0) <> 1
		AND ISNULL(rul.Value, 0) = 1

	DROP TABLE #tblEmp
END
