/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetWeekWorking    Script Date: 7/20/2016 10:19:05 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetWeekWorking    Script Date: 7/19/2016 5:22:13 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[EmployeeCalendar_GetWeekWorking] 
	@EmpId int,
	@date datetime
AS
/*
Created: Tom.Vo@2013-10-29
Task: get Calendar by EmpId and Weekly
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	select * from EmployeeCalendar WITH (NOLOCK)
	where EmpId = @EmpId and datediff(week,WorkDate,@date)=0
	order by WorkDate
END