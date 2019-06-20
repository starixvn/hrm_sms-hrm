
CREATE FUNCTION [dbo].[fnc_GetDisciplineSummary] (
	@LastModifiedDate DATETIME
	,@FromDate DATETIME
	,@ToDate DATETIME
	)
RETURNS @EmployeeDisciplineSummary TABLE (EmployeeId INT, 
	WorkDate DATE, 
	MissingCheckInOut DECIMAL(5, 2), 
	LateIn DECIMAL(5, 2),
	EarlyOut DECIMAL(5, 2),
	WorkingUndertime DECIMAL(5, 2),
	ChangeShift DECIMAL(5, 2),
	[Absent] DECIMAL(5, 2),
	UnplannedLeave DECIMAL(5, 2),
	WeekendLeave DECIMAL(5, 2),
	UnpaidLeave DECIMAL(5, 2),
	UnpaidLeaveAbsent DECIMAL(5, 2),
	OtherDisciplines DECIMAL(5, 2),
	SystemSubtractScore DECIMAL(5, 2),
	ModifiedDate DATETIME)
AS
/*
Created: Quinn.Vo@20161130
Task: Get employee summary by date
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	DECLARE @EmpIds TABLE(EmpId INT)

	INSERT INTO @EmpIds (EmpId)
	SELECT DISTINCT EmpId
	FROM dbo.[EmployeeDiscipline] WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate
		AND DATEDIFF(DD, @FromDate, WorkDate) >= 0
		AND DATEDIFF(DD, WorkDate, @ToDate) >= 0

	DECLARE @MaxModifiedDate DATETIME

	SELECT @MaxModifiedDate = MAX(ModifiedDate)
	FROM dbo.[EmployeeDiscipline] WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate
		AND DATEDIFF(DD, @FromDate, WorkDate) >= 0
		AND DATEDIFF(DD, WorkDate, @ToDate) >= 0
	
	INSERT INTO @EmployeeDisciplineSummary(
		EmployeeId,
		WorkDate,
		MissingCheckInOut,
		LateIn,
		EarlyOut,
		WorkingUndertime,
		ChangeShift,
		[Absent],
		UnplannedLeave,
		WeekendLeave,
		UnpaidLeave,
		UnpaidLeaveAbsent,
		OtherDisciplines,
		SystemSubtractScore,
		ModifiedDate
	)
	SELECT e.EmpId AS EmployeeId
		,@FromDate AS WorkDate
		,ISNULL(MissingCheckInOut, 0) AS MissingCheckInOut
		,ISNULL(LateIn, 0) AS LateIn
		,ISNULL(EarlyOut, 0) AS EarlyOut
		,ISNULL(WorkingUndertime, 0) AS WorkingUndertime
		,ISNULL(ChangeShift, 0) AS ChangeShift
		,ISNULL([Absent], 0) AS [Absent]
		,ISNULL(UnplannedLeave, 0) AS UnplannedLeave
		,ISNULL(WeekendLeave, 0) AS WeekendLeave
		,ISNULL(UnpaidLeave, 0) AS UnpaidLeave
		,ISNULL(UnpaidLeaveAbsent, 0) AS UnpaidLeaveAbsent
		,ISNULL(OtherDisciplines, 0) AS OtherDisciplines
		,ISNULL(SystemSubtractScore, 0) AS SystemSubtractScore
		,@MaxModifiedDate AS ModifiedDate
	FROM @EmpIds AS e
		LEFT JOIN (
		SELECT EmpId
			,SUM(CASE 
					WHEN (
							IsMissingCheckInOut = 1
							AND IsIgnoredMissingCheckInOut = 0
							) THEN 2.5
					ELSE 0
					END) AS MissingCheckInOut
			,SUM(CASE 
					WHEN (
							IsLateIn = 1
							AND IsIgnoredLateIn = 0
							) THEN 2.5
					ELSE 0
					END) AS LateIn
			,SUM(CASE 
					WHEN (
							IsEarlyOut = 1
							AND IsIgnoredEarlyOut = 0
							) THEN 2.5
					ELSE 0
					END) AS EarlyOut
			,SUM(CASE 
					WHEN (
							IsWorkingUnderTime = 1
							AND IsIgnoredWorkingUnderTime = 0
							) THEN 2.5
					ELSE 0
					END) AS WorkingUndertime
			,SUM(CASE 
					WHEN (
							IsChangeShift = 1
							AND IsIgnoredChangeShift = 0
							) THEN 2.5
					ELSE 0
					END) AS ChangeShift
			,SUM(CASE 
					WHEN (
							IsAbsent = 1
							AND IsIgnoredAbsent = 0
							) THEN 3.5
					ELSE 0
					END) AS [Absent]
			,SUM(CASE 
					WHEN (
							IsUnplannedLeave = 1
							AND IsIgnoredUnplannedLeave = 0
							) THEN 3.5
					ELSE 0
					END) AS UnplannedLeave
			,SUM(CASE 
					WHEN (
							IsWeekendLeave = 1
							AND IsIgnoreWeekendLeave = 0
							) THEN 3.5
					ELSE 0
					END) AS WeekendLeave
			,SUM(CASE 
					WHEN (
							IsUnpaidLeave = 1
							AND IsIgnoredUnpaidLeave = 0
							) THEN 2.5
					ELSE 0
					END) AS UnpaidLeave
			,SUM(CASE 
					WHEN (
							IsUnpaidLeaveAbsent = 1
							AND IsIgnoredUnpaidLeaveAbsent = 0
							) THEN 3.5
					ELSE 0
					END) AS UnpaidLeaveAbsent
			,(
				SUM(CASE 
						WHEN (DisciplineType = - 2) THEN 15
						ELSE 0
						END) + SUM(CASE 
						WHEN (DisciplineType = - 1) THEN 7
						ELSE 0
						END)
				) AS OtherDisciplines
			,(
				SUM(CASE 
						WHEN IsMissingCheckInOut = 1 THEN 2.5
						ELSE 0
						END) + SUM(CASE 
						WHEN IsLateIn = 1 THEN 2.5
						ELSE 0
						END) + SUM(CASE 
						WHEN IsEarlyOut = 1 THEN 2.5
						ELSE 0
						END) + SUM(CASE 
						WHEN IsWorkingUnderTime = 1 THEN 2.5
						ELSE 0
						END) + SUM(CASE 
						WHEN IsChangeShift = 1 THEN 2.5
						ELSE 0
						END) + SUM(CASE 
						WHEN IsAbsent = 1 THEN 3.5
						ELSE 0
						END) + SUM(CASE 
						WHEN IsUnplannedLeave = 1 THEN 3.5
						ELSE 0
							END) + SUM(CASE 
						WHEN IsWeekendLeave = 1 THEN 3.5
						ELSE 0
						END) + SUM(CASE 
						WHEN IsUnpaidLeave = 1 THEN 2.5
						ELSE 0
						END) + SUM(CASE 
						WHEN IsUnpaidLeaveAbsent = 1 THEN 3.5
						ELSE 0
						END)
				) AS SystemSubtractScore
		FROM dbo.[EmployeeDiscipline] WITH (NOLOCK)
		WHERE DATEDIFF(DD, @FromDate, WorkDate) >= 0
			AND DATEDIFF(DD, WorkDate, @ToDate) >= 0
			AND ISNULL(IsDeleted, 0) = 0
		GROUP BY EmpId
		) AS dis ON e.EmpId = dis.EmpId

	RETURN
END