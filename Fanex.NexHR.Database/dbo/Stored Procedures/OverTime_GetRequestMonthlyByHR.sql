
/****** Object:  Stored Procedure dbo.OverTime_GetRequestMonthlyByHR    Script Date: 7/20/2016 10:19:15 AM ******/
/****** Object:  Stored Procedure dbo.OverTime_GetRequestMonthlyByHR    Script Date: 7/19/2016 5:22:18 PM ******/
CREATE PROCEDURE [dbo].[OverTime_GetRequestMonthlyByHR] @SupervisorID INT = NULL
	,@EmpId INT = NULL
	,@GroupIds VARCHAR(max) = ''
	,@fromdate DATETIME
	,@todate DATETIME
	,@status VARCHAR(max) = NULL
	,@excludedEmpID INT = NULL
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
	IF (
			@fromdate IS NOT NULL
			AND @todate IS NOT NULL
			)
	BEGIN
		SELECT @fromdate = CONVERT(VARCHAR(10), @fromDate, 111) + ' 00:00:00'
			,@todate = CONVERT(VARCHAR(10), @toDate, 111) + ' 23:59:59'
	END

	CREATE TABLE #status (Currentstatus TINYINT)

	IF ISNULL(@status, '') = ''
	BEGIN
		SET @status = '1,2,3,4,5,6,7'
	END
	ELSE IF (@status = '-1')
	BEGIN
		SET @status = '1,2'
	END

	INSERT INTO #status (Currentstatus)
	SELECT DISTINCT val
	FROM dbo.fnc_strSplit(@status, ',')

	CREATE TABLE #Group (GroupId INT)

	SET @GroupIds = ISNULL(@GroupIds, '')

	IF @GroupIds <> ''
	BEGIN
		INSERT INTO #Group (GroupId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	END
	ELSE
	BEGIN
		INSERT INTO #Group (GroupId)
		SELECT g.GroupId
		FROM dbo.[Group] AS g WITH (NOLOCK)
		WHERE ISNULL(g.IsDeleted, 0) = 0
	END

	SELECT r.RequestId
		,r.EmpId
		,r.[Month] AS RequestDate
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
		,g.[Name] AS Team
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
	FROM [dbo].[OTMonthlyRequest] AS r WITH (NOLOCK)
	LEFT JOIN [dbo].Employee AS ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
	LEFT JOIN [dbo].Employee AS ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
	LEFT JOIN [dbo].Employee AS ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
	LEFT JOIN dbo.Employee AS e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.empID = r.empID
		AND ISNULL(e.isDeleted, 0) = 0
		AND ISNULL(e.[Enabled], 1) = 1
	LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupID = g.GroupID
		AND ISNULL(g.IsDeleted, 0) = 0
	LEFT JOIN (
		SELECT DISTINCT ot.RequestId
		FROM dbo.OTDailyRequest AS ot WITH (NOLOCK)
		LEFT JOIN dbo.Shift AS s WITH (NOLOCK) ON ot.Schedule = s.ShiftID
		LEFT JOIN dbo.EmployeeAttendance AS at WITH (NOLOCK) ON at.AttendanceDate = ot.OvertimeDate
			AND at.EmpId = ot.EmpId
		WHERE ot.TotalHours - ISNULL(at.LateIn, 0) / 60 >= s.TotalHours + 1
		) AS OTD ON OTD.RequestId = r.RequestId
	WHERE ((@fromdate IS NULL AND @todate IS NULL) OR (r.[Month] BETWEEN @fromdate
			AND @todate))
		AND (
			@EmpId IS NULL
			OR r.EmpId = @EmpId
			)
		AND (
			@excludedEmpID IS NULL
			OR r.EmpId <> @excludedEmpID
			)
		AND r.currentstatus IN (
			SELECT DISTINCT Currentstatus
			FROM #status
			)
		AND (
			ISNULL(@GroupIds, '') = ''
			OR e.GroupId IN (
				SELECT DISTINCT GroupId
				FROM #Group
				)
			)

	--AND (
	--	r.VerifierId = @SupervisorID
	--	OR r.ApproverId = @SupervisorID
	--	)
	DROP TABLE #Group

	DROP TABLE #status
END