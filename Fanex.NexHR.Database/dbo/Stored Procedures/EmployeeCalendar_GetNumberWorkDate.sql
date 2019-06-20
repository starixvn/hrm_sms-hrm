/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetNumberWorkDate    Script Date: 7/20/2016 10:19:04 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetNumberWorkDate    Script Date: 7/19/2016 5:22:13 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[EmployeeCalendar_GetNumberWorkDate]
	@EmpId int,
	@fromDate datetime,
	@toDate datetime
AS
/*
Created: Quinn.Vo@2013-11-05
Task: approve request form (main SP)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(1) AS numberDay 
	FROM dbo.EmployeeCalendar WITH(NOLOCK)
	WHERE (WorkDate BETWEEN @fromDate AND @toDate)
		AND EmpID= @EmpId 
		AND IsHoliday = 0 AND IsOffDay = 0 AND IsLeaveDay = 0
END


