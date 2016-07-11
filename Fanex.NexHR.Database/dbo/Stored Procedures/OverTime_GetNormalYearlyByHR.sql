CREATE PROCEDURE [dbo].[OverTime_GetNormalYearlyByHR] @empid INT = NULL
	,@groupids VARCHAR(max) = ''
	,@Year INT
	,@status VARCHAR(max) = '4,6'
	,@excludedEmpID INT = NULL
AS
/*
Created: Tom.Vo@2013-11-04
Task:Get normal calendar By year For Overtime
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

	CREATE TABLE #status (Currentstatus TINYINT)

	IF LEN(ISNULL(@status, '')) = 0
		SET @status = '4,6'

	IF ISNULL(@status, '') <> ''
		INSERT INTO #status (Currentstatus)
		SELECT DISTINCT val
		FROM fnc_strSplit(@status, ',')

	IF EXISTS (
			SELECT 1
			FROM #status
			WHERE Currentstatus = 1
			)
	BEGIN
		DELETE #status
		WHERE Currentstatus IN (2, 3)

		INSERT #status (Currentstatus)
		VALUES (2)
			,(3)
	END

	CREATE TABLE #Group (GroupId INT)

	SET @groupids = ISNULL(@groupids, '')

	IF @groupids <> ''
		INSERT INTO #Group (GroupId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@groupids, ',')
	ELSE
	BEGIN
		INSERT INTO #Group (GroupId)
		SELECT DISTINCT g.GroupId
		FROM dbo.[Group] AS g WITH (NOLOCK)
		WHERE ISNULL(g.IsDeleted, 0) = 0
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

	CREATE TABLE #OTSumaryMonthly (
		[Month] DATETIME
		,EmpId INT
		,NormalHours DECIMAL(18, 2)
		,OffHours DECIMAL(18, 2)
		,PublicHours DECIMAL(18, 2)
		)

	INSERT INTO #OTSumaryMonthly (
		[Month]
		,EmpId
		,NormalHours
		,OffHours
		,PublicHours
		)
	SELECT CAST(CAST(@Year AS VARCHAR(4)) + '/' + CAST(MONTH(OvertimeDate) AS VARCHAR(2)) + '/01' AS DATETIME)
		,dl.EmpId
		,SUM(NormalHours)
		,SUM(OffHours)
		,SUM(PublicHours)
	FROM [dbo].OTDailyRequest AS dl WITH (NOLOCK)
	WHERE [Status] IN (4, 6)
		AND OvertimeDate BETWEEN @fromdate
			AND @todate
	GROUP BY EmpId
		,MONTH(OvertimeDate)

	SELECT EmpId
		,Code
		,NickName
		,FullName
		,Team
		,TeamCode
		,[1] AS Jan
		,[2] AS Feb
		,[3] AS Mar
		,[4] AS Apr
		,[5] AS May
		,[6] AS Jun
		,[7] AS Jul
		,[8] AS Aug
		,[9] AS Sep
		,[10] AS Oct
		,[11] AS Nov
		,[12] AS [DEC]
		,(ISNULL([1], 0) + ISNULL([2], 0) + ISNULL([3], 0) + ISNULL([4], 0) + ISNULL([5], 0) + ISNULL([6], 0) + ISNULL([7], 0) + ISNULL([8], 0) + ISNULL([9], 0) + ISNULL([10], 0) + ISNULL([11], 0) + ISNULL([12], 0)) AS Total
	FROM (
		SELECT e.EmpId
			,e.Code
			,e.NickName
			,e.FullName
			,g.[Name] AS Team
			,g.Code AS TeamCode
			,cl._MONTH AS [Month]
			,(ot.[NormalHours] + ot.[OffHours] + ot.[PublicHours]) AS ToTalHours
		FROM #Calendar AS cl
			LEFT JOIN #OTSumaryMonthly AS ot ON cl._MONTH = MONTH(ot.[Month])
			LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON ot.EmpId = e.EmpId
			LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		WHERE ot.[Month] BETWEEN @fromdate
				AND @todate
			AND (
				@EmpId IS NULL
				OR ot.EmpId = @EmpId
				)
			AND (
				@excludedEmpID IS NULL
				OR ot.EmpId <> @excludedEmpID
				)
			AND (
				ISNULL(@GroupIds, '') = ''
				OR e.GroupId IN (
					SELECT DISTINCT GroupId
					FROM #Group
					)
				)
		) AS tmp
	Pivot(MAX(ToTalHours) FOR [Month] IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) AS tmp2
	WHERE EmpId IS NOT NULL

	DROP TABLE #Group

	DROP TABLE #status

	DROP TABLE #Calendar

	DROP TABLE #OTSumaryMonthly
END