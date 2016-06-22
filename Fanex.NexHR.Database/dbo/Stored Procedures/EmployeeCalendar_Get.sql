
CREATE PROCEDURE [dbo].[EmployeeCalendar_Get] -- '2014-05-01'
	@FromDate DATETIME = NULL
AS
/*
Created: 20141002@Quinn.Vo
Task: Get employee calendar from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SELECT TOP 5000
	  [WorkDate]
      ,[EmpId] AS UserId
      ,[ShiftId]
      ,[IsHoliday]
      ,[IsOffDay]
      ,[IsLeaveDay]
      ,[LeaveTypeId]
      ,[LeaveDay]
	  ,cl.ModifiedDate
	FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
	WHERE @FromDate IS NULL
	OR cl.ModifiedDate > @FromDate
	ORDER BY cl.ModifiedDate
END
