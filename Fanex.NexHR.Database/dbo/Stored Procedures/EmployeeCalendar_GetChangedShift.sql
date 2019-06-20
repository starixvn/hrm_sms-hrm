/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetChangedShift    Script Date: 7/20/2016 10:19:04 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetChangedShift    Script Date: 7/19/2016 5:22:13 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- EmployeeCalendar_GetChangedShift '2012/06/6'
CREATE PROCEDURE [dbo].[EmployeeCalendar_GetChangedShift]
	@reportDate datetime,
	@EmpId int = null,
	@groupid int = null
AS
/*
Created: Tom.Vo@2013-10-29
Task: get changed shift in a month (it's used for coloring the day in report)
DB: HRM_SMS
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

SELECT e.empid, e.NickName, e.FullName, DAY(workdate) as Day, s.Code , s.shiftid, cs.Code Code_From, cs.shiftid shiftid_From
FROM dbo.EmployeeCalendar c with (nolock)
	left join dbo.employee e with (nolock) on e.empid=c.empid
	left join dbo.Shift s with (nolock) on s.shiftid= c.shiftid
	left join dbo.ChangeShiftRequestDetail csd with (nolock) ON	c.WorkDate = csd.OnDate AND c.EmpId = csd.EmpId
	left join dbo.Shift cs with (nolock) on cs.shiftid= csd.fromshift	
WHERE WorkDate between @fromDate and @toDate
	and (@EmpId is null or e.empid=@EmpId)
	and (@groupid  is null or e.groupid=@groupid)
	and ChangeShiftId is not null 
	and csd.fromshift is not null
	and IsHoliday=0 and IsOffDay=0 and IsLeaveDay=0


