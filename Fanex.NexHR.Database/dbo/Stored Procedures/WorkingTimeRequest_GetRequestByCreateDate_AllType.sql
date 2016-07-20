
CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetRequestByCreateDate_AllType] @SupervisorID INT = NULL
	,@empid INT = NULL
	,@groupids VARCHAR(max) = ''
	,@fromdate DATETIME
	,@todate DATETIME
	,@status VARCHAR(max) = NULL
	,@excludedEmpID INT = NULL
	,@role VARCHAR(max) = ''
	,@isLocal BIT = NULL
	,@requestType INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
	,@isDebug BIT = 0
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
	SET NOCOUNT ON;

	SELECT @excludedEmpID = NULL

	CREATE TABLE #status (Currentstatus TINYINT)

	IF ISNULL(@status, '') <> ''
		INSERT INTO #status (Currentstatus)
		SELECT val
		FROM fnc_strSplit(@status, ',')

	CREATE TABLE #GroupsList (GroupId INT)

	CREATE TABLE #EmployeeId (EmpId INT)

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO #GroupsList (GroupId)
		SELECT val
		FROM fnc_strSplit(@GroupIds, ',')
	END
	ELSE
	BEGIN
		INSERT INTO #GroupsList (GroupId)
		SELECT GroupID
		FROM [Group] g WITH (NOLOCK)
	END

	IF (@empid IS NULL)
	BEGIN
		IF (
				@SupervisorID IS NULL
				OR @role LIKE '%6%'
				)
		BEGIN
			INSERT INTO #EmployeeId (EmpID)
			SELECT DISTINCT e.EmpId
			FROM dbo.Employee AS e WITH (NOLOCK)
		END
		ELSE IF (@role LIKE '%2%')
		BEGIN
			INSERT INTO #EmployeeId (EmpId)
			SELECT EmpId
			FROM [dbo].[fnc_GetEmployeeByActorId](@SupervisorId, 1, 0)
		END
		ELSE
		BEGIN
			INSERT INTO #EmployeeId (EmpId)
			SELECT EmpId
			FROM [dbo].[fnc_GetEmployeeByActorId](@SupervisorId, 1, 1)
		END
	END
	ELSE
	BEGIN
		INSERT INTO #EmployeeId (EmpID)
		VALUES (@empid)
	END

	SELECT @fromdate = CONVERT(VARCHAR(10), @fromdate, 111)
		,@todate = DATEADD(ms, - 2, DATEADD(d, 1, CONVERT(VARCHAR(10), @todate, 111)))

	CREATE TABLE #TotalDaysRequestInMonth (
		empid INT
		,RequestType INT
		,TotalDaysRequest INT
		)

	CREATE TABLE #RequestInDaily (
		RequestID BIGINT
		,empid INT
		,RequestType INT
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
		)

	INSERT INTO #TotalDaysRequestInMonth (
		empid
		,RequestType
		,TotalDaysRequest
		)
	SELECT wt.empid
		,wt.RequestType
		,count(1)
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) ON wt.RequestId = inout.RequestId
		INNER JOIN Employee ee WITH (NOLOCK) ON ee.EmpId = wt.EmpId
	WHERE wt.currentstatus = 6
		AND wt.RequestType IN (3, 4)
		AND ee.GroupId IN (
			SELECT GroupId
			FROM #GroupsList
			)
		AND (
			DATEDIFF(MM, inout.FromDate, @fromdate) = 0
			OR DATEDIFF(MM, inout.ToDate, @todate) = 0
			)
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.empid
		,wt.RequestType

	INSERT INTO #TotalDaysRequestInMonth (
		empid
		,RequestType
		,TotalDaysRequest
		)
	SELECT wt.empid
		,wt.RequestType
		,count(1)
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].LeaveRequest lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
		INNER JOIN Employee ee WITH (NOLOCK) ON ee.EmpId = wt.EmpId
	WHERE wt.currentstatus = 6
		AND ee.GroupId IN (
			SELECT GroupId
			FROM #GroupsList
			)
		AND (
			DATEDIFF(MM, lr.FromDate, @fromdate) = 0
			OR DATEDIFF(MM, lr.ToDate, @todate) = 0
			)
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.empid
		,wt.RequestType

	INSERT INTO #TotalDaysRequestInMonth (
		empid
		,RequestType
		,TotalDaysRequest
		)
	SELECT wt.empid
		,wt.RequestType
		,count(1)
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].ChangeShiftRequest cs WITH (NOLOCK) ON wt.RequestId = cs.RequestId
	WHERE wt.currentstatus = 6
		AND (
			DATEDIFF(MM, cs.FromDate, @fromdate) = 0
			OR DATEDIFF(MM, cs.ToDate, @todate) = 0
			)
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.empid
		,wt.RequestType

	INSERT INTO #TotalDaysRequestInMonth (
		empid
		,RequestType
		,TotalDaysRequest
		)
	SELECT @empid
		,wt.RequestType
		,count(1)
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].KeepInViewRequest lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
	WHERE wt.EmpId = @empid
		AND wt.currentstatus = 6
		AND (
			DATEDIFF(MM, lr.FromDate, @fromdate) = 0
			OR DATEDIFF(MM, lr.ToDate, @todate) = 0
			)
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.RequestType

	INSERT INTO #TotalDaysRequestInMonth (
		empid
		,RequestType
		,TotalDaysRequest
		)
	SELECT wt.empid
		,wt.RequestType
		,count(1)
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].[InoutMissRequest] lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
		INNER JOIN Employee ee WITH (NOLOCK) ON ee.EmpId = wt.EmpId
	WHERE wt.currentstatus = 6
		AND ee.GroupId IN (
			SELECT GroupId
			FROM #GroupsList
			)
		AND (
			DATEDIFF(MM, lr.[OnDate], @fromdate) = 0
			OR DATEDIFF(MM, lr.[OnDate], @todate) = 0
			)
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.empid
		,wt.RequestType

	INSERT INTO #RequestInDaily (
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
		,inout.FromDate
		,inout.ToDate
		,0
		,CASE 
			WHEN wt.RequestType = 3 THEN inout.LateInMins
			ELSE inout.EarlyOutMins
			END
		,CASE 
			WHEN wt.RequestType = 3 THEN att.LateIn
			ELSE att.EarlyOut
			END
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) ON wt.RequestId = inout.RequestId
		LEFT JOIN [dbo].[EmployeeAttendance] AS att WITH (NOLOCK) ON inout.EmpId = att.EmpId
		AND inout.FromDate = att.AttendanceDate
	WHERE wt.RequestType IN (3, 4)
		AND wt.CreatedDate >= @fromdate
		AND wt.CreatedDate <= @todate
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)

	INSERT INTO #RequestInDaily (
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
		INNER JOIN [dbo].LeaveRequest AS lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
		LEFT JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
	WHERE (
			wt.CreatedDate BETWEEN @fromdate
				AND @todate
			OR wt.ModifiedDate BETWEEN @fromdate
				AND @todate
			)
		AND lr.TotalDays = 0.5
		AND ldt.IsHalfDay = 1
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)

	INSERT INTO #RequestInDaily (
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
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].LeaveRequest lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
	WHERE (
			wt.CreatedDate BETWEEN @fromdate
				AND @todate
			OR wt.ModifiedDate BETWEEN @fromdate
				AND @todate
			)
		AND lr.TotalDays <> 0.5
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)

	INSERT INTO #RequestInDaily (
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
		)
	SELECT wt.RequestID
		,wt.EmpId
		,wt.RequestType
		,lr.FromDate
		,lr.FromDate
		,lr.ToDate
		,lr.FromTime
		,lr.ToTime
		,CASE 
			WHEN lr.IsHalfDay = 0 THEN 1
			ELSE 0.5
			END TotalDays
		,lr.ShiftId
		,lr.ShiftId
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].KeepInViewRequest lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
	WHERE wt.CreatedDate >= @fromdate
		AND wt.CreatedDate <= @todate
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)

	INSERT INTO #RequestInDaily (
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
		,lr.CheckTime
		,lr.OnDate
		,lr.OnDate
		,1 TotalDays
		,ec.ShiftId
		,CASE 
			WHEN ISNULL(att.TotalWorkHours, 0) = 0 THEN 1
			ELSE 0
			END
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].[InoutMissRequest] lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
		LEFT JOIN dbo.EmployeeCalendar ec WITH (NOLOCK) ON ec.WorkDate = lr.OnDate
		AND ec.EmpId = lr.EmpId
		LEFT JOIN dbo.EmployeeAttendance AS att WITH (NOLOCK) ON lr.OnDate = att.AttendanceDate
		AND lr.EmpId = att.EmpId
	WHERE wt.CreatedDate >= @fromdate
		AND wt.CreatedDate <= @todate
		AND wt.CurrentStatus = 4
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)

	INSERT INTO #RequestInDaily (
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
		,lr.CheckTime
		,lr.OnDate
		,lr.OnDate
		,1 TotalDays
		,ec.ShiftId
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].[InoutMissRequest] lr WITH (NOLOCK) ON wt.RequestId = lr.RequestId
		LEFT JOIN dbo.EmployeeCalendar ec WITH (NOLOCK) ON ec.WorkDate = lr.OnDate
		AND ec.EmpId = lr.EmpId
	WHERE wt.CreatedDate >= @fromdate
		AND wt.CreatedDate <= @todate
		AND wt.CurrentStatus <> 4
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)

	INSERT INTO #RequestInDaily (
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
	FROM dbo.WorkingTimeRequest wt WITH (NOLOCK)
		INNER JOIN [dbo].ChangeShiftRequest cs WITH (NOLOCK) ON wt.RequestId = cs.RequestId
		INNER JOIN [dbo].ChangeShiftRequestDetail csd WITH (NOLOCK) ON csd.RequestId = cs.RequestId
	WHERE wt.CreatedDate >= @fromdate
		AND wt.CreatedDate <= @todate
		AND EXISTS (
			SELECT 1
			FROM #EmployeeId
			WHERE EmpId = wt.EmpId
			)
	GROUP BY wt.RequestID
		,wt.EmpId
		,wt.RequestType
		,cs.FromDate
		,cs.ToDate

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
		,lv.LeaveTypeId
		,lv.NAME AS LeaveTypeName
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
					ELSE s1.NAME END
					)
			ELSE s3.NAME END
			) AS FromShiftName
		,s2.ShiftID AS ToShiftId
		,(CASE 
			WHEN s2.ShiftID IS NULL THEN 'OFF'
			ELSE s2.NAME END
			) AS ToShiftName
		,s2.ShiftID AS ToShiftId
		,s2.NAME AS ToShiftName
		,rd.InOutMins AS TotalMinutes
		,rd.ActualInOutMins
		,r.CreatedDate
		,e.EmpId AS RequesterId
		,e.NickName AS RequesterNickName
		,e.FullName AS RequesterFullName
		,g.GroupId
		,g.Code AS GroupCode
		,g.NAME AS GroupName
		,ap_V.EmpId AS VerifierId
		,ap_V.NickName AS VerifierNickName
		,ap_V.FullName AS VerifierFullName
		,r.VerifiedDate
		,r.VerifiedStatus
		,ap_A.EmpId AS ApproverId
		,ap_A.NickName AS ApproverNickName
		,ap_A.FullName AS ApproverFullName
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
	FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
		INNER JOIN #RequestInDaily rd ON r.RequestId = rd.RequestID
		LEFT JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
		LEFT JOIN [dbo].LeaveRequest lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
		LEFT JOIN [dbo].[InoutMissRequest] inoutmiss WITH (NOLOCK) ON r.RequestId = inoutmiss.RequestId
		LEFT JOIN [dbo].ChangeShiftRequest csr WITH (NOLOCK) ON r.RequestId = csr.RequestId
		LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
		LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
		LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
		LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
		LEFT JOIN #TotalDaysRequestInMonth rm ON r.empID = rm.empid
		AND r.RequestType = rm.RequestType
		LEFT JOIN Employee e WITH (NOLOCK) ON e.empID = r.empID
		LEFT JOIN dbo.EmployeeSummary e_s WITH (NOLOCK) ON e_s.EmpId = e.EmpId
		LEFT JOIN [dbo].EmployeeCalendar C WITH (NOLOCK) ON c.empid = r.empid
		AND c.WorkDate = convert(VARCHAR(10), rd.FromDate, 111)
		LEFT JOIN dbo.[Shift] s1 WITH (NOLOCK) ON s1.ShiftID = rd.FromShift
		LEFT JOIN dbo.[Shift] s2 WITH (NOLOCK) ON s2.ShiftID = rd.ToShift
		LEFT JOIN dbo.[Shift] s3 WITH (NOLOCK) ON s3.ShiftID = c.ShiftID
		LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		LEFT JOIN dbo.LeaveType lv WITH (NOLOCK) ON lv.LeaveTypeID = lr.LeaveType
	WHERE (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND (
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
			EXISTS (
				SELECT 1
				FROM #EmployeeId
				WHERE EmpId = r.EmpId
				)
			)
		AND (
			@excludedEmpID IS NULL
			OR r.empid <> @excludedEmpID
			)
		AND (
			ISNULL(@status, '') = ''
			OR r.currentstatus IN (
				SELECT Currentstatus
				FROM #status
				)
			)
		AND (
			e.GroupId IN (
				SELECT GroupId
				FROM #GroupsList
				)
			)
		AND (
			@isLocal IS NULL
			OR e_s.IsLocal = @isLocal
			)

	ORDER BY r.CreatedDate DESC

	DROP TABLE #GroupsList

	DROP TABLE #TotalDaysRequestInMonth

	DROP TABLE #RequestInDaily

	DROP TABLE #status
END
