---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[EmployeeCalendar_GetHalfDayOff]
	@reportDate datetime,
	@EmpId int = null,
	@groupid int = null
AS
/*
Created: Tom.Vo@2013-10-29
Task: get changed shift in a month (it's used for coloring the day in report)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

SET NOCOUNT ON

DECLARE @Days varchar(1000), @cols varchar(2000), @totalDays int, 
		@fromDate datetime, @toDate datetime

-- first day of month
set @fromDate= cast(YEAR(@reportDate) as varchar(4)) + '/' + cast(MONTH(@reportDate) as varchar(2)) + '/01'

-- last day of month
set @ToDate = dateadd(d,-1, dateadd(m,1,@fromdate))

SELECT e.empid, e.NickName, e.FullName, DAY(workdate) as Day, s.Code 
FROM dbo.EmployeeCalendar c with (nolock)
	join employee e with (nolock) on e.empid=c.empid
	left join Shift s with (nolock) on s.shiftid= c.shiftid			
WHERE 	WorkDate between @fromDate and @toDate
	and (@EmpId is null or e.empid=@EmpId)
	and (@groupid  is null or e.groupid=@groupid)
	and IsHoliday =0  and IsOffDay=0 and	IsLeaveDay = 1 
	and LeaveDay = 0.5


