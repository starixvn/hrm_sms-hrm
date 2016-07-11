CREATE PROCEDURE [dbo].[Report_OvertimeDetail] @fromDate DATETIME
	,@toDate DATETIME
	,@EmpID INT = NULL
	,@JobTitleId VARCHAR(1000) = ''
	,@groupIds VARCHAR(max) = ''
	,@isLocal BIT = NULL
AS
/*
Created: Tom.Vo@2013-10-23
Task: Get Overtime Detail Report 
DB: HRM_SMS
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT @fromDate = CONVERT(VARCHAR(50), @fromDate, 111)

	SELECT @toDate = CONVERT(VARCHAR(50), @toDate, 111)

	CREATE TABLE #Group (GroupId TINYINT)

	IF ISNULL(@groupids, '') <> ''
	BEGIN
		INSERT INTO #Group (GroupId)
		SELECT g.val
		FROM fnc_strSplit(@groupids, ',') AS g
	END

	CREATE TABLE #JobTitle (JobTitleId TINYINT)

	IF ISNULL(@JobTitleId, '') <> ''
	BEGIN
		INSERT INTO #JobTitle (JobTitleId)
		SELECT val
		FROM fnc_strSplit(@JobTitleId, ',')
	END

	SELECT ot.OvertimeDate
		,ot.EmpId
		,ee.NickName
		,ee.FullName
		,ee.Code
		,gg.GroupId AS TeamId
		,gg.Code AS Team
		,gg.[Name] AS TeamName
		,ee.JobTitleId AS [PositionId]
		,lk.[Name] AS [Positions]
		,s.ShiftId
		,s.Code [Schedule]
		,CONVERT(VARCHAR(5), ot.TimeIn, 108) AS TimeIn
		,CONVERT(VARCHAR(5), ot.[TimeOut], 108) AS [TimeOut]
		,ot.TotalHours AS TotalWorkHours
		,ot.NormalHours
		,ot.[OffHours]
		,ot.[PublicHours]
		,ot.[Remark]
		,ot.[Status]
	FROM [dbo].[OTDailyRequest] AS ot WITH (NOLOCK)
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
		INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON ee.EmpId = ot.EmpId
		INNER JOIN dbo.[Group] AS gg WITH (NOLOCK) ON ee.GroupId = gg.GroupId
		LEFT JOIN dbo.EmployeeSummary AS e_s WITH (NOLOCK) ON e_s.EmpId = ee.EmpId
		LEFT JOIN dbo.LookupData AS lk WITH (NOLOCK) ON ee.JobTitleId = lk.Id
		AND lk.LookupId = 1
	WHERE ot.OvertimeDate BETWEEN @fromDate
			AND @toDate
		AND (
			@EmpID IS NULL
			OR ot.EmpId = @EmpID
			)
		AND (
			@isLocal IS NULL
			OR e_s.IsLocal = @isLocal
			)
		AND (
			ISNULL(@groupids, '') = ''
			OR ee.GroupId IN (
				SELECT GroupId
				FROM #Group
				)
			)
		AND (
			ISNULL(@JobTitleId, '') = ''
			OR ee.JobTitleId IN (
				SELECT JobTitleId
				FROM #JobTitle
				)
			)
	ORDER BY gg.Code
		,ee.NickName
		,ot.OvertimeDate

	DROP TABLE #Group

	DROP TABLE #JobTitle
END