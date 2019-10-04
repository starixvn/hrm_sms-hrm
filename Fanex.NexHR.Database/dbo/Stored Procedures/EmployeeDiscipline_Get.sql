

--[dbo].[EmployeeDiscipline_Get] '2017-01-01', '2017-06-01'
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
		,dis.DisciplineScore
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
		,ISNULL([IsUnplannedLeave], 0) AS [IsUnplannedLeave]
		,ISNULL([IsIgnoredUnplannedLeave], 0) AS [IsIgnoredUnplannedLeave]
		,[IsUnpaidLeave]
		,[IsIgnoredUnpaidLeave]
		,ISNULL([IsWeekendLeave], 0) AS [IsWeekendLeave]
		,ISNULL([IsIgnoredWeekendLeave], 0) AS [IsWeekendLeave]
		,[IsUnpaidLeaveAbsent]
		,[IsIgnoredUnpaidLeaveAbsent]
		,[ModifiedDate]
		,[IsDeleted]
	FROM dbo.[EmployeeDiscipline] AS dis WITH (NOLOCK)
		INNER JOIN dbo.DisciplineType AS ty WITH (NOLOCK) ON dis.DisciplineType = ty.DisciplineTypeId
	WHERE (
			ModifiedDate > @LastModifiedDate
			AND DATEDIFF(DD, @FromDate, WorkDate) >= 0
			)
		OR DisciplineType < 0
	ORDER BY ModifiedDate
END
