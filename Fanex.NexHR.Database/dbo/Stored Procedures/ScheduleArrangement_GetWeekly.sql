
CREATE PROCEDURE [dbo].[ScheduleArrangement_GetWeekly] @ActorId INT
	,@fromtime DATE
	,@totime DATE
	,@EmpId INT = NULL
	,@GroupIds VARCHAR(MAX) = ''
	,@IsIFrame BIT = 0
AS
/*
Created: Tim.Trinh@2019-04-12
Task: Get weekly schedules
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromDate DATE = @fromtime
		,@toDate DATE = @totime
		,@Today DATE = GETDATE()
		,@FromTime1 DATETIME = GETDATE()
		,@EditScheduleUnlimited VARCHAR(100) = 'EditScheduleUnlimited'
		,@ShiftManagerRoleId TINYINT = 4
		,@GeneralLeaveId TINYINT = 0
		-- work type
		,@OffDay TINYINT = 1
		,@LeaveDay TINYINT = 2
		,@WorkDay TINYINT = 3
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	CREATE TABLE #tblDate (WorkDate DATE)

	CREATE TABLE #tblEmployeeCalendar (
		EmpId INT
		,WorkDate DATE
		,ShiftId INT
		,IsChangeShift BIT
		,IsOffDay BIT
		,IsFixedOffDay BIT
		,IsHoliday BIT
		,IsKeepInView BIT
		,LeaveTypeId INT
		,LeaveTypeId2 INT
		,LeaveDay DECIMAL(18, 1)
		,IsPublic BIT
		,ModifiedBy INT
		,ModifiedDate DATETIME
		)

	CREATE TABLE #tblEmployee (EmployeeId INT)

	CREATE TABLE #tblDefaultShift (
		EmployeeId INT
		,ShiftId INT
		)

	CREATE TABLE #tblGroupIds (GroupId INT)

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO #tblGroupIds (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	END

	INSERT INTO #tblEmployee (EmployeeId)
	SELECT OwnerId
	FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
	WHERE EmpId = @ActorId
		AND (
			@EmpId IS NULL
			OR es.OwnerId = @EmpId
			)
		AND ActionType = @ShiftManagerRoleId
		AND ISNULL(IsDeleted, 0) = 0
		AND EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN #tblGroupIds AS gr ON emp.GroupId = gr.GroupId
			WHERE emp.[Enabled] = 1
				AND emp.isDeleted = 0
				AND es.OwnerId = emp.EmpId
			)

	INSERT INTO #tblEmployee (EmployeeId)
	SELECT emp.EmpId
	FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gs.GroupId = emp.GroupId
	WHERE gs.OwnerId = @ActorId
		AND gs.ActionType = @ShiftManagerRoleId
		AND ISNULL(gs.IsDeleted, 0) = 0
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0
		AND (
			@EmpId IS NULL
			OR emp.EmpId = @EmpId
			)
		AND EXISTS (
			SELECT 1
			FROM #tblGroupIds AS gr
			WHERE emp.GroupId = gr.GroupId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #tblEmployee AS tbl
			WHERE tbl.EmployeeId = emp.EmpId
			)

	IF @IsIFrame = 1
		OR EXISTS (
			SELECT 1
			FROM dbo.RoleFunction AS rf WITH (NOLOCK)
				INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
				AND f.FunctionKey = @EditScheduleUnlimited
				AND f.IsActive = 1
				INNER JOIN dbo.EmployeeRole AS erole WITH (NOLOCK) ON rf.RoleId = erole.RoleId
				AND erole.EmployeeId = @ActorId
			WHERE rf.IsRead = 1
			)
	BEGIN
		INSERT INTO #tblEmployee (EmployeeId)
		SELECT EmpId
		FROM dbo.Employee AS emp WITH (NOLOCK)
		WHERE [Enabled] = 1
			AND isDeleted = 0
			AND (
				@EmpId IS NULL
				OR emp.EmpId = @EmpId
				)
			AND EXISTS (
				SELECT 1
				FROM #tblGroupIds AS gr
				WHERE emp.GroupId = gr.GroupId
				)
			AND NOT EXISTS (
				SELECT 1
				FROM #tblEmployee AS tbl
				WHERE emp.EmpId = tbl.EmployeeId
				)
	END

	DELETE emp
	FROM #tblEmployee AS emp
	WHERE EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary AS esum WITH (NOLOCK)
			WHERE emp.EmployeeId = esum.EmpId
				AND esum.IsTopManager = 1
			)

	WHILE (@fromDate <= @toDate)
	BEGIN
		INSERT INTO #tblDate (WorkDate)
		VALUES (@fromDate)

		SET @fromDate = DATEADD(d, 1, @fromDate)
	END

	SET @fromDate = @fromtime

	INSERT INTO #tblEmployeeCalendar (
		EmpId
		,WorkDate
		,ShiftId
		,IsChangeShift
		,IsOffDay
		,IsFixedOffDay
		,IsHoliday
		,IsKeepInView
		,LeaveTypeId
		,LeaveTypeId2
		,LeaveDay
		,IsPublic
		,ModifiedBy
		,ModifiedDate
		)
	SELECT empca.EmpId
		,WorkDate
		,ShiftId
		,(CASE 
			WHEN changeshift.EmpId IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsChangeShift
		,IsOffDay
		,IsFixedOffDay
		,IsHoliday
		,IsKeepInView
		,LeaveTypeId
		,LeaveTypeId2
		,LeaveDay
		,IsPublic
		,ModifiedBy
		,ModifiedDate
	FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
		INNER JOIN #tblEmployee AS emp ON empca.EmpId = emp.EmployeeId
		OUTER APPLY (
		SELECT csr.EmpId
		FROM dbo.ChangeShiftRequest AS csr WITH (NOLOCK)
			INNER JOIN dbo.ChangeShiftRequestDetail AS csrdt WITH (NOLOCK) ON csr.RequestId = csrdt.RequestId
			AND empca.WorkDate = csrdt.OnDate
			INNER JOIN dbo.WorkingTimeRequest AS wkr WITH (NOLOCK) ON csr.RequestId = wkr.RequestId
			AND wkr.CurrentStatus IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId)
		WHERE empca.EmpId = csr.EmpId
		) AS changeshift
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate
		AND (
			@IsIFrame = 0
			OR (
				@IsIFrame = 1
				AND empca.IsPublic = 1
				)
			)

	IF @IsIFrame = 1
	BEGIN
		DELETE emp
		FROM #tblEmployee AS emp
		WHERE NOT EXISTS (
				SELECT 1
				FROM #tblEmployeeCalendar AS empca
				WHERE emp.EmployeeId = empca.EmpId
				)
	END

	INSERT INTO #tblDefaultShift (
		EmployeeId
		,ShiftId
		)
	SELECT emp.EmployeeId
		,defaultshift.ShiftId
	FROM #tblEmployee AS emp
		INNER JOIN dbo.ScheduleArrangementDefaultShift AS defaultshift WITH (NOLOCK) ON emp.EmployeeId = defaultshift.EmployeeId
	WHERE defaultshift.FromDate = @fromtime

	INSERT INTO #tblDefaultShift (
		EmployeeId
		,ShiftId
		)
	SELECT data.EmployeeId
		,defaultshift.ShiftId
	FROM (
		SELECT EmployeeId
			,MAX(FromDate) AS FromDate
		FROM dbo.ScheduleArrangementDefaultShift WITH (NOLOCK)
		WHERE FromDate < @fromtime
		GROUP BY EmployeeId
		) data
		INNER JOIN dbo.ScheduleArrangementDefaultShift AS defaultshift WITH (NOLOCK) ON data.EmployeeId = defaultshift.EmployeeId
		AND data.FromDate = defaultshift.FromDate
	WHERE NOT EXISTS (
			SELECT 1
			FROM #tblDefaultShift AS tbl
			WHERE defaultshift.EmployeeId = tbl.EmployeeId
			)

	SELECT emp.EmpId AS EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,ISNULL(esum.OffDaysbyWeek, 0) AS OffDaysbyWeek
		,emp.PickupRemark
		,arrange.Remark
		,arrange.DisplayOrder
		,esum.IsDefaultSchedule AS IsUsedWeeklyDefaultShift
		,modifier.NickName AS Modifier
		,ec.ModifiedDate
		,gr.GroupId
		,gr.Code
		,gr.[Name]
		,shift.ShiftId AS DefaultShiftId
		,shift.Code
		,shift.[Name]
		,shift.BeginCheckInTime AS CheckIn
		,shift.BeginCheckOutTime AS CheckOut
		,shift.IsInAfterDay
		,shift.IsOutAfterDay
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblEmployee AS tblemp ON emp.EmpId = tblemp.EmployeeId
		LEFT JOIN dbo.EmployeeSummary AS esum WITH (NOLOCK) ON emp.EmpId = esum.EmpId
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.ScheduleArrangement AS arrange WITH (NOLOCK) ON emp.EmpId = arrange.EmployeeId
		LEFT JOIN dbo.ScheduleArrangementGroupOrder AS gorder WITH (NOLOCK) ON gr.GroupId = gorder.GroupId
		LEFT JOIN #tblDefaultShift AS defaultshift ON emp.EmpId = defaultshift.EmployeeId
		LEFT JOIN dbo.Shift AS shift WITH (NOLOCK) ON defaultshift.ShiftId = shift.ShiftId
		AND ISNULL(shift.IsDisabled, 0) = 0
		OUTER APPLY (
		SELECT TOP 1 ec.ModifiedBy
			,ec.ModifiedDate
		FROM #tblEmployeeCalendar AS ec
		WHERE ec.EmpId = emp.EmpId
			AND ec.ModifiedBy IS NOT NULL
			AND ec.ModifiedDate IS NOT NULL
			AND ec.ModifiedDate = (
				SELECT MAX(ec1.ModifiedDate)
				FROM #tblEmployeeCalendar AS ec1
				WHERE ec.EmpId = ec1.EmpId
				)
		) AS ec
		LEFT JOIN dbo.Employee AS modifier WITH (NOLOCK) ON ec.ModifiedBy = modifier.EmpId
	WHERE EXISTS (
			SELECT 1
			FROM #tblGroupIds AS tblgr
			WHERE emp.GroupId = tblgr.GroupId
			)
	ORDER BY ISNULL(gorder.DisplayOrder, 0)
		,gr.Code
		,ISNULL(arrange.DisplayOrder, 0)
		,emp.NickName

	SELECT emp.EmpId AS EmployeeId
		,tmp.WorkDate
		,(CASE 
			WHEN ot.[Status] IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId) THEN CONVERT(BIT, 1)
			ELSE CONVERT(BIT, 0) END
			) AS IsOvertime
		,(CASE 
			WHEN spwkr.WorkDate IS NOT NULL
				AND spwkr.WorkType = @OffDay THEN 1
			ELSE 0 END
			) AS IsLeaveOffday
		,ec.IsChangeShift
		,(CASE 
			WHEN (
					esum.IsLocal = 1
					OR (
						esum.IsLocal = 0
						AND esum.IsExpatUseHoliday = 1
						)
					)
				AND hld.Id IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,ec.IsOffDay
		,ec.IsFixedOffDay
		,(CASE 
			WHEN spwkr.WorkDate IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsLeaveDay
		,ec.IsKeepInView
		,(CASE 
			WHEN ISNULL(ec.LeaveDay, 0) = 0 THEN ISNULL(spwkr.TotalDays, 0)
			ELSE ec.LeaveDay END
			) AS LeaveDay
		,ec.IsPublic
		,(CASE 
			WHEN offrep.EmployeeId IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsOffDayReplacement
		,(CASE 
			WHEN ec.EmpId IS NOT NULL THEN 1
			WHEN @fromTime <= @Today THEN 1
			ELSE 0 END
			) AS IsModified
		,shift.ShiftId
		,shift.Code
		,shift.[Name]
		,shift.BeginCheckInTime AS CheckIn
		,shift.BeginCheckOutTime AS CheckOut
		,shift.IsInAfterDay
		,shift.IsOutAfterDay
		,lv1.LeaveTypeId
		,lv1.Code
		,lv1.[Name]
		,spwkr.FromTime
		,spwkr.ToTime
		,lv2.LeaveTypeId AS LeaveTypeId2
		,lv2.Code
		,lv2.[Name]
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN #tblEmployee AS tblemp ON emp.EmpId = tblemp.EmployeeId
		CROSS JOIN #tblDate AS tmp
		LEFT JOIN #tblEmployeeCalendar AS ec ON emp.EmpId = ec.EmpId
		AND ec.WorkDate = tmp.WorkDate
		LEFT JOIN dbo.EmployeeSummary AS esum WITH (NOLOCK) ON esum.EmpId = emp.EmpId
		LEFT JOIN dbo.OTDailyRequest AS ot WITH (NOLOCK) ON ot.OvertimeDate = tmp.WorkDate
		AND ot.EmpId = emp.EmpId
		AND esum.IsLocal = 1
		LEFT JOIN dbo.Holiday AS hld WITH (NOLOCK) ON (
			(
				hld.FromDate BETWEEN @fromDate
					AND @toDate
				OR hld.ToDate BETWEEN @fromDate
					AND @toDate
				)
			AND (
				tmp.WorkDate BETWEEN hld.FromDate
					AND hld.ToDate
				)
			)
		LEFT JOIN dbo.Shift AS shift WITH (NOLOCK) ON shift.ShiftId = ec.ShiftId
		LEFT JOIN dbo.LeaveType AS lv2 WITH (NOLOCK) ON ec.LeaveTypeId2 = lv2.LeaveTypeId
		LEFT JOIN (
		SELECT tmp.WorkDate
			,lrd.LeaveType
			,lrd.EmpId
			,lrd.WorkType
			,(CASE lrd.IsHalfDay
				WHEN 1 THEN 0.5
				ELSE 1 END
				) AS TotalDays
			,lrd.FromTime AS FromTime
			,lrd.ToTime AS ToTime
		FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON wkr.RequestId = lrd.RequestId
			INNER JOIN #tblDate AS tmp ON lrd.OnDate = tmp.WorkDate
			INNER JOIN #tblEmployeeCalendar AS cla ON lrd.OnDate = cla.WorkDate
			AND wkr.EmpId = cla.EmpId
		WHERE lrd.WorkType IN (@OffDay, @LeaveDay)
			AND wkr.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		
		UNION
		SELECT tmp.WorkDate
			,lr.LeaveType
			,lr.EmpId
			,@LeaveDay AS WorkType
			,1 AS TotalDays
			,NULL AS FromTime
			,NULL AS ToTime
		FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON lr.LeaveType <> @GeneralLeaveId
			AND wkr.RequestId = lr.RequestId
			INNER JOIN #tblDate AS tmp ON tmp.WorkDate BETWEEN lr.FromDate
				AND lr.ToDate
			INNER JOIN #tblEmployeeCalendar AS cla ON cla.WorkDate = tmp.WorkDate
			AND wkr.EmpId = cla.EmpId
		WHERE wkr.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
			AND cla.IsOffDay <> 1
			AND cla.IsHoliday <> 1
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.LeaveRequestDetail AS lrd WITH (NOLOCK)
				WHERE lr.RequestId = lrd.RequestId
				)
		) spwkr ON spwkr.WorkDate = tmp.WorkDate
		AND spwkr.EmpId = emp.EmpId
		LEFT JOIN dbo.LeaveType AS lv1 WITH (NOLOCK) ON spwkr.LeaveType = lv1.LeaveTypeId
		LEFT JOIN dbo.ScheduleArrangementOffDayReplacement AS offrep WITH (NOLOCK) ON ec.EmpId = offrep.EmployeeId
		AND ec.WorkDate = offrep.WorkDate
	WHERE EXISTS (
			SELECT 1
			FROM #tblGroupIds AS tblgr
			WHERE emp.GroupId = tblgr.GroupId
			)
	ORDER BY emp.NickName
		,tmp.WorkDate

	SELECT emp.EmployeeId
		,ISNULL(monshift.ShiftID, - 1) AS MonShiftId
		,ISNULL(tueshift.ShiftID, - 1) AS TueShiftId
		,ISNULL(wedshift.ShiftID, - 1) AS WedShiftId
		,ISNULL(thushift.ShiftID, - 1) AS ThuShiftId
		,ISNULL(frishift.ShiftID, - 1) AS FriShiftId
		,ISNULL(satshift.ShiftID, - 1) AS SatShiftId
		,ISNULL(sunshift.ShiftID, - 1) AS SunShiftId
	FROM #tblEmployee AS emp
		LEFT JOIN dbo.EmployeeDefaultShift AS eds WITH (NOLOCK) ON eds.EmpID = emp.EmployeeId
		LEFT JOIN dbo.Shift AS monshift WITH (NOLOCK) ON eds.MonShift = monshift.ShiftID
		AND ISNULL(monshift.IsDisabled, 0) = 0
		LEFT JOIN dbo.Shift AS tueshift WITH (NOLOCK) ON eds.TueShift = tueshift.ShiftID
		AND ISNULL(tueshift.IsDisabled, 0) = 0
		LEFT JOIN dbo.Shift AS wedshift WITH (NOLOCK) ON eds.WedShift = wedshift.ShiftID
		AND ISNULL(wedshift.IsDisabled, 0) = 0
		LEFT JOIN dbo.Shift AS thushift WITH (NOLOCK) ON eds.ThuShift = thushift.ShiftID
		AND ISNULL(thushift.IsDisabled, 0) = 0
		LEFT JOIN dbo.Shift AS frishift WITH (NOLOCK) ON eds.FriShift = frishift.ShiftID
		AND ISNULL(frishift.IsDisabled, 0) = 0
		LEFT JOIN dbo.Shift AS satshift WITH (NOLOCK) ON eds.SatShift = satshift.ShiftID
		AND ISNULL(satshift.IsDisabled, 0) = 0
		LEFT JOIN dbo.Shift AS sunshift WITH (NOLOCK) ON eds.SunShift = sunshift.ShiftID
		AND ISNULL(sunshift.IsDisabled, 0) = 0

	DROP TABLE #tblGroupIds

	DROP TABLE #tblDate

	DROP TABLE #tblEmployeeCalendar

	DROP TABLE #tblEmployee

	DROP TABLE #tblDefaultShift
END
