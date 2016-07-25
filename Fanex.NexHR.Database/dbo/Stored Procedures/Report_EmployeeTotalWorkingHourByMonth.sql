CREATE PROCEDURE [dbo].[Report_EmployeeTotalWorkingHourByMonth] @reportMonth DATETIME
	,@empId INT = NULL
	,@groupids VARCHAR(1000) = ''
	,@isLocal BIT = NULL
	,@isDebug BIT = 0
AS
/*
Created: Tom.Vo@2013-11-15
Task: view employee Total Working Hour by month
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Days VARCHAR(1000)
		,@cols VARCHAR(2000)
		,@totalDays INT
		,@fromDatetmp DATETIME
		,@temptbl NVARCHAR(4000)
		,@updtbl NVARCHAR(4000)
		,@FromDate DATETIME
		,@toDate DATETIME

	-- first day of month
	SET @fromDate = CAST(YEAR(@reportMonth) AS VARCHAR(4)) + '/' + CAST(MONTH(@reportMonth) AS VARCHAR(2)) + '/01'
	-- last day of month
	SET @ToDate = DATEADD(d, - 1, DATEADD(m, 1, @fromdate))
	SET @totalDays = DAY(@ToDate)

	CREATE TABLE #tmpCalendar (
		EmpId INT
		,EmpCode NVARCHAR(200)
		,fullname NVARCHAR(200)
		,nickname NVARCHAR(100)
		,Team NVARCHAR(100)
		,Positions NVARCHAR(100)
		)

	-- process the days string to use in pivot
	DECLARE @i INT

	SELECT @i = 1
		,@days = ''

	SELECT @temptbl = ''
		,@updtbl = ''

	WHILE (@i <= @totalDays)
	BEGIN
		SET @days = @days + ',[' + CAST(@i AS VARCHAR(2)) + ']'
		SET @temptbl = @temptbl + 'ALTER TABLE #tmpCalendar ADD [' + CAST(@i AS VARCHAR(2)) + '] NVARCHAR(100) NULL' + CHAR(13)
		SET @i = @i + 1
	END

	IF @isDebug = 1
		PRINT @temptbl

	EXECUTE sp_executesql @temptbl

	SELECT @days = SUBSTRING(@days, 2, LEN(@days) - 1)

	SET @cols = 'EmpId, EmpCode, FullName, NickName, Team, Positions,' + @days

	DECLARE @LeaveTypeDefaultId AS INT

	SELECT @LeaveTypeDefaultId = dbo.fnc_getLeaveTypeDefault()

	DECLARE @HolidayLeaveTypeCode VARCHAR(50)

	-- get leave type of holiday
	SELECT @HolidayLeaveTypeCode = code
	FROM dbo.LeaveType(NOLOCK)
	WHERE IsHoliday = 1

	CREATE TABLE #GroupsList (GroupId INT)

	SET @groupids = ISNULL(@groupids, '')

	IF ISNULL(@groupids, '') <> ''
		INSERT INTO #GroupsList (GroupId)
		SELECT val
		FROM fnc_strSplit(@groupids, ',')
	ELSE
	BEGIN
		INSERT INTO #GroupsList (GroupId)
		SELECT GRoupID
		FROM [Group] g WITH (NOLOCK)
	END

	CREATE TABLE #CalendarDF (
		WorkDate DATETIME
		,[Enabled] BIT
		,IsDeleted BIT
		)

	SELECT @fromDatetmp = @fromDate

	WHILE (@fromDatetmp <= @toDate)
	BEGIN
		INSERT INTO #CalendarDF (
			WorkDate
			,[Enabled]
			,IsDeleted
			)
		VALUES (
			@fromDatetmp
			,1
			,0
			)

		SET @fromDatetmp = CONVERT(VARCHAR(10), CONVERT(DATETIME, @fromDatetmp) + 1, 111) + ' 00:00:00'
	END

	SELECT g.[Rank]
		,g.Code AS [Team]
		,e.Code AS EmpCode
		,e.NickName
		,e.FullName
		,e.EmpId
		,DAY(DF.workdate) AS d
		,CONVERT(NVARCHAR(10), CONVERT(INT, e_att.TotalWorkHours)) + 'h'
		     + REPLICATE('0', 2 - LEN(CONVERT(NVARCHAR(10), CONVERT(INT, ROUND((e_att.TotalWorkHours % 1) * 60, 0)))))
		     + CONVERT(NVARCHAR(10), CONVERT(INT, ROUND((e_att.TotalWorkHours % 1) * 60, 0))) AS TotalWorkHours
		,(CASE 
			WHEN c.isHoliday = 1 THEN @HolidayLeaveTypeCode
			WHEN isLeaveDay = 1 THEN lv.code + (CASE 
					WHEN lv2.LeaveTypeId IS NOT NULL THEN '/' + lv2.code
					WHEN c.LeaveDay = 1 THEN ''
					WHEN c.LeaveDay = 0.5 THEN '½'
					ELSE '' END
					)
			WHEN esum.IsLocal = 1
				AND hld.HolidayName IS NOT NULL THEN @HolidayLeaveTypeCode
			WHEN s.code IS NOT NULL THEN s.code
			WHEN isOffday = 1 THEN 'OFF' END
			) AS Code
		,isLeaveDay
		,isOffday
		,(CASE 
			WHEN e_att.AttendanceDate < getdate() - 1
				AND e_att.ShiftId IS NOT NULL
				AND (
					InFixed IS NULL
					OR OutFixed IS NULL
					)
				AND NOT (
					InFixed IS NULL
					AND OutFixed IS NULL
					)
				AND e_att.TotalWorkHours IS NULL THEN 1
			ELSE 0 END
			) AS isMissIO
		,(CASE 
			WHEN e_att.AttendanceDate < getdate() - 1
				AND (
					(
						c.ShiftId IS NOT NULL
						AND e_s.IsDefaultSchedule = 0
						)
					OR (
						c.ShiftId IS NOT NULL
						AND e_s.IsLocal = 1
						AND e_s.IsDefaultSchedule = 1
						AND NOT (
							c.IsHoliday = 1
							OR c.IsOffDay = 1
							)
						)
					)
				AND (
					InFixed IS NULL
					AND OutFixed IS NULL
					)
				AND e_att.TotalWorkHours IS NULL THEN 1
			ELSE 0 END
			) AS isAbsent
		,(CASE 
			WHEN c.IsHoliday IS NOT NULL THEN c.IsHoliday
			WHEN esum.islocal = 1
				AND hld.HolidayName IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,lk.[Name] AS [Positions]
	INTO #Calendar
	FROM dbo.Employee AS e WITH (NOLOCK)
		LEFT JOIN #CalendarDF AS DF ON DF.[Enabled] = e.[Enabled]
		AND DF.IsDeleted = e.IsDeleted
		LEFT JOIN dbo.EmployeeCalendar AS c WITH (NOLOCK) ON DF.WorkDate = c.WorkDate
		AND e.EmpId = c.EmpId
		AND c.IsPublic = 1
		LEFT JOIN dbo.EmployeeAttendance AS e_att WITH (NOLOCK) ON DF.WorkDate = e_att.AttendanceDate
		AND e.EmpId = e_att.EmpId
		LEFT JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON lv.LeaveTypeId = c.LeaveTypeId
		LEFT JOIN dbo.LeaveType AS lv2 WITH (NOLOCK) ON lv2.LeaveTypeId = c.LeaveTypeId2
		LEFT JOIN dbo.Shift AS s WITH (NOLOCK) ON s.shiftid = c.shiftid
		LEFT JOIN dbo.EmployeeSummary AS esum WITH (NOLOCK) ON esum.EmpId = e.EmpId
		AND esum.IsLocal = 1
		LEFT JOIN dbo.Holiday hld WITH (NOLOCK) ON (
			hld.FromDate BETWEEN @fromDate
				AND @toDate
			OR hld.ToDate BETWEEN @fromDate
				AND @toDate
			)
		AND (
			DF.WorkDate BETWEEN hld.FromDate
				AND hld.ToDate
			)
		LEFT JOIN dbo.LookupData AS lk WITH (NOLOCK) ON e.JobTitleId = lk.Id
		AND lk.LookupId = 1
		LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.groupid = e.groupid
		LEFT JOIN dbo.EmployeeSummary AS e_s WITH (NOLOCK) ON e_s.EmpId = e.EmpId
	WHERE DF.WorkDate BETWEEN @fromDate
			AND @toDate
		AND (
			@empId IS NULL
			OR e.empid = @empId
			)
		AND (
			@isLocal IS NULL
			OR e_s.IsLocal = @isLocal
			)
		AND e.GroupId IN (
			SELECT GroupId
			FROM #GroupsList
			)

	SELECT a.EmpId
		,SUM(CASE 
				WHEN ISNULL(LeaveDay, 0) = 0 THEN 1
				ELSE ISNULL(LeaveDay, 0)
				END) AS TotalWorkDays
	INTO #Calendar_TotalWorkDays
	FROM dbo.EmployeeAttendance AS a WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON a.empid = e.empid
		LEFT JOIN dbo.EmployeeSummary AS e_s WITH (NOLOCK) ON e_s.EmpId = e.EmpId
	WHERE a.AttendanceDate BETWEEN @fromDate
			AND @toDate
		AND (
			@empId IS NULL
			OR a.EmpId = @empId
			)
		AND (
			@isLocal IS NULL
			OR e_s.IsLocal = @isLocal
			)
		AND e.GroupId IN (
			SELECT GroupId
			FROM #GroupsList
			)
		--and  isnull(a.Absence,0)=0	 
		AND a.InFixed IS NOT NULL
		AND a.OutFixed IS NOT NULL
	GROUP BY a.empid

	DECLARE @query NVARCHAR(4000)

	SET @query = 'INSERT INTO #tmpCalendar (' + @cols + ')
	SELECT ' + @cols + ' FROM (
		SELECT Team, Positions, EmpId, EmpCode, NickName, FullName, d,
		CASE WHEN isLeaveDay =1 THEN Code 
			WHEN isMissIO =1 AND ISNULL(TotalWorkHours,''0.00'') = ''0.00'' THEN ''?'' 
			WHEN isAbsent =1 AND ISNULL(TotalWorkHours,''0.00'') = ''0.00'' THEN ''Absent''			
			WHEN isOffday =1 AND Code = ''OFF'' THEN ''OFF'' 
			WHEN IsHoliday =1 AND ISNULL(TotalWorkHours,''0.00'') = ''0.00'' THEN ''PH'' 			
			ELSE REPLACE(TotalWorkHours,''.'',''h'')
			END Code
		FROM #Calendar	WITH(NOLOCK)
	) AS ea
	PIVOT (MAX(Code) FOR d IN (' + @days + ')' + ') AS pvt'

	IF @isDebug = 1
		PRINT @query

	EXECUTE sp_executesql @query

	SELECT @query = 'INSERT  INTO #tmpCalendar (Team,' + @days + ')' + CHAR(13)

	SELECT @query = @query + 'VALUES( '''','

	SELECT @totalDays = DAY(@ToDate)
		,@i = 1

	WHILE (@i <= @totalDays)
	BEGIN
		SET @query = @query + '''' + CONVERT(NVARCHAR(20), CONVERT(DATETIME, @fromDate), 106) + ' (' + LEFT(DATENAME(weekday, @fromDate), 3) + ')'',' + CHAR(13)
		SET @fromDate = CONVERT(VARCHAR(10), CONVERT(DATETIME, @fromDate) + 1, 111) + ' 00:00:00'
		SET @i = @i + 1
	END

	SELECT @query = LEFT(@query, LEN(@query) - 2) + ')'

	IF @isDebug = 1
		PRINT @query

	EXECUTE sp_executesql @query

	SELECT @query = 'SELECT EmpCode AS [Staff ID],
			Team AS Section,
			FullName AS [Full Name/Nick],
			NickName AS [Nick],
			ISNULL(TotalWorkDays,0) AS [Count WorkDay],' + @days

	SELECT @query = @query + ' FROM #tmpCalendar AS cl
	          LEFT JOIN #Calendar_TotalWorkDays AS ttd WITH(NOLOCK) ON cl.EmpId = ttd.EmpId
          ORDER BY Team, [Positions], FullName'

	IF @isDebug = 1
		PRINT @query

	EXECUTE sp_executesql @query

	DROP TABLE #Calendar_TotalWorkDays

	DROP TABLE #tmpCalendar

	DROP TABLE #Calendar

	DROP TABLE #GroupsList

	DROP TABLE #CalendarDF
END