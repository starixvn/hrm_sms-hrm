
CREATE FUNCTION [dbo].[fnc_GetDisciplineDetailByLeaveRequestId] (@RequestId BIGINT)
RETURNS @DisciplineDetail TABLE (
	EmployeeId INT
	,WorkDate DATETIME
	,IsOffOrHoliday BIT
	,ScheduleStatus NVARCHAR(MAX)
	,IsUnpaidLeaveAbsent BIT
	,IsUnpaidLeave BIT
	,IsAbsent BIT
	,IsUnplannedLeave BIT
	,IsWeekendLeave BIT
	,IsSevenDaysLeave BIT
	)
AS
/*
Created: Quinn.Vo@20161130
Task: get discipline detail by leave request id
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	DECLARE @LeaveType INT
		,@GeneralLeaveType INT = 0
		,@AnnualLeaveType INT = 2
		,@KeepInViewLeaveType INT = 10
		,@UnpaidLeaveType INT = 25
		,@PublicHolidayLeaveType INT = 13
		,@LeaveWorkType TINYINT = 2
		,@TotalDays DECIMAL(5, 1)
		,@Today DATE = GETDATE()

	SELECT @LeaveType = LeaveType
		,@TotalDays = TotalDays
	FROM dbo.LeaveRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	DECLARE @tblAbsent TABLE (WorkDate DATE, EmpId INT)
	DECLARE @tblUnplannedLeave TABLE (WorkDate DATE, EmpId INT)
	DECLARE @tblUnpaidLeaveAbsent TABLE (WorkDate DATE, EmpId INT)
	DECLARE @tblUnpaidLeave TABLE (WorkDate DATE, EmpId INT)
	DECLARE @tblWeekendLeave TABLE (WorkDate DATE, EmpId INT)
	DECLARE @tblSevenDaysLeave TABLE(WorkDate DATE, EmpId INT)

	INSERT INTO @tblUnpaidLeaveAbsent (WorkDate, EmpId)
	SELECT empC.WorkDate, empC.EmpId
	FROM dbo.EmployeeCalendar AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 8
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lv.FromDate
			,lv.ToDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType = @UnpaidLeaveType
			AND wk.CurrentStatus IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND DATEDIFF(dd, lv.FromDate, wk.CreatedDate) >= 0
		) AS lea ON empC.EmpId = lea.EmpId
		AND DATEDIFF(dd, empC.WorkDate, lea.FromDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1
	
	UNION ALL
	SELECT empC.WorkDate, empC.[EmpId]
	FROM dbo.EmployeeCalendar AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 8
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lvt.OnDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
			INNER JOIN dbo.LeaveRequestDetail lvt WITH (NOLOCK) ON lv.RequestId = lvt.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType = @GeneralLeaveType
			AND lvt.LeaveType = @UnpaidLeaveType
			AND wk.CurrentStatus IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND lvt.OnDate = (
				SELECT MIN(OnDate)
				FROM dbo.LeaveRequestDetail WITH (NOLOCK)
				WHERE LeaveType = @UnpaidLeaveType
					AND RequestId = lv.RequestId
				GROUP BY RequestId
				)
			AND DATEDIFF(dd, lv.FromDate, wk.CreatedDate) >= 0
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND DATEDIFF(dd, empC.WorkDate, lea.OnDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1

	INSERT INTO @tblAbsent (WorkDate, EmpId)
	SELECT empC.WorkDate, empC.EmpId
	FROM dbo.EmployeeCalendar AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 5
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lv.FromDate
			,lv.ToDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType IN (@AnnualLeaveType, @KeepInViewLeaveType, @PublicHolidayLeaveType)
			AND wk.CurrentStatus IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND DATEDIFF(dd, lv.FromDate, wk.CreatedDate) >= 0
		) AS lea ON empC.EmpId = lea.EmpId
		AND DATEDIFF(dd, empC.WorkDate, lea.FromDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1

	UNION ALL
	SELECT empC.WorkDate, empC.EmpId
	FROM dbo.EmployeeCalendar AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 5
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lvt.OnDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
			INNER JOIN dbo.LeaveRequestDetail AS lvt WITH (NOLOCK) ON lv.RequestId = lvt.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType = @GeneralLeaveType
			AND lvt.LeaveType IN (@AnnualLeaveType, @KeepInViewLeaveType, @PublicHolidayLeaveType)
			AND wk.CurrentStatus IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.LeaveRequestDetail WITH (NOLOCK)
				WHERE RequestId = lv.RequestId
					AND LeaveType = @UnpaidLeaveType
				)
			AND lvt.OnDate = (
				SELECT MIN(OnDate)
				FROM dbo.LeaveRequestDetail WITH (NOLOCK)
				WHERE lvt.LeaveType IN (@AnnualLeaveType, @KeepInViewLeaveType, @PublicHolidayLeaveType)
					AND RequestId = lv.RequestId
				GROUP BY RequestId
				)
			AND DATEDIFF(dd, lv.FromDate, wk.CreatedDate) >= 0
		) AS lea ON empC.EmpId = lea.EmpId
		AND DATEDIFF(dd, empC.WorkDate, lea.OnDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1

	-- must create Leave Request by FRIDAY, 11:59PM GMT+7 for leave on next week
	INSERT INTO @tblUnplannedLeave (WorkDate, EmpId)
	SELECT empC.WorkDate, empC.EmpId
	FROM dbo.EmployeeCalendar AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 9
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lv.FromDate
			,lv.ToDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType IN (@AnnualLeaveType, @UnpaidLeaveType, @KeepInViewLeaveType, @PublicHolidayLeaveType)
			AND wk.CurrentStatus IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND DATEDIFF(dd, lv.FromDate, wk.CreatedDate) < 0
			AND DATEDIFF(dd, DATEADD(wk, DATEDIFF(wk, 7, DATEADD(dd, - 1, lv.FromDate)), 4), wk.CreatedDate) > 0
		) AS lea ON empC.EmpId = lea.EmpId
		AND DATEDIFF(dd, empC.WorkDate, lea.FromDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1
	
	UNION ALL
	SELECT empC.WorkDate, empC.EmpId
	FROM dbo.EmployeeCalendar AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 9
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lvt.OnDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
			INNER JOIN dbo.LeaveRequestDetail AS lvt WITH (NOLOCK) ON lv.RequestId = lvt.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType = @GeneralLeaveType
			AND lvt.LeaveType IN (@AnnualLeaveType, @UnpaidLeaveType, @KeepInViewLeaveType, @PublicHolidayLeaveType)
			AND wk.CurrentStatus IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND lvt.OnDate = (
				SELECT MIN(OnDate)
				FROM dbo.LeaveRequestDetail AS ldt WITH (NOLOCK)
				WHERE ldt.LeaveType IN (@AnnualLeaveType, @UnpaidLeaveType, @KeepInViewLeaveType, @PublicHolidayLeaveType)
					AND ldt.RequestId = lv.RequestId
				GROUP BY ldt.RequestId
				)
			AND DATEDIFF(dd, lv.FromDate, wk.CreatedDate) < 0
			AND DATEDIFF(dd, DATEADD(wk, DATEDIFF(wk, 7, DATEADD(dd, - 1, lv.FromDate)), 4), wk.[CreatedDate]) > 0
		) AS lea ON empC.EmpId = lea.EmpId
		AND DATEDIFF(dd, empC.WorkDate, lea.OnDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1

	INSERT INTO @tblUnpaidLeave (WorkDate, EmpId)
	SELECT empC.WorkDate, empC.EmpId
	FROM dbo.EmployeeCalendar AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 6
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lv.FromDate
			,lv.ToDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType = @UnpaidLeaveType
			AND wk.CurrentStatus IN (1, 2, 4, 6) -- pending/verified/approved/processed
		) AS lea ON empC.EmpId = lea.EmpId
		AND CAST(empC.WorkDate AS DATE) BETWEEN CAST(lea.FromDate AS DATE) AND CAST(lea.ToDate AS DATE)
	WHERE ISNULL(rul.Value, 0) = 1
	
	UNION ALL
	SELECT empC.WorkDate, empC.EmpId
	FROM dbo.EmployeeCalendar AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 6
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lvt.OnDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
			INNER JOIN dbo.LeaveRequestDetail AS lvt WITH (NOLOCK) ON lv.RequestId = lvt.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType = @GeneralLeaveType
			AND lvt.LeaveType = @UnpaidLeaveType
			AND wk.CurrentStatus IN (1, 2, 4, 6) -- pending/verified/approved/processed
		) AS lea ON empC.EmpId = lea.EmpId
		AND empC.WorkDate = lea.OnDate
	WHERE ISNULL(rul.Value, 0) = 1

	INSERT INTO @tblWeekendLeave (WorkDate, EmpId)
	SELECT DISTINCT WorkDate, EmpId
	FROM dbo.WeekendLeaveTrans WITH (NOLOCK)
	WHERE RequestId = @RequestId
		AND IsDiscipline = 1

	INSERT INTO @tblSevenDaysLeave (WorkDate, EmpId)
	SELECT empca.WorkDate, empca.EmpId
	FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empca.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 11
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lv.FromDate
			,lv.ToDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType IN (@AnnualLeaveType, @UnpaidLeaveType, @KeepInViewLeaveType, @PublicHolidayLeaveType)
			AND wk.CurrentStatus IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND DATEDIFF(DAY, wk.CreatedDate, lv.FromDate) > 0
			AND DATEDIFF(DAY, wk.CreatedDate, lv.FromDate) < 7
		) AS lea ON empca.EmpId = lea.EmpId
		AND DATEDIFF(DAY, empca.WorkDate, lea.FromDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1
	
	UNION ALL
	SELECT empca.WorkDate, empca.EmpId
	FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empca.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 11
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lvt.OnDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
			INNER JOIN dbo.LeaveRequestDetail AS lvt WITH (NOLOCK) ON lv.RequestId = lvt.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType = @GeneralLeaveType
			AND lvt.LeaveType IN (@AnnualLeaveType, @UnpaidLeaveType, @KeepInViewLeaveType, @PublicHolidayLeaveType)
			AND wk.CurrentStatus IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND lvt.OnDate = (
				SELECT MIN(OnDate)
				FROM dbo.LeaveRequestDetail AS lrdt WITH (NOLOCK)
				WHERE lrdt.LeaveType IN (@AnnualLeaveType, @UnpaidLeaveType, @KeepInViewLeaveType, @PublicHolidayLeaveType)
					AND lrdt.RequestId = lv.RequestId
				GROUP BY RequestId
				)
			AND DATEDIFF(DAY, wk.CreatedDate, lv.FromDate) > 0
			AND DATEDIFF(dd, wk.CreatedDate, lv.FromDate) < 7
		) AS lea ON empca.EmpId = lea.EmpId
		AND DATEDIFF(dd, empca.WorkDate, lea.OnDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1

	IF @LeaveType = @GeneralLeaveType
		OR @TotalDays = 0.5
	BEGIN
		INSERT INTO @DisciplineDetail (
			WorkDate
			,IsOffOrHoliday
			,ScheduleStatus
			,IsUnpaidLeaveAbsent
			,IsUnpaidLeave
			,IsAbsent
			,IsUnplannedLeave
			,IsWeekendLeave
			,IsSevenDaysLeave
			)
		SELECT ldt.OnDate AS WorkDate
			,(CASE 
				WHEN ca.IsOffDay = 1
					OR ca.IsHoliday = 1 THEN 1
				ELSE 0 END
				) AS IsOffOrHoliDay
			,(CASE 
				WHEN ca.IsOffDay = 1 THEN 'Off Day'
				WHEN ca.IsHoliday = 1 THEN 'Holiday'
				WHEN s.ShiftID IS NOT NULL THEN s.[Name]
				WHEN ca.IsLeaveDay = 0 THEN 'N/A' END
				) AS ScheduleStatus
			,(CASE 
				WHEN upla.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsUnpaidLeaveAbsent
			,(CASE 
				WHEN upla.WorkDate IS NULL
					AND sevendays.WorkDate IS NULL
					AND lateleave.WorkDate IS NULL
					AND upl.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsUnpaidLeave
			,(CASE 
				WHEN upla.WorkDate IS NULL
					AND ab.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsAbsent
			,(CASE 
				WHEN lateleave.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsUnplannedLeave
			,(CASE 
				WHEN wkl.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsWeekendLeave
			,(CASE 
				WHEN lateleave.WorkDate IS NULL
					AND sevendays.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsSevenDaysLeave
		FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wk.RequestId = lr.RequestId
			INNER JOIN (
			SELECT DISTINCT RequestId
				,OnDate
			FROM dbo.LeaveRequestDetail WITH (NOLOCK)
			WHERE RequestId = @RequestId
			) AS ldt ON lr.RequestId = ldt.RequestId
			INNER JOIN dbo.EmployeeCalendar AS ca WITH (NOLOCK) ON CAST(ldt.OnDate AS DATE) = CAST(ca.WorkDate AS DATE)
			AND lr.EmpId = ca.EmpId
			LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON ca.ShiftId = s.ShiftID
			LEFT JOIN @tblUnpaidLeaveAbsent AS upla ON lr.EmpId = upla.EmpId
			AND CAST(ldt.OnDate AS DATE) = upla.WorkDate
			LEFT JOIN @tblUnpaidLeave AS upl ON lr.EmpId = upl.EmpId
			AND CAST(ldt.OnDate AS DATE) = upl.WorkDate
			LEFT JOIN @tblAbsent AS ab ON lr.EmpId = ab.EmpId
			AND CAST(ldt.OnDate AS DATE) = ab.WorkDate
			LEFT JOIN @tblUnplannedLeave AS lateleave ON lr.EmpId = lateleave.EmpId
			AND CAST(ldt.OnDate AS DATE) = lateleave.WorkDate
			LEFT JOIN @tblWeekendLeave AS wkl ON lr.EmpId = wkl.EmpId
			AND CAST(ldt.OnDate AS DATE) = wkl.WorkDate
			LEFT JOIN @tblSevenDaysLeave AS sevendays ON lr.EmpId = sevendays.EmpId
			AND CAST(ldt.OnDate AS DATE) = sevendays.WorkDate
		WHERE wk.RequestId = @RequestId
			AND EXISTS (
				SELECT 1
				FROM @tblUnpaidLeaveAbsent
				
				UNION
				SELECT 1
				FROM @tblUnpaidLeave
				
				UNION
				SELECT 1
				FROM @tblAbsent
				
				UNION
				SELECT 1
				FROM @tblUnplannedLeave
				
				UNION
				SELECT 1
				FROM @tblUnpaidLeave
				
				UNION
				SELECT 1
				FROM @tblWeekendLeave

				UNION
				SELECT 1
				FROM @tblSevenDaysLeave
				)
		ORDER BY ldt.OnDate
	END
	ELSE IF @LeaveType = @AnnualLeaveType
		OR @LeaveType = @KeepInViewLeaveType
		OR @LeaveType = @UnpaidLeaveType
		OR @LeaveType = @PublicHolidayLeaveType
	BEGIN
		INSERT INTO @DisciplineDetail (
			WorkDate
			,IsOffOrHoliday
			,ScheduleStatus
			,IsUnpaidLeaveAbsent
			,IsUnpaidLeave
			,IsAbsent
			,IsUnplannedLeave
			,IsWeekendLeave
			,IsSevenDaysLeave
			)
		SELECT ca.WorkDate AS WorkDate
			,(CASE 
				WHEN ca.IsOffDay = 1
					OR ca.IsHoliday = 1 THEN 1
				ELSE 0 END
				) AS IsOffOrHoliDay
			,(CASE 
				WHEN ca.IsOffDay = 1 THEN 'Off Day'
				WHEN ca.IsHoliday = 1 THEN 'Holiday'
				WHEN s.ShiftID IS NOT NULL THEN s.[Name]
				WHEN ca.IsLeaveDay = 0 THEN 'N/A' END
				) AS ScheduleStatus
			,(CASE 
				WHEN upla.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsUnpaidLeaveAbsent
			,(CASE 
				WHEN upla.WorkDate IS NULL
					AND sevendays.WorkDate IS NULL
					AND upl.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsUnpaidLeave
			,(CASE 
				WHEN ab.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsAbsent
			,(CASE 
				WHEN lateleave.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsUnplannedLeave
			,(CASE 
				WHEN wkl.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsWeekendLeave
			,(CASE 
				WHEN upla.WorkDate IS NULL
					AND sevendays.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsSevenDaysLeave
		FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wk.RequestId = lr.RequestId
			INNER JOIN dbo.EmployeeCalendar AS ca WITH (NOLOCK) ON lr.EmpId = ca.EmpId
			AND ca.WorkDate BETWEEN lr.FromDate
				AND lr.ToDate
			LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON ca.ShiftId = s.ShiftID
			LEFT JOIN @tblUnpaidLeaveAbsent AS upla ON ca.EmpId = upla.EmpId
			AND CAST(ca.WorkDate AS DATE) = upla.WorkDate
			LEFT JOIN @tblUnpaidLeave AS upl ON ca.EmpId = upl.EmpId
			AND CAST(ca.WorkDate AS DATE) = upl.WorkDate
			LEFT JOIN @tblAbsent AS ab ON ca.EmpId = ab.EmpId
			AND CAST(ca.WorkDate AS DATE) = ab.WorkDate
			LEFT JOIN @tblUnplannedLeave AS lateleave ON ca.EmpId = lateleave.EmpId
			AND CAST(ca.WorkDate AS DATE) = lateleave.WorkDate
			LEFT JOIN @tblWeekendLeave AS wkl ON ca.EmpId = wkl.EmpId
			AND CAST(ca.WorkDate AS DATE) = wkl.WorkDate
			LEFT JOIN @tblSevenDaysLeave AS sevendays ON ca.EmpId = sevendays.EmpId
			AND CAST(ca.WorkDate AS DATE) = sevendays.WorkDate
		WHERE wk.RequestId = @RequestId
			AND EXISTS (
				SELECT 1
				FROM @tblUnpaidLeaveAbsent
				
				UNION
				SELECT 1
				FROM @tblUnpaidLeave
				
				UNION
				SELECT 1
				FROM @tblAbsent
				
				UNION
				SELECT 1
				FROM @tblUnplannedLeave
				
				UNION
				SELECT 1
				FROM @tblUnpaidLeave
				
				UNION
				SELECT 1
				FROM @tblWeekendLeave

				UNION
				SELECT 1
				FROM @tblSevenDaysLeave
				)
	END

	UPDATE @tblUnpaidLeave
	SET WorkDate = GETDATE()

	RETURN
END