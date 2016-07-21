CREATE PROCEDURE [dbo].[EmployeeDiscipline_GetDetail] @FromDate DATETIME
	,@ToDate DATETIME
	,@ApproverId INT
	,@GroupId INT = NULL
	,@EmployeeId INT = NULL
	,@ReasonId INT = NULL
	,@IsLocal BIT = NULL
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

	DECLARE @HasEditDisciplineRole INT;

	SET @HasEditDisciplineRole = 0;

	--request type
	DECLARE @ChangeShift INT
		,@CheckIn INT
		,@CheckOut INT
		,@LateIn INT
		,@EarlyOut INT
		,@Leave INT;

	SET @CheckIn = 1;
	SET @CheckOut = 2;
	SET @LateIn = 3;
	SET @EarlyOut = 4;
	SET @ChangeShift = 5;
	SET @Leave = 6;

	--action type
	DECLARE @Approved INT;

	SET @Approved = 3;

	CREATE TABLE #Roles (RoleId INT);

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
		,IsConfirmed BIT
		,HasConfirmed BIT
		);

	INSERT INTO #Roles
	SELECT RoleId
	FROM dbo.[EmployeeRole] WITH (NOLOCK)
	WHERE EmployeeId = @ApproverId;

	SELECT @HasEditDisciplineRole = 1
	FROM #Roles
	WHERE RoleId = 8;-- edit discipline

	INSERT INTO #DisciplineDetail
	SELECT DISTINCT dis.WorkDate
		,dis.ShiftName
		,dis.CheckIn
		,dis.CheckOut
		,dis.TotalHours
		,dis.LateIn
		,dis.EarlyOut
		,emp.EmpId AS EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,empSum.IsLocal
		,gr.GroupId
		,gr.[Name] AS GroupName
		,gr.Code AS GroupCode
		,1 AS ChangeShiftId
		,dis.IsChangeShift AS ChangeShiftValue
		,dbo.fncGetDisciplineScoreById(1) AS ChangeShiftScore
		,(CASE 
			WHEN dis.IsChangeShift = 1
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
		,dis.IsIgnoredChangeShift AS ChangeShiftHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @ChangeShift) = 1 THEN 1
			ELSE 0 END
			) AS ChangeShiftHasApprovedBy
		,2 AS MissingCheckInOutId
		,dis.IsMissingCheckInOut AS MissingCheckInOutValue
		,dbo.fncGetDisciplineScoreById(2) AS MissingCheckInOutScore
		,(CASE 
			WHEN dis.IsMissingCheckInOut = 1
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
		,dis.IsIgnoredMissingCheckInOut AS MissingCheckInOutHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND (
					dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @CheckIn) = 1
					OR dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @CheckOut) = 1
					) THEN 1
			ELSE 0 END
			) AS MissingCheckInOutHasApprovedBy
		,3 AS LateInId
		,dis.IsLateIn AS LateInValue
		,dbo.fncGetDisciplineScoreById(3) AS LateInScore
		,(CASE 
			WHEN dis.IsLateIn = 1
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
		,dis.IsIgnoredLateIn AS LateInHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @LateIn) = 1 THEN 1
			ELSE 0 END
			) AS LateInHasApprovedBy
		,4 AS EarlyOutId
		,dis.IsEarlyOut AS EarlyOutValue
		,dbo.fncGetDisciplineScoreById(4) AS EarlyOutScore
		,(CASE 
			WHEN dis.IsEarlyOut = 1
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
		,dis.IsIgnoredEarlyOut AS EarlyOutHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @EarlyOut) = 1 THEN 1
			ELSE 0 END
			) AS EarlyOutHasApprovedBy
		,5 AS AbsentId
		,dis.IsAbsent AS AbsentValue
		,dbo.fncGetDisciplineScoreById(5) AS AbsentScore
		,(CASE 
			WHEN dis.IsAbsent = 1
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
		,dis.IsIgnoredAbsent AS AbsentHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @Leave) = 1 THEN 1
			ELSE 0 END
			) AS AbsentHasApprovedBy
		,6 AS UnpaidLeaveId
		,dis.IsUnpaidLeave AS UnpaidLeaveValue
		,dbo.fncGetDisciplineScoreById(6) AS UnpaidLeaveScore
		,(CASE 
			WHEN dis.IsUnpaidLeave = 1
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
		,dis.IsIgnoredUnpaidLeave AS UnpaidLeaveHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @Leave) = 1 THEN 1
			ELSE 0 END
			) AS UnpaidLeaveHasApprovedBy
		,7 AS WorkingUnderTimeId
		,dis.IsWorkingUnderTime AS WorkingUnderTimeValue
		,dbo.fncGetDisciplineScoreById(7) AS WorkingUnderTimeScore
		,(CASE 
			WHEN dis.IsWorkingUndertime = 1
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
		,dis.IsIgnoredWorkingUndertime AS WorkingUnderTimeHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, NULL) = 1 THEN 1
			ELSE 0 END
			) AS WorkingUnderTimeHasApprovedBy
		,8 AS UnpaidLeaveAbsentId
		,dis.IsUnpaidLeaveAbsent AS UnpaidLeaveAbsentValue
		,dbo.fncGetDisciplineScoreById(8) AS UnpaidLeaveAbsentScore
		,(CASE 
			WHEN dis.IsUnpaidLeaveAbsent = 1
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
		,dis.IsIgnoredUnpaidLeaveAbsent AS UnpaidLeaveAbsentHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @Leave) = 1 THEN 1
			ELSE 0 END
			) AS UnpaidLeaveAbsentHasApprovedBy
		,dis.EmployeeComment
		,dis.ApproverComment
		,(CASE 
			WHEN ISNULL(dis.IsConfirmed, '') = '' THEN 0
			ELSE dis.IsConfirmed END
			) AS IsConfirmed
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @ChangeShift) = 1
				AND (
					dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @CheckIn) = 1
					OR dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @CheckOut) = 1
					)
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @LateIn) = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @EarlyOut) = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @Leave) = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @Leave) = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, NULL) = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId, @Approved, @Leave) = 1 THEN 1
			ELSE 0 END
			) AS HasConfirmed
	FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON dis.EmpId = emp.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EmployeeSummary AS empSum WITH (NOLOCK) ON emp.EmpId = empSum.EmpId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND ISNULL(dis.IsDeleted, 0) = 0
		AND dis.DisciplineType NOT IN (- 1, - 2) -- verbal, written warning
		AND CAST(dis.WorkDate AS DATE) BETWEEN CAST(@FromDate AS DATE)
			AND CAST(@ToDate AS DATE)
		AND (
			ISNULL(@EmployeeId, '') = ''
			OR emp.EmpId = @EmployeeId
			)
		AND (
			dis.IsChangeShift > 0
			OR dis.IsMissingCheckInOut > 0
			OR dis.IsLateIn > 0
			OR dis.IsEarlyOut > 0
			OR dis.IsAbsent > 0
			OR dis.IsUnpaidLeave > 0
			OR dis.IsWorkingUnderTime > 0
			OR dis.IsUnpaidLeaveAbsent > 0
			)
		AND (
			ISNULL(@ReasonId, '') = ''
			OR (
				@ReasonId = 1
				AND dis.IsChangeShift = 1
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
				AND dis.IsMissingCheckInOut = 1
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
				AND dis.IsLateIn = 1
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
				AND dis.IsEarlyOut = 1
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
				AND dis.IsAbsent = 1
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
				AND dis.IsUnpaidLeave = 1
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
				AND dis.IsWorkingUndertime = 1
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
				AND dis.IsUnpaidLeaveAbsent = 1
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
			@IsLocal IS NULL
			OR empSum.IsLocal = @IsLocal
			)
		AND (
			@HasIgnored IS NULL
			OR (
				@HasIgnored = 1
				AND (
					(
						dis.IsChangeShift = 1
						AND dis.IsIgnoredChangeShift = 1
						)
					OR (
						dis.IsMissingCheckInOut = 1
						AND dis.IsIgnoredMissingCheckInOut = 1
						)
					OR (
						dis.IsLateIn = 1
						AND dis.IsIgnoredLateIn = 1
						)
					OR (
						dis.IsEarlyOut = 1
						AND dis.IsIgnoredEarlyOut = 1
						)
					OR (
						dis.IsAbsent = 1
						AND dis.IsIgnoredAbsent = 1
						)
					OR (
						dis.IsUnpaidLeave = 1
						AND dis.IsIgnoredUnpaidLeave = 1
						)
					OR (
						dis.IsWorkingUnderTime = 1
						AND dis.IsIgnoredWorkingUndertime = 1
						)
					OR (
						dis.IsUnpaidLeaveAbsent = 1
						AND dis.IsIgnoredUnpaidLeaveAbsent = 1
						)
					)
				)
			OR (
				@HasIgnored = 0
				AND (
					(
						dis.IsChangeShift = 1
						AND dis.IsIgnoredChangeShift = 0
						)
					OR (
						dis.IsMissingCheckInOut = 1
						AND dis.IsIgnoredMissingCheckInOut = 0
						)
					OR (
						dis.IsLateIn = 1
						AND dis.IsIgnoredLateIn = 0
						)
					OR (
						dis.IsEarlyOut = 1
						AND dis.IsIgnoredEarlyOut = 0
						)
					OR (
						dis.IsAbsent = 1
						AND dis.IsIgnoredAbsent = 0
						)
					OR (
						dis.IsUnpaidLeave = 1
						AND dis.IsIgnoredUnpaidLeave = 0
						)
					OR (
						dis.IsWorkingUnderTime = 1
						AND dis.IsIgnoredWorkingUndertime = 0
						)
					OR (
						dis.IsUnpaidLeaveAbsent = 1
						AND dis.IsIgnoredUnpaidLeaveAbsent = 0
						)
					)
				)
			)
	ORDER BY dis.WorkDate DESC;

	SELECT dis.*
	FROM #DisciplineDetail AS dis
	ORDER BY dis.WorkDate DESC;

	DROP TABLE #Roles;

	DROP TABLE #DisciplineDetail;
END;