


-- [WorkingTimeRequest_GetRequestHistory] 310,0 
CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetRequestHistory] @requestid BIGINT
	,@type INT
AS
/*
Created: Tom.Vo@2013-11-05
Task: view request history
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
	@Type: 0 -- view all; 1: checkin/out type; 2: latein/early out; 3: change shift; 4: leave;5: Keep In View
*/
BEGIN
	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@TotalDay INT
		,@RequestType INT
	DECLARE @Request TABLE (
		RequestId BIGINT
		,RequestType SMALLINT
		,[Status] TINYINT
		,FromDate DATETIME
		,ToDate DATETIME
		,TotalDays DECIMAL(18, 2)
		,IsHalfDay BIT
		,FromTime DATETIME
		,ToTime DATETIME
		,LeaveTypeId INT
		,LeaveTypeName NVARCHAR(100)
		,Remark NVARCHAR(2000)
		,TotalDaysRequestInMonth DECIMAL(2, 1)
		,RefNo NVARCHAR(200)
		,CreatedDate DATETIME
		,RequesterId INT
		,RequesterNickName NVARCHAR(20)
		,RequesterFullName NVARCHAR(50)
		,GroupId INT
		,GroupCode NVARCHAR(20)
		,GroupName NVARCHAR(150)
		,ApprovedDate DATETIME
		,VerifiedDate DATETIME
		,ProcessedDate DATETIME
		,VerifiedStatus INT
		,ApprovedStatus INT
		,ProcessedStatus INT
		,ModifiedDate DATETIME
		,ModifiedById INT
		,ModifiedByNickName NVARCHAR(20)
		,ModifiedByFullName NVARCHAR(50)
		,VerifierId INT
		,VerifierNickName NVARCHAR(20)
		,VerifierFullName NVARCHAR(50)
		,ApproverId INT
		,ApproverNickName NVARCHAR(20)
		,ApproverFullName NVARCHAR(50)
		,ProcessedId INT
		,ProcessedNickName NVARCHAR(20)
		,ProcessedFullName NVARCHAR(50)
		,FromShiftId INT
		,FromShiftName NVARCHAR(100)
		,ToShiftId INT
		,ToShiftName NVARCHAR(100)
		,RequestedDate DATETIME
		,TotalMinutes INT
		,IsWarning BIT
		,ReworkCount INT
		,InsertedDate DATETIME
		)

	EXEC dbo.WorkingTimeRequest_GetRangeDateByRequestID @requestid = @requestid
		,@FromDate = @FromDate OUTPUT
		,@ToDate = @ToDate OUTPUT
		,@TotalDay = @TotalDay OUTPUT
		,@RequestType = @RequestType OUTPUT

	IF (@type = 1) -- view CheckinOutType
	BEGIN
		INSERT INTO @Request (
			RequestId
			,RequestType
			,FromShiftId
			,FromShiftName
			,FromDate
			,ToDate
			,RequestedDate
			,Remark
			,[Status]
			,RefNo
			,CreatedDate
			,RequesterId
			,RequesterNickName
			,RequesterFullName
			,GroupId
			,GroupCode
			,GroupName
			,VerifierId
			,VerifierNickName
			,VerifierFullName
			,VerifiedDate
			,VerifiedStatus
			,ApproverId
			,ApproverNickName
			,ApproverFullName
			,ApprovedDate
			,ApprovedStatus
			,ProcessedId
			,ProcessedNickName
			,ProcessedFullName
			,ProcessedDate
			,ProcessedStatus
			,ModifiedById
			,ModifiedByNickName
			,ModifiedByFullName
			,ModifiedDate
			,InsertedDate
			)
		SELECT r.RequestId
			,r.RequestType
			,s.ShiftID AS FromShiftId
			,s.NAME
			,inout.OnDate AS FromDate
			,inout.OnDate AS ToDate
			,inout.CheckTime AS RequestedDate
			,r.Remark
			,r.currentstatus
			,inout.RefNo
			,r.CreatedDate
			,e.EmpId AS RequesterId
			,e.NickName
			,e.FullName
			,g.GroupId
			,g.Code
			,g.NAME
			,ap_V.EmpId AS VerifierId
			,ap_V.NickName
			,ap_V.FullName
			,r.VerifiedDate
			,r.VerifiedStatus
			,ap_A.EmpId AS ApproverId
			,ap_A.NickName
			,ap_A.FullName
			,r.ApprovedDate
			,r.ApprovedStatus
			,ap_H.EmpId AS ProcessedId
			,ap_H.NickName
			,ap_H.FullName
			,r.HRDate
			,r.HRStatus
			,r.ModifiedBy ModifiedById
			,e_m.NickName
			,e_m.FullName
			,r.ModifiedDate
			,r.InsertedDate
		FROM dbo.WorkingTimeRequestHistory r WITH (NOLOCK)
			LEFT JOIN dbo.InoutMissRequest inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
			LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
			LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
			LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
			LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
			LEFT JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid = r.empid
			AND c.WorkDate = convert(VARCHAR(10), inout.OnDate, 111)
			LEFT JOIN Shift s WITH (NOLOCK) ON s.ShiftID = c.ShiftID
			LEFT JOIN Employee e WITH (NOLOCK) ON e.empID = r.empID
			LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		WHERE r.RequestId = @requestid
		ORDER BY InsertedDate
	END
	ELSE IF (@type = 2) -- view LateInEarlyOutType
	BEGIN
		INSERT INTO @Request (
			RequestId
			,RequestType
			,FromShiftId
			,FromShiftName
			,FromDate
			,ToDate
			,RequestedDate
			,TotalMinutes
			,Remark
			,[Status]
			,RefNo
			,CreatedDate
			,RequesterId
			,RequesterNickName
			,RequesterFullName
			,GroupId
			,GroupCode
			,GroupName
			,VerifierId
			,VerifierNickName
			,VerifierFullName
			,VerifiedDate
			,VerifiedStatus
			,ApproverId
			,ApproverNickName
			,ApproverFullName
			,ApprovedDate
			,ApprovedStatus
			,ProcessedId
			,ProcessedNickName
			,ProcessedFullName
			,ProcessedDate
			,ProcessedStatus
			,ModifiedById
			,ModifiedByNickName
			,ModifiedByFullName
			,ModifiedDate
			,InsertedDate
			)
		SELECT r.RequestId
			,r.RequestType
			,s.ShiftID AS FromShiftId
			,s.NAME
			,inout.FromDate
			,inout.ToDate
			,inout.FromDate AS RequestedDate
			,CASE 
				WHEN RequestType = 3 THEN inout.LateInMins
				ELSE inout.EarlyOutMins
				END AS TotalMinutes
			,r.Remark
			,r.currentstatus
			,inout.RefNo
			,r.CreatedDate
			,e.EmpId AS RequesterId
			,e.NickName
			,e.FullName
			,g.GroupId
			,g.Code
			,g.NAME
			,ap_V.EmpId AS VerifierId
			,ap_V.NickName
			,ap_V.FullName
			,r.VerifiedDate
			,r.VerifiedStatus
			,ap_A.EmpId AS ApproverId
			,ap_A.NickName
			,ap_A.FullName
			,r.ApprovedDate
			,r.ApprovedStatus
			,ap_H.EmpId AS ProcessedId
			,ap_H.NickName
			,ap_H.FullName
			,r.HRDate
			,r.HRStatus
			,r.ModifiedBy AS ModifiedById
			,e_m.NickName
			,e_m.FullName
			,r.ModifiedDate
			,r.InsertedDate
		FROM dbo.WorkingTimeRequestHistory r WITH (NOLOCK)
			LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
			LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
			LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
			LEFT JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
			LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
			JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid = r.empid
			AND c.WorkDate = convert(VARCHAR(10), inout.FromDate, 111)
			LEFT JOIN Shift s WITH (NOLOCK) ON s.ShiftID = c.ShiftID
			JOIN Employee e WITH (NOLOCK) ON e.empID = r.empID
			LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		WHERE r.RequestId = @requestid
		ORDER BY InsertedDate
	END
	ELSE IF (@type = 3) -- view ChangeShiftType
	BEGIN
		INSERT INTO @Request (
			RequestId
			,RequestType
			,FromDate
			,ToDate
			,RequestedDate
			,Remark
			,[Status]
			,CreatedDate
			,RequesterId
			,RequesterNickName
			,RequesterFullName
			,GroupId
			,GroupCode
			,GroupName
			,VerifierId
			,VerifierNickName
			,VerifierFullName
			,VerifiedDate
			,VerifiedStatus
			,ApproverId
			,ApproverNickName
			,ApproverFullName
			,ApprovedDate
			,ApprovedStatus
			,ProcessedId
			,ProcessedNickName
			,ProcessedFullName
			,ProcessedDate
			,ProcessedStatus
			,ModifiedById
			,ModifiedByNickName
			,ModifiedByFullName
			,ModifiedDate
			,InsertedDate
			)
		SELECT r.RequestId
			,r.RequestType
			,cs.FromDate
			,cs.ToDate
			,cs.FromDate AS RequestedDate
			,r.Remark
			,r.currentstatus
			,r.CreatedDate
			,e.EmpId AS RequesterId
			,e.NickName
			,e.FullName
			,g.GroupId
			,g.Code
			,g.NAME
			,ap_V.EmpId AS VerifierId
			,ap_V.NickName
			,ap_V.FullName
			,r.VerifiedDate
			,r.VerifiedStatus
			,ap_A.EmpId AS ApproverId
			,ap_A.NickName
			,ap_A.FullName
			,r.ApprovedDate
			,r.ApprovedStatus
			,ap_H.EmpId AS ProcessedId
			,ap_H.NickName
			,ap_H.FullName
			,r.HRDate
			,r.HRStatus
			,r.ModifiedBy ModifiedById
			,e_m.NickName
			,e_m.FullName
			,r.ModifiedDate
			,r.InsertedDate
		FROM dbo.WorkingTimeRequestHistory r WITH (NOLOCK)
			LEFT JOIN [dbo].ChangeShiftRequest cs WITH (NOLOCK) ON r.RequestId = cs.RequestId
			LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
			LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
			LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
			LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
			LEFT JOIN [dbo].Employee e WITH (NOLOCK) ON e.empID = r.empID
			LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		WHERE r.RequestID = @RequestID
		ORDER BY r.InsertedDate
	END
	ELSE IF (@type = 4) -- view LeaveType
	BEGIN
		INSERT INTO @Request (
			RequestId
			,RequestType
			,FromDate
			,ToDate
			,RequestedDate
			,FromTime
			,ToTime
			,TotalDays
			,IsHalfDay
			,LeaveTypeId
			,LeaveTypeName
			,Remark
			,RefNo
			,[Status]
			,CreatedDate
			,RequesterId
			,RequesterNickName
			,RequesterFullName
			,GroupId
			,GroupCode
			,GroupName
			,VerifierId
			,VerifierNickName
			,VerifierFullName
			,VerifiedDate
			,VerifiedStatus
			,ApproverId
			,ApproverNickName
			,ApproverFullName
			,ApprovedDate
			,ApprovedStatus
			,ProcessedId
			,ProcessedNickName
			,ProcessedFullName
			,ProcessedDate
			,ProcessedStatus
			,ModifiedById
			,ModifiedByNickName
			,ModifiedByFullName
			,ModifiedDate
			,InsertedDate
			)
		SELECT DISTINCT r.RequestId
			,r.RequestType
			,lrh.FromDate
			,lrh.ToDate
			,lrh.FromDate AS RequestedDate
			,lrh.FromDate AS FromTime
			,lrh.ToDate AS ToTime
			,lrh.TotalDays
			,(CASE 
				WHEN lrh.TotalDays = 0.5 THEN 1
				ELSE 0 END
				) AS IsHalfDay
			,lt.LeaveTypeId
			,lt.NAME AS LeaveTypeName
			,r.Remark
			,lrh.RefNo
			,r.currentstatus
			,r.CreatedDate
			,e.EmpId AS RequesterId
			,e.NickName
			,e.FullName
			,g.GroupId
			,g.Code
			,g.NAME
			,ap_V.EmpId AS VerifierId
			,ap_V.NickName
			,ap_V.FullName
			,r.VerifiedDate
			,r.VerifiedStatus
			,ap_A.EmpId AS ApproverId
			,ap_A.NickName
			,ap_A.FullName
			,r.ApprovedDate
			,r.ApprovedStatus
			,ap_H.EmpId AS ProcessedId
			,ap_H.NickName
			,ap_H.FullName
			,r.HRDate
			,r.HRStatus
			,r.ModifiedBy AS ModifiedById
			,e_m.NickName
			,e_m.FullName
			,r.ModifiedDate
			,r.InsertedDate
		FROM dbo.WorkingTimeRequestHistory r WITH (NOLOCK)
			INNER JOIN (
			SELECT lr.[RequestId]
				,lr.FromDate
				,lr.ToDate
				,lr.TotalDays
				,lr.RefNo
				,lr.LeaveType
				,lr.[ReworkCount]
			FROM dbo.LeaveRequest lr WITH (NOLOCK)
			
			UNION ALL
			SELECT lr.[RequestId]
				,lr.FromDate
				,lr.ToDate
				,lr.TotalDays
				,lr.RefNo
				,lr.LeaveType
				,lr.[ReworkCount]
			FROM dbo.LeaveRequestHistory lr WITH (NOLOCK)
			) lrh ON r.RequestId = lrh.RequestId
			AND r.ReWorkCount = lrh.ReWorkCount
			LEFT JOIN dbo.LeaveType lt WITH (NOLOCK) ON lrh.LeaveType = lt.LeaveTypeId
			LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
			LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
			LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
			LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
			LEFT JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid = r.empid
			AND c.WorkDate = convert(VARCHAR(10), lrh.FromDate, 111)
			LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.ShiftID = c.ShiftID
			LEFT JOIN dbo.Employee e WITH (NOLOCK) ON e.empID = r.empID
			LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		WHERE r.RequestID = @RequestID
		ORDER BY r.InsertedDate
	END
	ELSE
	BEGIN
		INSERT INTO @Request (
			RequestId
			,RequestType
			,FromShiftId
			,FromShiftName
			,FromDate
			,ToDate
			,RequestedDate
			,IsHalfDay
			,FromTime
			,ToTime
			,Remark
			,RefNo
			,[Status]
			,CreatedDate
			,RequesterId
			,RequesterNickName
			,RequesterFullName
			,GroupId
			,GroupCode
			,GroupName
			,VerifierId
			,VerifierNickName
			,VerifierFullName
			,VerifiedDate
			,VerifiedStatus
			,ApproverId
			,ApproverNickName
			,ApproverFullName
			,ApprovedDate
			,ApprovedStatus
			,ProcessedId
			,ProcessedNickName
			,ProcessedFullName
			,ProcessedDate
			,ProcessedStatus
			,ModifiedById
			,ModifiedByNickName
			,ModifiedByFullName
			,ModifiedDate
			,InsertedDate
			)
		SELECT r.RequestId
			,RequestType
			,s.ShiftID AS FromShiftId
			,s.NAME
			,kiv.FromDate
			,kiv.ToDate
			,kiv.FromDate AS RequestedDate
			,kiv.IsHalfDay
			,kiv.[FromTime]
			,kiv.[ToTime]
			,r.Remark
			,kiv.RefNo
			,r.currentstatus
			,r.CreatedDate
			,e.EmpId AS RequesterId
			,e.NickName
			,e.FullName
			,g.GroupId
			,g.Code
			,g.NAME
			,ap_V.EmpId AS VerifierId
			,ap_V.NickName
			,ap_V.FullName
			,r.VerifiedDate
			,r.VerifiedStatus
			,ap_A.EmpId AS ApproverId
			,ap_A.NickName
			,ap_A.FullName
			,r.ApprovedDate
			,r.ApprovedStatus
			,ap_H.EmpId AS ProcessedId
			,ap_H.NickName
			,ap_H.FullName
			,r.HRDate
			,r.HRStatus
			,r.ModifiedBy AS ModifiedById
			,e_m.NickName
			,e_m.FullName
			,r.ModifiedDate
			,r.InsertedDate
		FROM dbo.WorkingTimeRequestHistory r WITH (NOLOCK)
			INNER JOIN [dbo].[KeepInViewRequest] kiv WITH (NOLOCK) ON r.RequestId = kiv.RequestId
			LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
			LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
			LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
			LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
			LEFT JOIN Employee e WITH (NOLOCK) ON e.empID = r.empID
			LEFT JOIN [dbo].EmployeeCalendar C WITH (NOLOCK) ON c.empid = r.empid
			AND c.WorkDate BETWEEN kiv.FromDate
				AND kiv.ToDate
			LEFT JOIN [dbo].[Shift] s WITH (NOLOCK) ON s.ShiftID = kiv.ShiftID
			LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		WHERE r.RequestId = @requestid
		ORDER BY r.InsertedDate
	END

	CREATE TABLE #GeneralLeaveDetail (
		RequestId BIGINT
		,LeaveCountDetails NVARCHAR(MAX)
		)

	INSERT INTO #GeneralLeaveDetail (
		RequestId
		,LeaveCountDetails
		)
	SELECT DISTINCT rd.RequestId
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
	FROM dbo.LeaveRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequestDetail AS rd WITH (NOLOCK) ON r.RequestId = rd.RequestId
		INNER JOIN dbo.LeaveType AS lt WITH (NOLOCK) ON lt.LeaveTypeId = rd.LeaveType
	WHERE r.RequestId = @RequestId

	SELECT r.RequestId
		,RequestType
		,[Status]
		,FromDate
		,ToDate
		,TotalDays
		,IsHalfDay
		,FromTime
		,ToTime
		,LeaveTypeId
		,LeaveTypeName
		,Remark
		,TotalDaysRequestInMonth
		,RefNo
		,CreatedDate
		,RequesterId
		,RequesterNickName
		,RequesterFullName
		,GroupId
		,GroupCode
		,GroupName
		,ApprovedDate
		,VerifiedDate
		,ProcessedDate
		,VerifiedStatus
		,ApprovedStatus
		,ProcessedStatus
		,ModifiedDate
		,ModifiedById
		,ModifiedByNickName
		,ModifiedByFullName
		,VerifierId
		,VerifierNickName
		,VerifierFullName
		,ApproverId
		,ApproverNickName
		,ApproverFullName
		,ProcessedId
		,ProcessedNickName
		,ProcessedFullName
		,FromShiftId
		,FromShiftName
		,ToShiftId
		,ToShiftName
		,RequestedDate
		,TotalMinutes
		,0 AS ActualInOutMins
		,IsWarning
		,ReworkCount
		,gdt.LeaveCountDetails
		,e_s.AnnualLeaveBalance
	FROM @Request AS r
	INNER JOIN dbo.EmployeeSummary AS e_s WITH (NOLOCK) ON r.RequesterId = e_s.EmpId
		LEFT JOIN #GeneralLeaveDetail AS gdt ON r.RequestId = gdt.RequestId
	ORDER BY InsertedDate
END