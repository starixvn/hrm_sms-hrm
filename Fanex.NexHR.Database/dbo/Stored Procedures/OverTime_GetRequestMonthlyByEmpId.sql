
CREATE PROCEDURE [dbo].[OverTime_GetRequestMonthlyByEmpId] @Empid INT
	,@Year INT
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
	SET NOCOUNT ON;

	DECLARE @fromdate DATETIME
		,@todate DATETIME
		,@Month DATETIME

	IF YEAR(GETDATE()) = @Year
		SELECT @Month = CONVERT(VARCHAR(10), @Year) + '/' + CONVERT(VARCHAR(10), MONTH(GETDATE())) + + '/' + CONVERT(VARCHAR(10), DAY(GETDATE())) + ' 00:00:00'
	ELSE
		SELECT @Month = CONVERT(VARCHAR(10), @Year) + '/12/01 00:00:00'

	SELECT @Month = CASE 
			WHEN DAY(GETDATE()) BETWEEN 10
					AND 31 THEN DATEADD(MONTH, 1, @Month)
			ELSE @Month
			END

	IF YEAR(@Month) > @Year
		SELECT @Month = CONVERT(VARCHAR(10), @Year) + '/12/01 00:00:00'

	SELECT @fromDate = CONVERT(VARCHAR(10), @Year) + '/01/01 00:00:00'
		,@toDate = @Month

	CREATE TABLE #Calendar (
		[Month] DATETIME
		,[Enabled] BIT
		,IsDeleted BIT
		)

	WHILE (CONVERT(DATETIME, @fromDate) <= CONVERT(DATETIME, @toDate))
	BEGIN
		INSERT INTO #Calendar (
			[Month]
			,[Enabled]
			,IsDeleted
			)
		VALUES (
			@fromDate
			,1
			,0
			)

		SET @fromDate = DATEADD(MONTH, 1, @fromDate)
	END

	SELECT @fromDate = CONVERT(VARCHAR(10), @Year) + '/01/01 00:00:00'

	SELECT r.RequestId
		,r.empID
		,cl.Month AS RequestDate
		,r.CreatedDate
		,r.VerifiedDate
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
		,@empid AS RequesterID
		,e.NickName AS Requester
		,e.FullName AS FullNameRequester
		,g.GroupId AS GroupId
		,g.NAME AS Team
		,g.Code AS TeamCode
		,ap_V.EmpId AS VerifierID
		,ap_V.NickName AS NickNameVerifier
		,ap_V.FullName AS FullNameVerifierBy
		,ap_A.EmpId AS ApproverID
		,ap_A.NickName AS NickNameApprover
		,ap_A.FullName AS FullNameApproverBy
		,8 RequestTypeId
		,r.currentstatus StatusId
		,ap_H.EmpId AS HRID
		,ap_H.NickName AS HRBy
		,ap_H.FullName AS FullNameHRBy
		,r.ModifiedBy ModifiedByID
		,e_m.NickName AS NickNameModified
		,e_m.FullName AS FullNameModified
		,r.ModifiedDate
	FROM #Calendar cl
		LEFT JOIN [dbo].[OTMonthlyRequest] r WITH (NOLOCK) ON r.Month = cl.[Month]
		AND r.Month >= @fromdate
		AND r.Month <= @todate
		AND r.empid = @empid
		LEFT JOIN dbo.Employee e WITH (NOLOCK) ON e.empID = r.empID
		LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
		LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
		LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
		LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy
		LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
	WHERE EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary WITH (NOLOCK)
			WHERE EmpId = @Empid
				AND CanOT = 1
			)
	ORDER BY cl.Month DESC

	DROP TABLE #Calendar
END



