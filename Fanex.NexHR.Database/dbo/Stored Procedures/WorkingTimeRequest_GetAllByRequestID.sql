
CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetAllByRequestID]
	@RequestId BIGINT
AS
/*
Created: Tom.Vo@2013-11-05
Task:Get Request By RequestID
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RequestType INT
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
		)

	SELECT TOP 1 @RequestType = RequestType
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestID = @RequestId

	IF @RequestType IN (3, 4)
	BEGIN
		INSERT INTO @Request (
			RequestId,
			RequestType,
			FromShiftId,
			FromShiftName,
			FromDate,
			ToDate,
			RequestedDate,
			TotalMinutes,
			Remark,
			[Status],
			RefNo,
			CreatedDate,
			RequesterId,
			RequesterNickName,
			RequesterFullName,
			GroupId,
			GroupCode,
			GroupName,
			VerifierId,
			VerifierNickName,
			VerifierFullName,
			VerifiedDate,
			VerifiedStatus,
			ApproverId,
			ApproverNickName,
			ApproverFullName,
			ApprovedDate,
			ApprovedStatus,
			ProcessedId,
			ProcessedNickName,
			ProcessedFullName,
			ProcessedDate,
			ProcessedStatus,
			ModifiedById,
			ModifiedByNickName,
			ModifiedByFullName,
			ModifiedDate,
			ReworkCount
		)
		SELECT r.RequestId
			,RequestType
			,s.ShiftId
			,s.NAME
			,inout.FromDate
			,inout.ToDate
			,inout.FromDate
			,CASE 
				WHEN RequestType = 3 THEN LateInMins
				ELSE EarlyOutMins
				END AS [Minutes]
			,Remark
			,r.currentstatus
			,inout.RefNo
			,r.CreatedDate
			,e.empID AS RequesterID
			,e.NickName
			,e.FullName
			,g.GroupId
			,g.Code
			,g.Name
			,ap_V.empID AS VerifierID
			,ap_V.NickName
			,ap_V.FullName
			,r.VerifiedDate
			,r.VerifiedStatus
			,ap_A.empID AS ApproverID
			,ap_A.NickName
			,ap_A.FullName
			,r.ApprovedDate
			,r.ApprovedStatus
			,ap_H.empID AS ProcessedId
			,ap_H.NickName
			,ap_H.FullName
			,r.HRDate
			,r.HRStatus
			,r.ModifiedBy ModifiedById
			,e_m.NickName
			,e_m.FullName
			,r.ModifiedDate
			,r.ReworkCount
		FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
			INNER JOIN [dbo].[InOutRequest] inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
			LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
			LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
			LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
			LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
			LEFT JOIN Employee e WITH (NOLOCK) ON e.empID = r.empID
			LEFT JOIN [dbo].EmployeeCalendar C WITH (NOLOCK) ON c.empid = r.empid
			AND c.WorkDate = convert(VARCHAR(10), inout.FromDate, 111)
			LEFT JOIN [dbo].[Shift] s WITH (NOLOCK) ON s.ShiftID = c.ShiftID
			LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		WHERE r.RequestID = @RequestID
	END
	ELSE IF @RequestType IN (1, 2)
	BEGIN
		INSERT INTO @Request (
			RequestId,
			RequestType,
			FromShiftId,
			FromShiftName,
			FromDate,
			ToDate,
			RequestedDate,
			Remark,
			[Status],
			RefNo,
			CreatedDate,
			RequesterId,
			RequesterNickName,
			RequesterFullName,
			GroupId,
			GroupCode,
			GroupName,
			VerifierId,
			VerifierNickName,
			VerifierFullName,
			VerifiedDate,
			VerifiedStatus,
			ApproverId,
			ApproverNickName,
			ApproverFullName,
			ApprovedDate,
			ApprovedStatus,
			ProcessedId,
			ProcessedNickName,
			ProcessedFullName,
			ProcessedDate,
			ProcessedStatus,
			ModifiedById,
			ModifiedByNickName,
			ModifiedByFullName,
			ModifiedDate,
			ReworkCount
		)
		SELECT r.RequestId
			,r.RequestType
			,s.ShiftID
			,s.NAME
			,lr.[OnDate] AS FromDate
			,lr.[OnDate] AS ToDate
			,lr.[CheckTime] AS RequestTime
			,r.Remark
			,r.currentstatus
			,lr.RefNo
			,r.CreatedDate
			,e.empID AS RequesterId
			,e.NickName
			,e.FullName
			,g.GroupId
			,g.Code
			,g.Name
			,ap_V.empID AS VerifierId
			,ap_V.NickName
			,ap_V.FullName
			,r.VerifiedDate
			,r.VerifiedStatus
			,ap_A.empID AS ApproverId
			,ap_A.NickName
			,ap_A.FullName
			,r.ApprovedDate
			,r.ApprovedStatus
			,ap_H.empID AS ProcessedId
			,ap_A.NickName
			,ap_A.FullName
			,r.HRDate
			,r.HRStatus
			,r.ModifiedBy ModifiedById
			,e_m.NickName
			,e_m.FullName
			,r.ModifiedDate
			,r.ReworkCount
		FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
			LEFT JOIN [dbo].[InoutMissRequest] lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
			LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
			LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
			LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
			LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
			LEFT JOIN dbo.EmployeeCalendar C WITH (NOLOCK) ON c.empid = r.empid
			AND c.WorkDate = convert(VARCHAR(10), lr.[OnDate], 111)
			LEFT JOIN dbo.Shift s WITH (NOLOCK) ON s.ShiftID = c.ShiftID
			LEFT JOIN dbo.Employee e WITH (NOLOCK) ON e.empID = r.empID
			LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		WHERE r.RequestID = @RequestID
	END
	ELSE IF @RequestType = 6
	BEGIN
		DECLARE @IsHalfDay BIT,
			@TotalDay DECIMAL(18,2)

		SELECT @IsHalfDay = (CASE 
				WHEN TotalDays = 0.5
					AND LeaveType <> 0 THEN 1
				ELSE 0 END
				),
				@TotalDay = TotalDays
		FROM dbo.LeaveRequest WITH (NOLOCK)
		WHERE RequestId = @RequestID

		IF (@TotalDay = 0.5)
		BEGIN
			INSERT INTO @Request (
				RequestId,
				RequestType,
				FromDate,
				ToDate,
				RequestedDate,
				FromTime,
				ToTime,
				TotalDays,
				IsHalfDay,
				LeaveTypeId,
				LeaveTypeName,
				Remark,
				RefNo,
				[Status],
				CreatedDate,
				RequesterId,
				RequesterNickName,
				RequesterFullName,
				GroupId,
				GroupCode,
				GroupName,
				VerifierId,
				VerifierNickName,
				VerifierFullName,
				VerifiedDate,
				VerifiedStatus,
				ApproverId,
				ApproverNickName,
				ApproverFullName,
				ApprovedDate,
				ApprovedStatus,
				ProcessedId,
				ProcessedNickName,
				ProcessedFullName,
				ProcessedDate,
				ProcessedStatus,
				ModifiedById,
				ModifiedByNickName,
				ModifiedByFullName,
				ModifiedDate,
				ReworkCount
			)
			SELECT r.RequestId
				,r.RequestType
				,lr.FromDate
				,lr.ToDate
				,lr.FromDate AS RequestedDate
				,CASE WHEN ldt.FromTime IS NULL THEN lr.FromDate ELSE ldt.FromTime END
				,CASE WHEN ldt.ToTime IS NULL THEN lr.ToDate ELSE ldt.ToTime END
				,lr.TotalDays
				,@IsHalfDay
				,lt.LeaveTypeId
				,lt.NAME
				,r.Remark
				,lr.RefNo
				,r.currentstatus
				,r.CreatedDate
				,e.empID AS RequesterId
				,e.NickName
				,e.FullName
				,g.GroupId
				,g.Code
				,g.Name
				,ap_V.empID AS VerifierId
				,ap_V.NickName
				,ap_V.FullName
				,r.VerifiedDate
				,r.VerifiedStatus
				,ap_A.empID AS ApproverId
				,ap_A.NickName
				,ap_A.FullName
				,r.ApprovedDate
				,r.ApprovedStatus
				,ap_H.empID AS ProcessedId
				,ap_A.NickName
				,ap_A.FullName
				,r.HRDate
				,r.HRStatus
				,r.ModifiedBy ModifiedById
				,e_m.NickName
				,e_m.FullName
				,r.ModifiedDate
				,r.ReworkCount
			FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
				LEFT JOIN [dbo].LeaveRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
				LEFT JOIN dbo.LeaveRequestDetail AS ldt WITH (NOLOCK) ON lr.RequestId = ldt.RequestId
				LEFT JOIN [dbo].[LeaveType] AS lt WITH (NOLOCK) ON lt.[LeaveTypeId] = lr.[LeaveType]
				LEFT JOIN [dbo].Employee AS ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
				LEFT JOIN [dbo].Employee AS ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
				LEFT JOIN [dbo].Employee AS ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
				LEFT JOIN Employee AS e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
				LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.empID = r.empID
				LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupID = g.GroupID
			WHERE r.RequestID = @RequestID
		END
		ELSE
		BEGIN
			INSERT INTO @Request (
				RequestId,
				RequestType,
				FromDate,
				ToDate,
				RequestedDate,
				FromTime,
				ToTime,
				TotalDays,
				IsHalfDay,
				LeaveTypeId,
				LeaveTypeName,
				Remark,
				RefNo,
				[Status],
				CreatedDate,
				RequesterId,
				RequesterNickName,
				RequesterFullName,
				GroupId,
				GroupCode,
				GroupName,
				VerifierId,
				VerifierNickName,
				VerifierFullName,
				VerifiedDate,
				VerifiedStatus,
				ApproverId,
				ApproverNickName,
				ApproverFullName,
				ApprovedDate,
				ApprovedStatus,
				ProcessedId,
				ProcessedNickName,
				ProcessedFullName,
				ProcessedDate,
				ProcessedStatus,
				ModifiedById,
				ModifiedByNickName,
				ModifiedByFullName,
				ModifiedDate,
				ReworkCount
			)
			SELECT r.RequestId
				,r.RequestType
				,lr.FromDate
				,lr.ToDate
				,lr.FromDate AS RequestedDate
				,lr.FromDate AS FromTime
				,lr.ToDate AS ToTime
				,lr.TotalDays
				,@IsHalfDay
				,lt.LeaveTypeId
				,lt.NAME AS LeaveTypeName
				,r.Remark
				,lr.RefNo
				,r.currentstatus
				,r.CreatedDate
				,e.empID AS RequesterId
				,e.NickName
				,e.FullName
				,g.GroupId
				,g.Code
				,g.Name
				,ap_V.empID AS VerifierId
				,ap_V.NickName
				,ap_V.FullName
				,r.VerifiedDate
				,r.VerifiedStatus
				,ap_A.empID AS ApproverId
				,ap_A.NickName
				,ap_A.FullName
				,r.ApprovedDate
				,r.ApprovedStatus
				,ap_H.empID AS ProcessedId
				,ap_A.NickName
				,ap_A.FullName
				,r.HRDate
				,r.HRStatus
				,r.ModifiedBy AS ModifiedById
				,e_m.NickName
				,e_m.FullName
				,r.ModifiedDate
				,r.ReworkCount
			FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
				LEFT JOIN [dbo].LeaveRequest lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
				LEFT JOIN [dbo].[LeaveType] lt WITH (NOLOCK) ON lt.[LeaveTypeId] = lr.[LeaveType]
				LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
				LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
				LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
				LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
				LEFT JOIN dbo.Employee e WITH (NOLOCK) ON e.empID = r.empID
				LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
			WHERE r.RequestID = @RequestID
		END
	END
	ELSE IF @RequestType IN (5)
	BEGIN
		DECLARE @IsOneDay BIT
		SELECT @IsOneDay = CASE WHEN DATEDIFF(d, FromDate, ToDate) = 0 THEN 1 ELSE 0 END
		FROM dbo.ChangeShiftRequest WITH (NOLOCK)
		WHERE RequestId = @RequestId

		IF(@IsOneDay = 1)
		BEGIN
			INSERT INTO @Request (
				RequestId,
				RequestType,
				FromDate,
				ToDate,
				FromShiftId,
				FromShiftName,
				ToShiftId,
				ToShiftName,
				RequestedDate,
				Remark,
				[Status],
				CreatedDate,
				RequesterId,
				RequesterNickName,
				RequesterFullName,
				GroupId,
				GroupCode,
				GroupName,
				VerifierId,
				VerifierNickName,
				VerifierFullName,
				VerifiedDate,
				VerifiedStatus,
				ApproverId,
				ApproverNickName,
				ApproverFullName,
				ApprovedDate,
				ApprovedStatus,
				ProcessedId,
				ProcessedNickName,
				ProcessedFullName,
				ProcessedDate,
				ProcessedStatus,
				ModifiedById,
				ModifiedByNickName,
				ModifiedByFullName,
				ModifiedDate,
				ReworkCount
			)
			SELECT r.RequestId
				,r.RequestType
				,cs.FromDate
				,cs.ToDate
				,fs.ShiftID AS FromShiftId
				,CASE WHEN fs.ShiftID IS NULL THEN 'OFF' ELSE fs.Name END
				,ts.ShiftID AS ToShiftId
				,CASE WHEN ts.ShiftID IS NULL THEN 'OFF' ELSE ts.Name END
				,cs.FromDate AS RequestedDate
				,r.Remark
				,r.currentstatus
				,r.CreatedDate
				,e.empID AS RequesterId
				,e.NickName
				,e.FullName
				,g.GroupId
				,g.Code
				,g.Name
				,ap_V.empID AS VerifierId
				,ap_V.NickName
				,ap_V.FullName
				,r.VerifiedDate
				,r.VerifiedStatus
				,ap_A.empID AS ApproverId
				,ap_A.NickName 
				,ap_A.FullName
				,r.ApprovedDate
				,r.ApprovedStatus
				,ap_H.empID AS ProcessedId
				,ap_H.NickName
				,ap_H.FullName
				,r.HRDate
				,r.HRStatus
				,r.ModifiedBy ModifiedById
				,e_m.NickName
				,e_m.FullName
				,r.ModifiedDate
				,r.ReworkCount
			FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
				INNER JOIN [dbo].ChangeShiftRequest AS cs WITH (NOLOCK) ON r.RequestId = cs.RequestId
				INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK) ON cs.RequestId = dt.RequestId
				LEFT JOIN dbo.[Shift] AS fs WITH (NOLOCK) ON dt.FromShift = fs.ShiftID
				LEFT JOIN dbo.[Shift] AS ts WITH (NOLOCK) ON dt.ToShift = ts.ShiftID
				LEFT JOIN [dbo].Employee AS ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
				LEFT JOIN [dbo].Employee AS ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
				LEFT JOIN [dbo].Employee AS ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
				LEFT JOIN Employee AS e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
				LEFT JOIN [dbo].Employee AS e WITH (NOLOCK) ON e.empID = r.empID
				LEFT JOIN [Group] AS g WITH (NOLOCK) ON e.GroupID = g.GroupID
			WHERE r.RequestID = @RequestID
			ORDER BY r.CreatedDate DESC
		END
		ELSE
		BEGIN
			INSERT INTO @Request (
				RequestId,
				RequestType,
				FromDate,
				ToDate,
				RequestedDate,
				Remark,
				[Status],
				CreatedDate,
				RequesterId,
				RequesterNickName,
				RequesterFullName,
				GroupId,
				GroupCode,
				GroupName,
				VerifierId,
				VerifierNickName,
				VerifierFullName,
				VerifiedDate,
				VerifiedStatus,
				ApproverId,
				ApproverNickName,
				ApproverFullName,
				ApprovedDate,
				ApprovedStatus,
				ProcessedId,
				ProcessedNickName,
				ProcessedFullName,
				ProcessedDate,
				ProcessedStatus,
				ModifiedById,
				ModifiedByNickName,
				ModifiedByFullName,
				ModifiedDate,
				ReworkCount
			)
			SELECT r.RequestId
				,r.RequestType
				,cs.FromDate
				,cs.ToDate
				,cs.FromDate AS RequestedDate
				,r.Remark
				,r.currentstatus
				,r.CreatedDate
				,e.empID AS RequesterId
				,e.NickName
				,e.FullName
				,g.GroupId
				,g.Code
				,g.Name
				,ap_V.empID AS VerifierId
				,ap_V.NickName
				,ap_V.FullName
				,r.VerifiedDate
				,r.VerifiedStatus
				,ap_A.empID AS ApproverId
				,ap_A.NickName 
				,ap_A.FullName
				,r.ApprovedDate
				,r.ApprovedStatus
				,ap_H.empID AS ProcessedId
				,ap_H.NickName
				,ap_H.FullName
				,r.HRDate
				,r.HRStatus
				,r.ModifiedBy ModifiedById
				,e_m.NickName
				,e_m.FullName
				,r.ModifiedDate
				,r.ReworkCount
			FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
				INNER JOIN [dbo].ChangeShiftRequest cs WITH (NOLOCK) ON r.RequestId = cs.RequestId
				LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
				LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
				LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
				LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
				LEFT JOIN [dbo].Employee e WITH (NOLOCK) ON e.empID = r.empID
				LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
			WHERE r.RequestID = @RequestID
			ORDER BY r.CreatedDate DESC
		END
	END
	ELSE IF @RequestType IN (7)
	BEGIN
		INSERT INTO @Request (
			RequestId,
			RequestType,
			FromShiftId,
			FromShiftName,
			FromDate,
			ToDate,
			RequestedDate,
			IsHalfDay,
			FromTime,
			ToTime,
			Remark,
			RefNo,
			[Status],
			CreatedDate,
			RequesterId,
			RequesterNickName,
			RequesterFullName,
			GroupId,
			GroupCode,
			GroupName,
			VerifierId,
			VerifierNickName,
			VerifierFullName,
			VerifiedDate,
			VerifiedStatus,
			ApproverId,
			ApproverNickName,
			ApproverFullName,
			ApprovedDate,
			ApprovedStatus,
			ProcessedId,
			ProcessedNickName,
			ProcessedFullName,
			ProcessedDate,
			ProcessedStatus,
			ModifiedById,
			ModifiedByNickName,
			ModifiedByFullName,
			ModifiedDate,
			ReworkCount
		)
		SELECT r.RequestId
			,RequestType
			,s.shiftid AS FromShiftId
			,s.NAME
			,kiv.FromDate
			,kiv.ToDate
			,kiv.FromDate AS RequestedDate
			,kiv.IsHalfDay
			,kiv.[FromTime]
			,kiv.[ToTime]
			,Remark
			,kiv.RefNo
			,currentstatus
			,r.CreatedDate
			,e.EmpId AS RequesterId
			,e.NickName
			,e.FullName
			,g.GroupId
			,g.Code
			,g.Name
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
			,r.ReworkCount
		FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
			INNER JOIN [dbo].[KeepInViewRequest] kiv WITH (NOLOCK) ON r.RequestId = kiv.RequestId
			LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
			LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
			LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
			LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
			LEFT JOIN Employee e WITH (NOLOCK) ON e.empID = r.empID
			LEFT JOIN [dbo].EmployeeCalendar C WITH (NOLOCK) ON c.empid = r.empid
			AND c.WorkDate BETWEEN kiv.FromDate
				AND kiv.ToDate
			LEFT JOIN [dbo].[Shift] s WITH (NOLOCK) ON s.ShiftID = c.ShiftID
			LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		WHERE r.RequestID = @RequestID
		ORDER BY r.CreatedDate DESC
	END

	SELECT RequestId 
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
	FROM @Request
END
