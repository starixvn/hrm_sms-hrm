CREATE PROCEDURE [dbo].[Report_EmployeeCalendarByWeekly] @EmpId INT = NULL
	,@fromtime DATETIME
	,@totime DATETIME
	,@GroupIds VARCHAR(1000) = ''
	,@JobTitleId VARCHAR(1000) = ''
	,@isLocal BIT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
	,@isDebug BIT = 0
AS
/*
Created: Tom.Vo@2013-11-15
Task: view employee calendar by weekly
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromDate NVARCHAR(100)
		,@toDate NVARCHAR(100)

	SELECT @fromDate = CONVERT(VARCHAR(10), @fromtime, 111) + ' 00:00:00'
		,@toDate = CONVERT(VARCHAR(10), @totime, 111) + ' 00:00:00'

	IF (
			DATEPART(dw, @fromDate) <> 2
			OR DATEPART(dw, @toDate) <> 1
			OR DATEDIFF(dd, @fromdate, @toDate) <> 6
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Parameter not correct.'

		RETURN
	END

	DECLARE @LeaveTypeDefaultId AS INT

	SELECT @LeaveTypeDefaultId = dbo.fnc_getLeaveTypeDefault()

	DECLARE @HolidayLeaveTypeCode VARCHAR(50)

	-- get leave type of holiday
	SELECT @HolidayLeaveTypeCode = code
	FROM dbo.LeaveType(NOLOCK)
	WHERE IsHoliday = 1

	CREATE TABLE #Group (GroupId INT)

	SET @GroupIds = ISNULL(@GroupIds, '')

	IF @GroupIds <> ''
		INSERT INTO #Group (GroupId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	ELSE
	BEGIN
		INSERT INTO #Group (GroupId)
		SELECT g.GroupId
		FROM dbo.[Group] AS g WITH (NOLOCK)
		WHERE ISNULL(g.IsDeleted, 0) = 0
	END

	CREATE TABLE #JobTitle (JobTitleId INT)

	IF ISNULL(@JobTitleId, '') <> ''
		INSERT INTO #JobTitle (JobTitleId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@JobTitleId, ',')
	ELSE
	BEGIN
		INSERT INTO #JobTitle (JobTitleId)
		SELECT DISTINCT lk.Id
		FROM dbo.LookupData AS lk WITH (NOLOCK)
		WHERE lk.LookupId = 1
	END

	CREATE TABLE #Calendar (
		WorkDate DATETIME
		,[Enabled] BIT
		,IsDeleted BIT
		)

	WHILE (@fromDate <= @toDate)
	BEGIN
		INSERT INTO #Calendar (
			WorkDate
			,[Enabled]
			,IsDeleted
			)
		VALUES (
			@fromDate
			,1
			,0
			)

		SET @fromDate = CONVERT(VARCHAR(10), CONVERT(DATETIME, @fromDate) + 1, 111) + ' 00:00:00'
	END

	SET @fromDate = CONVERT(VARCHAR(10), @fromtime, 111) + ' 00:00:00'

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
		,@temptbl NVARCHAR(4000)
		,@updtbl NVARCHAR(4000)
	DECLARE @cols VARCHAR(2000)
		,@totalDays INT

	SELECT @i = 1
		,@totalDays = 7

	SELECT @temptbl = ''
		,@updtbl = ''

	WHILE (@i <= @totalDays)
	BEGIN
		SET @temptbl = @temptbl + 'ALTER TABLE #tmpCalendar ADD [' + cast(@i AS VARCHAR(2)) + '] NVARCHAR(100) NULL' + CHAR(13)
		SET @temptbl = @temptbl + 'ALTER TABLE #tmpCalendar ADD [' + cast(@i + 100 AS VARCHAR(10)) + '] NVARCHAR(100) NULL' + CHAR(13)
		SET @updtbl = @updtbl + ', tmp.[' + cast(@i + 100 AS VARCHAR(10)) + '] = tblC.[' + cast(@i AS VARCHAR(10)) + ']'
		SET @i = @i + 1
	END

	SET @updtbl = RIGHT(@updtbl, LEN(@updtbl) - 1)

	IF @isDebug = 1
		PRINT @temptbl

	EXECUTE sp_executesql @temptbl

	SELECT e.EmpId
		,DATEPART(dw, tmp.WorkDate) [Weekday]
		,(CASE 
			WHEN isLeaveDay = 1 THEN lv.code + (CASE 
					WHEN lv2.LeaveTypeId IS NOT NULL THEN '/' + lv2.code
					WHEN c.LeaveDay = 1 THEN ''
					WHEN c.LeaveDay = 0.5 THEN '½'
					ELSE '' END
					)
			WHEN s.code IS NOT NULL THEN s.code
			WHEN c.isHoliday = 1 THEN @HolidayLeaveTypeCode
			WHEN esum.islocal = 1
				AND hld.HolidayName IS NOT NULL THEN @HolidayLeaveTypeCode
			WHEN isOffday = 1 THEN 'OFF' END
			) AS Code
		,isOffday
		,(CASE 
			WHEN c.IsHoliday IS NOT NULL THEN c.IsHoliday
			WHEN esum.islocal = 1
				AND hld.HolidayName IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,lk.[Name] AS [Positions]
	INTO #CalendarWeekLy
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #Calendar tmp WITH (NOLOCK) ON e.[Enabled] = tmp.[Enabled]
		AND e.[IsDeleted] = tmp.[IsDeleted]
		LEFT JOIN dbo.LookupData lk WITH (NOLOCK) ON e.JobTitleId = lk.Id
		AND LookupId = 1
		LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.WorkDate = tmp.WorkDate
		AND e.empid = c.empid
		AND c.IsPublic = 1
		LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.LeaveTypeId = c.LeaveTypeId
		LEFT JOIN dbo.LeaveType lv2 WITH (NOLOCK) ON lv2.LeaveTypeId = c.LeaveTypeId2
		LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.shiftid = c.shiftid
		LEFT JOIN dbo.EmployeeSummary esum WITH (NOLOCK) ON esum.EmpId = e.EmpId
		AND esum.islocal = 1
		LEFT JOIN dbo.Holiday hld WITH (NOLOCK) ON (
			hld.FromDate BETWEEN @fromDate
				AND @toDate
			OR hld.ToDate BETWEEN @fromDate
				AND @toDate
			)
		AND (
			tmp.WorkDate BETWEEN hld.FromDate
				AND hld.ToDate
			)
		LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON g.groupid = e.groupid
	WHERE (
			@EmpId IS NULL
			OR e.empid = @EmpId
			)
		AND (
			@JobTitleId = ''
			OR e.JobTitleId IN (
				SELECT JobTitleId
				FROM #JobTitle
				)
			)
		AND (
			@GroupIds = ''
			OR e.GroupId IN (
				SELECT GroupId
				FROM #Group
				)
			)

	DECLARE @query NVARCHAR(4000)

	SET @cols = 'EmpId, [2],[3],[4],[5],[6],[7],[1]'
	SET @query = 'INSERT INTO #tmpCalendar (' + @cols + ')
	SELECT ' + @cols + ' FROM (
		SELECT EmpId, Weekday, Code FROM #CalendarWeekLy WITH (NOLOCK)	
	) AS ea
	PIVOT (MAX(Code) FOR [Weekday] IN ([2],[3],[4],[5],[6],[7],[1])' + ') AS pvt'

	IF @isDebug = 1
		PRINT @query

	EXECUTE sp_executesql @query

	SET @query = '
	UPDATE tmp
	SET ' + @updtbl + '
	FROM #tmpCalendar tmp
		INNER JOIN (
		SELECT ' + @cols + ' FROM ( 
			SELECT c.EmpId, c.[Weekday], 
				CASE WHEN c.isOffday = 1 THEN ''#F8F89C'' 
					WHEN c.IsHoliday = 1 THEN ''#F3AA4B''
					WHEN s.Color IS NOT NULL THEN s.Color
					WHEN lt.Color IS NOT NULL THEN lt.Color 
					WHEN c.Code IN (''OFF'') THEN ''#F8F89C''
					WHEN c.Code IN (''AL½'') THEN ''#C7F391''
					WHEN c.Code IN (''UPL½'') THEN ''#EACDED''
					WHEN c.Code IN (''WL½'') THEN ''#7CD3E9''
					WHEN c.Code IN (''BT½'') THEN ''#C1D6ED''
					WHEN c.Code LIKE (''%/%'') THEN ''#D4E5D5''
					ELSE ''''
					END AS Color
			 FROM #CalendarWeekLy c LEFT JOIN  Shift s WITH(NOLOCK) on s.Code= c.Code
				LEFT JOIN  LeaveType lt WITH(NOLOCK) on lt.Code= c.Code
		) ea
		PIVOT (MAX(Color) FOR [Weekday] IN ([2],[3],[4],[5],[6],[7],[1])' + ') AS pvt ) tblC
		ON tblC.EmpId = tmp.EmpId 
		'

	IF @isDebug = 1
		PRINT @query

	EXECUTE sp_executesql @query

	IF @isLocal IS NULL
		SELECT tmp.EmpId
			,ee.Code AS EmpCode
			,ee.FullName
			,ee.NickName
			,lk.[Name] AS [Positions]
			,g.[Name] AS Team
			,[2]
			,[102]
			,[3]
			,[103]
			,[4]
			,[104]
			,[5]
			,[105]
			,[6]
			,[106]
			,[7]
			,[107]
			,[1]
			,[101]
		FROM #tmpCalendar AS tmp WITH (NOLOCK)
			LEFT JOIN dbo.Employee AS ee WITH (NOLOCK) ON tmp.EmpId = ee.EmpId
			LEFT JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON tmp.EmpId = es.EmpId
			LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.groupid = ee.groupid
			LEFT JOIN dbo.LookupData AS lk WITH (NOLOCK) ON ee.JobTitleId = lk.Id
			AND LookupId = 1
		ORDER BY Team
			,es.IsLocal DESC
			,ee.Code
	ELSE
		SELECT tmp.EmpId
			,ee.Code AS EmpCode
			,ee.FullName
			,ee.NickName
			,lk.[Name] AS [Positions]
			,g.[Name] AS Team
			,[2]
			,[102]
			,[3]
			,[103]
			,[4]
			,[104]
			,[5]
			,[105]
			,[6]
			,[106]
			,[7]
			,[107]
			,[1]
			,[101]
		FROM #tmpCalendar AS tmp WITH (NOLOCK)
			INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON tmp.EmpId = es.EmpId
			AND es.IsLocal = @isLocal
			LEFT JOIN dbo.Employee AS ee WITH (NOLOCK) ON tmp.EmpId = ee.EmpId
			LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.groupid = ee.groupid
			LEFT JOIN dbo.LookupData AS lk WITH (NOLOCK) ON ee.JobTitleId = lk.Id
			AND LookupId = 1
		ORDER BY Team
			,ee.Code

	DROP TABLE #CalendarWeekLy

	DROP TABLE #Calendar

	DROP TABLE #tmpCalendar

	DROP TABLE #Group

	SELECT @ErrCode = 0
END