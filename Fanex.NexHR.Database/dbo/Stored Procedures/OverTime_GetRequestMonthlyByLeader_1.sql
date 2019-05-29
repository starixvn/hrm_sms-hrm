
CREATE PROCEDURE [dbo].[OverTime_GetRequestMonthlyByLeader] @SupervisorID INT = NULL
	,@empid INT = NULL
	,@groupids VARCHAR(max) = ''
	,@fromdate DATETIME
	,@todate DATETIME
	,@status VARCHAR(max) = NULL
	,@excludedEmpID INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160708Quinn.Vo
Task:Get Request By leader For Overtime
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (
			@fromdate IS NOT NULL
			AND @todate IS NOT NULL
			)
	BEGIN
		SELECT @fromdate = CONVERT(VARCHAR(10), @fromDate, 111) + ' 00:00:00'
			,@todate = CONVERT(VARCHAR(10), @toDate, 111) + ' 23:59:59'
	END

	CREATE TABLE #status (Currentstatus TINYINT)

	CREATE TABLE #tblEmployeeId (EmpId INT)

	IF (@empid IS NULL)
	BEGIN
		INSERT INTO #tblEmployeeId (EmpId)
		SELECT EmpId
		FROM dbo.[fnc_GetEmployeeByActorId](@SupervisorId, 1, 0)
	END
	ELSE
	BEGIN
		INSERT INTO #tblEmployeeId (EmpID)
		VALUES (@empid)
	END

	IF ISNULL(@status, '') = ''
	BEGIN
		SET @status = '1,2,3,4,5,6,7'
	END
	ELSE IF (@status = '-1')
	BEGIN
		SET @status = '1,2'
	END

	INSERT INTO #status (Currentstatus)
	SELECT val
	FROM fnc_strSplit(@status, ',')

	CREATE TABLE #GroupsList (GroupId INT)

	SET @groupids = ISNULL(@groupids, '')

	IF @groupids <> ''
		INSERT INTO #GroupsList (GroupId)
		SELECT val
		FROM fnc_strSplit(@groupids, ',')
	ELSE
	BEGIN
		INSERT INTO #GroupsList (GroupId)
		SELECT GRoupID
		FROM [Group] g WITH (NOLOCK)
	END

	SELECT r.RequestId
		,r.empID
		,r.Month AS RequestDate
		,r.CreatedDate
		,r.VerifiedDate
		,r.currentstatus
		,r.VerifiedStatus
		,r.ApprovedDate
		,r.ApprovedStatus
		,r.HRDate
		,r.HRStatus
		,r.[NormalHours]
		,r.[OffHours]
		,r.[PublicHours]
		,(r.[NormalHours] + r.[OffHours] + r.[PublicHours]) ToTalHours
		,NULL Remark
		,r.ModifiedDate
		,CONVERT(BIT, CASE 
				WHEN OTD.RequestId IS NOT NULL
					THEN 1
				ELSE 0
				END) IsShowWarning
		,e.EmpId AS RequesterID
		,e.NickName AS Requester
		,e.FullName AS FullNameRequester
		,g.GroupId AS GroupId
		,g.NAME AS Team
		,g.Code AS TeamCode
		,ap_V.EmpId AS VerifierID
		,ap_V.NickName AS VerifierBy
		,ap_V.FullName AS FullNameVerifierBy
		,ap_A.EmpId AS ApproverID
		,ap_A.NickName AS ApproverBy
		,ap_A.FullName AS FullNameApproverBy
		,ap_H.EmpId AS HRID
		,ap_H.NickName AS HRBy
		,ap_H.FullName AS FullNameHRBy
		,r.ModifiedBy ModifiedByID
		,e_m.NickName AS ModifiedBy
		,e_m.FullName AS FullNameModified
	FROM [dbo].[OTMonthlyRequest] r WITH (NOLOCK)
	LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
	LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
	LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
	LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
	LEFT JOIN Employee e WITH (NOLOCK) ON e.empID = r.empID
	LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
	LEFT JOIN (
		SELECT DISTINCT ot.RequestId
		FROM dbo.OTDailyRequest ot WITH (NOLOCK)
		LEFT JOIN dbo.Shift s WITH (NOLOCK) ON ot.Schedule = s.ShiftID
		LEFT JOIN dbo.EmployeeAttendance at WITH (NOLOCK) ON at.AttendanceDate = ot.OvertimeDate
			AND at.EmpId = ot.EmpId
		WHERE ot.TotalHours - ISNULL(at.LateIn, 0) / 60 >= s.TotalHours + 1
		) OTD ON OTD.RequestId = r.RequestId
	WHERE (
			e.isDeleted IS NULL
			OR e.isDeleted = 0
			)
		AND (
			(
				@fromdate IS NULL
				AND @todate IS NULL
				)
			OR (
				r.Month BETWEEN @fromdate
					AND @todate
				)
			)
		AND (
			@empid IS NULL
			OR r.EmpId = @empid
			)
		AND (
			@excludedEmpID IS NULL
			OR r.EmpId <> @excludedEmpID
			)
		AND r.currentstatus IN (
			SELECT Currentstatus
			FROM #status
			)
		AND (
			ISNULL(@groupids, '') = ''
			OR e.GroupId IN (
				SELECT GroupId
				FROM #GroupsList
				)
			)
		AND (
			EXISTS (
				SELECT 1
				FROM #tblEmployeeId
				WHERE EmpId = r.EmpId
				)
			OR r.VerifierId = @SupervisorID
			OR r.ApproverId = @SupervisorID
			)

	SELECT @ErrCode = 0
		,@ErrMsg = ''

	DROP TABLE #GroupsList

	DROP TABLE #status
END