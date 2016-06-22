
CREATE PROCEDURE [dbo].[TimeSheet_GetLateInEarlyOut]
	@EmployeeId INT
	,@workDate DATETIME
AS
/*
Created: 20151027@Quinn.Vo
Task: Get late in/ early out of employee by a date.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
		att.AttendanceDate AS WorkDate
		,att.LateIn
		,att.EarlyOut
		,e.EmpId
		,e.NickName
		,e.FullName
	FROM dbo.EmployeeAttendance AS att WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON att.EmpId = e.EmpId
	WHERE att.EmpId = @EmployeeId 
		AND DATEDIFF(DD, AttendanceDate, @workDate) = 0
END
