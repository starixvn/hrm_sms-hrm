
CREATE PROCEDURE [dbo].[Report_YearlyOvertime] @ActorId INT
	,@Year INT
	,@CalendarType TINYINT = 0
	,@GroupIds VARCHAR(MAX) = ''
	,@Employee NVARCHAR(200) = NULL
	,@EmployeeStatusId TINYINT = NULL
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
		-- employee status
		,@Enable TINYINT = 1
		,@Disabled TINYINT = 2
		,@Deleted TINYINT = 3
		-- request status
		,@ApprovedStatusId TINYINT = 4
		,@ProcessedStatusId TINYINT = 6

	CREATE TABLE #tblEmployeeIds (EmpId INT)

	CREATE TABLE #tblGroupIds (GroupId INT)

	CREATE TABLE #tblMonth (
		[Month] TINYINT
		,FromDate DATE
		,ToDate DATE
		)

	CREATE TABLE #tblOvertimeSummary (
		EmpId INT
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

	INSERT INTO #tblEmployeeIds (EmpId)
	SELECT emp.EmpId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN (
		SELECT EmpId
		FROM dbo.fnc_GetEmployeeByActorId(@ActorId, 0, 1)
		) AS member ON emp.EmpId = member.EmpId
	WHERE (
			@EmployeeStatusId IS NULL
			OR (
				@EmployeeStatusId = @Enable
				AND emp.[Enabled] = 1
				AND emp.isDeleted = 0
				)
			OR (
				@EmployeeStatusId = @Disabled
				AND emp.[Enabled] = 0
				AND emp.isDeleted = 0
				)
			OR (
				@EmployeeStatusId = @Deleted
				AND emp.isDeleted = 1
				)
			)
		AND (
			emp.NickName LIKE '%' + ISNULL(@Employee, '') + '%'
			OR emp.FullName LIKE '%' + ISNULL(@Employee, '') + '%'
			)
		AND EXISTS (
			SELECT 1
			FROM #tblGroupIds AS tbl
			WHERE tbl.GroupId = emp.GroupId
			)

	INSERT INTO #tblOvertimeSummary (
		EmpId
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
		INNER JOIN #tblEmployeeIds AS empid ON emp.EmpId = empid.EmpId
		INNER JOIN #tblMonth AS tbl ON req.OvertimeDate BETWEEN tbl.FromDate
			AND tbl.ToDate
	WHERE req.[Status] IN (@ApprovedStatusId, @ProcessedStatusId)
	GROUP BY req.EmpId
		,tbl.[Month]

	SELECT EmpId
		,Code
		,NickName
		,FullName
		,GroupId
		,Section
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
			,gr.[Name] AS Section
			,cl.[Month] AS [Month]
			,(otsum.NormalHours + otsum.OffHours + otsum.PublicHours + otsum.SpecialHours) AS TotalHours
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN #tblEmployeeIds AS empid ON emp.EmpId = empid.EmpId
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			CROSS JOIN #tblMonth AS cl
			INNER JOIN #tblOvertimeSummary AS otsum ON emp.EmpId = otsum.EmpId
			AND cl.[Month] = otsum.[Month]
		) AS tmp
	PIVOT(MAX(TotalHours) FOR [Month] IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) AS tmp2
	ORDER BY NickName

	DROP TABLE #tblEmployeeIds

	DROP TABLE #tblGroupIds

	DROP TABLE #tblMonth

	DROP TABLE #tblOvertimeSummary
END
