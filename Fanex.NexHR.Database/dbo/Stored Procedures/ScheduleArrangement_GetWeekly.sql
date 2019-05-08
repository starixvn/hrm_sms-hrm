
CREATE PROCEDURE [dbo].[ScheduleArrangement_GetWeekly] @ActorId INT
	,@fromtime DATE
	,@totime DATE
	,@EmpId INT = NULL
	,@GroupIds VARCHAR(MAX) = ''
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
		,@FromTime1 DATETIME = GETDATE()
		,@ViewFullDataFunctionKey VARCHAR(100) = 'ViewFullScheduleData'
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

	CREATE TABLE #Calendar (WorkDate DATE)

	CREATE TABLE #tmpCalendar (
		EmpId INT
		,WorkDate DATE
		,ShiftId INT
		,ChangeShiftId INT
		,IsOffDay BIT
		,IsHoliday BIT
		,IsKeepInView BIT
		,LeaveTypeId INT
		,LeaveTypeId2 INT
		,LeaveDay DECIMAL(18, 1)
		,IsPublic BIT
		,ModifiedBy INT
		,ModifiedDate DATETIME
		)

	CREATE TABLE #tblEmployeeSupervisor (OwnerId INT)

	CREATE TABLE #tblGroupIds (GroupId INT)

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO #tblGroupIds (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	END

	INSERT INTO #tblEmployeeSupervisor (OwnerId)
	SELECT OwnerId
	FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
	WHERE EmpId = @ActorId
		AND ActionType = @ShiftManagerRoleId
		AND ISNULL(IsDeleted, 0) = 0
		AND EXISTS (
			SELECT 1
			FROM dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN #tblGroupIds AS gr ON e.GroupId = gr.GroupId
			WHERE e.[Enabled] = 1
				AND e.isDeleted = 0
				AND es.OwnerId = e.EmpId
			)

	INSERT INTO #tblEmployeeSupervisor (OwnerId)
	SELECT e.EmpId
	FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gs.GroupId = e.GroupId
	WHERE gs.OwnerId = @ActorId
		AND gs.ActionType = @ShiftManagerRoleId
		AND ISNULL(gs.IsDeleted, 0) = 0
		AND e.[Enabled] = 1
		AND e.isDeleted = 0
		AND EXISTS (
			SELECT 1
			FROM #tblGroupIds AS gr
			WHERE e.GroupId = gr.GroupId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM #tblEmployeeSupervisor AS tbl
			WHERE tbl.OwnerId = e.EmpId
			)

	IF EXISTS (
			SELECT 1
			FROM dbo.RoleFunction AS rf WITH (NOLOCK)
				INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
				AND f.FunctionKey = @ViewFullDataFunctionKey
				AND f.IsActive = 1
				INNER JOIN dbo.EmployeeRole AS erole WITH (NOLOCK) ON rf.RoleId = erole.RoleId
				AND erole.EmployeeId = @ActorId
			WHERE rf.IsRead = 1
			)
	BEGIN
		INSERT INTO #tblEmployeeSupervisor (OwnerId)
		SELECT EmpId
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE [Enabled] = 1
			AND isDeleted = 0
			AND EXISTS (
				SELECT 1
				FROM #tblGroupIds AS gr
				WHERE e.GroupId = gr.GroupId
				)
			AND NOT EXISTS (
				SELECT 1
				FROM #tblEmployeeSupervisor AS tbl
				WHERE e.EmpId = tbl.OwnerId
				)
	END

	DELETE e
	FROM #tblEmployeeSupervisor AS e
	WHERE EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary AS esum WITH (NOLOCK)
			WHERE e.OwnerId = esum.EmpId
				AND esum.IsTopManager = 1
			)

	WHILE (@fromDate <= @toDate)
	BEGIN
		INSERT INTO #Calendar (WorkDate)
		VALUES (@fromDate)

		SET @fromDate = DATEADD(d, 1, @fromDate)
	END

	SET @fromDate = @fromtime

	INSERT INTO #tmpCalendar (
		EmpId
		,WorkDate
		,ShiftId
		,ChangeShiftId
		,IsOffDay
		,IsHoliday
		,IsKeepInView
		,LeaveTypeId
		,LeaveTypeId2
		,LeaveDay
		,IsPublic
		,ModifiedBy
		,ModifiedDate
		)
	SELECT EmpId
		,WorkDate
		,ShiftId
		,ChangeShiftId
		,IsOffDay
		,IsHoliday
		,IsKeepInView
		,LeaveTypeId
		,LeaveTypeId2
		,LeaveDay
		,IsPublic
		,ModifiedBy
		,ModifiedDate
	FROM dbo.EmployeeCalendar AS ec WITH (NOLOCK)
		INNER JOIN #tblEmployeeSupervisor AS es ON ec.EmpId = es.OwnerId
	WHERE WorkDate BETWEEN @fromDate
			AND @toDate

	SELECT e.EmpId AS EmployeeId
		,e.NickName AS Nickname
		,e.FullName
		,ISNULL(esum.OffDaysbyWeek, 0) AS OffDaysbyWeek
		,arrange.Remark
		,arrange.DisplayOrder
		,esum.IsDefaultSchedule AS IsUsedWeeklyDefaultShift
		,modifier.NickName AS Modifier
		,ec.ModifiedDate
		,g.GroupId
		,g.Code
		,g.[Name]
		,shift.ShiftId AS DefaultShiftId
		,shift.Code
		,shift.[Name]
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #tblEmployeeSupervisor AS es ON e.EmpId = es.OwnerId
		LEFT JOIN dbo.EmployeeSummary AS esum WITH (NOLOCK) ON e.EmpId = esum.EmpId
		LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN dbo.ScheduleArrangement AS arrange WITH (NOLOCK) ON e.EmpId = arrange.EmployeeId
		LEFT JOIN dbo.ScheduleArrangementGroupOrder AS gorder WITH (NOLOCK) ON g.GroupId = gorder.GroupId
		OUTER APPLY (
		SELECT tbl.EmployeeId
			,spl.ShiftId
		FROM (
			SELECT MAX(FromDate) AS FromDate
				,EmployeeId
			FROM dbo.ScheduleArrangementDefaultShift WITH (NOLOCK)
			WHERE FromDate <= @fromDate
				AND ShiftId > 0
			GROUP BY EmployeeId
			) tbl
			LEFT JOIN dbo.ScheduleArrangementDefaultShift AS spl WITH (NOLOCK) ON tbl.FromDate = spl.FromDate
			AND tbl.EmployeeId = spl.EmployeeId
		WHERE tbl.EmployeeId = e.EmpId
		) defaultshift
		LEFT JOIN dbo.Shift AS shift WITH (NOLOCK) ON defaultshift.ShiftId = shift.ShiftId
		OUTER APPLY (
		SELECT DISTINCT ec.EmpId
			,ec.ModifiedBy
			,ec.ModifiedDate
		FROM #tmpCalendar AS ec
		WHERE ec.EmpId = e.EmpId
			AND ec.ModifiedBy IS NOT NULL
			AND ec.ModifiedDate IS NOT NULL
			AND ec.ModifiedDate = (
				SELECT MAX(ec1.ModifiedDate)
				FROM #tmpCalendar AS ec1
				WHERE ec.EmpId = ec1.EmpId
				)
		) AS ec
		LEFT JOIN dbo.Employee AS modifier WITH (NOLOCK) ON ec.ModifiedBy = modifier.EmpId
	WHERE e.[Enabled] = 1
		AND e.isDeleted = 0
		AND (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
			)
		AND EXISTS (
			SELECT 1
			FROM #tblGroupIds AS tblgr
			WHERE e.GroupId = tblgr.GroupId
			)
	ORDER BY ISNULL(gorder.DisplayOrder, 0)
		,g.Code
		,ISNULL(arrange.DisplayOrder, 0)
		,e.NickName

	SELECT e.EmpId AS EmployeeId
		,tmp.WorkDate
		,DATEPART(dw, tmp.WorkDate) AS [Weekday]
		,(CASE 
			WHEN ot.[Status] IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId) THEN CONVERT(BIT, 1)
			ELSE CONVERT(BIT, 0) END
			) AS IsOvertime
		,(CASE 
			WHEN spwkr.WorkDate IS NOT NULL
				AND spwkr.WorkType = @OffDay THEN 1
			ELSE 0 END
			) AS IsLeaveOffday
		,(CASE 
			WHEN ec.ChangeShiftId IS NULL THEN 0
			ELSE 1 END
			) AS IsChangeShift
		,(CASE 
			WHEN (
					esum.IsLocal = 1
					OR (
						esum.IsLocal = 0
						AND esum.IsExpatUseHoliday = 1
						)
					)
				AND hld.HolidayName IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,ec.IsOffDay
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
		,ec.ShiftId
		,s.Code
		,s.[Name]
		,lv1.LeaveTypeId
		,lv1.Code
		,lv1.[Name]
		,lv2.LeaveTypeId AS LeaveTypeId2
		,lv2.Code
		,lv2.[Name]
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #tblEmployeeSupervisor AS es ON e.EmpId = es.OwnerId
		CROSS JOIN #Calendar AS tmp
		LEFT JOIN #tmpCalendar AS ec ON ec.WorkDate = tmp.WorkDate
		AND e.EmpId = ec.EmpId
		LEFT JOIN dbo.EmployeeSummary AS esum WITH (NOLOCK) ON esum.EmpId = e.EmpId
		LEFT JOIN dbo.OTDailyRequest AS ot WITH (NOLOCK) ON ot.OvertimeDate = tmp.WorkDate
		AND ot.EmpId = e.EmpId
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
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ec.ShiftId
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
		FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON wkr.RequestId = lrd.RequestId
			INNER JOIN #Calendar AS tmp ON lrd.OnDate = tmp.WorkDate
			INNER JOIN #tmpCalendar AS cla ON lrd.OnDate = cla.WorkDate
			AND wkr.EmpId = cla.EmpId
		WHERE lrd.WorkType IN (@OffDay, @LeaveDay)
			AND wkr.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		
		UNION
		SELECT tmp.WorkDate
			,lr.LeaveType
			,lr.EmpId
			,@LeaveDay AS WorkType
			,1 AS TotalDays
		FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON lr.LeaveType <> @GeneralLeaveId
			AND wkr.RequestId = lr.RequestId
			INNER JOIN #Calendar AS tmp ON tmp.WorkDate BETWEEN lr.FromDate
				AND lr.ToDate
			INNER JOIN #tmpCalendar AS cla ON cla.WorkDate = tmp.WorkDate
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
		AND spwkr.EmpId = e.EmpId
		LEFT JOIN dbo.LeaveType AS lv1 WITH (NOLOCK) ON spwkr.LeaveType = lv1.LeaveTypeId
	WHERE e.[Enabled] = 1
		AND e.isDeleted = 0
		AND (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
			)
		AND EXISTS (
			SELECT 1
			FROM #tblGroupIds AS tblgr
			WHERE e.GroupId = tblgr.GroupId
			)
	ORDER BY e.NickName
		,tmp.WorkDate

	SELECT e.OwnerId AS EmployeeId
		,ISNULL(monshift.ShiftID, -1) AS MonShiftId
		,ISNULL(monshift.Code, 'Off') AS MonShiftCode
		,ISNULL(tueshift.ShiftID, -1) AS TueShiftId
		,ISNULL(tueshift.Code, 'Off') AS TueShiftCode
		,ISNULL(wedshift.ShiftID, -1) AS WedShiftId
		,ISNULL(wedshift.Code, 'Off') AS WedShiftCode
		,ISNULL(thushift.ShiftID, -1) AS ThuShiftId
		,ISNULL(thushift.Code, 'Off') AS ThuShiftCode
		,ISNULL(frishift.ShiftID, -1) AS FriShiftId
		,ISNULL(frishift.Code, 'Off') AS FriShiftCode
		,ISNULL(satshift.ShiftID, -1) AS SatShiftId
		,ISNULL(satshift.Code, 'Off') AS SatShiftCode
		,ISNULL(sunshift.ShiftID, -1) AS SunShiftId
		,ISNULL(sunshift.Code, 'Off') AS SunShiftCode
	FROM #tblEmployeeSupervisor AS e
		LEFT JOIN dbo.EmployeeDefaultShift AS eds WITH (NOLOCK) ON eds.EmpID = e.OwnerId
		LEFT JOIN dbo.Shift AS monshift WITH (NOLOCK) ON eds.MonShift = monshift.ShiftID
		LEFT JOIN dbo.Shift AS tueshift WITH (NOLOCK) ON eds.TueShift = tueshift.ShiftID
		LEFT JOIN dbo.Shift AS wedshift WITH (NOLOCK) ON eds.WedShift = wedshift.ShiftID
		LEFT JOIN dbo.Shift AS thushift WITH (NOLOCK) ON eds.ThuShift = thushift.ShiftID
		LEFT JOIN dbo.Shift AS frishift WITH (NOLOCK) ON eds.FriShift = frishift.ShiftID
		LEFT JOIN dbo.Shift AS satshift WITH (NOLOCK) ON eds.SatShift = satshift.ShiftID
		LEFT JOIN dbo.Shift AS sunshift WITH (NOLOCK) ON eds.SunShift = sunshift.ShiftID

	DROP TABLE #tblGroupIds

	DROP TABLE #Calendar

	DROP TABLE #tmpCalendar

	DROP TABLE #tblEmployeeSupervisor
END
