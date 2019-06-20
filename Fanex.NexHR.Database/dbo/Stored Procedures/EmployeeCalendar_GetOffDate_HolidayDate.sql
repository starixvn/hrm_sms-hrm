/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetOffDate_HolidayDate    Script Date: 7/20/2016 10:19:04 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetOffDate_HolidayDate    Script Date: 7/19/2016 5:22:13 PM ******/

CREATE PROCEDURE [dbo].[EmployeeCalendar_GetOffDate_HolidayDate] 
	@EmpId int,
	@fromDate datetime,
	@toDate datetime
AS
/*
Created: Tom.Vo@2013-10-29
Task: get list days which is Off Day and Holiday for request
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SELECT WorkDate 
	FROM dbo.EmployeeCalendar WITH(NOLOCK)
	WHERE (WorkDate BETWEEN @fromDate AND @toDate)
		AND EmpID= @EmpId 
		AND (
			(IsHoliday = 1 OR IsOffDay = 1) 
			--AND ShiftId IS NOT NULL
			)
		
END

