/****** Object:  Stored Procedure dbo.EmployeeCalendar_CheckExistedCalendar    Script Date: 7/20/2016 10:19:04 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeCalendar_CheckExistedCalendar    Script Date: 7/19/2016 5:22:13 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[EmployeeCalendar_CheckExistedCalendar]
	@empid int, 
	@fromdate datetime, 
	@todate datetime
AS	
/*
Created: Tom.Vo@2013-10-29
Task: approve request form (main SP)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON;

declare	@maxDate datetime, @minDate datetime, @result bit
set @result=0

select @maxDate=max(workdate),@minDate = min(workdate)
from dbo.EmployeeCalendar  WITH (NOLOCK)
where empid=@empid
	and workdate between @fromdate and @todate	

if (@fromdate = @minDate and @todate = @maxDate)
	set @result = 1

select @result


