
CREATE PROCEDURE [dbo].[Report_YearlyOvertime] @Year INT
	,@CalendarType TINYINT = 0
	,@GroupIds VARCHAR(MAX) = ''
	,@EmployeeId INT = NULL
AS
/*
Created: Tim.Trinh@2019-07-09
Task: Get yearly overtime
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
    @CalendarType: 0 - normal, 1 - fiscal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromdate DATE
		,@todate DATE
		-- request status
		,@ApprovedStatusId TINYINT = 4
		,@ProcessedStatusId TINYINT = 6

	CREATE TABLE #tblGroupIds (GroupId INT)

	CREATE TABLE #tblMonth (
		[Month] TINYINT
		,FromDate DATE
		,ToDate DATE
		)

	CREATE TABLE #tblOvertimeSummary (
		EmployeeId INT
		,[Month] TINYINT
		,NormalHours DECIMAL(18, 2)
		,OffHours DECIMAL(18, 2)
		,PublicHours DECIMAL(18, 2)
		,SpecialHours DECIMAL(18, 2)
		)

	IF ISNULL(@GroupIds, '') <> ''
		INSERT INTO #tblGroupIds (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	ELSE
	BEGIN
		INSERT INTO #tblGroupIds (GroupId)
		SELECT GroupId
		FROM dbo.[Group] WITH (NOLOCK)
		WHERE IsDeleted = 0
	END

	DECLARE @fromMonth INT = 1
		,@toMonth INT = 12

	WHILE @fromMonth <= @toMonth
	BEGIN
		SET @fromdate = CONVERT(VARCHAR(5), @Year) + '/' + CONVERT(VARCHAR(2), @fromMonth) + '/01'
		SET @todate = DATEADD(DAY, - 1, DATEADD(MONTH, 1, @fromdate))

		IF @CalendarType = 1
		BEGIN
			SET @fromdate = DATEADD(MONTH, - 1, @fromdate)
			SET @fromdate = CONVERT(VARCHAR(5), YEAR(@fromdate)) + '/' + CONVERT(VARCHAR(2), MONTH(@fromdate)) + '/10'
			SET @todate = CONVERT(VARCHAR(5), YEAR(@todate)) + '/' + CONVERT(VARCHAR(2), MONTH(@todate)) + '/09'
		END

		INSERT INTO #tblMonth (
			[Month]
			,FromDate
			,ToDate
			)
		VALUES (
			@fromMonth
			,@fromdate
			,@todate
			)

		SET @fromMonth = @fromMonth + 1
	END

	INSERT INTO #tblOvertimeSummary (
		EmployeeId
		,[Month]
		,NormalHours
		,OffHours
		,PublicHours
		,SpecialHours
		)
	SELECT req.EmpId
		,tbl.[Month]
		,SUM(req.NormalHours)
		,SUM(req.OffHours)
		,SUM(req.PublicHours)
		,SUM(ISNULL(req.SpecialHours, 0))
	FROM dbo.OTDailyRequest AS req WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON req.EmpId = emp.EmpId
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0
		INNER JOIN #tblMonth AS tbl ON req.OvertimeDate BETWEEN tbl.FromDate
			AND tbl.ToDate
	WHERE req.[Status] IN (@ApprovedStatusId, @ProcessedStatusId)
		AND (
			@EmployeeId IS NULL
			OR req.EmpId = @EmployeeId
			)
	GROUP BY req.EmpId
		,tbl.[Month]

	SELECT EmpId AS EmployeeId
		,Code
		,NickName
		,FullName
		,GroupId
		,GroupName AS [Name]
		,GroupCode AS Code
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
		,ISNULL([12], 0) AS [Dec]
	FROM (
		SELECT emp.EmpId
			,emp.Code
			,emp.NickName
			,emp.FullName
			,gr.GroupId
			,gr.[Name] AS GroupName
			,gr.Code AS GroupCode
			,cl.[Month] AS [Month]
			,(otsum.NormalHours + otsum.OffHours + otsum.PublicHours + otsum.SpecialHours) AS TotalHours
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			CROSS JOIN #tblMonth AS cl
			INNER JOIN #tblOvertimeSummary AS otsum ON emp.EmpId = otsum.EmployeeId
			AND cl.[Month] = otsum.[Month]
		WHERE (
				@EmployeeId IS NULL
				OR emp.EmpId = @EmployeeId
				)
			AND emp.[Enabled] = 1
			AND emp.isDeleted = 0
			AND EXISTS (
				SELECT 1
				FROM #tblGroupIds AS tblgr
				WHERE emp.GroupId = tblgr.GroupId
				)
		) AS tmp
	PIVOT(MAX(TotalHours) FOR [Month] IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) AS tmp2
	ORDER BY NickName

	DROP TABLE #tblGroupIds

	DROP TABLE #tblMonth

	DROP TABLE #tblOvertimeSummary
END
