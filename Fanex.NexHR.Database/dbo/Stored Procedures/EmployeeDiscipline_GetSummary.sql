


CREATE PROCEDURE [dbo].[EmployeeDiscipline_GetSummary] @FromDate DATE
	,@ToDate DATE
	,@ApproverId INT
	,@GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@ReasonId INT = NULL
	,@IsLocal BIT = NULL
	,@IsShowAll BIT = 0
AS
/*
Created: 20160415@Tim.Trinh
Task: Get discipline summary of employees.
DB: SMSHRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblEmployeeDiscipline (
		WorkDate DATE
		,EmpId INT
		,ChangeShift INT
		,MissingCheckInOut INT
		,LateIn INT
		,EarlyOut INT
		,[Absent] INT
		,UnplannedLeave INT
		,WeekendLeave DECIMAL(5, 2)
		,UnpaidLeave INT
		,WorkingUnderTime INT
		,UnpaidLeaveAbsent INT
		,VerbalWarning INT
		,WrittenWarning INT
		,OtherViolation INT
		,OtherDisciplineScore DECIMAL(5, 2)
		)

	INSERT INTO #tblEmployeeDiscipline (
		WorkDate
		,EmpId
		,ChangeShift
		,MissingCheckInOut
		,LateIn
		,EarlyOut
		,[Absent]
		,UnplannedLeave
		,WeekendLeave
		,UnpaidLeave
		,WorkingUnderTime
		,UnpaidLeaveAbsent
		,VerbalWarning
		,WrittenWarning
		,OtherViolation
		,OtherDisciplineScore
		)
	SELECT d.WorkDate
		,d.EmpId
		,(CASE (
				SELECT drule.[Value]
				FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
				WHERE drule.DisciplineTypeId = 1 -- change shift
					AND drule.GroupId = emp.GroupId
					AND drule.IsLocal = empSum.IsLocal
				)
			WHEN 1 THEN d.IsChangeShift
			ELSE 0 END
			) AS ChangeShift
		,(CASE (
				SELECT drule.[Value]
				FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
				WHERE drule.DisciplineTypeId = 2 -- missing check in / out
					AND drule.GroupId = emp.GroupId
					AND drule.IsLocal = empSum.IsLocal
				)
			WHEN 1 THEN d.IsMissingCheckInOut
			ELSE 0 END
			) AS MissingCheckInOut
		,(CASE (
				SELECT drule.[Value]
				FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
				WHERE drule.DisciplineTypeId = 3 -- late in
					AND drule.GroupId = emp.GroupId
					AND drule.IsLocal = empSum.IsLocal
				)
			WHEN 1 THEN d.IsLateIn
			ELSE 0 END
			) AS LateIn
		,(CASE (
				SELECT drule.[Value]
				FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
				WHERE drule.DisciplineTypeId = 4 -- early out
					AND drule.GroupId = emp.GroupId
					AND drule.IsLocal = empSum.IsLocal
				)
			WHEN 1 THEN d.IsEarlyOut
			ELSE 0 END
			) AS EarlyOut
		,(CASE (
				SELECT drule.[Value]
				FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
				WHERE drule.DisciplineTypeId = 5 -- absent
					AND drule.GroupId = emp.GroupId
					AND drule.IsLocal = empSum.IsLocal
				)
			WHEN 1 THEN d.IsAbsent
			ELSE 0 END
			) AS [Absent]
		,(CASE (
				SELECT drule.[Value]
				FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
				WHERE drule.DisciplineTypeId = 9 -- unplanned leave
					AND drule.GroupId = emp.GroupId
					AND drule.IsLocal = empSum.IsLocal
				)
			WHEN 1 THEN ISNULL(d.IsUnplannedLeave, 0)
			ELSE 0 END
			) AS UnplannedLeave
		,ISNULL(d.WeekendLeave, 0) AS WeekendLeave
		,(CASE (
				SELECT drule.[Value]
				FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
				WHERE drule.DisciplineTypeId = 6 -- unpaid leave
					AND drule.GroupId = emp.GroupId
					AND drule.IsLocal = empSum.IsLocal
				)
			WHEN 1 THEN d.IsUnpaidLeave
			ELSE 0 END
			) AS UnpaidLeave
		,(CASE (
				SELECT drule.[Value]
				FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
				WHERE drule.DisciplineTypeId = 7 -- working under time
					AND drule.GroupId = emp.GroupId
					AND drule.IsLocal = empSum.IsLocal
				)
			WHEN 1 THEN d.IsWorkingUndertime
			ELSE 0 END
			) AS WorkingUnderTime
		,(CASE (
				SELECT drule.[Value]
				FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
				WHERE drule.DisciplineTypeId = 8 -- unpaid leave absent
					AND drule.GroupId = emp.GroupId
					AND drule.IsLocal = empSum.IsLocal
				)
			WHEN 1 THEN d.IsUnpaidLeaveAbsent
			ELSE 0 END
			) AS UnpaidLeaveAbsent
		,0 AS VerbalWarning
		,0 AS WrittenWarning
		,0 AS OtherViolation
		,0 AS OtherDisciplineScore
	FROM dbo.EmployeeDiscipline AS d WITH (NOLOCK)
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON d.DisciplineType = dtype.DisciplineTypeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON d.EmpId = emp.EmpId
		AND ISNULL(emp.isDeleted, 0) = 0
		AND emp.[Enabled] = 1
		INNER JOIN dbo.EmployeeSummary AS empSum WITH (NOLOCK) ON emp.EmpId = empSum.EmpId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE d.DisciplineType > 0
		AND ISNULL(d.IsDeleted, 0) = 0
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(@GroupId, gr.GroupId) = gr.GroupId
		AND ISNULL(@EmployeeId, emp.EmpId) = emp.EmpId
		AND ISNULL(@IsLocal, empSum.IsLocal) = empSum.IsLocal
	
	UNION ALL
	SELECT dis.WorkDate
		,dis.EmpId
		,0 AS ChangeShift
		,0 AS MissingCheckInOut
		,0 AS LateIn
		,0 AS EarlyOut
		,0 AS [Absent]
		,0 AS UnplannedLeave
		,0 AS WeekendLeave
		,0 AS UnpaidLeave
		,0 AS WorkingUnderTime
		,0 AS UnpaidLeaveAbsent
		,(CASE 
			WHEN dtype.DisciplineTypeId = - 1 THEN 1
			ELSE 0 END
			) AS VerbalWarning
		,(CASE 
			WHEN dtype.DisciplineTypeId = - 2 THEN 1
			ELSE 0 END
			) AS WrittenWarning
		,(CASE 
			WHEN dtype.DisciplineTypeId = - 3 THEN 1
			ELSE 0 END
			) AS OtherViolation
		,dis.DisciplineScore AS OtherDisciplineScore
	FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineType = dtype.DisciplineTypeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON dis.EmpId = emp.EmpId
		INNER JOIN dbo.EmployeeSummary AS empSum WITH (NOLOCK) ON emp.EmpId = empSum.EmpId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE dis.DisciplineType < 0 -- other violation
		AND ISNULL(dis.IsDeleted, 0) = 0
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(@GroupId, gr.GroupId) = gr.GroupId
		AND ISNULL(@EmployeeId, emp.EmpId) = emp.EmpId
		AND ISNULL(@IsLocal, empSum.IsLocal) = empSum.IsLocal

	CREATE TABLE #tblEmployeeDisciplineFinalScore (
		WorkDate DATE
		,EmpId INT
		,SystemSubtractScore FLOAT
		,ApproverSubtractScore FLOAT
		)

	INSERT INTO #tblEmployeeDisciplineFinalScore (
		WorkDate
		,EmpId
		,SystemSubtractScore
		,ApproverSubtractScore
		)
	SELECT empD.WorkDate
		,empD.EmpId
		,(ISNULL(empD.ChangeShift, 0) * dbo.fncGetDisciplineScoreById(1)
		 + ISNULL(empD.MissingCheckInOut, 0) * dbo.fncGetDisciplineScoreById(2)
		 + ISNULL(empD.LateIn, 0) * dbo.fncGetDisciplineScoreById(3)
		 + ISNULL(empD.EarlyOut, 0) * dbo.fncGetDisciplineScoreById(4)
		 + ISNULL(empD.[Absent], 0) * dbo.fncGetDisciplineScoreById(5)
		 + ISNULL(empD.UnpaidLeave, 0) * dbo.fncGetDisciplineScoreById(6)
		 + ISNULL(empD.WorkingUnderTime, 0) * dbo.fncGetDisciplineScoreById(7)
		 + ISNULL(empD.UnpaidLeaveAbsent, 0) * dbo.fncGetDisciplineScoreById(8)
		 + ISNULL(empD.UnplannedLeave, 0) * dbo.fncGetDisciplineScoreById(9)
		 + ISNULL(empD.WeekendLeave, 0)) AS SystemSubtractScore
		,CASE 
			WHEN ignored.ApproverSubtractScore IS NOT NULL THEN ignored.ApproverSubtractScore
			ELSE 0
			END
	FROM #tblEmployeeDiscipline AS empD WITH (NOLOCK)
		LEFT JOIN (
		SELECT dis.EmpId
			,dis.WorkDate
			,(
				(CASE 
					WHEN dis.IsChangeShift = 1
						AND ISNULL(dis.IsIgnoredChangeShift, 0) = 0 THEN dbo.fncGetDisciplineScoreById(1)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsMissingCheckInOut = 1
						AND ISNULL(dis.IsIgnoredMissingCheckInOut, 0) = 0 THEN dbo.fncGetDisciplineScoreById(2)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsLateIn = 1
						AND ISNULL(dis.IsIgnoredLateIn, 0) = 0 THEN dbo.fncGetDisciplineScoreById(3)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsEarlyOut = 1
						AND ISNULL(dis.IsIgnoredEarlyOut, 0) = 0 THEN dbo.fncGetDisciplineScoreById(4)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsAbsent = 1
						AND ISNULL(dis.IsIgnoredAbsent, 0) = 0 THEN dbo.fncGetDisciplineScoreById(5)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsUnplannedLeave = 1
						AND ISNULL(dis.IsIgnoredUnplannedLeave, 0) = 0 THEN dbo.fncGetDisciplineScoreById(9)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsUnpaidLeave = 1
						AND ISNULL(dis.IsIgnoredUnpaidLeave, 0) = 0 THEN dbo.fncGetDisciplineScoreById(6)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsWorkingUndertime = 1
						AND ISNULL(dis.IsIgnoredWorkingUndertime, 0) = 0 THEN dbo.fncGetDisciplineScoreById(7)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsUnpaidLeaveAbsent = 1
						AND ISNULL(dis.IsIgnoredUnpaidLeaveAbsent, 0) = 0 THEN dbo.fncGetDisciplineScoreById(8)
					ELSE 0 END
					)
				) AS ApproverSubtractScore
		FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
			INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineType = dtype.DisciplineTypeId
		WHERE dis.DisciplineType = 1
			AND dis.WorkDate BETWEEN @FromDate
				AND @ToDate
		) AS ignored ON empD.EmpId = ignored.EmpId
		AND empD.WorkDate = ignored.WorkDate
	WHERE (
			[Absent] > 0
			OR UnplannedLeave > 0
			OR WeekendLeave > 0
			OR ChangeShift > 0
			OR EarlyOut > 0
			OR LateIn > 0
			OR UnpaidLeave > 0
			OR WorkingUnderTime > 0
			OR UnpaidLeaveAbsent > 0
			OR MissingCheckInOut > 0
			)
		AND empD.OtherDisciplineScore = 0

	DECLARE @Is_HR_SA BIT

	CREATE TABLE #tblRoles (RoleId INT)

	INSERT INTO #tblRoles (RoleId)
	SELECT RoleId
	FROM dbo.[EmployeeRole] WITH (NOLOCK)
	WHERE EmployeeId = @ApproverId;

	SET @Is_HR_SA = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM #tblRoles WITH (NOLOCK)
					WHERE RoleId IN (6, 7) -- HR, SA
					) THEN 1
			ELSE 0
			END

	SELECT SUM(ISNULL(empD.ChangeShift, 0)) AS ChangeShift
		,SUM(ISNULL(empD.MissingCheckInOut, 0)) AS MissingCheckInOut
		,SUM(ISNULL(empD.LateIn, 0)) AS LateIn
		,SUM(ISNULL(empD.EarlyOut, 0)) AS EarlyOut
		,SUM(ISNULL(empD.[Absent], 0)) AS [Absent]
		,SUM(ISNULL(empD.UnplannedLeave, 0)) AS UnplannedLeave
		,SUM(ISNULL(empD.WeekendLeave, 0)) AS WeekendLeave
		,SUM(ISNULL(empD.UnpaidLeave, 0)) AS UnpaidLeave
		,SUM(ISNULL(empD.WorkingUnderTime, 0)) AS WorkingUnderTime
		,SUM(ISNULL(empD.UnpaidLeaveAbsent, 0)) AS UnpaidLeaveAbsent
		,SUM(ISNULL(empD.OtherDisciplineScore, 0)) AS OtherDisciplineScore
		,SUM(CASE 
				WHEN ISNULL(empD.OtherDisciplineScore, 0) = 0 THEN ISNULL(empF.SystemSubtractScore, 0)
				ELSE ISNULL(empD.OtherDisciplineScore, 0)
				END) AS SystemSubtractScore
		,SUM(CASE 
				WHEN ISNULL(empD.OtherDisciplineScore, 0) = 0 THEN ISNULL(empF.ApproverSubtractScore, 0)
				ELSE ISNULL(empD.OtherDisciplineScore, 0)
				END) + SUM(ISNULL(empD.WeekendLeave, 0)) AS ApproverSubtractScore
		,Permission = (CASE 
			WHEN EXISTS (
					SELECT 1
					FROM #tblRoles
					WHERE RoleId = 8 -- edit discipline
					)
				AND (
					EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId = @ApproverId
							AND empS.EmpId = emp.EmpId
							AND empS.ActionType = 8 -- edit discipline
							AND empS.EmpId <> @ApproverId
							AND ISNULL(empS.IsDeleted, 0) = 0
						)
					OR EXISTS (
						SELECT 1
						FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
						WHERE gr.GroupId = grS.GroupId
							AND @ApproverId = grS.OwnerId
							AND grS.ActionType = 8 -- edit discipline
							-- AND emp.EmpId <> @ApproverId
							AND ISNULL(grS.IsDeleted, 0) = 0
							AND NOT EXISTS (
								SELECT 1
								FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
								WHERE empS.OwnerId <> @ApproverId
									AND empS.EmpId = emp.EmpId
									AND empS.ActionType = 8
									AND ISNULL(empS.IsDeleted, 0) = 0
								)
						)
					) THEN 2 -- Host				
			ELSE 1 END
			)
		,emp.EmpId AS EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,empSum.IsLocal
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
	FROM dbo.Employee AS emp WITH (NOLOCK)
		LEFT JOIN #tblEmployeeDiscipline AS empD WITH (NOLOCK) ON emp.EmpId = empD.EmpId
		LEFT JOIN #tblEmployeeDisciplineFinalScore AS empF WITH (NOLOCK) ON empF.WorkDate = empD.WorkDate
		AND empF.EmpId = empD.EmpId
		INNER JOIN dbo.EmployeeSummary AS empSum WITH (NOLOCK) ON emp.EmpId = empSum.EmpId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE emp.[Enabled] = 1
		AND ISNULL(emp.IsDeleted, 0) = 0
		AND ISNULL(@GroupId, gr.GroupId) = gr.GroupId
		AND ISNULL(@EmployeeId, emp.EmpId) = emp.EmpId
		AND ISNULL(@IsLocal, empSum.IsLocal) = empSum.IsLocal
		AND (
			@IsShowAll = 1
			OR (
				@IsShowAll = 0
				AND EXISTS (
					SELECT 1
					FROM #tblEmployeeDiscipline WITH (NOLOCK)
					WHERE emp.EmpId = EmpId
					)
				)
			)
		AND (
			@Is_HR_SA = 1 -- SA, HR can view all member of company
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
				WHERE empS.OwnerId = @ApproverId
					AND empS.EmpId = emp.EmpId
					AND empS.ActionType = 8 -- edit discipline
					AND empS.EmpId <> @ApproverId
					AND ISNULL(empS.IsDeleted, 0) = 0
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE gr.GroupId = grS.GroupId
					AND @ApproverId = grS.OwnerId
					AND grS.ActionType = 8 -- edit discipline
					AND emp.EmpId <> @ApproverId
					AND ISNULL(grS.IsDeleted, 0) = 0
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @ApproverId
							AND empS.EmpId = emp.EmpId
							AND empS.ActionType = 8
							AND ISNULL(emps.IsDeleted, 0) = 0
						)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
				WHERE empS.OwnerId = @ApproverId
					AND empS.EmpId = emp.EmpId
					AND empS.ActionType IN (2, 3) -- Approver and Verifier
					AND ISNULL(empS.IsDeleted, 0) = 0
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE gr.GroupId = grS.GroupId
					AND @ApproverId = grS.OwnerId
					AND grS.ActionType IN (2, 3) -- Approver and Verifier
					AND ISNULL(grS.IsDeleted, 0) = 0
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @ApproverId
							AND empS.EmpId = emp.EmpId
							AND empS.ActionType IN (2, 3)
							AND ISNULL(emps.IsDeleted, 0) = 0
						)
				)
			OR (
				EXISTS (
					SELECT 1
					FROM #tblRoles WITH (NOLOCK)
					WHERE RoleId = 1 -- Employee
					)
				AND emp.EmpId = @ApproverId
				)
			)
		AND (
			ISNULL(@ReasonId, '') = ''
			OR (
				@ReasonId = 1
				AND empD.ChangeShift > 0
				)
			OR (
				@ReasonId = 2
				AND empD.MissingCheckInOut > 0
				)
			OR (
				@ReasonId = 3
				AND empD.LateIn > 0
				)
			OR (
				@ReasonId = 4
				AND empD.EarlyOut > 0
				)
			OR (
				@ReasonId = 5
				AND empD.[Absent] > 0
				)
			OR (
				@ReasonId = 9
				AND empD.UnplannedLeave > 0
				)
			OR (
				@ReasonId = 10
				AND empD.WeekendLeave > 0
				)
			OR (
				@ReasonId = 6
				AND empD.UnpaidLeave > 0
				)
			OR (
				@ReasonId = 7
				AND empD.WorkingUnderTime > 0
				)
			OR (
				@ReasonId = 8
				AND empD.UnpaidLeaveAbsent > 0
				)
			OR (
				@ReasonId = - 1
				AND empD.VerbalWarning > 0
				)
			OR (
				@ReasonId = - 2
				AND empD.WrittenWarning > 0
				)
			OR (
				@ReasonId = - 3
				AND empD.OtherViolation > 0
				)
			)
	GROUP BY empD.EmpId
		,emp.EmpId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,empSum.IsLocal
		,gr.GroupId
		,gr.[Name]
		,gr.Code
	ORDER BY Permission DESC
		,gr.[Name] ASC
		,emp.Code ASC

	DROP TABLE #tblEmployeeDiscipline

	DROP TABLE #tblEmployeeDisciplineFinalScore

	DROP TABLE #tblRoles
END