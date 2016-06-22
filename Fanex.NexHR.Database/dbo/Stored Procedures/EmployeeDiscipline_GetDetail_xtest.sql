
CREATE PROCEDURE [dbo].[EmployeeDiscipline_GetDetail_xtest] @FromDate DATETIME
	,@ToDate DATETIME
	,@GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@ReasonId INT = NULL
	,@IsLocal BIT = NULL
	,@ApproverId INT
	,@HasIgnored BIT = NULL
AS
/*
Created: 20160420@Tim.Trinh
Task: Get discipline detail of employees.
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Roles NVARCHAR(100)

	CREATE TABLE #Roles (RoleId INT)

	CREATE TABLE #DisciplineDetail (
		WorkDate DATE
		,ShiftName NVARCHAR(100)
		,CheckIn DATETIME
		,CheckOut DATETIME
		,TotalHours FLOAT
		,LateIn INT
		,EarlyOut INT
		,EmployeeId INT
		,Code NVARCHAR(20)
		,NickName NVARCHAR(20)
		,FullName NVARCHAR(100)
		,IsLocal BIT
		,GroupId INT
		,GroupName NVARCHAR(150)
		,GroupCode NVARCHAR(20)
		,ChangeShiftId INT
		,ChangeShiftValue INT
		,ChangeShiftScore DECIMAL(5, 2)
		,ChangeShiftHasViolated BIT
		,ChangeShiftHasIgnored BIT
		,ChangeShiftHasApprovedBy BIT
		,MissingCheckInOutId INT
		,MissingCheckInOutValue INT
		,MissingCheckInOutScore DECIMAL(5, 2)
		,MissingCheckInOutHasViolated BIT
		,MissingCheckInOutHasIgnored BIT
		,MissingCheckInOutHasApprovedBy BIT
		,LateInId INT
		,LateInValue INT
		,LateInScore DECIMAL(5, 2)
		,LateInHasViolated BIT
		,LateInHasIgnored BIT
		,LateInHasApprovedBy BIT
		,EarlyOutId INT
		,EarlyOutValue INT
		,EarlyOutScore DECIMAL(5, 2)
		,EarlyOutHasViolated BIT
		,EarlyOutHasIgnored BIT
		,EarlyOutHasApprovedBy BIT
		,AbsentId INT
		,AbsentValue INT
		,AbsentScore DECIMAL(5, 2)
		,AbsentHasViolated BIT
		,AbsentHasIgnored BIT
		,AbsentHasApprovedBy BIT
		,UnpaidLeaveId INT
		,UnpaidLeaveValue INT
		,UnpaidLeaveScore DECIMAL(5, 2)
		,UnpaidLeaveHasViolated BIT
		,UnpaidLeaveHasIgnored BIT
		,UnpaidLeaveHasApprovedBy BIT
		,WorkingUnderTimeId INT
		,WorkingUnderTimeValue INT
		,WorkingUnderTimeScore DECIMAL(5, 2)
		,WorkingUnderTimeHasViolated BIT
		,WorkingUnderTimeHasIgnored BIT
		,WorkingUnderTimeHasApprovedBy BIT
		,UnpaidLeaveAbsentId INT
		,UnpaidLeaveAbsentValue INT
		,UnpaidLeaveAbsentScore DECIMAL(5, 2)
		,UnpaidLeaveAbsentHasViolated BIT
		,UnpaidLeaveAbsentHasIgnored BIT
		,UnpaidLeaveAbsentHasApprovedBy BIT
		,EmployeeComment NVARCHAR(MAX)
		,ApproverComment NVARCHAR(MAX)
		)

	SELECT @Roles = emp.Roles
	FROM dbo.Employee AS emp
	WHERE emp.EmpId = @ApproverId

	IF ISNULL(@Roles, '') <> ''
	BEGIN
		INSERT INTO #Roles
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@Roles, ',')
	END

	INSERT INTO #DisciplineDetail
	SELECT DISTINCT empV.WorkDate
		,empV.ShiftName
		,empV.CheckIn
		,empV.CheckOut
		,empV.TotalHours
		,empV.LateIn
		,empV.EarlyOut
		,emp.EmpId AS EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,empSum.IsLocal
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
		,1 AS ChangeShiftId
		,empV.IsChangeShift AS ChangeShiftValue
		,dbo.fncGetDisciplineScoreById(1) AS ChangeShiftScore
		,(CASE 
			WHEN empV.IsChangeShift = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = 1
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS ChangeShiftHasViolated
		,empv.IsIgnoredChangeShift AS ChangeShiftHasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
					WHERE empS.OwnerId = @ApproverId
						AND empS.EmpId = emp.EmpId
						AND empS.ActionType = 3
						AND empS.RequestType = 5 -- Change shift
					)
				OR EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
					WHERE grS.GroupId = emp.GroupId
						AND grS.OwnerId = @ApproverId
						AND grS.RequestType = 5 -- Change shift
						AND grS.ActionType = 3
						AND emp.EmpId <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
							WHERE empS.OwnerId <> @ApproverId
								AND empS.EmpId = emp.EmpId
								AND empS.ActionType = 3
								AND empS.RequestType = 5
							)
					) THEN 1
			ELSE 0 END
			) AS ChangeShiftHasApprovedBy
		,2 AS MissingCheckInOutId
		,empV.IsMissingCheckInOut AS MissingCheckInOutValue
		,dbo.fncGetDisciplineScoreById(2) AS MissingCheckInOutScore
		,(CASE 
			WHEN empV.IsMissingCheckInOut = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = 2
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS MissingCheckInOutHasViolated
		,empv.IsIgnoredMissingCheckInOut AS MissingCheckInOutHasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
					WHERE empS.OwnerId = @ApproverId
						AND empS.EmpId = emp.EmpId
						AND empS.ActionType = 3 -- approver
						AND empS.RequestType IN (1, 2) -- check in/out
					)
				OR EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
					WHERE grS.GroupId = emp.GroupId
						AND grS.OwnerId = @ApproverId
						AND grS.RequestType IN (1, 2)
						AND grS.ActionType = 3
						AND emp.EmpId <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
							WHERE empS.OwnerId <> @ApproverId
								AND empS.EmpId = emp.EmpId
								AND empS.ActionType = 3
								AND empS.RequestType IN (1, 2)
							)
					) THEN 1
			ELSE 0 END
			) AS MissingCheckInOutHasApprovedBy
		,3 AS LateInId
		,empV.IsLateIn AS LateInValue
		,dbo.fncGetDisciplineScoreById(3) AS LateInScore
		,(CASE 
			WHEN empV.IsLateIn = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = 3
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS LateInHasViolated
		,empV.IsIgnoredLateIn AS LateInHasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
					WHERE empS.OwnerId = @ApproverId
						AND empS.EmpId = emp.EmpId
						AND empS.ActionType = 3
						AND empS.RequestType = 3 -- Late In
					)
				OR EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
					WHERE grS.GroupId = emp.GroupId
						AND grS.OwnerId = @ApproverId
						AND grS.RequestType = 3 -- Late In
						AND grS.ActionType = 3
						AND emp.EmpId <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
							WHERE empS.OwnerId <> @ApproverId
								AND empS.EmpId = emp.EmpId
								AND empS.ActionType = 3
								AND empS.RequestType = 3
							)
					) THEN 1
			ELSE 0 END
			) AS LateInHasApprovedBy
		,4 AS EarlyOutId
		,empV.IsEarlyOut AS EarlyOutValue
		,dbo.fncGetDisciplineScoreById(4) AS EarlyOutScore
		,(CASE 
			WHEN empV.IsEarlyOut = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = 4
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS EarlyOutHasViolated
		,empV.IsIgnoredEarlyOut AS EarlyOutHasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
					WHERE empS.OwnerId = @ApproverId
						AND empS.EmpId = emp.EmpId
						AND empS.ActionType = 3
						AND empS.RequestType = 4 -- Early Out
					)
				OR EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
					WHERE grS.GroupId = emp.GroupId
						AND grS.OwnerId = @ApproverId
						AND grS.RequestType = 4 -- Early Out
						AND grS.ActionType = 3
						AND emp.EmpId <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
							WHERE empS.OwnerId <> @ApproverId
								AND empS.EmpId = emp.EmpId
								AND empS.ActionType = 3
								AND empS.RequestType = 4
							)
					) THEN 1
			ELSE 0 END
			) AS EarlyOutHasApprovedBy
		,5 AS AbsentId
		,empV.IsAbsent AS AbsentValue
		,dbo.fncGetDisciplineScoreById(5) AS AbsentScore
		,(CASE 
			WHEN empV.IsAbsent = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = 5
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS AbsentHasViolated
		,empV.IsIgnoredAbsent AS AbsentHasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
					WHERE empS.OwnerId = @ApproverId
						AND empS.EmpId = emp.EmpId
						AND empS.ActionType = 3
						AND empS.RequestType = 6 -- leave
					)
				OR EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
					WHERE grS.GroupId = emp.GroupId
						AND grS.OwnerId = @ApproverId
						AND grS.RequestType = 6 -- leave
						AND grS.ActionType = 3
						AND emp.EmpId <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
							WHERE empS.OwnerId <> @ApproverId
								AND empS.EmpId = emp.EmpId
								AND empS.ActionType = 3
								AND empS.RequestType = 6
							)
					) THEN 1
			ELSE 0 END
			) AS AbsentHasApprovedBy
		,6 AS UnpaidLeaveId
		,empV.IsUnpaidLeave AS UnpaidLeaveValue
		,dbo.fncGetDisciplineScoreById(6) AS UnpaidLeaveScore
		,(CASE 
			WHEN empV.IsUnpaidLeave = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = 6
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS UnpaidLeaveHasViolated
		,empV.IsIgnoredUnpaidLeave AS UnpaidLeaveHasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
					WHERE empS.OwnerId = @ApproverId
						AND empS.EmpId = emp.EmpId
						AND empS.ActionType = 3
						AND empS.RequestType = 6 -- leave
					)
				OR EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
					WHERE grS.GroupId = emp.GroupId
						AND grS.OwnerId = @ApproverId
						AND grS.RequestType = 6 -- leave
						AND grS.ActionType = 3
						AND emp.EmpId <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
							WHERE empS.OwnerId <> @ApproverId
								AND empS.EmpId = emp.EmpId
								AND empS.ActionType = 3
								AND empS.RequestType = 6
							)
					) THEN 1
			ELSE 0 END
			) AS UnpaidLeaveHasApprovedBy
		,7 AS WorkingUnderTimeId
		,empV.IsWorkingUnderTime AS WorkingUnderTimeValue
		,dbo.fncGetDisciplineScoreById(7) AS WorkingUnderTimeScore
		,(CASE 
			WHEN empV.IsWorkingUndertime = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = 7
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS WorkingUnderTimeHasViolated
		,empV.IsIgnoredWorkingUndertime AS WorkingUnderTimeHasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
					WHERE empS.OwnerId = @ApproverId
						AND empS.EmpId = emp.EmpId
						AND empS.ActionType = 3
					)
				OR EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
					WHERE grS.GroupId = emp.GroupId
						AND grS.OwnerId = @ApproverId
						AND grS.ActionType = 3
						AND emp.EmpId <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
							WHERE empS.OwnerId <> @ApproverId
								AND empS.EmpId = emp.EmpId
								AND empS.ActionType = 3
							)
					) THEN 1
			ELSE 0 END
			) AS WorkingUnderTimeHasApprovedBy
		,8 AS UnpaidLeaveAbsentId
		,empV.IsUnpaidLeaveAbsent AS UnpaidLeaveAbsentValue
		,dbo.fncGetDisciplineScoreById(8) AS UnpaidLeaveAbsentScore
		,(CASE 
			WHEN empV.IsUnpaidLeaveAbsent = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = 8
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS UnpaidLeaveAbsentHasViolated
		,empV.IsIgnoredUnpaidLeaveAbsent AS UnpaidLeaveAbsentHasIgnored
		,(CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
					WHERE empS.OwnerId = @ApproverId
						AND empS.EmpId = emp.EmpId
						AND empS.ActionType = 3
						AND empS.RequestType = 6 -- leave
					)
				OR EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
					WHERE grS.GroupId = emp.GroupId
						AND grS.OwnerId = @ApproverId
						AND grS.RequestType = 6 -- leave
						AND grS.ActionType = 3
						AND emp.EmpId <> @ApproverId
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
							WHERE empS.OwnerId <> @ApproverId
								AND empS.EmpId = emp.EmpId
								AND empS.ActionType = 3
								AND empS.RequestType = 6
							)
					) THEN 1
			ELSE 0 END
			) AS UnpaidLeaveAbsentHasApprovedBy
		,empV.EmployeeComment
		,empV.ApproverComment
	FROM dbo.EmployeeDiscipline AS empV WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empV.EmpId = emp.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EmployeeSummary AS empSum WITH (NOLOCK) ON emp.EmpId = empSum.EmpId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND empV.DisciplineType NOT IN (- 1, - 2) -- verbal, written warning
		AND CAST(empV.WorkDate AS DATE) BETWEEN CAST(@FromDate AS DATE)
			AND CAST(@ToDate AS DATE)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR emp.EmpId = @EmployeeId
			)
		AND (
			ISNULL(@ReasonId, '') = ''
			OR (
				@ReasonId = 1
				AND empV.IsChangeShift = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = 1 -- Change Shift
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = 2
				AND empV.IsMissingCheckInOut = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = 2 -- Missing Check In Out
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = 3
				AND empV.IsLateIn = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = 3 -- Late In
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = 4
				AND empV.IsEarlyOut = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = 4 -- Early Out
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = 5
				AND empV.IsAbsent = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = 5 -- Absent
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = 6
				AND empV.IsUnpaidLeave = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = 6 -- Unpaid Leave
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = 7
				AND empV.IsWorkingUndertime = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = 7 -- Working Under Time
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = 8
				AND empV.IsUnpaidLeaveAbsent = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = 8 -- Unpaid Leave Absent
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			)
		AND (
			ISNULL(@GroupId, '') = ''
			OR emp.GroupId = @GroupId
			)
		AND (
			EXISTS (
				SELECT 1
				FROM #Roles
				WHERE RoleId IN (6, 7) -- HR, SA
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
				WHERE empS.OwnerId = @ApproverId
					AND empS.EmpId = emp.EmpId
					AND empS.ActionType IN (2, 3)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE emp.GroupId = grS.GroupId
					AND @ApproverId = grS.OwnerId
					AND grS.ActionType IN (2, 3)
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @ApproverId
							AND empS.EmpId = emp.EmpId
							AND empS.ActionType IN (2, 3)
						)
				)
			OR EXISTS (
				SELECT 1
				FROM #Roles
				WHERE RoleId = 6 -- Employee
				)
			)
		AND (
			@IsLocal IS NULL
			OR empSum.IsLocal = @IsLocal
			)
		AND (
			@HasIgnored IS NULL
			OR (
				@HasIgnored = 0
				AND (
					empV.IsIgnoredChangeShift = 1
					OR empV.IsIgnoredMissingCheckInOut = 1
					OR empV.IsIgnoredLateIn = 1
					OR empV.IsIgnoredEarlyOut = 1
					OR empV.IsIgnoredAbsent = 1
					OR empV.IsIgnoredUnpaidLeave = 1
					OR empV.IsIgnoredWorkingUndertime = 1
					OR empV.IsIgnoredUnpaidLeaveAbsent = 1
					)
				)
			OR (
				@HasIgnored = 1
				AND (
					empV.IsIgnoredChangeShift = 0
					OR empV.IsIgnoredMissingCheckInOut = 0
					OR empV.IsIgnoredLateIn = 0
					OR empV.IsIgnoredEarlyOut = 0
					OR empV.IsIgnoredAbsent = 0
					OR empV.IsIgnoredUnpaidLeave = 0
					OR empV.IsIgnoredWorkingUndertime = 0
					OR empV.IsIgnoredUnpaidLeaveAbsent = 0
					)
				)
			)
	ORDER BY empV.WorkDate DESC

	SELECT d.*
	FROM #DisciplineDetail AS d

	DROP TABLE #Roles

	DROP TABLE #DisciplineDetail
END
