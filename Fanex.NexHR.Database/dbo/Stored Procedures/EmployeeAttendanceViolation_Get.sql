
CREATE PROCEDURE [dbo].[EmployeeAttendanceViolation_Get]
	@FromDate DATETIME
	,@LastModifiedDate DATETIME
AS
/*
Created: 20150914@Quinn.Vo
Task: Get all employee attendance violation.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT [WorkDate]
      ,[EmpId] AS UserId
      ,[MissingCheckInOut]
      ,[LateIn]
      ,[EarlyOut]
      ,[ChangeShift]
      ,[WorkingBelowTime]
      ,[Absent]
      ,[UnpaidLeaveAbsent]
      ,[UnpaidLeave]
      ,[ModifiedDate]
      ,[ShiftName]
      ,[InFixed]
      ,[OutFixed]
      ,[LateInValue]
      ,[EarlyOutValue]
      ,[TotalWorkHours]
      ,[LateInApproverComment]
      ,[LateInEmployeeComment]
      ,[EarlyOutApproverComment]
      ,[EarlyOutEmployeeComment]
      ,[ChangeShiftApproverComment]
      ,[ChangeShiftEmployeeComment]
      ,[LeaveApproverComment]
      ,[LeaveEmployeeComment]
	  ,[InOutMissApproverComment]
	  ,[InOutMissEmployeeComment]
	  ,[IsDeleted]
	FROM dbo.[EmployeeAttendanceViolation] WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate
		AND DATEDIFF(DD, @FromDate, WorkDate) >= 0
	ORDER BY ModifiedDate
END
