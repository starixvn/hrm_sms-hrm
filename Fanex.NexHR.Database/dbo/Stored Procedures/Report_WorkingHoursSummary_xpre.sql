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
CREATE PROCEDURE [dbo].[Report_WorkingHoursSummary_xpre] 
	@fromdate datetime,
	@todate datetime,
	@empid int=NULL,
	@groupids varchar(max) = '',
	@isLocal bit = NULL
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
	WHERE Code='UPL'	

	CREATE TABLE #GroupsList (GroupId TINYINT)
	INSERT INTO #GroupsList (GroupId)
	SELECT val FROM fnc_strSplit (@groupids, ',')

	SELECT 
		es.WorkingDays as [WorkingDay],
		SUM(CASE WHEN (ISNULL(a.TotalWorkHours, 0) = 0 OR (c.IsLeaveDay = 1 AND c.LeaveDay = 1)) THEN 0 ELSE 
		(CASE WHEN c.IsLeaveDay = 0 THEN 1 ELSE (CASE WHEN c.LeaveDay < 1 THEN 0.5 ELSE 0 END) END)
		 END) AS [ActualWorkingDay],
		--sum(case when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=0 then 1 
		--		when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=1 and c.LeaveDay < 1 then 0.5
		--		else 0 end) [ActualWorkingDay],
		sum(case when isOffday=0 and isHoliday=0 and IsLeaveDay=1 and c.LeaveTypeId <> @UnpaidLeaveType then c.LeaveDay else 0 end) [LeaveTaken],
		sum(case when isOffday=0 and isHoliday=0 and IsLeaveDay=1 and c.LeaveTypeId = @UnpaidLeaveType  then c.LeaveDay else 0 end) [UnpaidLeave],
		SUM(ISNULL(a.LateIn,0)) LateInMins,
		SUM(ISNULL(a.EarlyOut,0)) EarlyOutMins,
		SUM(ISNULL(a.TotalWorkHours,0)) TotalHours,
		SUM(a.TotalWorkHours)/sum(case when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=0 then 1 
				when Absence IS NOT NULL AND Absence = 0 and IsLeaveDay=1 and c.LeaveDay < 1 then 0.5
				else NULL end) as AverageHours,
				c.EmpId, e.Code, FullName, NickName, g.GroupId, g.Code [Name]
	FROM	
		dbo.EmployeeAttendance a with (nolock)
		right join dbo.EmployeeCalendar c with (nolock)  on c.empid = a.empid and c.workdate = a.AttendanceDate
		join dbo.employee e with (nolock) on e.empid = c.empid	
		join dbo.EmployeeSummary es with (nolock) on e.empid = es.empid	
		left join dbo.[Group] g with (nolock) on g.GroupId = e.GroupId		
	WHERE (@empid is null or c.empid= @empid)
		and (e.isDeleted is null or e.isDeleted = 0)
		and WorkDate between @fromdate and @todate
		and (@isLocal is null or es.IsLocal = @isLocal)
		AND (@groupids IS NULL OR @groupids = '' OR e.GroupId IN (SELECT GroupId FROM #GroupsList))
	GROUP BY c.empid, e.Code,es.WorkingDays, e.FullName, NickName, g.GroupId, g.Code, [Rank]
	ORDER BY [Rank] , g.Code , e.code

DROP TABLE #GroupsList


