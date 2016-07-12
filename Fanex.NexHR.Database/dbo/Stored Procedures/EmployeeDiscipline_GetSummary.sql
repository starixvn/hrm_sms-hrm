CREATE PROCEDURE [dbo].[EmployeeDiscipline_GetSummary] @FromDate DATE
	,@ToDate DATE
	,@ApproverId INT
	,@GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@ReasonId INT = NULL
	,@IsLocal BIT = NULL
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

	CREATE TABLE #EmployeeDiscipline (
		WorkDate DATE
		,EmpId INT
		,ChangeShift INT
		,MissingCheckInOut INT
		,LateIn INT
		,EarlyOut INT
		,[Absent] INT
		,UnpaidLeave INT
		,WorkingUnderTime INT
		,UnpaidLeaveAbsent INT
		,VerbalWarning INT
		,WrittenWarning INT
		,OtherDisciplineScore FLOAT
		)

	INSERT INTO #EmployeeDiscipline
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
		,0 AS OtherDisciplineScore
	FROM dbo.EmployeeDiscipline AS d WITH (NOLOCK)
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON d.DisciplineType = dtype.DisciplineTypeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON d.EmpId = emp.EmpId
		AND emp.isDeleted = 0
		AND emp.[Enabled] = 1
		INNER JOIN dbo.EmployeeSummary AS empSum WITH (NOLOCK) ON emp.EmpId = empSum.EmpId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE d.DisciplineType NOT IN (- 1, - 2) -- other violation
		AND ISNULL(d.IsDeleted, 0) = 0
		AND CAST(WorkDate AS DATE) BETWEEN CAST(@FromDate AS DATE)
			AND CAST(@ToDate AS DATE)
		AND (
			ISNULL(@GroupId, '') = ''
			OR gr.GroupId = @GroupId
			)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR emp.EmpId = @EmployeeId
			)
		AND (
			@IsLocal IS NULL
			OR empSum.IsLocal = @IsLocal
			)
	
	UNION ALL
	SELECT dis.WorkDate
		,dis.EmpId
		,0 AS ChangeShift
		,0 AS MissingCheckInOut
		,0 AS LateIn
		,0 AS EarlyOut
		,0 AS [Absent]
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
		,dtype.Score AS OtherDisciplineScore
	FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineType = dtype.DisciplineTypeId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON dis.EmpId = emp.EmpId
		INNER JOIN dbo.EmployeeSummary AS empSum WITH (NOLOCK) ON emp.EmpId = empSum.EmpId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE dis.DisciplineType IN (- 1, - 2) -- other violation
		AND ISNULL(dis.IsDeleted, 0) = 0
		AND CAST(WorkDate AS DATE) BETWEEN CAST(@FromDate AS DATE)
			AND CAST(@ToDate AS DATE)
		AND (
			ISNULL(@GroupId, '') = ''
			OR gr.GroupId = @GroupId
			)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR emp.EmpId = @EmployeeId
			)
		AND (
			@IsLocal IS NULL
			OR empSum.IsLocal = @IsLocal
			)

	CREATE TABLE #EmployeeDisciplineFinalScore (
		WorkDate DATE
		,EmpId INT
		,SystemSubtractScore FLOAT
		,ApproverSubtractScore FLOAT
		)

	INSERT INTO #EmployeeDisciplineFinalScore
	SELECT empD.WorkDate
		,empD.EmpId
		,(empD.ChangeShift * dbo.fncGetDisciplineScoreById(1) + empD.MissingCheckInOut * dbo.fncGetDisciplineScoreById(2) + empD.LateIn * dbo.fncGetDisciplineScoreById(3) + empD.EarlyOut * dbo.fncGetDisciplineScoreById(4) + empD.[Absent] * dbo.fncGetDisciplineScoreById(5) + empD.UnpaidLeave * dbo.fncGetDisciplineScoreById(6) + empD.WorkingUnderTime * dbo.fncGetDisciplineScoreById(7) + empD.UnpaidLeaveAbsent * dbo.fncGetDisciplineScoreById(8)) AS SystemSubtractScore
		,CASE 
			WHEN ignored.ApproverIgnoredScore IS NOT NULL THEN ignored.ApproverIgnoredScore
			ELSE 0
			END
	FROM #EmployeeDiscipline AS empD
		LEFT JOIN (
		SELECT dis.EmpId
			,dis.WorkDate
			,(
				(CASE 
					WHEN dis.IsChangeShift = 1
						AND dis.IsIgnoredChangeShift = 0 THEN dbo.fncGetDisciplineScoreById(1)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsMissingCheckInOut = 1
						AND dis.IsIgnoredMissingCheckInOut = 0 THEN dbo.fncGetDisciplineScoreById(2)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsLateIn = 1
						AND dis.IsIgnoredLateIn = 0 THEN dbo.fncGetDisciplineScoreById(3)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsEarlyOut = 1
						AND dis.IsIgnoredEarlyOut = 0 THEN dbo.fncGetDisciplineScoreById(4)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsAbsent = 1
						AND dis.IsIgnoredAbsent = 0 THEN dbo.fncGetDisciplineScoreById(5)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsUnpaidLeave = 1
						AND dis.IsIgnoredUnpaidLeave = 0 THEN dbo.fncGetDisciplineScoreById(6)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsWorkingUndertime = 1
						AND dis.IsIgnoredWorkingUndertime = 0 THEN dbo.fncGetDisciplineScoreById(7)
					ELSE 0 END
					) + (CASE 
					WHEN dis.IsUnpaidLeaveAbsent = 1
						AND dis.IsIgnoredUnpaidLeaveAbsent = 0 THEN dbo.fncGetDisciplineScoreById(8)
					ELSE 0 END
					)
				) AS ApproverIgnoredScore
		FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
			INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON dis.DisciplineType = dtype.DisciplineTypeId
		WHERE dis.DisciplineType = 1
			AND dis.WorkDate BETWEEN @FromDate
				AND @ToDate
		) AS ignored ON empD.EmpId = ignored.EmpId
		AND empD.WorkDate = ignored.WorkDate
	WHERE (
			[Absent] > 0
			OR ChangeShift > 0
			OR EarlyOut > 0
			OR LateIn > 0
			OR UnpaidLeave > 0
			OR empD.WorkingUnderTime > 0
			OR UnpaidLeaveAbsent > 0
			OR OtherDisciplineScore > 0
			OR MissingCheckInOut > 0
			)

	DECLARE @Is_HR_SA BIT

	CREATE TABLE #Roles (RoleId INT)

	INSERT INTO #Roles
	SELECT RoleId
	FROM dbo.[EmployeeRole] WITH (NOLOCK)
	WHERE EmployeeId = @ApproverId;

	SET @Is_HR_SA = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM #Roles
					WHERE RoleId IN (6, 7) -- HR, SA
					) THEN 1
			ELSE 0
			END

	SELECT SUM(empD.ChangeShift) AS ChangeShift
		,SUM(empD.MissingCheckInOut) AS MissingCheckInOut
		,SUM(empD.LateIn) AS LateIn
		,SUM(empD.EarlyOut) AS EarlyOut
		,SUM(empD.[Absent]) AS [Absent]
		,SUM(empD.UnpaidLeave) AS UnpaidLeave
		,SUM(empD.WorkingUnderTime) AS WorkingUnderTime
		,SUM(empD.UnpaidLeaveAbsent) AS UnpaidLeaveAbsent
		,SUM(empD.OtherDisciplineScore) AS OtherDisciplineScore
		,SUM(empF.SystemSubtractScore + empD.OtherDisciplineScore) AS SystemSubtractScore
		,SUM(empF.ApproverSubtractScore + empD.OtherDisciplineScore) AS ApproverSubtractScore
		,Permission = CASE 
			WHEN EXISTS (
					SELECT 1
					FROM #Roles
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
						)
					OR EXISTS (
						SELECT 1
						FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
						WHERE gr.GroupId = grS.GroupId
							AND @ApproverId = grS.OwnerId
							AND grS.ActionType = 8 -- edit discipline
							AND emp.EmpId <> @ApproverId
							AND NOT EXISTS (
								SELECT 1
								FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
								WHERE empS.OwnerId <> @ApproverId
									AND empS.EmpId = emp.EmpId
									AND empS.ActionType = 8
								)
						)
					)
				AND (
					EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId = @ApproverId
							AND empS.EmpId = emp.EmpId
							AND empS.ActionType = 3 -- approver
							AND empS.EmpId <> @ApproverId
						)
					OR EXISTS (
						SELECT 1
						FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
						WHERE gr.GroupId = grS.GroupId
							AND @ApproverId = grS.OwnerId
							AND grS.ActionType = 3 -- approver
							AND emp.EmpId <> @ApproverId
							AND NOT EXISTS (
								SELECT 1
								FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
								WHERE empS.OwnerId <> @ApproverId
									AND empS.EmpId = emp.EmpId
									AND empS.ActionType = 3
								)
						)
					) THEN 2 -- Host				
			ELSE 1
			END
		,empD.EmpId AS EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,empSum.IsLocal
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
	FROM #EmployeeDisciplineFinalScore AS empF
		INNER JOIN #EmployeeDiscipline AS empD ON empF.WorkDate = empD.WorkDate
		AND empF.EmpId = empD.EmpId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empD.EmpId = emp.EmpId
		INNER JOIN dbo.EmployeeSummary AS empSum WITH (NOLOCK) ON emp.EmpId = empSum.EmpId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND (
			@Is_HR_SA = 1 -- SA, HR can view all member of company
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
				WHERE empS.OwnerId = @ApproverId
					AND empS.EmpId = emp.EmpId
					AND empS.ActionType = 8 -- edit discipline
					AND empS.EmpId <> @ApproverId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE gr.GroupId = grS.GroupId
					AND @ApproverId = grS.OwnerId
					AND grS.ActionType = 8 -- edit discipline
					AND emp.EmpId <> @ApproverId
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @ApproverId
							AND empS.EmpId = emp.EmpId
							AND empS.ActionType = 8
						)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
				WHERE empS.OwnerId = @ApproverId
					AND empS.EmpId = emp.EmpId
					AND empS.ActionType IN (2, 3) -- Approver and Verifier
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE gr.GroupId = grS.GroupId
					AND @ApproverId = grS.OwnerId
					AND grS.ActionType IN (2, 3) -- Approver and Verifier
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @ApproverId
							AND empS.EmpId = emp.EmpId
							AND empS.ActionType IN (2, 3)
						)
				)
			OR (
				EXISTS (
					SELECT 1
					FROM #Roles
					WHERE RoleId = 1 -- Employee
					)
				AND emp.EmpId = @ApproverId
				)
			)
		AND (
			ISNULL(@GroupId, '') = ''
			OR gr.GroupId = @GroupId
			)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR empD.EmpId = @EmployeeId
			)
		AND (
			ISNULL(@ReasonId, '') = ''
			OR (
				@ReasonId = 1
				AND EXISTS (
					SELECT 1
					FROM #EmployeeDiscipline AS dis
					WHERE empD.EmpId = dis.EmpId
						AND dis.ChangeShift > 0
					)
				)
			OR (
				@ReasonId = 2
				AND EXISTS (
					SELECT 1
					FROM #EmployeeDiscipline AS dis
					WHERE empD.EmpId = dis.EmpId
						AND dis.MissingCheckInOut > 0
					)
				)
			OR (
				@ReasonId = 3
				AND EXISTS (
					SELECT 1
					FROM #EmployeeDiscipline AS dis
					WHERE empD.EmpId = dis.EmpId
						AND dis.LateIn > 0
					)
				)
			OR (
				@ReasonId = 4
				AND EXISTS (
					SELECT 1
					FROM #EmployeeDiscipline AS dis
					WHERE empD.EmpId = dis.EmpId
						AND dis.EarlyOut > 0
					)
				)
			OR (
				@ReasonId = 5
				AND EXISTS (
					SELECT 1
					FROM #EmployeeDiscipline AS dis
					WHERE empD.EmpId = dis.EmpId
						AND dis.[Absent] > 0
					)
				)
			OR (
				@ReasonId = 6
				AND EXISTS (
					SELECT 1
					FROM #EmployeeDiscipline AS dis
					WHERE empD.EmpId = dis.EmpId
						AND dis.UnpaidLeave > 0
					)
				)
			OR (
				@ReasonId = 7
				AND EXISTS (
					SELECT 1
					FROM #EmployeeDiscipline AS dis
					WHERE empD.EmpId = dis.EmpId
						AND dis.WorkingUnderTime > 0
					)
				)
			OR (
				@ReasonId = 8
				AND EXISTS (
					SELECT 1
					FROM #EmployeeDiscipline AS dis
					WHERE empD.EmpId = dis.EmpId
						AND dis.UnpaidLeaveAbsent > 0
					)
				)
			OR (
				@ReasonId = - 1
				AND EXISTS (
					SELECT 1
					FROM #EmployeeDiscipline AS dis
					WHERE empD.EmpId = dis.EmpId
						AND dis.VerbalWarning > 0
					)
				)
			OR (
				@ReasonId = - 2
				AND EXISTS (
					SELECT 1
					FROM #EmployeeDiscipline AS dis
					WHERE empD.EmpId = dis.EmpId
						AND dis.WrittenWarning > 0
					)
				)
			)
		AND (
			@IsLocal IS NULL
			OR empSum.IsLocal = @IsLocal
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
	ORDER BY gr.GroupId
		,emp.Code

	DROP TABLE #EmployeeDiscipline

	DROP TABLE #EmployeeDisciplineFinalScore

	DROP TABLE #Roles
END