
CREATE PROCEDURE [dbo].[EmployeeAttendance_GetByType] @fromtime DATE
	,@totime DATE
	,@EmpId INT = NULL
	,@GroupIds VARCHAR(MAX) = ''
	,@excludeUserDelete INT = 1
	,@isLocal BIT = NULL
	,@SupervisorId INT = NULL
	,@type SMALLINT
	,@isRealDate BIT = 0
AS
/*
Created: Tom.Vo@2013-10-31
Task: get attentance list of employees by type
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
	@type: 0-all, 1-missing checkin; 2-missing check out, 3: latein; 4-earlyout, 5- not work
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @today DATETIME
		,@now DATETIME
		,@StartDate DATETIME
		,@EndDate DATETIME
		-- request type
		,@LateInRequestTypeId TINYINT = 3
		,@EarlyOutRequestTypeId TINYINT = 4
		,@ChangeShiftRequestTypeId TINYINT = 5
		,@LeaveRequestTypeId TINYINT = 6
		,@KeepInViewRequestTypeId TINYINT = 7
		-- request status
		,@NotVerifiedStatusId TINYINT = 3
		,@RejectedStatusId TINYINT = 5
		,@VoidStatusId TINYINT = 7
	DECLARE @tblGroupId TABLE (GroupId INT)
	DECLARE @tblEmployeeId TABLE (EmpId INT)

	IF(@isRealDate = 1)
	BEGIN
		SET @fromtime = DATEADD(d, -1, @fromtime)
	END

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO @tblGroupId (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	END
	ELSE
	BEGIN
		INSERT INTO @tblGroupId (GroupId)
		SELECT GroupId
		FROM dbo.[Group] WITH (NOLOCK)
		WHERE ISNULL(IsDeleted, 0) = 0
	END

	IF @EmpId IS NULL
	BEGIN
		INSERT INTO @tblEmployeeId (EmpId)
		SELECT DISTINCT EmpId
		FROM dbo.fnc_GetEmployeeByActorId(@SupervisorId, @excludeUserDelete, 1)
	END
	ELSE
	BEGIN
		INSERT INTO @tblEmployeeId (EmpId)
		VALUES (@EmpId)
	END

	SET @now = dateadd(hh, 7, GETUTCDATE())
	SET @today = convert(VARCHAR(10), @now, 111) -- use UTC because the server use GMT-4

	CREATE TABLE #tblDailyRequest (
		RequestId BIGINT
		,EmpId INT
		,RequestType INT
		,InOutMins INT
		,RequestDate DATETIME
		,FromDate DATETIME
		,ToDate DATETIME
		,FromTime DATETIME
		,ToTime DATETIME
		,FromShift INT
		,ToShift INT
		,TotalDays NUMERIC(18, 2)
		,CurrentStatus TINYINT
		,KeepInViewTypeId TINYINT
		)

	-- check in/out
	INSERT INTO #tblDailyRequest (
		RequestDate
		,RequestId
		,EmpId
		,RequestType
		,FromDate
		,ToDate
		,TotalDays
		,CurrentStatus
		)
	SELECT r.CreatedDate
		,r.RequestId
		,r.EmpId
		,r.RequestType
		,(CASE 
			WHEN DATEDIFF(d, cio.OnDate, cio.RequestTime) = 0 THEN cio.RequestTime
			ELSE DATEADD(d, - 1, cio.RequestTime) END
			)
		,(CASE 
			WHEN DATEDIFF(d, cio.OnDate, cio.RequestTime) = 0 THEN cio.RequestTime
			ELSE DATEADD(d, - 1, cio.RequestTime) END
			)
		,0
		,r.CurrentStatus
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.CheckInOutRequest AS cio WITH (NOLOCK) ON r.RequestId = cio.RequestId
		AND cio.OnDate BETWEEN @fromtime
			AND @totime
		AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
	WHERE EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS emp
			WHERE r.EmpId = emp.EmpId
			)

	-- late in/early out
	INSERT INTO #tblDailyRequest (
		RequestDate
		,RequestId
		,EmpId
		,RequestType
		,FromDate
		,ToDate
		,TotalDays
		,InOutMins
		,CurrentStatus
		)
	SELECT r.CreatedDate
		,r.RequestId
		,r.EmpId
		,r.RequestType
		,lieo.OnDate
		,lieo.OnDate
		,0
		,(CASE 
			WHEN r.RequestType = @LateInRequestTypeId THEN lieo.LateInMins
			ELSE lieo.EarlyOutMins END
			)
		,r.CurrentStatus
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.LateInEarlyOutRequest AS lieo WITH (NOLOCK) ON r.RequestId = lieo.RequestId
		AND lieo.OnDate BETWEEN @fromtime
			AND @totime
		AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
	WHERE EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS emp
			WHERE r.EmpId = emp.EmpId
			)

	-- keep in view
	INSERT INTO #tblDailyRequest (
		RequestDate
		,RequestId
		,EmpId
		,RequestType
		,FromDate
		,ToDate
		,FromTime
		,ToTime
		,TotalDays
		,CurrentStatus
		,KeepInViewTypeId
		)
	SELECT r.CreatedDate RequestDate
		,r.RequestId
		,r.EmpId
		,r.RequestType
		,kiv.OnDate
		,kiv.OnDate
		,kiv.FromTime
		,kiv.ToTime
		,(CASE 
			WHEN kiv.IsHalfDay = 1 THEN 0.5
			ELSE 1 END
			)
		,r.CurrentStatus
		,kiv.KeepInViewType
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON r.RequestId = kiv.RequestId
		AND kiv.OnDate BETWEEN @fromtime
			AND @totime
		AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
	WHERE EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS emp
			WHERE r.EmpId = emp.EmpId
			)

	-- Leave Request <> 0.5
	INSERT INTO #tblDailyRequest (
		RequestDate
		,RequestId
		,EmpId
		,RequestType
		,FromDate
		,ToDate
		,TotalDays
		,CurrentStatus
		)
	SELECT r.CreatedDate
		,r.RequestId
		,r.EmpId
		,r.RequestType
		,lr.FromDate
		,lr.ToDate
		,lr.TotalDays
		,CurrentStatus
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
		AND lr.TotalDays <> 0.5
		AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
	WHERE (
			(
				lr.FromDate BETWEEN @fromtime
					AND @totime
				AND lr.ToDate >= @totime
				)
			OR (
				lr.FromDate <= @fromtime
				AND lr.ToDate >= @totime
				)
			OR lr.ToDate BETWEEN @fromtime
				AND @totime
			)
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS emp
			WHERE r.EmpId = emp.EmpId
			)

	-- Leave Request = 0.5
	INSERT INTO #tblDailyRequest (
		RequestDate
		,RequestId
		,EmpId
		,RequestType
		,FromDate
		,ToDate
		,TotalDays
		,FromTime
		,ToTime
		,CurrentStatus
		)
	SELECT r.CreatedDate
		,r.RequestId
		,r.EmpId
		,r.RequestType
		,lr.FromDate
		,lr.ToDate
		,lr.TotalDays
		,(CASE 
			WHEN ldt.FromTime IS NULL THEN lr.FromDate
			ELSE ldt.FromTime END
			)
		,(CASE 
			WHEN ldt.ToTime IS NULL THEN lr.ToDate
			ELSE ldt.ToTime END
			)
		,r.CurrentStatus
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN [dbo].LeaveRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
		AND lr.TotalDays = 0.5
		AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		LEFT JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
	WHERE (
			(
				datediff(d, @fromtime, lr.FromDate) <= 0
				AND datediff(d, @totime, lr.ToDate) >= 0
				)
			OR (
				datediff(d, @fromtime, lr.FromDate) >= 0
				AND datediff(d, @totime, lr.FromDate) <= 0
				)
			OR (
				datediff(d, @fromtime, lr.ToDate) >= 0
				AND datediff(d, @totime, lr.ToDate) <= 0
				)
			)
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS emp
			WHERE r.EmpId = emp.EmpId
			)

	-- change shift
	INSERT INTO #tblDailyRequest (
		RequestDate
		,RequestId
		,empid
		,RequestType
		,FromDate
		,ToDate
		,TotalDays
		,FromShift
		,ToShift
		,CurrentStatus
		)
	SELECT r.CreatedDate
		,r.RequestId
		,r.EmpId
		,r.RequestType
		,cs.FromDate
		,cs.ToDate
		,COUNT(1) TotalDays
		,MAX(csd.FromShift) FromShift
		,MAX(csd.ToShift) ToShift
		,r.CurrentStatus
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON r.RequestId = cs.RequestId
		AND r.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		INNER JOIN dbo.ChangeShiftRequestDetail AS csd WITH (NOLOCK) ON csd.RequestId = cs.RequestId
	WHERE (
			(
				cs.FromDate BETWEEN @fromtime
					AND @totime
				AND cs.ToDate >= @totime
				)
			OR (
				cs.FromDate <= @fromtime
				AND cs.ToDate >= @totime
				)
			OR cs.ToDate BETWEEN @fromtime
				AND @totime
			)
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS emp
			WHERE r.EmpId = emp.EmpId
			)
	GROUP BY r.CreatedDate
		,r.RequestId
		,r.EmpId
		,r.RequestType
		,cs.FromDate
		,cs.ToDate
		,r.CurrentStatus

	CREATE TABLE #CalendarDF (AttendanceDate DATETIME)

	IF (@isRealDate = 1)
	BEGIN
		SET @fromtime = DATEADD(d, 1, @fromtime)
	END

	SET @StartDate = @fromtime
	SET @EndDate = @totime

	WHILE DATEPART(dw, @StartDate) <> 2
	BEGIN
		SET @StartDate = DATEADD(d, - 1, @StartDate)
	END

	WHILE DATEPART(dw, @EndDate) <> 1
	BEGIN
		SET @EndDate = DATEADD(d, 1, @EndDate)
	END

	DECLARE @OutOffDay TABLE (
		WorkDate DATETIME
		,EmpId INT
		)
	DECLARE @StartDate_tmp DATETIME
		,@EndDate_tmp DATETIME

	WHILE @StartDate <= @EndDate
	BEGIN
		SET @StartDate_tmp = @StartDate
		SET @EndDate_tmp = DATEADD(d, 6, @StartDate)
		SET @StartDate = DATEADD(d, 7, @StartDate)

		INSERT INTO @OutOffDay (
			WorkDate
			,EmpId
			)
		SELECT WorkDate
			,cl.EmpId
		FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
			INNER JOIN (
			SELECT c.EmpId
			FROM dbo.EmployeeCalendar AS c WITH (NOLOCK)
				INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON c.EmpId = es.EmpId
			WHERE WorkDate BETWEEN @StartDate_tmp
					AND @EndDate_tmp
				AND c.IsOffDay = 1
				AND (
					@EmpId IS NULL
					OR c.EmpId = @EmpId
					)
			GROUP BY c.EmpId
				,es.OffDaysbyWeek
			HAVING COUNT(c.EmpId) > es.OffDaysbyWeek
			) AS a ON cl.EmpId = a.EmpId
		WHERE WorkDate BETWEEN @StartDate_tmp
				AND @EndDate_tmp
			AND cl.IsOffDay = 1
			AND (
				@EmpId IS NULL
				OR cl.EmpId = @EmpId
				)
	END

	DECLARE @fromtime_tmp DATETIME = @fromtime

	WHILE @fromtime_tmp <= @totime
	BEGIN
		INSERT INTO #CalendarDF (AttendanceDate)
		VALUES (@fromtime_tmp)

		SET @fromtime_tmp = DATEADD(DAY, 1, @fromtime_tmp)
	END

	SELECT T.Code
		,T.EmpId
		,T.FullName
		,T.NickName
		,T.[Team]
		,T.[TeamCode]
		,T.[Rank]
		,T.AttendanceDate
		,T.ShiftId
		,COALESCE(T.ShiftCode, CASE 
				WHEN ISNULL(T.IsHoliday, 0) = 0
					AND T.ShiftId IS NULL
					AND isOffday = 1 THEN 'Off day'
				WHEN ISNULL(T.IsHoliday, 0) = 1
					AND (
						T.InFixed IS NULL
						AND T.OutFixed IS NULL
						) THEN 'PH'
				ELSE ''
				END, '') ShiftCode
		,T.ExpectedInTime
		,T.ExpectedOutTime
		,CASE 
			WHEN T.IsShiftCrossDay = 1
				AND ISNULL(T.IsHoliday, 0) = 0
				AND T.ShiftId IS NULL
				AND isOffday = 1 THEN NULL
			ELSE T.InFixed
			END InFixed
		,CASE 
			WHEN T.IsShiftCrossDay = 1
				AND ISNULL(T.IsHoliday, 0) = 0
				AND T.ShiftId IS NULL
				AND isOffday = 1 THEN NULL
			ELSE T.OutFixed
			END OutFixed
		,T.LateIn
		,T.EarlyOut
		,CASE 
			WHEN T.IsShiftCrossDay = 1
				AND ISNULL(T.IsHoliday, 0) = 0
				AND T.ShiftId IS NULL
				AND isOffday = 1 THEN NULL
			ELSE T.TotalWorkHours
			END TotalWorkHours
		,T.Absence
		,T.isNotWork
		,T.isOffday
		,(CASE 
			WHEN h.HolidayName IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsPublicHoliday
		,(CASE 
			WHEN (
					(
						od.EmpId IS NOT NULL
						OR (
							AttendanceDate1 IS NOT NULL
							AND isMissingCheckin = 1
							)
						OR (
							AttendanceDate1 IS NOT NULL
							AND isMissingCheckOut = 1
							)
						)
					AND CONVERT(NVARCHAR(20), T.AttendanceDate1, 111) < CONVERT(NVARCHAR(20), GETDATE(), 111)
					) THEN 1
			ELSE 0 END
			) AS IsWarning
		,T.IsOvertime
		,T.OvertimeHours
		,T.OvertimeStatus
		,ISNULL(stuff((CASE 
					WHEN AttendanceDate1 IS NOT NULL
						AND (
							isLateIn = 1
							AND LateIn > 0
							) THEN (', Late In')
					ELSE '' END
					) + (CASE 
					WHEN AttendanceDate1 IS NOT NULL
						AND (
							isEarlyOut = 1
							AND EarlyOut > 0
							) THEN ', Early Out'
					ELSE '' END
					) + (CASE 
					WHEN AttendanceDate1 IS NOT NULL
						AND isMissingCheckin = 1 THEN ', Missing Check-in'
					ELSE '' END
					) + (CASE 
					WHEN AttendanceDate1 IS NOT NULL
						AND isMissingCheckOut = 1
						AND CONVERT(NVARCHAR(20), T.AttendanceDate1, 111) < CONVERT(NVARCHAR(20), GETDATE(), 111) THEN ', Missing Check-out'
					ELSE '' END
					) + (CASE 
					WHEN AttendanceDate1 IS NOT NULL
						AND IsKeepInView = 1 THEN ', KIV day' + CASE 
							WHEN LeaveDay = 0.5 THEN ' (Half day)'
							ELSE ''
							END
					ELSE '' END
					) + (CASE 
					WHEN ISNULL(T.IsHoliday, 0) = 0
						AND isOffday = 1
						AND ShiftId IS NULL THEN (CASE 
							WHEN od.EmpId IS NULL THEN ', Off day'
							ELSE ', Off day no match' END
							)
					ELSE '' END
					) + (CASE 
					WHEN ISNULL(T.IsHoliday, 0) = 0
						AND isOffday = 1
						AND ShiftId IS NOT NULL
						AND od.EmpId IS NOT NULL THEN ', Off day no match'
					ELSE '' END
					) + (CASE 
					WHEN ISNULL(T.IsHoliday, 0) = 1
						AND ShiftId IS NULL THEN ', PH'
					ELSE '' END
					) + (CASE 
					WHEN isNotWork = 1
						AND r.LeaveType IS NULL THEN ', Absent'
					ELSE (
							(CASE 
								WHEN lv.[Name] IS NULL THEN ''
								ELSE ', ' + lv.[Name] + (CASE 
										WHEN lv2.[Name] IS NULL THEN ''
										ELSE ' & ' + lv2.[Name] END
										) + (CASE 
										WHEN LeaveDay = 0.5 THEN ' (Half day)'
										ELSE '' END
										) END
								) + CASE 
							WHEN LeaveDay = 0.5
								AND isNotWork = 1 THEN ', Absent'
							ELSE '' END
							) END
					), 1, 2, ''), CASE 
				WHEN AttendanceDate1 IS NULL THEN 'No Data'
				ELSE NULL
				END) AS Notes
		,r.RequestType
		,r.RequestDate AS CreatedDate
		,r.FromDate
		,r.ToDate
		,r.TotalDays
		,r.Remark
		,r.LeaveTypeId
		,r.LeaveType
		,r.FromShift
		,r.ToShift
		,r.[Time]
		,r.FromTime
		,r.ToTime
		,r.CurrentStatus
		,r.KeepInViewTypeId
	FROM (
		SELECT e.Code
			,e.EmpId
			,FullName
			,NickName
			,g.[Name] [Team]
			,g.Code [TeamCode]
			,g.[Rank]
			,tbldate.AttendanceDate
			,a.AttendanceDate AS AttendanceDate1
			,c.ShiftId
			,s.code AS ShiftCode
			,s.BeginCheckInTime AS ExpectedInTime
			,s.BeginCheckOutTime AS ExpectedOutTime
			,a.InFixed
			,a.OutFixed
			,a.LateIn
			,a.EarlyOut
			,TotalWorkHours
			,Absence
			,(CASE 
				WHEN isnull(Absence, 0) = 0
					AND isnull(a.LateIn, 0) > 0 THEN 1
				ELSE 0 END
				) AS isLateIn
			,(CASE 
				WHEN isnull(Absence, 0) = 0
					AND isnull(a.EarlyOut, 0) > 15 THEN 1
				ELSE 0 END
				) AS isEarlyOut
			,(CASE 
				WHEN (
						isnull(Absence, 0) = 0
						AND InFixed IS NULL
						AND @now > (
							(CASE 
								WHEN s.IsInAfterDay = 1 THEN DATEADD(DD, 1, a.AttendanceDate)
								ELSE a.AttendanceDate END
								) + ' ' + s.EndCheckInTime
							)
						) THEN 1
				ELSE 0 END
				) AS isMissingCheckin
			,(CASE 
				WHEN (
						isnull(Absence, 0) = 0
						AND OutFixed IS NULL
						AND @now > (
							(CASE 
								WHEN s.IsOutAfterDay = 1 THEN DATEADD(DD, 1, a.AttendanceDate)
								ELSE a.AttendanceDate END
								) + ' ' + s.EndCheckOutTime
							)
						) THEN 1
				ELSE 0 END
				) AS isMissingCheckOut
			,(CASE 
				WHEN (
						a.ShiftId IS NOT NULL
						AND a.InFixed IS NULL
						AND a.OutFixed IS NULL
						AND a.TotalWorkHours IS NULL
						AND @now > (
							(CASE 
								WHEN s.IsInAfterDay = 1 THEN DATEADD(DD, 1, a.AttendanceDate)
								ELSE a.AttendanceDate END
								) + ' ' + s.EndCheckInTime
							)
						) THEN 1
				ELSE 0 END
				) AS isNotWork
			,c.isOffday
			,CONVERT(BIT, CASE 
					WHEN ot.EmpId IS NULL THEN 0
					ELSE 1
					END) IsOvertime
			,(ISNULL(ot.NormalHours, 0) + ISNULL(ot.OffHours, 0) + ISNULL(ot.PublicHours, 0) + ISNULL(ot.SpecialHours, 0)) OvertimeHours
			,ot.[Status] AS OvertimeStatus
			,c.IsHoliday
			,(CASE 
				WHEN ISNULL(s_CrossDay.IsOutAfterDay, 0) = 1 THEN 1
				ELSE 0 END
				) AS IsShiftCrossDay
		FROM dbo.Employee AS e WITH (NOLOCK)
			CROSS APPLY #CalendarDF AS tbldate
			INNER JOIN @tblEmployeeId AS tblemp ON e.EmpId = tblemp.EmpId
			LEFT JOIN dbo.EmployeeSummary AS e_s WITH (NOLOCK) ON e_s.EmpId = e.EmpId
			LEFT JOIN dbo.EmployeeAttendance AS a WITH (NOLOCK) ON tbldate.AttendanceDate = a.AttendanceDate
			AND a.empid = e.empid
			LEFT JOIN dbo.EmployeeCalendar AS c WITH (NOLOCK) ON c.WorkDate = tbldate.AttendanceDate
			AND c.EmpId = e.EmpId
			LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = c.ShiftId
			LEFT JOIN dbo.EmployeeCalendar AS c_CrossDay WITH (NOLOCK) ON c_CrossDay.WorkDate = (tbldate.AttendanceDate - 1)
			AND c_CrossDay.EmpId = e.EmpId
			LEFT JOIN dbo.[Shift] AS s_CrossDay WITH (NOLOCK) ON s_CrossDay.ShiftId = c_CrossDay.ShiftId
			LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.groupid = g.groupid
			INNER JOIN @tblGroupId AS gId ON g.GroupId = gId.GroupId
			LEFT JOIN dbo.OTDailyRequest AS ot WITH (NOLOCK) ON a.attendancedate = ot.OvertimeDate
			AND a.EmpId = ot.EmpId
		WHERE @isLocal IS NULL
			OR e_s.IsLocal = @isLocal
		) AS T
		LEFT JOIN (
		SELECT c.WorkDate
			,a.EmpId
			,a.RequestType
			,rd.RequestDate
			,rd.FromDate
			,rd.ToDate
			,rd.FromTime
			,rd.ToTime
			,rd.TotalDays
			,rd.CurrentStatus
			,rd.KeepInViewTypeId
			,a.Remark
			,l.[Name] AS LeaveType
			,(CASE 
				WHEN a.RequestType = 5
					OR a.RequestType = 7 THEN (CASE 
						WHEN s1.ShiftID IS NULL THEN 'OFF'
						ELSE s1.[Name] END
						)
				ELSE s3.[Name] END
				) AS FromShift
			,(CASE 
				WHEN s2.ShiftID IS NULL THEN 'OFF'
				ELSE s2.[Name] END
				) AS ToShift
			,c.IsKeepInView
			,c.LeaveTypeId
			,c.LeaveTypeId2
			,c.LeaveDay
			,s3.IsInAfterDay
			,(CASE 
				WHEN a.RequestType = 3 THEN lieo.LateInMins
				ELSE lieo.EarlyOutMins END
				) AS [Time]
		FROM dbo.WorkingTimeRequest AS a WITH (NOLOCK)
			INNER JOIN #tblDailyRequest AS rd ON a.RequestId = rd.RequestId
			LEFT JOIN [dbo].LeaveRequest AS lr WITH (NOLOCK) ON a.RequestId = lr.RequestId
			LEFT JOIN [dbo].LateInEarlyOutRequest AS lieo WITH (NOLOCK) ON a.RequestId = lieo.RequestId
			LEFT JOIN dbo.EmployeeCalendar AS c WITH (NOLOCK) ON (
				c.WorkDate BETWEEN CONVERT(VARCHAR(10), rd.FromDate, 101)
					AND CONVERT(VARCHAR(10), rd.ToDate, 101)
				)
			AND c.EmpId = rd.EmpId
			LEFT JOIN dbo.LeaveType AS l WITH (NOLOCK) ON l.LeaveTypeID = lr.LeaveType
			LEFT JOIN dbo.Shift AS s1 WITH (NOLOCK) ON s1.ShiftId = rd.FromShift
			LEFT JOIN dbo.Shift AS s2 WITH (NOLOCK) ON s2.ShiftId = rd.ToShift
			LEFT JOIN dbo.Shift AS s3 WITH (NOLOCK) ON s3.ShiftId = c.ShiftID
		WHERE (
				c.LeaveDay <> 0
				AND rd.RequestType = 6
				)
			OR rd.RequestType <> 6
		) AS r ON (
			T.EmpId = r.EmpId
			AND
				(
				(@isRealDate = 1 AND r.IsInAfterDay = 1 AND T.AttendanceDate = r.WorkDate + 1)
				OR 
				((@isRealDate = 0 OR @isRealDate IS NULL OR r.IsInAfterDay = 0) AND T.AttendanceDate = r.WorkDate)
				)
			)
		LEFT JOIN LeaveType AS lv WITH (NOLOCK) ON lv.LeaveTypeId = r.LeaveTypeId
		LEFT JOIN LeaveType AS lv2 WITH (NOLOCK) ON lv2.LeaveTypeId = r.LeaveTypeId2
		LEFT JOIN @OutOffDay AS od ON t.AttendanceDate = od.WorkDate
		AND t.EmpId = od.EmpId
		LEFT JOIN dbo.Holiday AS h WITH (NOLOCK) ON T.AttendanceDate BETWEEN h.FromDate
			AND h.ToDate
	WHERE @type = 0
		OR (
			@type = 1
			AND ISNULL(T.isMissingCheckin, 1) = 1
			)
		OR (
			@type = 2
			AND T.isMissingCheckout = 1
			)
		OR (
			@type = 3
			AND T.isLateIn = 1
			)
		OR (
			@type = 4
			AND T.isEarlyOut = 1
			)
		OR (
			@type = 5
			AND T.isNotWork = 1
			)
	ORDER BY attendancedate DESC
		,[Rank]
		,TeamCode
		,T.code

	DROP TABLE #tblDailyRequest

	DROP TABLE #CalendarDF
END