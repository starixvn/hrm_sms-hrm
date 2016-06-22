
CREATE PROCEDURE [dbo].[EmployeeDiscipline_Get] @FromDate DATETIME
	,@LastModifiedDate DATETIME
AS
/*
Created: 20160428@Razor.Nguyen
Task: Get all employee attendance violation.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT [WorkDate]
		,[EmpId]
		,[ShiftName]
		,[CheckIn]
		,[CheckOut]
		,[TotalHours]
		,[LateIn]
		,[EarlyOut]
		,[DisciplineType]
		,[DisciplineScore]
		,[EmployeeComment]
		,[ApproverComment]
		,[IsMissingCheckInOut]
		,[IsIgnoredMissingCheckInOut]
		,[IsLateIn]
		,[IsIgnoredLateIn]
		,[IsEarlyOut]
		,[IsIgnoredEarlyOut]
		,[IsWorkingUndertime]
		,[IsIgnoredWorkingUndertime]
		,[IsChangeShift]
		,[IsIgnoredChangeShift]
		,[IsAbsent]
		,[IsIgnoredAbsent]
		,[IsUnpaidLeave]
		,[IsIgnoredUnpaidLeave]
		,[IsUnpaidLeaveAbsent]
		,[IsIgnoredUnpaidLeaveAbsent]
		,[ModifiedDate]
		,[IsDeleted]
	FROM dbo.[EmployeeDiscipline] WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate
		AND DATEDIFF(DD, @FromDate, WorkDate) >= 0
	ORDER BY ModifiedDate
END
