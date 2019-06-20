/****** Object:  Stored Procedure dbo.OverTime_GetNormalMonthlyByHR    Script Date: 7/20/2016 10:19:14 AM ******/
/****** Object:  Stored Procedure dbo.OverTime_GetNormalMonthlyByHR    Script Date: 7/19/2016 5:22:18 PM ******/

CREATE PROCEDURE [dbo].[OverTime_GetNormalMonthlyByHR] @EmpId INT = NULL
	,@GroupIds VARCHAR(max) = ''
	,@fromdate DATETIME
	,@todate DATETIME
	,@excludedEmpID INT = NULL
AS
/*
Created: Tom.Vo@2013-11-04
Task:Get  normal calendar  By Date For Overtime
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT @fromdate = CONVERT(VARCHAR(10), @fromDate, 111) + ' 00:00:00'
		,@todate = CONVERT(VARCHAR(10), @toDate, 111) + ' 23:59:59'

	CREATE TABLE #Group (GroupId INT)

	SET @GroupIds = ISNULL(@GroupIds, '')

	IF @GroupIds <> ''
		INSERT INTO #Group (GroupId)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	ELSE
	BEGIN
		INSERT INTO #Group (GroupId)
		SELECT g.GroupId
		FROM dbo.[Group] AS g WITH (NOLOCK)
		WHERE ISNULL(g.IsDeleted, 0) = 0
	END

	DECLARE @SQL VARCHAR(8000)

	SELECT r.EmpId
		,r.[Month] AS RequestedDate
		,r.NormalHours
		,r.OffHours
		,r.PublicHours
		,(r.[NormalHours] + r.[OffHours] + r.[PublicHours]) AS ToTalHours
		,e.EmpId AS RequesterID
		,e.NickName AS Requester
		,e.FullName AS FullNameRequester
		,g.GroupId AS GroupId
		,g.[Name] AS Team
		,g.Code AS TeamCode
	FROM dbo.OTMonthlySum AS r WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON r.EmpId = e.EmpId
		AND ISNULL(e.isDeleted, 0) = 0
		AND ISNULL(e.[Enabled], 1) = 1
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		AND ISNULL(g.IsDeleted, 0) = 0
	WHERE r.[Month] BETWEEN @FromDate
			AND @ToDate
		AND (
			@EmpId IS NULL
			OR r.EmpId = @EmpId
			)
		AND (
			@excludedEmpID IS NULL
			OR r.EmpId <> @excludedEmpID
			)
		AND (
			ISNULL(@GroupIds, '') = ''
			OR e.GroupId IN (
				SELECT DISTINCT GroupId
				FROM #Group
				)
			)
	ORDER BY r.[Month] DESC

	DROP TABLE #Group
END