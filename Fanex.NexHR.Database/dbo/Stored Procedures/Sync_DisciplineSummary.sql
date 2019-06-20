
CREATE PROCEDURE [dbo].[Sync_DisciplineSummary] @LastModifiedDate DATETIME
AS
/*
Created: 20171031@Tim.Trinh
Task: sync discipline to EPA
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblDisciplineSummary (
		EmployeeId INT
		,WorkDate DATE
		,ChangeShift DECIMAL(5, 2)
		,MissingCheckInOut DECIMAL(5, 2)
		,LateIn DECIMAL(5, 2)
		,EarlyOut DECIMAL(5, 2)
		,[Absent] DECIMAL(5, 2)
		,UnpaidLeave DECIMAL(5, 2)
		,WorkingUndertime DECIMAL(5, 2)
		,UnpaidLeaveAbsent DECIMAL(5, 2)
		,UnplannedLeave DECIMAL(5, 2)
		,WeekendLeave DECIMAL(5, 2)
		,OtherDisciplines DECIMAL(5, 2)
		,SystemSubtractScore DECIMAL(5, 2)
		,ModifiedDate DATETIME
		,IsDeleted BIT
		)

	CREATE TABLE #tblDisciplineSummaryResult (
		EmployeeId INT
		,WorkDate DATE
		,ChangeShift DECIMAL(5, 2)
		,MissingCheckInOut DECIMAL(5, 2)
		,LateIn DECIMAL(5, 2)
		,EarlyOut DECIMAL(5, 2)
		,[Absent] DECIMAL(5, 2)
		,UnpaidLeave DECIMAL(5, 2)
		,WorkingUndertime DECIMAL(5, 2)
		,UnpaidLeaveAbsent DECIMAL(5, 2)
		,UnplannedLeave DECIMAL(5, 2)
		,WeekendLeave DECIMAL(5, 2)
		,OtherDisciplines DECIMAL(5, 2)
		,SystemSubtractScore DECIMAL(5, 2)
		,ModifiedDate DATETIME
		,IsDeleted BIT
		)

	INSERT INTO #tblDisciplineSummary (
		EmployeeId
		,WorkDate
		,ChangeShift
		,MissingCheckInOut
		,LateIn
		,EarlyOut
		,[Absent]
		,UnpaidLeave
		,WorkingUndertime
		,UnpaidLeaveAbsent
		,UnplannedLeave
		,WeekendLeave
		,OtherDisciplines
		,SystemSubtractScore
		,ModifiedDate
		,IsDeleted
		)
	SELECT EmpId
		,WorkDate
		,0 AS ChangeShift
		,0 AS MissingCheckInOut
		,0 AS LateIn
		,0 AS EarlyOut
		,0 AS [Absent]
		,0 AS UnpaidLeave
		,0 AS WorkingUndertime
		,0 AS UnpaidLeaveAbsent
		,0 AS UnplannedLeave
		,0 AS WeekendLeave
		,0 AS OtherDisciplines
		,0 AS SystemSubtractScore
		,MAX(ModifiedDate) AS ModifiedDate
		,1
	FROM dbo.EmployeeDiscipline WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate
		AND IsDeleted = 1
	GROUP BY EmpId
		,WorkDate
	
	UNION
	SELECT d.EmpId
		,d.WorkDate
		,SUM(CASE 
				WHEN IsChangeShift = 1
					AND ISNULL(IsIgnoredChangeShift, 0) = 0
					AND cs.DisciplineTypeId IS NOT NULL THEN 2.5
				ELSE 0
				END) AS ChangeShift
		,SUM(CASE 
				WHEN IsMissingCheckInOut = 1
					AND ISNULL(IsIgnoredMissingCheckInOut, 0) = 0
					AND mio.DisciplineTypeId IS NOT NULL THEN 2.5
				ELSE 0
				END) AS MissingCheckInOut
		,SUM(CASE 
				WHEN IsLateIn = 1
					AND ISNULL(IsIgnoredLateIn, 0) = 0
					AND li.DisciplineTypeId IS NOT NULL THEN 2.5
				ELSE 0
				END) AS LateIn
		,SUM(CASE 
				WHEN IsEarlyOut = 1
					AND ISNULL(IsIgnoredEarlyOut, 0) = 0
					AND eo.DisciplineTypeId IS NOT NULL THEN 2.5
				ELSE 0
				END) AS EarlyOut
		,SUM(CASE 
				WHEN IsAbsent = 1
					AND ISNULL(IsIgnoredAbsent, 0) = 0
					AND ab.DisciplineTypeId IS NOT NULL THEN 3.5
				ELSE 0
				END) AS [Absent]
		,SUM(CASE 
				WHEN IsUnpaidLeave = 1
					AND ISNULL(IsIgnoredUnpaidLeave, 0) = 0
					AND upl.DisciplineTypeId IS NOT NULL THEN 2.5
				ELSE 0
				END) AS UnpaidLeave
		,SUM(CASE 
				WHEN IsWorkingUnderTime = 1
					AND ISNULL(IsIgnoredWorkingUnderTime, 0) = 0
					AND wut.DisciplineTypeId IS NOT NULL THEN 2.5
				ELSE 0
				END) AS WorkingUndertime
		,SUM(CASE 
				WHEN IsUnpaidLeaveAbsent = 1
					AND ISNULL(IsIgnoredUnpaidLeaveAbsent, 0) = 0
					AND upla.DisciplineTypeId IS NOT NULL THEN 3.5
				ELSE 0
				END) AS UnpaidLeaveAbsent
		,SUM(CASE 
				WHEN IsUnplannedLeave = 1
					AND ISNULL(IsIgnoredUnplannedLeave, 0) = 0
					AND unplanned.DisciplineTypeId IS NOT NULL THEN 3.5
				ELSE 0
				END) AS UnplannedLeave
		,SUM(ISNULL(WeekendLeave, 0)) AS WeekendLeave
		,SUM(CASE 
				WHEN DisciplineType < 0 THEN DisciplineScore
				ELSE 0
				END) AS OtherDisciplines
		,SUM(CASE 
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
				END) + SUM(ISNULL(WeekendLeave, 0)) + SUM(CASE 
				WHEN IsUnpaidLeave = 1 THEN 2.5
				ELSE 0
				END) + SUM(CASE 
				WHEN IsUnpaidLeaveAbsent = 1 THEN 3.5
				ELSE 0
				END) AS SystemSubtractScore
		,MAX(d.ModifiedDate) AS ModifiedDate
		,0 AS IsDeleted
	FROM dbo.EmployeeDiscipline AS d WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON d.EmpId = emp.EmpId
		AND emp.[Enabled] = 1
		AND ISNULL(emp.isDeleted, 0) = 0
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON emp.EmpId = es.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	-- change shift
		LEFT JOIN dbo.DisciplineSectionRule AS cs WITH (NOLOCK) ON cs.DisciplineTypeId = 1
		AND cs.GroupId = emp.GroupId
		AND cs.IsLocal = es.IsLocal
		AND cs.Value = 1
	-- missing check in/out
		LEFT JOIN dbo.DisciplineSectionRule AS mio WITH (NOLOCK) ON mio.DisciplineTypeId = 2
		AND mio.GroupId = emp.GroupId
		AND mio.IsLocal = es.IsLocal
		AND mio.Value = 1
	-- late in
		LEFT JOIN dbo.DisciplineSectionRule AS li WITH (NOLOCK) ON li.DisciplineTypeId = 3
		AND li.GroupId = emp.GroupId
		AND li.IsLocal = es.IsLocal
		AND li.Value = 1
	-- early out
		LEFT JOIN dbo.DisciplineSectionRule AS eo WITH (NOLOCK) ON eo.DisciplineTypeId = 4
		AND eo.GroupId = emp.GroupId
		AND eo.IsLocal = es.IsLocal
		AND eo.Value = 1
	-- absent
		LEFT JOIN dbo.DisciplineSectionRule AS ab WITH (NOLOCK) ON ab.DisciplineTypeId = 5
		AND ab.GroupId = emp.GroupId
		AND ab.IsLocal = es.IsLocal
		AND ab.Value = 1
	-- unpaid leave
		LEFT JOIN dbo.DisciplineSectionRule AS upl WITH (NOLOCK) ON upl.DisciplineTypeId = 6
		AND upl.GroupId = emp.GroupId
		AND upl.IsLocal = es.IsLocal
		AND upl.Value = 1
	-- working under time
		LEFT JOIN dbo.DisciplineSectionRule AS wut WITH (NOLOCK) ON wut.DisciplineTypeId = 7
		AND wut.GroupId = emp.GroupId
		AND wut.IsLocal = es.IsLocal
		AND wut.Value = 1
	-- unpaid leave absent
		LEFT JOIN dbo.DisciplineSectionRule AS upla WITH (NOLOCK) ON upla.DisciplineTypeId = 8
		AND upla.GroupId = emp.GroupId
		AND upla.IsLocal = es.IsLocal
		AND upla.Value = 1
	-- unplanned leave
		LEFT JOIN dbo.DisciplineSectionRule AS unplanned WITH (NOLOCK) ON unplanned.DisciplineTypeId = 9
		AND unplanned.GroupId = emp.GroupId
		AND unplanned.IsLocal = es.IsLocal
		AND unplanned.Value = 1
	WHERE d.ModifiedDate > @LastModifiedDate
		AND d.IsDeleted = 0
	GROUP BY d.EmpId
		,d.WorkDate

	INSERT INTO #tblDisciplineSummaryResult (
		EmployeeId
		,WorkDate
		,ChangeShift
		,MissingCheckInOut
		,LateIn
		,EarlyOut
		,[Absent]
		,UnpaidLeave
		,WorkingUndertime
		,UnpaidLeaveAbsent
		,UnplannedLeave
		,WeekendLeave
		,OtherDisciplines
		,SystemSubtractScore
		,ModifiedDate
		,IsDeleted
		)
	SELECT EmployeeId
		,WorkDate
		,SUM(ChangeShift) AS ChangeShift
		,SUM(MissingCheckInOut) AS MissingCheckInOut
		,SUM(LateIn) AS LateIn
		,SUM(EarlyOut) AS EarlyOut
		,SUM([Absent]) AS [Absent]
		,SUM(UnpaidLeave) AS UnpaidLeave
		,SUM(WorkingUndertime) AS WorkingUndertime
		,SUM(UnpaidLeaveAbsent) AS UnpaidLeaveAbsent
		,SUM(UnplannedLeave) AS UnplannedLeave
		,SUM(WeekendLeave) AS WeekendLeave
		,SUM(OtherDisciplines) AS OtherDisciplines
		,SUM(SystemSubtractScore) AS SystemSubtractScore
		,MAX(ModifiedDate) AS ModifiedDate
		,CAST(MIN(CAST(IsDeleted AS TINYINT)) AS BIT) AS IsDeleted
	FROM #tblDisciplineSummary WITH (NOLOCK)
	GROUP BY EmployeeId
		,WorkDate

	IF (
			SELECT COUNT(1)
			FROM #tblDisciplineSummaryResult WITH (NOLOCK)
			) > 1000
	BEGIN
		UPDATE dis
		SET dis.ModifiedDate = DATEADD(SECOND, - 1, dis.ModifiedDate)
		FROM (
			SELECT TOP 1000 *
			FROM #tblDisciplineSummaryResult WITH (NOLOCK)
			ORDER BY ModifiedDate
				,WorkDate
				,EmployeeId
			) AS d
			INNER JOIN dbo.EmployeeDiscipline AS dis WITH (ROWLOCK, UPDLOCK) ON d.EmployeeId = dis.EmpId
			AND d.WorkDate = dis.WorkDate

		SELECT TOP 1000 *
		FROM #tblDisciplineSummaryResult WITH (NOLOCK)
		ORDER BY ModifiedDate
			,WorkDate
			,EmployeeId
	END
	ELSE
	BEGIN
		SELECT *
		FROM #tblDisciplineSummaryResult WITH (NOLOCK)
		ORDER BY ModifiedDate
			,WorkDate
			,EmployeeId
	END

	DROP TABLE #tblDisciplineSummary

	DROP TABLE #tblDisciplineSummaryResult
END