
CREATE PROCEDURE [dbo].[Report_EmployeeCalendarByDateRange] 
	@EmployeeId INT = NULL
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@GroupIds VARCHAR(1000) = ''
	,@JobTitleId VARCHAR(1000) = ''
	,@IsLocal BIT = NULL
	,@IsDebug BIT = 0
AS
/*
Created: 20140711@Quinn.Vo
Task: Get employee calendar by date range.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
SET NOCOUNT ON;

CREATE TABLE #GroupsList (GroupId INT)

SET @groupids = ISNULL(@groupids, '')

IF @groupids <> ''
	INSERT INTO #GroupsList (GroupId)
	SELECT val
	FROM fnc_strSplit(@groupids, ',')
ELSE
BEGIN
	INSERT INTO #GroupsList (GroupId)
	SELECT GRoupID
	FROM [Group] g WITH (NOLOCK)
END

CREATE TABLE #JobTitleList (JobTitleId INT)

IF ISNULL(@JobTitleId, '') <> ''
	INSERT INTO #JobTitleList (JobTitleId)
	SELECT val
	FROM fnc_strSplit(@JobTitleId, ',')
ELSE
BEGIN
	INSERT INTO #JobTitleList (JobTitleId)
	SELECT ID
	FROM dbo.LookupData lk WITH (NOLOCK)
	WHERE LookupId = 1
END

CREATE TABLE #Calendar (WorkDate DATETIME)

WHILE (@FromDate <= @ToDate)
BEGIN
	INSERT INTO #Calendar (WorkDate)
	VALUES (@FromDate)

	SET @FromDate = DATEADD(d, 1, @FromDate)
END

SELECT e.EmpId
	,e.NickName
	,e.FullName
	,g.GroupId
	,g.Code
	,g.NAME
	,cl.WorkDate
	,(CASE WHEN ec.WorkDate IS NULL THEN ''
		WHEN ec.IsHoliday = 1 THEN 'PH'
		WHEN ec.IsOffDay = 1 THEN 'OFF'
		WHEN ec.IsLeaveDay = 1 THEN (lv.code + CASE WHEN lv2.LeaveTypeId IS NOT NULL THEN '/' + lv2.code WHEN ec.LeaveDay = 1 THEN '' WHEN ec.LeaveDay = 0.5 THEN '½' ELSE '' END)
		WHEN s.code IS NOT NULL THEN s.code
		ELSE '' END
	) AS Schedule
FROM #Calendar AS cl
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON 1 = 1
	INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
	INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	LEFT JOIN dbo.EmployeeCalendar AS ec WITH (NOLOCK) ON ec.EmpId = e.EmpId
	AND ec.WorkDate = cl.WorkDate
	LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON ec.ShiftId = s.ShiftID
	LEFT JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON ec.LeaveTypeId = lv.LeaveTypeId
	LEFT JOIN dbo.LeaveType AS lv2 WITH (NOLOCK) ON ec.LeaveTypeId2 = lv2.LeaveTypeId
WHERE e.[Enabled] = 1
	AND e.isDeleted = 0
	AND (
		@EmployeeId IS NULL
		OR e.empid = @EmployeeId
		)
	AND (
		@JobTitleId = ''
		OR e.JobTitleId IN (
			SELECT JobTitleId
			FROM #JobTitleList
			)
		)
	AND (
		@GroupIds = ''
		OR e.GroupId IN (
			SELECT GroupId
			FROM #GroupsList
			)
		)
	AND (@IsLocal IS NULL OR es.IsLocal = @IsLocal)
ORDER BY e.NickName
	,cl.WorkDate

DROP TABLE #Calendar
DROP TABLE #GroupsList
DROP TABLE #JobTitleList
