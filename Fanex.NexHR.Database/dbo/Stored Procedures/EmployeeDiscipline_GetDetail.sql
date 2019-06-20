

CREATE PROCEDURE [dbo].[EmployeeDiscipline_GetDetail] @FromDate DATE
	,@ToDate DATE
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

	DECLARE @HasEditDisciplineRole INT = 0
	--request type
	DECLARE @ChangeShift INT = 5
		,@CheckIn INT = 1
		,@CheckOut INT = 2
		,@LateIn INT = 3
		,@EarlyOut INT = 4
		,@Leave INT = 6
	-- discipline type
	DECLARE @ChangeShiftId INT = 1
		,@MissingCheckInOutId INT = 2
		,@LateInId INT = 3
		,@EarlyOutId INT = 4
		,@AbsentId INT = 5
		,@UnpaidLeaveId INT = 6
		,@WorkingUnderTimeId INT = 7
		,@UnpaidLeaveAbsentId INT = 8
		,@UnplannedLeaveId INT = 9
		,@WeekendLeaveId INT = 10

	CREATE TABLE #tblRoles (RoleId INT)

	CREATE TABLE #tblDisciplineDetail (
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
		,UnplannedLeaveId INT
		,UnplannedLeaveValue INT
		,UnplannedLeaveScore DECIMAL(5, 2)
		,UnplannedLeaveHasViolated BIT
		,UnplannedLeaveHasIgnored BIT
		,UnplannedLeaveHasApprovedBy BIT
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
		,WeekendLeaveId INT
		,WeekendLeaveValue INT
		,WeekendLeaveScore DECIMAL(5, 2)
		,WeekendLeaveHasViolated BIT
		,WeekendLeaveHasIgnored BIT
		,WeekendLeaveHasApprovedBy BIT
		,EmployeeComment NVARCHAR(MAX)
		,ApproverComment NVARCHAR(MAX)
		,IsConfirmed BIT
		,HasConfirmed BIT
		,IsOutOfControlReason BIT
		);

	INSERT INTO #tblRoles (RoleId)
	SELECT RoleId
	FROM dbo.[EmployeeRole] WITH (NOLOCK)
	WHERE EmployeeId = @ApproverId;

	SELECT @HasEditDisciplineRole = 1
	FROM #tblRoles WITH (NOLOCK)
	WHERE RoleId = 8 -- edit discipline

	INSERT INTO #tblDisciplineDetail
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
		,@ChangeShiftId AS ChangeShiftId
		,dis.IsChangeShift AS ChangeShiftValue
		,dbo.fncGetDisciplineScoreById(@ChangeShiftId) AS ChangeShiftScore
		,(CASE 
			WHEN dis.IsChangeShift = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = @ChangeShiftId
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS ChangeShiftHasViolated
		,ISNULL(dis.IsIgnoredChangeShift, 0) AS ChangeShiftHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1 THEN 1
			ELSE 0 END
			) AS ChangeShiftHasApprovedBy
		,@MissingCheckInOutId AS MissingCheckInOutId
		,dis.IsMissingCheckInOut AS MissingCheckInOutValue
		,dbo.fncGetDisciplineScoreById(@MissingCheckInOutId) AS MissingCheckInOutScore
		,(CASE 
			WHEN dis.IsMissingCheckInOut = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = @MissingCheckInOutId
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS MissingCheckInOutHasViolated
		,ISNULL(dis.IsIgnoredMissingCheckInOut, 0) AS MissingCheckInOutHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND (dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1) THEN 1
			ELSE 0 END
			) AS MissingCheckInOutHasApprovedBy
		,@LateInId AS LateInId
		,dis.IsLateIn AS LateInValue
		,dbo.fncGetDisciplineScoreById(@LateInId) AS LateInScore
		,(CASE 
			WHEN dis.IsLateIn = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = @LateInId
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS LateInHasViolated
		,ISNULL(dis.IsIgnoredLateIn, 0) AS LateInHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1 THEN 1
			ELSE 0 END
			) AS LateInHasApprovedBy
		,@EarlyOutId AS EarlyOutId
		,dis.IsEarlyOut AS EarlyOutValue
		,dbo.fncGetDisciplineScoreById(@EarlyOutId) AS EarlyOutScore
		,(CASE 
			WHEN dis.IsEarlyOut = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = @EarlyOutId
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS EarlyOutHasViolated
		,ISNULL(dis.IsIgnoredEarlyOut, 0) AS EarlyOutHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1 THEN 1
			ELSE 0 END
			) AS EarlyOutHasApprovedBy
		,@AbsentId AS AbsentId
		,dis.IsAbsent AS AbsentValue
		,dbo.fncGetDisciplineScoreById(@AbsentId) AS AbsentScore
		,(CASE 
			WHEN dis.IsAbsent = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = @AbsentId
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS AbsentHasViolated
		,ISNULL(dis.IsIgnoredAbsent, 0) AS AbsentHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1 THEN 1
			ELSE 0 END
			) AS AbsentHasApprovedBy
		,@UnplannedLeaveId AS UnplannedLeaveId
		,ISNULL(dis.IsUnplannedLeave, 0) AS UnplannedLeaveValue
		,dbo.fncGetDisciplineScoreById(@UnplannedLeaveId) AS UnplannedLeaveScore
		,(CASE 
			WHEN dis.IsUnplannedLeave = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = @UnplannedLeaveId
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS UnplannedLeaveHasViolated
		,ISNULL(dis.IsIgnoredUnplannedLeave, 0) AS UnplannedLeaveHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1 THEN 1
			ELSE 0 END
			) AS UnplannedLeaveHasApprovedBy
		,@UnpaidLeaveId AS UnpaidLeaveId
		,dis.IsUnpaidLeave AS UnpaidLeaveValue
		,dbo.fncGetDisciplineScoreById(@UnpaidLeaveId) AS UnpaidLeaveScore
		,(CASE 
			WHEN dis.IsUnpaidLeave = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = @UnpaidLeaveId
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS UnpaidLeaveHasViolated
		,ISNULL(dis.IsIgnoredUnpaidLeave, 0) AS UnpaidLeaveHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1 THEN 1
			ELSE 0 END
			) AS UnpaidLeaveHasApprovedBy
		,@WorkingUnderTimeId AS WorkingUnderTimeId
		,dis.IsWorkingUnderTime AS WorkingUnderTimeValue
		,dbo.fncGetDisciplineScoreById(@WorkingUnderTimeId) AS WorkingUnderTimeScore
		,(CASE 
			WHEN dis.IsWorkingUndertime = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = @WorkingUnderTimeId
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS WorkingUnderTimeHasViolated
		,ISNULL(dis.IsIgnoredWorkingUndertime, 0) AS WorkingUnderTimeHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1 THEN 1
			ELSE 0 END
			) AS WorkingUnderTimeHasApprovedBy
		,@UnpaidLeaveAbsentId AS UnpaidLeaveAbsentId
		,dis.IsUnpaidLeaveAbsent AS UnpaidLeaveAbsentValue
		,dbo.fncGetDisciplineScoreById(@UnpaidLeaveAbsentId) AS UnpaidLeaveAbsentScore
		,(CASE 
			WHEN dis.IsUnpaidLeaveAbsent = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = @UnpaidLeaveAbsentId
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS UnpaidLeaveAbsentHasViolated
		,ISNULL(dis.IsIgnoredUnpaidLeaveAbsent, 0) AS UnpaidLeaveAbsentHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1 THEN 1
			ELSE 0 END
			) AS UnpaidLeaveAbsentHasApprovedBy
		,@WeekendLeaveId AS WeekendLeaveId
		,ISNULL(dis.IsWeekendLeave, 0) AS WeekendLeaveValue
		,dbo.fncGetDisciplineScoreById(@WeekendLeaveId) AS WeekendLeaveScore
		,(CASE 
			WHEN dis.IsWeekendLeave = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.GroupId = emp.GroupId
						AND drule.DisciplineTypeId = @WeekendLeaveId
						AND drule.Value = 1
						AND drule.IsLocal = empSum.IsLocal
					) THEN 1
			ELSE 0 END
			) AS WeekendLeaveHasViolated
		,ISNULL(dis.IsIgnoredWeekendLeave, 0) AS WeekendLeaveHasIgnored
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1 THEN 1
			ELSE 0 END
			) AS WeekendLeaveHasApprovedBy
		,dis.EmployeeComment
		,dis.ApproverComment
		,(CASE 
			WHEN ISNULL(dis.IsConfirmed, '') = '' THEN 0
			ELSE dis.IsConfirmed END
			) AS IsConfirmed
		,(CASE 
			WHEN @HasEditDisciplineRole = 1
				AND dbo.fncCheckPermissionIgnoreDisciplineInReport(@ApproverId, gr.GroupId, emp.EmpId) = 1 THEN 1
			ELSE 0 END
			) AS HasConfirmed
		,ISNULL(dis.IsOutOfControlReason, 0)
	FROM dbo.EmployeeDiscipline AS dis WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON dis.EmpId = emp.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EmployeeSummary AS empSum WITH (NOLOCK) ON emp.EmpId = empSum.EmpId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND ISNULL(dis.IsDeleted, 0) = 0
		AND dis.DisciplineType > 0
		AND dis.WorkDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(@EmployeeId, emp.EmpId) = emp.EmpId
		AND (
			dis.IsChangeShift > 0
			OR dis.IsMissingCheckInOut > 0
			OR dis.IsLateIn > 0
			OR dis.IsEarlyOut > 0
			OR dis.IsAbsent > 0
			OR dis.IsUnplannedLeave > 0
			OR dis.IsUnpaidLeave > 0
			OR dis.IsWorkingUnderTime > 0
			OR dis.IsUnpaidLeaveAbsent > 0
			OR dis.IsWeekendLeave > 0
			)
		AND (
			ISNULL(@ReasonId, '') = ''
			OR (
				@ReasonId = @ChangeShiftId
				AND dis.IsChangeShift = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = @ChangeShiftId
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = @MissingCheckInOutId
				AND dis.IsMissingCheckInOut = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = @MissingCheckInOutId
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = @LateInId
				AND dis.IsLateIn = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = @LateInId
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = @EarlyOutId
				AND dis.IsEarlyOut = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = @EarlyOutId
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = @AbsentId
				AND dis.IsAbsent = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = @AbsentId
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = @UnplannedLeaveId
				AND dis.IsUnplannedLeave = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = @UnplannedLeaveId
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = @UnpaidLeaveId
				AND dis.IsUnpaidLeave = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = @UnpaidLeaveId
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = @WorkingUnderTimeId
				AND dis.IsWorkingUndertime = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = @WorkingUnderTimeId
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = @UnpaidLeaveAbsentId
				AND dis.IsUnpaidLeaveAbsent = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = @UnpaidLeaveAbsentId
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			OR (
				@ReasonId = @WeekendLeaveId
				AND dis.IsIgnoredWeekendLeave = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.DisciplineSectionRule AS drule WITH (NOLOCK)
					WHERE drule.DisciplineTypeId = @WeekendLeaveId
						AND drule.Value = 1
						AND drule.GroupId = emp.GroupId
						AND drule.IsLocal = empSum.IsLocal
					)
				)
			)
		AND ISNULL(@GroupId, emp.GroupId) = emp.GroupId
		AND (
			EXISTS (
				SELECT 1
				FROM #tblRoles WITH (NOLOCK)
				WHERE RoleId IN (6, 7) -- HR, SA
				)
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
							AND ISNULL(empS.IsDeleted, 0) = 0
						)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
				WHERE empS.OwnerId = @ApproverId
					AND empS.EmpId = emp.EmpId
					AND empS.ActionType IN (2, 3)
					AND ISNULL(empS.IsDeleted, 0) = 0
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE emp.GroupId = grS.GroupId
					AND @ApproverId = grS.OwnerId
					AND grS.ActionType IN (2, 3)
					AND ISNULL(grS.IsDeleted, 0) = 0
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @ApproverId
							AND empS.EmpId = emp.EmpId
							AND empS.ActionType IN (2, 3)
							AND ISNULL(empS.IsDeleted, 0) = 0
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
						dis.IsUnplannedLeave = 1
						AND dis.IsIgnoredUnplannedLeave = 1
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
					OR (
						dis.IsWeekendLeave = 1
						AND dis.IsIgnoredWeekendLeave = 1
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
						dis.IsUnplannedLeave = 1
						AND dis.IsIgnoredUnplannedLeave = 0
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
					OR (
						dis.IsWeekendLeave = 1
						AND dis.IsIgnoredWeekendLeave = 0
						)
					)
				)
			)
	ORDER BY dis.WorkDate DESC

	SELECT dis.*
	FROM #tblDisciplineDetail AS dis WITH (NOLOCK)
	ORDER BY dis.WorkDate DESC

	DROP TABLE #tblRoles

	DROP TABLE #tblDisciplineDetail
END;