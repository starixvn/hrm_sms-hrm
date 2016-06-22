---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[EmployeeCalendar_GetRangeDate]
	@EmpId int,
	@fromDate datetime,
	@toDate datetime
AS
/*
Created: Tom.Vo@2013-10-29
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


