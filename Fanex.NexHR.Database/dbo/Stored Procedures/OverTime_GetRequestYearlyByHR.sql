
CREATE PROCEDURE [dbo].[OverTime_GetRequestYearlyByHR] @SupervisorId INT
	,@empid INT = NULL
	,@GroupIds VARCHAR(max) = ''
	,@Year INT
	,@status VARCHAR(max) = '4,6'
	,@ExcludedEmpId INT = NULL
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

	SELECT @fromdate = CONVERT(VARCHAR(10), @Year) + '/01/01 00:00:00'
		,@todate = CONVERT(VARCHAR(10), @Year) + '/12/31 23:59:59'

	CREATE TABLE #Status (Currentstatus TINYINT)

	IF LEN(ISNULL(@status, '')) = 0
		SET @status = '4,6'

	IF ISNULL(@status, '') <> ''
		INSERT INTO #Status (Currentstatus)
		SELECT val
		FROM fnc_strSplit(@status, ',')

	CREATE TABLE #Role (roleid TINYINT)

	DECLARE @isHR BIT

	IF @SupervisorId IS NOT NULL
	BEGIN
		INSERT INTO #Role
		SELECT RoleId
		FROM dbo.EmployeeRole WITH (NOLOCK)
		WHERE EmployeeId = @SupervisorId

		IF EXISTS (
				SELECT 1
				FROM #Role
				WHERE roleid = 6
				)
		BEGIN
			SET @isHR = 1
		END
	END

	IF EXISTS (
			SELECT 1
			FROM #Status
			WHERE Currentstatus = 1
			)
		AND EXISTS (
			SELECT 1
			FROM #Role
			WHERE roleid = 3
			)
	BEGIN
		DELETE #Status
		WHERE Currentstatus IN (2, 3)

		INSERT #Status (Currentstatus)
		VALUES (2)
			,(3)
	END

	CREATE TABLE #Group (GroupId INT)

	SET @GroupIds = ISNULL(@GroupIds, '')

	IF @GroupIds <> ''
		INSERT INTO #Group (GroupId)
		SELECT val
		FROM fnc_strSplit(@GroupIds, ',')
	ELSE
	BEGIN
		INSERT INTO #Group (GroupId)
		SELECT g.GroupId
		FROM dbo.[Group] AS g WITH (NOLOCK)
	END

	CREATE TABLE #Calendar (
		_MONTH INT
		,isDeleted BIT
		)

	DECLARE @FromMonth INT = 1
		,@ToMonth INT = 12

	WHILE (@FromMonth <= @ToMonth)
	BEGIN
		INSERT INTO #Calendar (_MONTH)
		VALUES (@FromMonth)

		SET @FromMonth = @FromMonth + 1
	END

	SELECT EmpId
		,Code
		,NickName
		,FullName
		,Team
		,TeamCode
		,ISNULL([1], 0) AS Jan
		,ISNULL([2], 0) AS Feb
		,ISNULL([3], 0) AS Mar
		,ISNULL([4], 0) AS Apr
		,ISNULL([5], 0) AS May
		,ISNULL([6], 0) AS Jun
		,ISNULL([7], 0) AS Jul
		,ISNULL([8], 0) AS Aug
		,ISNULL([9], 0) AS Sep
		,ISNULL([10], 0) AS Oct
		,ISNULL([11], 0) AS Nov
		,ISNULL([12], 0) AS [DEC]
		,(ISNULL([1], 0) + ISNULL([2], 0) + ISNULL([3], 0) + ISNULL([4], 0) + ISNULL([5], 0) + ISNULL([6], 0) + ISNULL([7], 0) + ISNULL([8], 0) + ISNULL([9], 0) + ISNULL([10], 0) + ISNULL([11], 0) + ISNULL([12], 0)) AS Total
	FROM (
		SELECT e.EmpId
			,e.Code
			,e.NickName
			,e.FullName
			,g.[Name] AS Team
			,g.Code AS TeamCode
			,cl._MONTH AS [Month]
			,(ot.[NormalHours] + ot.[OffHours] + ot.[PublicHours]) ToTalHours
		FROM #Calendar AS cl
			LEFT JOIN [dbo].[OTMonthlyRequest] AS ot WITH (NOLOCK) ON cl._MONTH = Month(Ot.Month)
			LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = ot.EmpId
			LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		WHERE ot.[Month] BETWEEN @FromDate
				AND @todate
			AND (
				@EmpId IS NULL
				OR ot.EmpId = @EmpId
				)
			AND (
				@ExcludedEmpId IS NULL
				OR ot.EmpId <> @ExcludedEmpId
				)
			AND (
				ISNULL(@status, '') = ''
				OR ot.CurrentStatus IN (
					SELECT CurrentStatus
					FROM #Status
					)
				)
			AND (
				ISNULL(@GroupIds, '') = ''
				OR e.GroupId IN (
					SELECT GroupId
					FROM #Group
					)
				)
			AND (
				@isHR = 1
				OR (
					ot.ApproverId = @SupervisorId
					OR ot.VerifierId = @SupervisorId
					)
				)
		) AS tmp
	Pivot(MAX(ToTalHours) FOR [Month] IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) AS tmp2
	WHERE EmpId IS NOT NULL

	DROP TABLE #Group

	DROP TABLE #Role

	DROP TABLE #Status

	DROP TABLE #Calendar
END
