
CREATE PROCEDURE [dbo].[API_GO_GetPendingRequestByUserId] @UserId INT
AS
/*
Created: Tim.Trinh@23032023
Task: get pending request
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@CheckInRequestTypeId TINYINT = 1
		,@CheckOutRequestTypeId TINYINT = 2
		,@LateInRequestTypeId TINYINT = 3
		,@EarlyOutRequestTypeId TINYINT = 4
		,@ChangeShiftRequestTypeId TINYINT = 5
		,@LeaveShiftRequestTypeId TINYINT = 6
		,@KeepInViewRequestTypeId TINYINT = 7
		-- work type
		,@OffDay TINYINT = 1
		,@LeaveDay TINYINT = 2
		,@Onduty TINYINT = 3

	SELECT @EmpId = EmpId
	FROM dbo.Employee WITH (NOLOCK)
	WHERE UserId = @UserId

	CREATE TABLE #tmpRequest (
		RequestId INT PRIMARY KEY
		,EmpId INT
		,VerifierId INT
		,ApproverId INT
		,StatusId TINYINT
		,Remark NVARCHAR(MAX)
		,CreatedDate DATETIME
		)

	CREATE TABLE #tmpDailyRequest (
		RequestId BIGINT
		,EmpId INT
		,RequestTypeId TINYINT
		,InOutMinutes SMALLINT
		,ActualInOutMinutes SMALLINT
		,RequestedDate DATETIME
		,FromDate DATETIME
		,ToDate DATETIME
		,FromTime DATETIME
		,ToTime DATETIME
		,FromShiftId INT
		,ToShiftId INT
		,TotalDays DECIMAL(18, 1)
		,IsHalfDay BIT
		,KeepInViewTypeId TINYINT
		)

	CREATE TABLE #tmpGeneralLeaveDetail (
		RequestId BIGINT
		,OnDate DATE
		,LeaveTypeId SMALLINT
		,LeaveTypeCode VARCHAR(20)
		,LeaveTypeName NVARCHAR(20)
		,LeaveDay DECIMAL(18, 1)
		,IsOffDay BIT
		,IsHoliday BIT
		)

	INSERT INTO #tmpRequest (
		RequestId
		,EmpId
		,VerifierId
		,ApproverId
		,StatusId
		,Remark
		,CreatedDate
		)
	SELECT r.RequestId
		,r.EmpId
		,r.VerifierId
		,r.ApproverId
		,r.CurrentStatus
		,r.Remark
		,r.CreatedDate
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = r.EmpId
	WHERE emp.[Enabled] = 1
		AND emp.isDeleted = 0
		AND (
			(
				r.CurrentStatus = @PendingStatusId
				AND (
					r.VerifierId = @EmpId
					OR r.ApproverId = @EmpId
					)
				)
			OR (
				r.CurrentStatus = @VerifiedStatusId
				AND r.ApproverId = @EmpId
				)
			)

	INSERT INTO #tmpDailyRequest (
		RequestId
		,EmpId
		,RequestTypeId
		,FromDate
		,ToDate
		,TotalDays
		,InOutMinutes
		,ActualInOutMinutes
		)
	SELECT r.RequestId
		,r.EmpId
		,r.RequestType
		,inout.OnDate
		,inout.OnDate
		,0
		,CASE 
			WHEN r.RequestType = @LateInRequestTypeId THEN inout.LateInMins
			ELSE inout.EarlyOutMins
			END
		,CASE 
			WHEN r.RequestType = @LateInRequestTypeId THEN att.LateIn
			ELSE att.EarlyOut
			END
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN #tmpRequest AS tmpr ON r.RequestId = tmpr.RequestId
		INNER JOIN dbo.LateInEarlyOutRequest AS inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
		LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON inout.EmpId = att.EmpId
		AND inout.OnDate = att.AttendanceDate

	INSERT INTO #tmpDailyRequest (
		RequestId
		,EmpId
		,RequestTypeId
		,FromDate
		,ToDate
		,FromTime
		,ToTime
		,TotalDays
		,IsHalfDay
		)
	SELECT r.RequestId
		,r.EmpId
		,r.RequestType
		,lr.FromDate
		,lr.ToDate
		,CASE 
			WHEN ldt.FromTime IS NULL THEN lr.FromDate
			ELSE ldt.FromTime
			END
		,CASE 
			WHEN ldt.ToTime IS NULL THEN lr.ToDate
			ELSE ldt.ToTime
			END
		,lr.TotalDays
		,1
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN #tmpRequest AS tmpr ON r.RequestId = tmpr.RequestId
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
		LEFT JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
	WHERE lr.TotalDays = 0.5
		AND ldt.IsHalfDay = 1

	INSERT INTO #tmpDailyRequest (
		RequestId
		,EmpId
		,RequestTypeId
		,RequestedDate
		,FromDate
		,ToDate
		,TotalDays
		,IsHalfDay
		)
	SELECT r.RequestId
		,r.EmpId
		,r.RequestType
		,lr.FromDate
		,lr.FromDate
		,lr.ToDate
		,lr.TotalDays
		,0
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN #tmpRequest AS tmpr ON r.RequestId = tmpr.RequestId
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
	WHERE lr.TotalDays != 0.5

	INSERT INTO #tmpDailyRequest (
		RequestId
		,EmpId
		,RequestTypeId
		,RequestedDate
		,FromDate
		,ToDate
		,FromTime
		,ToTime
		,TotalDays
		,FromShiftId
		,ToShiftId
		,IsHalfDay
		,KeepInViewTypeId
		)
	SELECT r.RequestId
		,r.EmpId
		,r.RequestType
		,kivr.OnDate
		,kivr.OnDate
		,kivr.OnDate
		,kivr.FromTime
		,kivr.ToTime
		,CASE 
			WHEN kivr.IsHalfDay = 0 THEN 1
			ELSE 0.5
			END TotalDays
		,NULL
		,NULL
		,kivr.IsHalfDay
		,kivr.KeepInViewType
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN #tmpRequest AS tmpr ON r.RequestId = tmpr.RequestId
		INNER JOIN dbo.KeepInViewRequest AS kivr WITH (NOLOCK) ON r.RequestId = kivr.RequestId

	INSERT INTO #tmpDailyRequest (
		RequestId
		,EmpId
		,RequestTypeId
		,RequestedDate
		,FromDate
		,ToDate
		,TotalDays
		,FromShiftId
		)
	SELECT r.RequestId
		,r.EmpId
		,r.RequestType
		,ior.RequestTime
		,ior.OnDate
		,ior.OnDate
		,1 AS TotalDays
		,ec.ShiftId
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN #tmpRequest AS tmpr ON r.RequestId = tmpr.RequestId
		INNER JOIN dbo.CheckInOutRequest AS ior WITH (NOLOCK) ON r.RequestId = ior.RequestId
		LEFT JOIN dbo.EmployeeCalendar AS ec WITH (NOLOCK) ON ec.WorkDate = ior.OnDate
		AND ec.EmpId = ior.EmpId

	INSERT INTO #tmpDailyRequest (
		RequestId
		,EmpId
		,RequestTypeId
		,FromDate
		,ToDate
		,TotalDays
		,FromShiftId
		,ToShiftId
		)
	SELECT r.RequestId
		,r.EmpId
		,r.RequestType
		,cs.FromDate
		,cs.ToDate
		,COUNT(1) AS TotalDays
		,MAX(csd.FromShift) AS FromShift
		,MAX(csd.ToShift) AS ToShift
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN #tmpRequest AS tmpr ON r.RequestId = tmpr.RequestId
		INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON r.RequestId = cs.RequestId
		INNER JOIN dbo.ChangeShiftRequestDetail AS csd WITH (NOLOCK) ON csd.RequestId = cs.RequestId
	GROUP BY r.RequestId
		,r.EmpId
		,r.RequestType
		,cs.FromDate
		,cs.ToDate

	INSERT INTO #tmpGeneralLeaveDetail (
		RequestId
		,OnDate
		,LeaveTypeId
		,LeaveTypeCode
		,LeaveTypeName
		,LeaveDay
		,IsOffDay
		,IsHoliday
		)
	SELECT r.RequestId
		,ldt.OnDate
		,ldt.LeaveType AS LeaveTypeId
		,ltype.Code AS LeaveTypeCode
		,ltype.[Name] AS LeaveTypeName
		,CASE 
			WHEN ldt.LeaveType != 0
				AND ldt.IsHalfDay = 0 THEN 1
			WHEN ldt.LeaveType != 0
				AND ldt.IsHalfDay = 1 THEN 0.5
			ELSE 0
			END AS LeaveDay
		,CASE 
			WHEN ldt.WorkType = @OffDay
				OR (
					WorkType != @Onduty
					AND empca.IsOffDay = 1
					) THEN 1
			ELSE 0
			END AS IsOffDay
		,CASE 
			WHEN ldt.WorkType = @OffDay
				OR (
					WorkType != @Onduty
					AND empca.IsHoliday = 1
					) THEN 1
			ELSE 0
			END AS IsHoliday
	FROM #tmpRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON r.RequestId = ldt.RequestId
		LEFT JOIN dbo.LeaveType AS ltype WITH (NOLOCK) ON ltype.LeaveTypeId = ldt.LeaveType
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON ldt.OnDate = empca.WorkDate
		AND ldt.EmpId = empca.EmpId
	WHERE ldt.WorkType != @Onduty

	SELECT rd.RequestId
		,rd.RequestTypeId
		,rq.StatusId
		,rd.EmpId
		,e.Nickname
		,e.FullName
		,rq.VerifierId
		,rq.ApproverId
		,rd.FromDate
		,rd.ToDate
		,rd.TotalDays
		,rd.IsHalfDay
		,rd.RequestedDate
		,rd.FromTime
		,rd.ToTime
		,rd.KeepInViewTypeId
		,csr.TypeId AS ChangeShiftTypeId
		,ltype.LeaveTypeId
		,ltype.[Name] AS LeaveTypeName
		,(CASE 
			WHEN rd.RequestTypeId IN (@CheckInRequestTypeId, @CheckOutRequestTypeId, @ChangeShiftRequestTypeId, @KeepInViewRequestTypeId) THEN shift1.ShiftId
			ELSE shift1.ShiftId END
			) AS FromShiftId
		,(CASE 
			WHEN rd.RequestTypeId IN (@CheckInRequestTypeId, @CheckOutRequestTypeId, @ChangeShiftRequestTypeId, @KeepInViewRequestTypeId) THEN (CASE 
					WHEN shift1.ShiftId IS NULL THEN 'OFF'
					ELSE shift1.[Name] END
					)
			ELSE shift2.[Name] END
			) AS FromShiftName
		,toshift.ShiftId AS ToShiftId
		,(CASE 
			WHEN toshift.ShiftId IS NULL THEN 'OFF'
			ELSE toshift.[Name] END
			) AS ToShiftName
		,rd.InOutMinutes
		,rd.ActualInOutMinutes
		,rq.CreatedDate
		,rq.Remark
	FROM #tmpRequest AS rq WITH (NOLOCK)
		INNER JOIN #tmpDailyRequest AS rd WITH (NOLOCK) ON rq.RequestId = rd.RequestId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = rd.EmpId
		LEFT JOIN dbo.LateInEarlyOutRequest AS inout WITH (NOLOCK) ON rq.RequestId = inout.RequestId
		LEFT JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON rq.RequestId = lr.RequestId
		LEFT JOIN dbo.CheckInOutRequest AS inoutmiss WITH (NOLOCK) ON rq.RequestId = inoutmiss.RequestId
		LEFT JOIN dbo.ChangeShiftRequest AS csr WITH (NOLOCK) ON rq.RequestId = csr.RequestId
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON empca.EmpId = rd.EmpId
		AND empca.WorkDate = CAST(rd.FromDate AS DATE)
		LEFT JOIN dbo.[Shift] AS shift1 WITH (NOLOCK) ON shift1.ShiftId = rd.FromShiftId
		LEFT JOIN dbo.[Shift] AS shift2 WITH (NOLOCK) ON shift2.ShiftId = empca.ShiftId
		LEFT JOIN dbo.[Shift] AS toshift WITH (NOLOCK) ON toshift.ShiftId = rd.ToShiftId
		LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN dbo.LeaveType AS ltype WITH (NOLOCK) ON ltype.LeaveTypeId = lr.LeaveType
	ORDER BY rq.CreatedDate DESC

	SELECT RequestId
		,OnDate
		,LeaveTypeId
		,LeaveTypeCode
		,LeaveTypeName
		,LeaveDay
		,IsOffDay
		,IsHoliday
	FROM #tmpGeneralLeaveDetail WITH (NOLOCK)
	WHERE LeaveTypeId != 0
		OR IsOffDay = 1
		OR IsHoliday = 1

	SELECT csrd.RequestId
		,csrd.OnDate
		,csrd.ToDate
	FROM #tmpRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.ChangeShiftRequest AS csr WITH (NOLOCK) ON csr.RequestId = r.RequestId
		INNER JOIN dbo.ChangeShiftRequestDetail AS csrd WITH (NOLOCK) ON csr.RequestId = csrd.RequestId

	DROP TABLE #tmpGeneralLeaveDetail

	DROP TABLE #tmpRequest
END