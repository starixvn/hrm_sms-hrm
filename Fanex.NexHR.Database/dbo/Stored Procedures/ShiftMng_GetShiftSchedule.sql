CREATE PROCEDURE [dbo].[ShiftMng_GetShiftSchedule] @SupervisorID INT
	,@EmpID INT = NULL
	,@fromtime DATETIME
	,@totime DATETIME
	,@groupIds VARCHAR(max) = ''
	,@JobTitleId VARCHAR(1000) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
	,@isDebug BIT = 0
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

	SELECT @fromDate = CONVERT(VARCHAR(10), @fromtime, 111) + ' 00:00:00'
		,@toDate = CONVERT(VARCHAR(10), @totime, 111) + ' 00:00:00'

	IF (
			DATEPART(dw, @fromDate) <> 2
			OR DATEPART(dw, @toDate) <> 1
			OR DATEDIFF(dd, @fromdate, @toDate) <> 6
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Parameter not correct.'

		RETURN
	END	

	CREATE TABLE #Calendar (
		WorkDate DATETIME
		,[Enabled] BIT
		,IsDeleted BIT
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
	LEFT JOIN dbo.ShiftArrangementOrder AS sd WITH (NOLOCK) 
		ON es.OwnerId = sd.OwnerId 
		AND es.EmpId = sd.EmpId
		AND es.ActionType = 4
	WHERE es.EmpID = @SupervisorID
		AND ActionType = 4
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
		LEFT JOIN dbo.ShiftArrangementOrder AS sd ON esu.EmpId = sd.OwnerId
				AND gs.OwnerId = sd.EmpId
		LEFT JOIN #EmployeeSupervisor AS TEMP ON TEMP.OwnerId = esu.EmpId
		AND TEMP.EmpId = gs.OwnerId
	WHERE gs.OwnerId = @SupervisorID
		AND gs.ActionType = 4
		AND TEMP.EmpId IS NULL
		AND ISNULL(gs.IsDeleted, 0) = 0

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

	CREATE CLUSTERED INDEX IDX_C_Calendar_DeviceID ON #Calendar (WorkDate)

	CREATE TABLE #GroupsList (GroupId INT)

	IF ISNULL(@groupids, '') <> ''
		INSERT INTO #GroupsList (GroupId)
		SELECT g.val
		FROM fnc_strSplit(@groupids, ',') AS g
	ELSE
	BEGIN
		INSERT INTO #GroupsList (GroupId)
		SELECT g.GRoupID
		FROM dbo.[Group] g WITH (NOLOCK)
	END

	CREATE TABLE #JobTitleList (JobTitleId INT)

	IF ISNULL(@JobTitleId, '') <> ''
	BEGIN
		INSERT INTO #JobTitleList (JobTitleId)
		SELECT val
		FROM fnc_strSplit(@JobTitleId, ',')
	END

	SELECT tmp.WorkDate
		,DATEPART(dw, tmp.WorkDate) [Weekday]
		,es.OrderCode
		,CASE 
			WHEN ot.STATUS IN (2, 4, 6) THEN CONVERT(BIT, 1)
			ELSE CONVERT(BIT, 0)
			END IsOvertime
		,CASE 
			WHEN spwkr.WorkDate IS NOT NULL
				AND spwkr.WorkType = 1 THEN 1
			ELSE 0
			END IsLeaveOffday
		,CASE 
			WHEN ec.ChangeShiftId IS NULL THEN 0
			ELSE 1
			END IsChangeShift
		,CASE 
			WHEN (
					esum.islocal = 1
					OR (
						esum.IsLocal = 0
						AND esum.IsExpatUseHoliday = 1
						)
					)
				AND hld.HolidayName IS NOT NULL THEN 1
			ELSE 0
			END IsHoliday
		,ec.IsOffDay
		,CASE 
			WHEN spwkr.WorkDate IS NOT NULL THEN 1
			ELSE 0
			END IsLeaveDay
		,ec.IsKeepInView
		,ISNULL(ec.LeaveDay, 0) LeaveDay
		,IsPublic
		,s2.ShiftId ShiftMultipleID
		,s2.Code ShiftMultipleCode
		,s2.NAME ShiftMultipleName
		,ee.NickName AS ModifierName
		,ec.ModifiedDate
		,ISNULL(esum.OffDaysbyWeek, 0) OffDaysbyWeek
		,e.EmpId
		,e.NickName
		,e.FullName
		,g.GroupId
		,g.Code
		,g.NAME
		,ec.ShiftId
		,s.Code
		,s.NAME
		,lv1.LeaveTypeId
		,lv1.Code
		,lv1.NAME
		,lv2.LeaveTypeId AS LeaveTypeId2
		,lv2.Code
		,lv2.NAME
		,e.JobTitleId [PositionID]
		,lk.NAME [Positions]
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #EmployeeSupervisor AS es ON e.EmpId = es.OwnerID
		INNER JOIN #Calendar tmp ON e.[Enabled] = tmp.[Enabled]
		AND e.[IsDeleted] = tmp.[IsDeleted]
		LEFT JOIN dbo.EmployeeCalendar ec WITH (NOLOCK) ON ec.WorkDate = tmp.WorkDate
		AND e.EmpId = ec.EmpId
		LEFT JOIN dbo.EmployeeSummary esum WITH (NOLOCK) ON esum.EmpId = e.EmpId
		LEFT JOIN dbo.OTDailyRequest ot WITH (NOLOCK) ON ot.OvertimeDate = tmp.WorkDate
		AND ot.EmpId = e.EmpId
		AND esum.islocal = 1
		LEFT JOIN dbo.Holiday hld WITH (NOLOCK) ON (
			(hld.FromDate BETWEEN @fromDate
				AND @toDate
			OR hld.ToDate BETWEEN @fromDate
				AND @toDate)
			AND (
				tmp.WorkDate BETWEEN hld.FromDate
					AND hld.ToDate
				)
			)
		LEFT JOIN dbo.LookupData lk WITH (NOLOCK) ON e.JobTitleId = lk.Id
		AND LookupId = 1
		LEFT JOIN dbo.Employee ee WITH (NOLOCK) ON ec.ModifiedBy = ee.EmpId
		LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ShiftId = ec.ShiftId
		LEFT JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON ec.LeaveTypeId = lv.LeaveTypeId
		LEFT JOIN dbo.LeaveType AS lv2 WITH (NOLOCK) ON ec.LeaveTypeId2 = lv2.LeaveTypeId
		LEFT JOIN (
			SELECT tbl.ForDate
				,tbl.EmpID
				,spl.ShiftId
			FROM (
				SELECT MAX(ForDate) ForDate
					,EmpID
				FROM dbo.EmployeeScheduleShiftMultiple AS spl WITH (NOLOCK)
				WHERE ForDate <= @fromDate
					AND spl.ShiftId > 0
				GROUP BY EmpID
				) tbl
				LEFT JOIN dbo.EmployeeScheduleShiftMultiple AS spl WITH (NOLOCK) ON tbl.ForDate = spl.ForDate
				AND tbl.EmpID = spl.EmpID
		) spl ON spl.ForDate <= tmp.WorkDate
		AND spl.EmpID = e.EmpID
		LEFT JOIN dbo.Shift AS s2 WITH (NOLOCK) ON spl.ShiftId = s2.ShiftId
		LEFT JOIN (
			SELECT tmp.WorkDate
				,lrd.LeaveType
				,lrd.EmpId
				,lrd.WorkType
			FROM dbo.WorkingTimeRequest wkr WITH (NOLOCK)
				INNER JOIN dbo.LeaveRequestDetail lrd WITH (NOLOCK) ON wkr.RequestId = lrd.RequestId
				AND wkr.CurrentStatus NOT IN (3, 5, 7)
				INNER JOIN #Calendar tmp ON lrd.OnDate = tmp.WorkDate
			WHERE lrd.WorkType IN (1, 2)
				AND wkr.CurrentStatus NOT IN (3, 5, 7)
		
			UNION
			SELECT tmp.WorkDate
				,lrd.LeaveType
				,lrd.EmpId
				,2 WorkType
			FROM dbo.WorkingTimeRequest wkr WITH (NOLOCK)
				INNER JOIN dbo.LeaveRequest lrd WITH (NOLOCK) ON lrd.LeaveType <> 0
				AND wkr.RequestId = lrd.RequestId
				INNER JOIN #Calendar tmp ON (
					tmp.WorkDate BETWEEN lrd.FromDate
						AND lrd.ToDate
					)
				INNER JOIN dbo.EmployeeCalendar cla ON cla.WorkDate = tmp.WorkDate
				AND wkr.EmpId = cla.EmpId
			WHERE wkr.CurrentStatus NOT IN (3, 5, 7)
				--AND cla.IsOffDay <> 1
				--AND cla.IsHoliday <> 1
		) spwkr ON spwkr.WorkDate = tmp.WorkDate
		AND spwkr.EmpId = e.EmpId
		LEFT JOIN dbo.LeaveType AS lv1 WITH (NOLOCK) ON spwkr.LeaveType = lv1.LeaveTypeId
	WHERE (
			@EmpID IS NULL
			OR e.EmpId = @EmpID
			)
		AND (
			ISNULL(@groupids, '') = ''
			OR e.GroupId IN (
				SELECT GroupId
				FROM #GroupsList
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

	DROP TABLE #GroupsList

	DROP TABLE #Calendar

	DROP TABLE #JobTitleList

	DROP TABLE #EmployeeSupervisor
END
