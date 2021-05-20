


CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByCreateDate_AllType] @SupervisorID INT = NULL
	,@empid INT = NULL
	,@TeamIds VARCHAR(max) = ''
	,@fromdate DATETIME
	,@todate DATETIME
	,@status VARCHAR(max) = NULL
	,@role VARCHAR(max) = ''
	,@isLocal BIT = NULL
	,@requestType INT = NULL
	,@leaveTypeId INT = NULL
AS
/*
Created: Tom.Vo@2013-11-04
Task:Get Request By Date For LateInEarlyOutType
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	CREATE TABLE #tblstatus (Currentstatus TINYINT)
	CREATE TABLE #tblGroupsList (GroupId INT)
	CREATE TABLE #tblEmployeeId (EmpId INT)
	CREATE TABLE #tblRequestId (RequestId INT)
	CREATE TABLE #tblTotalDaysRequestInMonth (
		empid INT
		,RequestType TINYINT
		,TotalDaysRequest INT
		)
	CREATE TABLE #tblRequestInDaily (
		RequestID BIGINT
		,empid INT
		,RequestType TINYINT
		,InOutMins INT
		,ActualInOutMins INT
		,RequestedTime DATETIME
		,FromDate DATETIME
		,ToDate DATETIME
		,FromTime DATETIME
		,ToTime DATETIME
		,FromShift INT
		,ToShift INT
		,TotalDays NUMERIC(18, 1)
		,IsHalfDay BIT
		,IsWarning BIT
		,KeepInViewType TINYINT
		)
	CREATE TABLE #tblGeneralLeaveDetail (
		RequestId BIGINT
		,LeaveCountDetails NVARCHAR(MAX)
		)

	IF ISNULL(@status, '') <> ''
	BEGIN
		INSERT INTO #tblstatus (Currentstatus)
		SELECT val
		FROM dbo.fnc_strSplit(@status, ',')
	END

	IF ISNULL(@TeamIds, '') <> ''
	BEGIN
		INSERT INTO #tblGroupsList (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	IF @empid IS NULL
	BEGIN
		IF @SupervisorID IS NULL
			OR @role LIKE '%6%'
		BEGIN
			INSERT INTO #tblEmployeeId (EmpID)
			SELECT DISTINCT e.EmpId
			FROM dbo.Employee AS e WITH (NOLOCK)
		END
		ELSE IF @role LIKE '%2%'
		BEGIN
			INSERT INTO #tblEmployeeId (EmpId)
			SELECT EmpId
			FROM dbo.fnc_GetEmployeeByActorId(@SupervisorId, 1, 0)
		END
		ELSE
		BEGIN
			INSERT INTO #tblEmployeeId (EmpId)
			SELECT EmpId
			FROM dbo.fnc_GetEmployeeByActorId(@SupervisorId, 1, 1)
		END
	END
	ELSE
	BEGIN
		INSERT INTO #tblEmployeeId (EmpID)
		VALUES (@empid)
	END

	SELECT @fromdate = CONVERT(VARCHAR(10), @fromdate, 111)
		,@todate = DATEADD(ms, - 2, DATEADD(d, 1, CONVERT(VARCHAR(10), @todate, 111)))

	INSERT INTO #tblRequestId (RequestId)
	SELECT r.RequestId
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN #tblEmployeeId AS eid ON r.EmpId = eid.EmpId
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.empID = eid.empID
		AND (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		LEFT JOIN dbo.EmployeeSummary AS e_s WITH (NOLOCK) ON e_s.EmpId = eid.EmpId
	WHERE (
			@requestType = 0
			OR (
				@requestType = 1
				AND r.RequestType IN (1, 2)
				)
			OR (
				@requestType = 2
				AND r.RequestType IN (3, 4)
				)
			OR (
				@requestType = 3
				AND r.RequestType = 5
				)
			OR (
				@requestType = 4
				AND r.RequestType = 6
				)
			OR (
				@requestType = 5
				AND r.RequestType = 7
				)
			)
		AND (
			(
				@empid IS NULL
				AND (
					EXISTS (
						SELECT 1
						FROM #tblEmployeeId
						WHERE EmpId = r.EmpId
						)
					OR r.VerifierId = @SupervisorID
					OR r.ApproverId = @SupervisorID
					)
				)
			OR (
				@empid IS NOT NULL
				AND (
					EXISTS (
						SELECT 1
						FROM #tblEmployeeId
						WHERE EmpId = r.EmpId
						)
					)
				)
			)
		AND (
			ISNULL(@status, '') = ''
			OR r.currentstatus IN (
				SELECT Currentstatus
				FROM #tblstatus
				)
			)
		AND (
			e.GroupId IS NULL
			OR e.GroupId IN (
				SELECT GroupId
				FROM #tblGroupsList
				)
			)
		AND (
			@isLocal IS NULL
			OR e_s.IsLocal = @isLocal
			)
		AND r.CreatedDate BETWEEN @fromdate
			AND @todate

	INSERT INTO #tblTotalDaysRequestInMonth (
		empid
		,RequestType
		,TotalDaysRequest
		)
	SELECT wt.empid
		,wt.RequestType
		,count(1)
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN dbo.LateInEarlyOutRequest AS inout WITH (NOLOCK) ON wt.RequestId = inout.RequestId
		INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON ee.EmpId = wt.EmpId
	WHERE wt.currentstatus = 6
		AND ee.GroupId IN (
			SELECT GroupId
			FROM #tblGroupsList
			)
		AND (
			DATEDIFF(MM, inout.OnDate, @fromdate) = 0
			OR DATEDIFF(MM, inout.OnDate, @todate) = 0
			)
		AND EXISTS (
			SELECT 1
			FROM #tblEmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.empid
		,wt.RequestType

	INSERT INTO #tblTotalDaysRequestInMonth (
		empid
		,RequestType
		,TotalDaysRequest
		)
	SELECT wt.empid
		,wt.RequestType
		,count(1)
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
		INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON ee.EmpId = wt.EmpId
	WHERE wt.currentstatus = 6
		AND ee.GroupId IN (
			SELECT GroupId
			FROM #tblGroupsList
			)
		AND (
			DATEDIFF(MM, lr.FromDate, @fromdate) = 0
			OR DATEDIFF(MM, lr.ToDate, @todate) = 0
			)
		AND EXISTS (
			SELECT 1
			FROM #tblEmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.empid
		,wt.RequestType

	INSERT INTO #tblTotalDaysRequestInMonth (
		empid
		,RequestType
		,TotalDaysRequest
		)
	SELECT wt.empid
		,wt.RequestType
		,count(1)
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON wt.RequestId = cs.RequestId
	WHERE wt.currentstatus = 6
		AND (
			DATEDIFF(MM, cs.FromDate, @fromdate) = 0
			OR DATEDIFF(MM, cs.ToDate, @todate) = 0
			)
		AND EXISTS (
			SELECT 1
			FROM #tblEmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.empid
		,wt.RequestType

	INSERT INTO #tblTotalDaysRequestInMonth (
		empid
		,RequestType
		,TotalDaysRequest
		)
	SELECT @empid
		,wt.RequestType
		,count(1)
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN dbo.KeepInViewRequest AS lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
	WHERE wt.EmpId = @empid
		AND wt.currentstatus = 6
		AND (
			DATEDIFF(MM, lr.OnDate, @fromdate) = 0
			OR DATEDIFF(MM, lr.OnDate, @todate) = 0
			)
		AND EXISTS (
			SELECT 1
			FROM #tblEmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.RequestType

	INSERT INTO #tblTotalDaysRequestInMonth (
		empid
		,RequestType
		,TotalDaysRequest
		)
	SELECT wt.empid
		,wt.RequestType
		,count(1)
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN dbo.CheckInOutRequest AS lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
		INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON ee.EmpId = wt.EmpId
	WHERE wt.currentstatus = 6
		AND ee.GroupId IN (
			SELECT GroupId
			FROM #tblGroupsList
			)
		AND (
			DATEDIFF(MM, lr.OnDate, @fromdate) = 0
			OR DATEDIFF(MM, lr.OnDate, @todate) = 0
			)
		AND EXISTS (
			SELECT 1
			FROM #tblEmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.empid
		,wt.RequestType

	INSERT INTO #tblRequestInDaily (
		RequestID
		,empid
		,RequestType
		,FromDate
		,ToDate
		,TotalDays
		,InOutMins
		,ActualInOutMins
		)
	SELECT wt.RequestID
		,wt.EmpId
		,wt.RequestType
		,inout.OnDate
		,inout.OnDate
		,0
		,CASE 
			WHEN wt.RequestType = 3 THEN inout.LateInMins
			ELSE inout.EarlyOutMins
			END
		,CASE 
			WHEN wt.RequestType = 3 THEN att.LateIn
			ELSE att.EarlyOut
			END
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN #tblRequestId AS rq ON wt.RequestId = rq.RequestId
		INNER JOIN dbo.LateInEarlyOutRequest AS inout WITH (NOLOCK) ON wt.RequestId = inout.RequestId
		LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON inout.EmpId = att.EmpId
		AND inout.OnDate = att.AttendanceDate

	INSERT INTO #tblRequestInDaily (
		RequestID
		,empid
		,RequestType
		,FromDate
		,ToDate
		,FromTime
		,ToTime
		,TotalDays
		,IsHalfDay
		)
	SELECT wt.RequestID
		,wt.EmpId
		,wt.RequestType
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
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN #tblRequestId AS rq ON wt.RequestId = rq.RequestId
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
		LEFT JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
	WHERE lr.TotalDays = 0.5
		AND ldt.IsHalfDay = 1

	INSERT INTO #tblRequestInDaily (
		RequestID
		,empid
		,RequestType
		,FromDate
		,ToDate
		,TotalDays
		,IsHalfDay
		)
	SELECT wt.RequestID
		,wt.EmpId
		,wt.RequestType
		,lr.FromDate
		,lr.ToDate
		,lr.TotalDays
		,0
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN #tblRequestId AS rq ON wt.RequestId = rq.RequestId
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
	WHERE lr.TotalDays <> 0.5

	INSERT INTO #tblRequestInDaily (
		RequestID
		,empid
		,RequestType
		,RequestedTime
		,FromDate
		,ToDate
		,FromTime
		,ToTime
		,TotalDays
		,FromShift
		,ToShift
		,KeepInViewType
		)
	SELECT wt.RequestID
		,wt.EmpId
		,wt.RequestType
		,lr.OnDate
		,lr.OnDate
		,lr.OnDate
		,lr.FromTime
		,lr.ToTime
		,CASE 
			WHEN lr.IsHalfDay = 0 THEN 1
			ELSE 0.5
			END TotalDays
		,NULL
		,NULL
		,lr.KeepInViewType
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN #tblRequestId AS rq ON wt.RequestId = rq.RequestId
		INNER JOIN dbo.KeepInViewRequest AS lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId

	INSERT INTO #tblRequestInDaily (
		RequestID
		,empid
		,RequestType
		,RequestedTime
		,FromDate
		,ToDate
		,TotalDays
		,FromShift
		,IsWarning
		)
	SELECT wt.RequestID
		,wt.EmpId
		,wt.RequestType
		,lr.RequestTime
		,lr.OnDate
		,lr.OnDate
		,1 TotalDays
		,ec.ShiftId
		,CASE 
			WHEN ISNULL(att.TotalWorkHours, 0) = 0 THEN 1
			ELSE 0
			END
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN #tblRequestId AS rq ON wt.RequestId = rq.RequestId
		INNER JOIN dbo.CheckInOutRequest AS lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
		LEFT JOIN dbo.EmployeeCalendar AS ec WITH (NOLOCK) ON ec.WorkDate = lr.OnDate
		AND ec.EmpId = lr.EmpId
		LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON lr.OnDate = att.AttendanceDate
		AND lr.EmpId = att.EmpId
	WHERE wt.CurrentStatus = 4

	INSERT INTO #tblRequestInDaily (
		RequestID
		,empid
		,RequestType
		,RequestedTime
		,FromDate
		,ToDate
		,TotalDays
		,FromShift
		)
	SELECT wt.RequestID
		,wt.EmpId
		,wt.RequestType
		,lr.RequestTime
		,lr.OnDate
		,lr.OnDate
		,1 TotalDays
		,ec.ShiftId
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN #tblRequestId AS rq ON wt.RequestId = rq.RequestId
		INNER JOIN dbo.CheckInOutRequest AS lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
		LEFT JOIN dbo.EmployeeCalendar AS ec WITH (NOLOCK) ON ec.WorkDate = lr.OnDate
		AND ec.EmpId = lr.EmpId
	WHERE wt.CurrentStatus <> 4

	INSERT INTO #tblRequestInDaily (
		RequestID
		,empid
		,RequestType
		,FromDate
		,ToDate
		,TotalDays
		,FromShift
		,ToShift
		)
	SELECT wt.RequestID
		,wt.EmpId
		,wt.RequestType
		,cs.FromDate
		,cs.ToDate
		,count(1) TotalDays
		,MAX(csd.FromShift) FromShift
		,MAX(csd.ToShift) ToShift
	FROM dbo.WorkingTimeRequest AS wt WITH (NOLOCK)
		INNER JOIN #tblRequestId AS rq ON wt.RequestId = rq.RequestId
		INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON wt.RequestId = cs.RequestId
		INNER JOIN dbo.ChangeShiftRequestDetail AS csd WITH (NOLOCK) ON csd.RequestId = cs.RequestId
	GROUP BY wt.RequestID
		,wt.EmpId
		,wt.RequestType
		,cs.FromDate
		,cs.ToDate

	INSERT INTO #tblGeneralLeaveDetail (
		RequestId
		,LeaveCountDetails
		)
	SELECT DISTINCT wt.RequestId
		,STUFF((
				SELECT ', ' + (
						CAST(SUM(CASE 
									WHEN rd1.IsHalfDay = 0 THEN 1
									ELSE 0.5
									END) AS NVARCHAR(100)) + ' days ' + lt1.Code
						)
				FROM dbo.LeaveRequestDetail AS rd1 WITH (NOLOCK)
					INNER JOIN dbo.LeaveType AS lt1 WITH (NOLOCK) ON lt1.LeaveTypeId = rd1.LeaveType
				WHERE rd1.RequestId = rd.RequestId
					AND rd1.LeaveType <> 0
				GROUP BY rd1.RequestId
					,rd1.LeaveType
					,lt1.Code
				FOR XML PATH('')
				), 1, 2, '') AS LeaveCountDetails
	FROM (
		SELECT rq.RequestId
			,CreatedDate
			,EmpId
		FROM dbo.WorkingTimeRequest AS rq WITH (NOLOCK)
			INNER JOIN #tblRequestId AS r ON r.RequestId = rq.RequestId
		) AS wt
		INNER JOIN #tblRequestInDaily AS rd ON wt.RequestId = rd.RequestID
		INNER JOIN #tblRequestId AS r1 ON r1.RequestId = rd.RequestId
	ORDER BY wt.RequestId

	SELECT r.RequestId
		,r.RequestType
		,r.Currentstatus AS [Status]
		,r.empID
		,rd.FromDate
		,rd.ToDate
		,rd.TotalDays
		,rd.IsHalfDay
		,rd.RequestedTime AS RequestedDate
		,rd.FromTime
		,rd.ToTime
		,rd.KeepInViewType
		,gdt.LeaveCountDetails
		,lv.LeaveTypeId
		,lv.[Name] AS LeaveTypeName
		,r.Remark
		,ISNULL(rm.TotalDaysRequest, 0) TotalDaysRequestInMonth
		,CASE 
			WHEN r.RequestType IN (1, 2, 6, 7) THEN lr.RefNo
			WHEN r.RequestType IN (3, 4) THEN inout.RefNo
			WHEN r.RequestType IN (5) THEN NULL
			END RefNo
		,(CASE 
			WHEN r.RequestType IN (1, 2, 5, 7) THEN s1.ShiftID
			ELSE s3.ShiftID END
			) AS FromShiftId
		,(CASE 
			WHEN r.RequestType IN (1, 2, 5, 7) THEN (CASE 
					WHEN s1.ShiftID IS NULL THEN 'OFF'
					ELSE s1.[Name] END
					)
			ELSE s3.[Name] END
			) AS FromShiftName
		,s2.ShiftID AS ToShiftId
		,(CASE 
			WHEN s2.ShiftID IS NULL THEN 'OFF'
			ELSE s2.[Name] END
			) AS ToShiftName
		,s2.ShiftID AS ToShiftId
		,s2.[Name] AS ToShiftName
		,csr.TypeId AS ChangeShiftType
		,rd.InOutMins AS TotalMinutes
		,rd.ActualInOutMins
		,r.CreatedDate
		,e.EmpId AS RequesterId
		,e.NickName AS RequesterNickName
		,e.FullName AS RequesterFullName
		,g.GroupId AS TeamId
		,g.Code AS TeamCode
		,g.[Name] AS TeamName
		,sec.SectionId
		,sec.Code AS SectionCode
		,dep.DepartmentId
		,dep.Code AS DepartmentCode
		,ap_V.EmpId AS VerifierId
		,ap_V.NickName AS VerifierNickName
		,ap_V.FullName AS VerifierFullName
		,r.VerifiedDate
		,r.VerifiedStatus
		,ap_A.EmpId AS ApproverId
		,ap_A.NickName AS ApproverNickName
		,ap_A.FullName AS ApproverFullName
		,ap_AH.EmpId AS ApprovedId
		,ap_AH.NickName AS ApprovedNickName
		,ap_AH.FullName AS ApprovedFullName
		,r.ApprovedDate
		,r.ApprovedStatus
		,ap_H.EmpId AS ProcessedId
		,ap_H.NickName AS ProcessedNickName
		,ap_H.FullName AS ProcessedFullName
		,r.HRDate AS ProcessedDate
		,r.HRStatus AS ProcessedStatus
		,r.ModifiedBy ModifiedById
		,e_m.NickName AS ModifiedByNickName
		,e_m.FullName AS ModifiedByFullName
		,r.ModifiedDate
		,ISNULL(rd.IsWarning, 0) AS IsWarning
		,r.ReworkCount
		,dbo.fnc_GetAnnualLeaveBalanceByYear(r.EmpId, YEAR(rd.FromDate), r.RequestId) AS AnnualLeaveBalance
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN #tblRequestInDaily AS rd ON r.RequestId = rd.RequestID
		INNER JOIN #tblRequestId AS rq ON r.RequestId = rq.RequestId
		LEFT JOIN dbo.LateInEarlyOutRequest AS inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
		LEFT JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
		LEFT JOIN dbo.CheckInOutRequest AS inoutmiss WITH (NOLOCK) ON r.RequestId = inoutmiss.RequestId
		LEFT JOIN dbo.ChangeShiftRequest AS csr WITH (NOLOCK) ON r.RequestId = csr.RequestId
		LEFT JOIN dbo.Employee AS ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
		LEFT JOIN dbo.Employee AS ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
		LEFT JOIN dbo.Employee AS ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
		LEFT JOIN dbo.Employee AS e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
		LEFT JOIN dbo.Employee AS ap_AH WITH (NOLOCK) ON ap_AH.empid = (SELECT TOP 1 ModifiedBy FROM WorkingTimeRequestHistory WITH (NOLOCK) WHERE RequestId = r.RequestId ORDER BY LogDate DESC)
		LEFT JOIN #tblTotalDaysRequestInMonth AS rm ON r.empID = rm.empid
		AND r.RequestType = rm.RequestType
		LEFT JOIN #tblGeneralLeaveDetail AS gdt ON r.RequestId = gdt.RequestId
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.empID = r.empID
		LEFT JOIN dbo.EmployeeCalendar AS C WITH (NOLOCK) ON c.empid = r.empid
		AND c.WorkDate = convert(VARCHAR(10), rd.FromDate, 111)
		LEFT JOIN dbo.[Shift] AS s1 WITH (NOLOCK) ON s1.ShiftID = rd.FromShift
		LEFT JOIN dbo.[Shift] AS s2 WITH (NOLOCK) ON s2.ShiftID = rd.ToShift
		LEFT JOIN dbo.[Shift] AS s3 WITH (NOLOCK) ON s3.ShiftID = c.ShiftID
		LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupID = g.GroupID
		LEFT JOIN dbo.LeaveType AS lv WITH (NOLOCK) ON lv.LeaveTypeID = lr.LeaveType
		LEFT JOIN dbo.Section AS sec WITH (NOLOCK) ON sec.SectionId = g.SectionId
		LEFT JOIN dbo.Department AS dep WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId

		OUTER APPLY ( SELECT TOP 1 ApproverId
                  FROM WorkingTimeRequestHistory
                  WHERE RequestId = r.RequestId
                  ORDER BY LogDate DESC) AS rh
	WHERE @leaveTypeId IS NULL
		OR lr.LeaveType = @leaveTypeId
	ORDER BY r.CreatedDate DESC

	SELECT csrd.RequestId
		,csrd.OnDate
		,csrd.ToDate
	FROM #tblRequestId AS r
		INNER JOIN ChangeShiftRequest AS csr WITH (NOLOCK) ON csr.RequestId = r.RequestId
		INNER JOIN ChangeShiftRequestDetail AS csrd WITH (NOLOCK) ON csr.RequestId = csrd.RequestId

	DROP TABLE #tblstatus
	DROP TABLE #tblGroupsList
	DROP TABLE #tblEmployeeId
	DROP TABLE #tblTotalDaysRequestInMonth
	DROP TABLE #tblRequestInDaily
	DROP TABLE #tblGeneralLeaveDetail
	DROP TABLE #tblRequestId
END
