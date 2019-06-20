
CREATE PROCEDURE [dbo].[ShiftMng_GetShiftSchedule] @SupervisorId INT
	,@EmpId INT = NULL
	,@fromtime DATETIME
	,@totime DATETIME
	,@groupIds VARCHAR(max) = ''
	,@JobTitleId VARCHAR(1000) = ''
	,@isDebug BIT = 0 WITH RECOMPILE
AS
/*
Created: Tom.Vo@2013-10-23
Task: Get Shift Scheduling
DB: HRM_SMS
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromDate NVARCHAR(100)
		,@toDate NVARCHAR(100)
		,@FromTime1 DATETIME = GETDATE()
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

	SELECT @fromDate = CONVERT(VARCHAR(10), @fromtime, 111) + ' 00:00:00'
		,@toDate = CONVERT(VARCHAR(10), @totime, 111) + ' 00:00:00'

	CREATE TABLE #Calendar (
		WorkDate DATE
		,[Enabled] BIT
		,IsDeleted BIT
		)

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

	CREATE TABLE #EmployeeSupervisor (
		OwnerId INT
		,EmpId INT
		,OrderNo SMALLINT
		,OrderCode VARCHAR(10)
		)

	INSERT INTO #EmployeeSupervisor (
		OwnerId
		,EmpId
		,OrderNo
		,OrderCode
		)
	SELECT es.OwnerId
		,es.EmpId
		,sd.OrderNo
		,sd.OrderCode
	FROM dbo.EmployeeSupervisor AS es WITH (NOLOCK)
		LEFT JOIN dbo.ShiftArrangementOrder AS sd WITH (NOLOCK) ON es.OwnerId = sd.OwnerId
		AND es.EmpId = sd.EmpId
	WHERE es.EmpID = @SupervisorId
		AND es.ActionType = @ShiftManagerRoleId
		AND ISNULL(es.IsDeleted, 0) = 0

	INSERT INTO #EmployeeSupervisor (
		OwnerId
		,EmpId
		,OrderNo
		,OrderCode
		)
	SELECT esu.EmpId AS OwnerId
		,gs.OwnerId AS EmpId
		,sd.OrderNo
		,sd.OrderCode
	FROM dbo.GroupSupervisor AS gs WITH (NOLOCK)
		INNER JOIN dbo.Employee AS esu WITH (NOLOCK) ON gs.GroupId = esu.GroupId
		LEFT JOIN dbo.ShiftArrangementOrder AS sd WITH (NOLOCK) ON esu.EmpId = sd.OwnerId
		AND gs.OwnerId = sd.EmpId
	WHERE gs.OwnerId = @SupervisorId
		AND gs.ActionType = @ShiftManagerRoleId
		AND ISNULL(gs.IsDeleted, 0) = 0
		AND NOT EXISTS (
			SELECT 1
			FROM #EmployeeSupervisor AS tbl
			WHERE tbl.OwnerId = esu.EmpId
				AND tbl.EmpId = gs.OwnerId
			)

	WHILE (@fromDate <= @toDate)
	BEGIN
		INSERT INTO #Calendar (
			WorkDate
			,[Enabled]
			,IsDeleted
			)
		VALUES (
			@fromDate
			,1
			,0
			)

		SET @fromDate = CONVERT(VARCHAR(10), CONVERT(DATETIME, @fromDate) + 1, 111) + ' 00:00:00'
	END

	SELECT @fromDate = CONVERT(VARCHAR(10), @fromtime, 111) + ' 00:00:00'

	CREATE TABLE #tblGroupIds (GroupId INT)

	IF ISNULL(@groupids, '') <> ''
		INSERT INTO #tblGroupIds (GroupId)
		SELECT g.val
		FROM fnc_strSplit(@groupids, ',') AS g
	ELSE
	BEGIN
		INSERT INTO #tblGroupIds (GroupId)
		SELECT GroupId
		FROM dbo.[Group] WITH (NOLOCK)
	END

	CREATE TABLE #JobTitleList (JobTitleId INT)

	IF ISNULL(@JobTitleId, '') <> ''
	BEGIN
		INSERT INTO #JobTitleList (JobTitleId)
		SELECT val
		FROM fnc_strSplit(@JobTitleId, ',')
	END

	PRINT DATEDIFF(MILLISECOND, @FromTime1, GETDATE())

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
	FROM dbo.EmployeeCalendar WITH (NOLOCK)
	WHERE WorkDate BETWEEN @fromDate
			AND @toDate

	SELECT tmp.WorkDate
		,DATEPART(dw, tmp.WorkDate) AS [Weekday]
		,es.OrderCode
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
		,s2.ShiftId AS ShiftMultipleId
		,s2.Code AS ShiftMultipleCode
		,s2.[Name] AS ShiftMultipleName
		,ee.NickName AS ModifierName
		,ec.ModifiedDate
		,ISNULL(esum.OffDaysbyWeek, 0) AS OffDaysbyWeek
		,e.EmpId
		,e.NickName
		,e.FullName
		,g.GroupId
		,g.Code
		,g.[Name]
		,ec.ShiftId
		,s.Code
		,s.[Name]
		,lv1.LeaveTypeId
		,lv1.Code
		,lv1.[Name]
		,lv2.LeaveTypeId AS LeaveTypeId2
		,lv2.Code
		,lv2.[Name]
		,e.JobTitleId AS [PositionId]
		,lk.[Name] AS [Positions]
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #EmployeeSupervisor AS es ON e.EmpId = es.OwnerId
		INNER JOIN #Calendar AS tmp ON e.[Enabled] = tmp.[Enabled]
		AND e.[IsDeleted] = tmp.[IsDeleted]
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
		LEFT JOIN dbo.LookupData AS lk WITH (NOLOCK) ON e.JobTitleId = lk.Id
		AND LookupId = 1
		LEFT JOIN dbo.Employee AS ee WITH (NOLOCK) ON ec.ModifiedBy = ee.EmpId
		LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ec.ShiftId
		LEFT JOIN dbo.LeaveType AS lv2 WITH (NOLOCK) ON ec.LeaveTypeId2 = lv2.LeaveTypeId
		LEFT JOIN (
		SELECT tbl.ForDate
			,tbl.EmpId
			,spl.ShiftId
		FROM (
			SELECT MAX(ForDate) ForDate
				,EmpId
			FROM dbo.EmployeeScheduleShiftMultiple WITH (NOLOCK)
			WHERE ForDate <= @fromDate
				AND ShiftId > 0
			GROUP BY EmpId
			) tbl
			LEFT JOIN dbo.EmployeeScheduleShiftMultiple AS spl WITH (NOLOCK) ON tbl.ForDate = spl.ForDate
			AND tbl.EmpId = spl.EmpId
		) spl ON spl.ForDate <= tmp.WorkDate
		AND spl.EmpId = e.EmpId
		LEFT JOIN dbo.Shift AS s2 WITH (NOLOCK) ON spl.ShiftId = s2.ShiftId
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
	WHERE (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
			)
		AND (
			ISNULL(@groupids, '') = ''
			OR e.GroupId IN (
				SELECT GroupId
				FROM #tblGroupIds
				)
			)
		AND (
			ISNULL(@JobTitleId, '') = ''
			OR e.JobTitleId IN (
				SELECT JobTitleId
				FROM #JobTitleList
				)
			)
	ORDER BY es.OrderCode
		,es.OrderNo
		,e.NickName
		,tmp.WorkDate

	PRINT DATEDIFF(MILLISECOND, @FromTime1, GETDATE())

	DROP TABLE #tblGroupIds

	DROP TABLE #Calendar

	DROP TABLE #tmpCalendar

	DROP TABLE #JobTitleList

	DROP TABLE #EmployeeSupervisor
END