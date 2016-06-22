
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
/*
declare @fromdate datetime,
		@todate datetime,
		@empid int,
		@groupid int
set @fromdate = '2014/09/01'
set @todate = '2014/09/30'
set @empid=245
EXEC Report_WorkingHoursSummary @fromdate , @todate ,	@empid , @groupid 
*/
CREATE PROCEDURE [dbo].[Report_WorkingHoursSummary] @fromdate DATETIME
	,@todate DATETIME
	,@empid INT = NULL
	,@groupids VARCHAR(max) = ''
	,@isLocal BIT = NULL
AS
/*
Created:  Tom.Vo@2013-11-21
Task: Working Hours Summary REPORT
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
DECLARE @UnpaidLeaveType INT

SELECT @UnpaidLeaveType = LeaveTypeId
FROM dbo.LeaveType WITH (NOLOCK)
WHERE Code = 'UPL'

CREATE TABLE #GroupsList (GroupId TINYINT)

INSERT INTO #GroupsList (GroupId)
SELECT val
FROM fnc_strSplit(@groupids, ',')

SELECT es.WorkingDays AS [WorkingDay]
	,SUM(CASE 
			WHEN (
					ISNULL(a.TotalWorkHours, 0) = 0
					OR (
						c.IsLeaveDay = 1
						AND c.LeaveDay = 1
						)
					) THEN 0
			ELSE (CASE 
					WHEN c.IsLeaveDay = 0 THEN 1
					ELSE (CASE 
							WHEN c.LeaveDay < 1 THEN 0.5
							ELSE 0 END
							) END
					)
			END) AS [ActualWorkingDay]
	,
	--sum(case when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=0 then 1 
	--		when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=1 and c.LeaveDay < 1 then 0.5
	--		else 0 end) [ActualWorkingDay],
	sum(CASE 
			WHEN isOffday = 0
				AND isHoliday = 0
				AND IsLeaveDay = 1
				AND (
					c.LeaveTypeId <> @UnpaidLeaveType
					OR (
						c.LeaveTypeId2 IS NOT NULL
						AND c.LeaveTypeId2 <> @UnpaidLeaveType
						)
					) THEN CASE 
					WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
						AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0) THEN 0.5
					ELSE c.LeaveDay
					END
			ELSE 0
			END) [LeaveTaken]
	,
	-- sum(case when isOffday=0 and isHoliday=0 and IsLeaveDay=1 and c.LeaveTypeId = @UnpaidLeaveType  then c.LeaveDay else 0 end) [UnpaidLeave],
	SUM((CASE 
			WHEN isOffday = 0
				AND c.isHoliday = 0
				AND IsLeaveDay = 1
				AND (c.LeaveTypeId = @UnpaidLeaveType
				OR c.LeaveTypeId2 = @UnpaidLeaveType) THEN CASE 
					WHEN ISNULL(c.LeaveTypeId2, 0) <> 0
						AND ISNULL(c.LeaveTypeId, 0) <> ISNULL(c.LeaveTypeId2, 0) THEN 0.5
					ELSE c.LeaveDay
					END
			ELSE 0 END
			)) AS [UnpaidLeave]
	,SUM(ISNULL(a.LateIn, 0)) LateInMins
	,SUM(ISNULL(a.EarlyOut, 0)) EarlyOutMins
	,SUM(ISNULL(a.TotalWorkHours, 0)) TotalHours
	,SUM(a.TotalWorkHours) / sum(CASE 
			WHEN Absence IS NOT NULL
				AND Absence = 0
				AND IsLeaveDay = 0 THEN 1
			WHEN Absence IS NOT NULL
				AND Absence = 0
				AND IsLeaveDay = 1
				AND c.LeaveDay < 1 THEN 0.5
			ELSE NULL
			END) AS AverageHours
	,c.EmpId
	,e.Code
	,FullName
	,NickName
	,g.GroupId
	,g.Code [Name]
FROM dbo.EmployeeAttendance a WITH (NOLOCK)
	RIGHT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.empid = a.empid
	AND c.workdate = a.AttendanceDate
	JOIN dbo.employee e WITH (NOLOCK) ON e.empid = c.empid
	JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON e.empid = es.empid
	LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON g.GroupId = e.GroupId
WHERE (
		@empid IS NULL
		OR c.empid = @empid
		)
	AND (
		e.isDeleted IS NULL
		OR e.isDeleted = 0
		)
	AND WorkDate BETWEEN @fromdate
		AND @todate
	AND (
		@isLocal IS NULL
		OR es.IsLocal = @isLocal
		)
	AND (
		@groupids IS NULL
		OR @groupids = ''
		OR e.GroupId IN (
			SELECT GroupId
			FROM #GroupsList
			)
		)
GROUP BY c.empid
	,e.Code
	,es.WorkingDays
	,e.FullName
	,NickName
	,g.GroupId
	,g.Code
	,[Rank]
ORDER BY [Rank]
	,g.Code
	,e.code

DROP TABLE #GroupsList
