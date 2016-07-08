
CREATE PROCEDURE [dbo].[OverTime_GetRequestMonthlyByHR] @SupervisorID INT = NULL
	,@empid INT = NULL
	,@groupids VARCHAR(max) = ''
	,@fromdate DATETIME
	,@todate DATETIME
	,@status VARCHAR(max) = NULL
	,@excludedEmpID INT = NULL
	,@role VARCHAR(max) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
	,@isDebug BIT = 0
AS
/*
Created: Tom.Vo@2013-11-04
Task:Get Request By Date For Overtime
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SELECT @fromdate = CONVERT(VARCHAR(10), @fromDate, 111) + ' 00:00:00'
		,@todate = CONVERT(VARCHAR(10), @toDate, 111) + ' 23:59:59'

	CREATE TABLE #status (Currentstatus TINYINT)

	--			FROM #RolesList
	--			WHERE roleid IN (2, 3)
	--			)
	--	BEGIN
	--		IF @role LIKE '%2%'
	--			AND @role LIKE '%3%'
	--		BEGIN
	--			SET @RoleType = 23
	IF ISNULL(@status, '') = ''
	BEGIN
		SET @status = '1,2,3,4,5,6,7'
	--			FROM #RolesList
	ELSE IF (@status = '-1')
	--			)
		SET @status = '1,2'
	--		IF @role LIKE '%2%'
	--		BEGIN
	--			SET @RoleType = 2

	--			IF ISNULL(@status, '') = ''
	--				SET @status = '1,2,3'
	--		END
	--		ELSE
	--		BEGIN
	--			SELECT @ErrCode = 102
	--				,@ErrMsg = 'You don''t have permission on this funcion.'

	--			RETURN
	--		END
	--				--SET @RoleType=1 
	--	END
	--	ELSE IF EXISTS (
	--			SELECT TOP 1 1
	--			FROM #RolesList
	--			WHERE roleid IN (3)
	--			)
	--	BEGIN
	--		IF @role LIKE '%3%'
	--		BEGIN
	--			SET @RoleType = 3

	--			IF ISNULL(@status, '') = ''
	--				SET @status = '2,4,5'
	--		END
	--		ELSE
	--		BEGIN
	--			SELECT @ErrCode = 103
	--				,@ErrMsg = 'You don''t have permission on this funcion.'

	--			RETURN
	--		END
	--				--SET @RoleType=1 
	--	END
	--	ELSE
	--		SET @RoleType = 1
	--END
	--ELSE
	--BEGIN
	--	SET @RoleType = 1
	--END
	select 1
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
				WHEN OTD.RequestId IS NOT NULL THEN 1
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
		AND r.Month >= @fromdate
		AND r.Month <= @todate
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
		--AND (
		--	r.VerifierId = @SupervisorID
		--	OR r.ApproverId = @SupervisorID
		--	)

	SELECT @ErrCode = 0
		,@ErrMsg = ''

	DROP TABLE #GroupsList

	DROP TABLE #status
END

END
