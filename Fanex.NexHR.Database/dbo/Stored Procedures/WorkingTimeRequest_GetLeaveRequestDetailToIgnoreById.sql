CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetLeaveRequestDetailToIgnoreById] @RequestId BIGINT
AS
/*
Created: 20160511@Tim.Trinh
Task: get leave request detail
DB: SMSHRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @LeaveType INT

	SELECT @LeaveType = lvr.LeaveType
	FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest AS lvr WITH (NOLOCK) ON wkr.RequestId = lvr.RequestId
	WHERE wkr.RequestId = @RequestId

	CREATE TABLE #LeaveRequestDetail (
		LeaveDate DATE
		,LeaveTypeId INT
		,LeaveTypeName NVARCHAR(200)
		,IsHalfDay BIT
		,FromTime DATETIME
		,ToTime DATETIME
		,IsOffOrHoliday BIT
		,ScheduleStatus NVARCHAR(MAX)
		,IsFromAfterday BIT
		,IsToAfterday BIT
		,IsUnpaidLeaveAbsent BIT
		,IsAbsent BIT
		)

	CREATE TABLE #Absent (
		WorkDate DATE
		,EmpId INT
		)

	CREATE TABLE #UnpaidLeaveAbsent (
		WorkDate DATE
		,EmpId INT
		)

	CREATE TABLE #UnpaidLeave (
		WorkDate DATE
		,EmpId INT
		)

	INSERT INTO #UnpaidLeaveAbsent (
		WorkDate
		,EmpId
		)
	SELECT empC.WorkDate
		,empC.EmpId
	FROM dbo.EmployeeCalendar AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
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
			AND lv.LeaveType = 25 -- unpaid leave
			AND wk.[CurrentStatus] IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND DATEDIFF(dd, lv.FromDate, wk.[CreatedDate]) >= 0
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND DATEDIFF(dd, empC.WorkDate, lea.FromDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1
	
	UNION ALL
	SELECT empC.WorkDate
		,empC.[EmpId]
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
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
			AND lv.LeaveType = 0 -- general leave
			AND lvt.LeaveType = 25 -- unpaid leave
			AND wk.[CurrentStatus] IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND lvt.OnDate = (
				SELECT MIN(OnDate)
				FROM dbo.LeaveRequestDetail
				WHERE LeaveType = 25
					AND RequestId = lv.RequestId
				GROUP BY RequestId
				)
			AND DATEDIFF(dd, lv.FromDate, wk.[CreatedDate]) >= 0
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND DATEDIFF(dd, empC.WorkDate, lea.OnDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1

	INSERT INTO #Absent (
		WorkDate
		,EmpId
		)
	SELECT empC.WorkDate
		,empC.[EmpId]
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
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
			AND lv.LeaveType = 2 -- annual leave
			AND wk.[CurrentStatus] IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND DATEDIFF(dd, lv.FromDate, wk.[CreatedDate]) >= 0
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND DATEDIFF(dd, empC.WorkDate, lea.FromDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1
	
	UNION ALL
	SELECT empC.WorkDate
		,empC.[EmpId]
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 5
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lvt.OnDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
			INNER JOIN dbo.LeaveRequestDetail lvt WITH (NOLOCK) ON lv.RequestId = lvt.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType = 0 -- general leave
			AND lvt.LeaveType = 2 -- annual leave
			AND wk.[CurrentStatus] IN (1, 2, 4, 6) -- pending/verified/approved/processed
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.LeaveRequestDetail WITH (NOLOCK)
				WHERE RequestId = lv.RequestId
					AND LeaveType = 25 -- unpaid leave
				)
			AND lvt.OnDate = (
				SELECT MIN(OnDate)
				FROM dbo.LeaveRequestDetail
				WHERE LeaveType = 2 -- annual leave
					AND RequestId = lv.RequestId
				GROUP BY RequestId
				)
			AND DATEDIFF(dd, lv.FromDate, wk.[CreatedDate]) >= 0
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND DATEDIFF(dd, empC.WorkDate, lea.OnDate) = 0
	WHERE ISNULL(rul.Value, 0) = 1

	INSERT INTO #UnpaidLeave (
		WorkDate
		,EmpId
		)
	SELECT empC.WorkDate
		,empC.[EmpId]
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
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
			AND lv.LeaveType = 25 -- unpaid leave
			AND wk.[CurrentStatus] IN (1, 2, 4, 6) -- pending/verified/approved/processed
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND CAST(empC.[WorkDate] AS DATE) >= CAST(lea.[FromDate] AS DATE)
		AND CAST(empC.[WorkDate] AS DATE) <= CAST(lea.[ToDate] AS DATE)
	WHERE ISNULL(rul.Value, 0) = 1
	
	UNION ALL
	SELECT empC.WorkDate
		,empC.[EmpId]
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON empC.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.DisciplineSectionRule AS rul WITH (NOLOCK) ON e.GroupId = rul.GroupId
		AND es.IsLocal = rul.IsLocal
		AND rul.DisciplineTypeId = 6
		INNER JOIN (
		SELECT lv.RequestId
			,lv.EmpId
			,lvt.OnDate
		FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
			INNER JOIN dbo.LeaveRequestDetail lvt WITH (NOLOCK) ON lv.RequestId = lvt.RequestId
		WHERE wk.RequestId = @RequestId
			AND lv.LeaveType = 0 -- general leave
			AND lvt.LeaveType = 25 -- unpaid leave
			AND wk.[CurrentStatus] IN (1, 2, 4, 6) -- pending/verified/approved/processed
		) AS lea ON empC.[EmpId] = lea.[EmpId]
		AND empC.[WorkDate] = lea.OnDate
	WHERE ISNULL(rul.Value, 0) = 1

	IF @LeaveType = 0 -- General Leave
	BEGIN
		INSERT INTO #LeaveRequestDetail (
			LeaveDate
			,LeaveTypeId
			,LeaveTypeName
			,IsHalfDay
			,FromTime
			,ToTime
			,IsOffOrHoliday
			,ScheduleStatus
			,IsFromAfterday
			,IsToAfterday
			,IsUnpaidLeaveAbsent
			,IsAbsent
			)
		SELECT ldt.OnDate AS LeaveDate
			,lt.LeaveTypeId
			,lt.[Name] AS LeaveTypeName
			,ldt.IsHalfDay
			,ldt.FromTime
			,ldt.ToTime
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
				WHEN CAST(ldt.FromTime AS DATE) > CAST(ldt.OnDate AS DATE) THEN 1
				ELSE 0 END
				) AS IsFromAfterday
			,(CASE 
				WHEN CAST(ldt.ToTime AS DATE) > CAST(ldt.OnDate AS DATE) THEN 1
				ELSE 0 END
				) AS IsToAfterday
			,(CASE 
				WHEN upa.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsUnpaidLeaveAbsent
			,(CASE 
				WHEN upa.WorkDate IS NULL
					AND ab.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsAbsent
		FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wk.RequestId = lr.RequestId
			INNER JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
			INNER JOIN dbo.EmployeeCalendar AS ca WITH (NOLOCK) ON CAST(ldt.OnDate AS DATE) = CAST(ca.WorkDate AS DATE)
			AND ldt.EmpId = ca.EmpId
			LEFT JOIN dbo.Shift AS s WITH (NOLOCK) ON ca.ShiftId = s.ShiftID
			LEFT JOIN dbo.LeaveType AS lt WITH (NOLOCK) ON lt.LeaveTypeId = ldt.LeaveType
			LEFT JOIN #UnpaidLeaveAbsent AS upa ON ldt.EmpId = upa.EmpId
			AND CAST(ldt.OnDate AS DATE) = upa.WorkDate
			LEFT JOIN #Absent AS ab ON ldt.EmpId = ab.EmpId
			AND CAST(ldt.OnDate AS DATE) = ab.WorkDate
			LEFT JOIN #UnpaidLeave AS up ON ldt.EmpId = up.EmpId
			AND CAST(ldt.OnDate AS DATE) = up.WorkDate
		WHERE wk.RequestId = @RequestId
			AND EXISTS (
				SELECT 1
				FROM #UnpaidLeaveAbsent
				
				UNION
				SELECT 1
				FROM #Absent
				
				UNION
				SELECT 1
				FROM #UnpaidLeave
				)
		ORDER BY ldt.OnDate
	END
	ELSE IF @LeaveType = 2 -- Annual Leave
	BEGIN
		INSERT INTO #LeaveRequestDetail (
			LeaveDate
			,LeaveTypeId
			,LeaveTypeName
			,IsHalfDay
			,FromTime
			,ToTime
			,IsOffOrHoliday
			,ScheduleStatus
			,IsFromAfterday
			,IsToAfterday
			,IsUnpaidLeaveAbsent
			,IsAbsent
			)
		SELECT ab.WorkDate AS LeaveDate
			,lt.LeaveTypeId
			,lt.[Name] AS LeaveTypeName
			,(CASE 
				WHEN lr.TotalDays = 0.5
					AND lr.LeaveType <> 0 THEN 1
				ELSE 0 END
				) AS IsHalfDay
			,(CASE 
				WHEN ldt.FromTime IS NULL THEN lr.FromDate
				ELSE ldt.FromTime END
				) AS FromTime
			,(CASE 
				WHEN ldt.ToTime IS NULL THEN lr.ToDate
				ELSE ldt.ToTime END
				) AS ToTime
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
			,0 AS IsFromAfterday
			,0 AS IsToAfterday
			,0 AS IsUnpaidLeaveAbsent
			,(CASE 
				WHEN ab.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsAbsent
		FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wk.RequestId = lr.RequestId
			INNER JOIN dbo.EmployeeCalendar AS ca WITH (NOLOCK) ON lr.EmpId = ca.EmpId
			LEFT JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
			LEFT JOIN dbo.Shift AS s WITH (NOLOCK) ON ca.ShiftId = s.ShiftID
			LEFT JOIN dbo.LeaveType AS lt WITH (NOLOCK) ON lt.LeaveTypeId = lr.LeaveType
			INNER JOIN #Absent AS ab ON ca.EmpId = ab.EmpId
			AND CAST(ca.WorkDate AS DATE) = ab.WorkDate
		WHERE wk.RequestId = @RequestId
	END
	ELSE IF @LeaveType = 25 -- unpaid leave
	BEGIN
		INSERT INTO #LeaveRequestDetail (
			LeaveDate
			,LeaveTypeId
			,LeaveTypeName
			,IsHalfDay
			,FromTime
			,ToTime
			,IsOffOrHoliday
			,ScheduleStatus
			,IsFromAfterday
			,IsToAfterday
			,IsUnpaidLeaveAbsent
			,IsAbsent
			)
		SELECT (CASE 
				WHEN upa.WorkDate IS NOT NULL THEN upa.WorkDate
				ELSE up.WorkDate END
				) AS LeaveDate
			,lt.LeaveTypeId
			,lt.[Name] AS LeaveTypeName
			,(CASE 
				WHEN lr.TotalDays = 0.5
					AND lr.LeaveType <> 0 THEN 1
				ELSE 0 END
				) AS IsHalfDay
			,lr.FromDate AS FromTime
			,lr.ToDate AS ToTime
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
				WHEN CAST(lr.FromDate AS DATE) > CAST(upa.WorkDate AS DATE) THEN 1
				ELSE 0 END
				) AS IsFromAfterday
			,(CASE 
				WHEN CAST(lr.ToDate AS DATE) > CAST(upa.WorkDate AS DATE) THEN 1
				ELSE 0 END
				) AS IsToAfterday
			,(CASE 
				WHEN upa.WorkDate IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsUnpaidLeaveAbsent
			,0 AS IsAbsent
		FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wk.RequestId = lr.RequestId
			INNER JOIN dbo.EmployeeCalendar AS ca WITH (NOLOCK) ON lr.EmpId = ca.EmpId
			LEFT JOIN #UnpaidLeaveAbsent AS upa ON ca.EmpId = upa.EmpId
			AND upa.WorkDate = CAST(ca.WorkDate AS DATE)
			LEFT JOIN #UnpaidLeave AS up ON ca.EmpId = up.EmpId
			AND up.WorkDate = CAST(ca.WorkDate AS DATE)
			LEFT JOIN dbo.Shift AS s WITH (NOLOCK) ON ca.ShiftId = s.ShiftID
			LEFT JOIN dbo.LeaveType AS lt WITH (NOLOCK) ON lt.LeaveTypeId = lr.LeaveType
		WHERE wk.RequestId = @RequestId
			AND (
				upa.WorkDate IS NOT NULL
				OR up.WorkDate IS NOT NULL
				)
	END
	ELSE
	BEGIN
		INSERT INTO #LeaveRequestDetail (
			LeaveDate
			,LeaveTypeId
			,LeaveTypeName
			,IsHalfDay
			,FromTime
			,ToTime
			,IsOffOrHoliday
			,ScheduleStatus
			,IsFromAfterday
			,IsToAfterday
			,IsUnpaidLeaveAbsent
			,IsAbsent
			)
		SELECT NULL AS LeaveDate
			,NULL AS LeaveTypeId
			,NULL AS LeaveTypeName
			,0 AS IsHalfDay
			,NULL AS FromTime
			,NULL AS ToTime
			,0 AS IsOffOrHoliday
			,NULL AS ScheduleStatus
			,0 AS IsFromAfterday
			,0 AS IsToAfterday
			,0 AS IsUnpaidLeaveAbsent
			,0 AS IsAbsent
		WHERE 0 = 1
	END

	SELECT *
	FROM #LeaveRequestDetail
	ORDER BY LeaveDate

	DROP TABLE #Absent

	DROP TABLE #UnpaidLeaveAbsent

	DROP TABLE #UnpaidLeave

	DROP TABLE #LeaveRequestDetail
END