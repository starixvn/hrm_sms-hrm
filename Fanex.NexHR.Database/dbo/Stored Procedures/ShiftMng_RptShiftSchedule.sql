/****** Object:  Stored Procedure dbo.ShiftMng_RptShiftSchedule    Script Date: 7/20/2016 10:19:19 AM ******/
/****** Object:  Stored Procedure dbo.ShiftMng_RptShiftSchedule    Script Date: 7/19/2016 5:22:20 PM ******/

CREATE PROCEDURE [dbo].[ShiftMng_RptShiftSchedule] @SupervisorID INT = NULL
	,@fromtime DATETIME
	,@totime DATETIME
	,@groupIds VARCHAR(max) = ''
	,@isShowWholeCompany BIT = 0
	,@isLocal BIT = NULL
AS
/*
Created: Tom.Vo@2013-10-23
Task: Get Shift Scheduling Report
DB: HRM_SMS
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #GroupsList (GroupId INT)

	IF ISNULL(@groupids, '') <> ''
		INSERT INTO #GroupsList (GroupId)
		SELECT g.val
		FROM fnc_strSplit(@groupids, ',') AS g
	ELSE
	BEGIN
		INSERT INTO #GroupsList (GroupId)
		SELECT g.GRoupID
		FROM dbo.[Group] g WITH (NOLOCK)
	END

	CREATE TABLE #tmpEmployee (EmpId INT)

	IF (@isShowWholeCompany = 0)
	BEGIN
		INSERT INTO #tmpEmployee (EmpId)
		SELECT es.OwnerId
		FROM dbo.EmployeeSupervisor es WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON es.OwnerId = e.EmpId
		WHERE es.ActionType = 4
			AND es.EmpID = @SupervisorID
			AND e.Enabled = 1
			AND ISNULL(es.IsDeleted, 0) = 0
		
		UNION
		SELECT e.EmpId
		FROM dbo.GroupSupervisor AS gsp WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gsp.GroupId = e.GroupId
		WHERE gsp.OwnerId = @SupervisorID
			AND gsp.ActionType = 4
			AND e.Enabled = 1
			AND ISNULL(gsp.IsDeleted, 0) = 0
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor WITH (NOLOCK)
				WHERE e.EmpId = OwnerId
					AND ActionType = 4
					AND ISNULL(IsDeleted, 0) = 0
				)
	END
	ELSE
	BEGIN
		INSERT INTO #tmpEmployee (EmpId)
		SELECT e.EmpId
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE e.Enabled = 1
	END

	CREATE TABLE #tmpSchedule (
		GroupId INT
		,ShiftId INT
		,[Weekday] INT
		,[Total] INT
		)

	INSERT INTO #tmpSchedule (
		GroupId
		,ShiftId
		,[Weekday]
		,[Total]
		)
	SELECT 1
		,ISNULL(ec.ShiftId, 0)
		,DATEPART(dw, ec.WorkDate) [Weekday]
		,Count(1) ToTal
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #tmpEmployee AS es ON e.EmpId = es.EmpId
		INNER JOIN dbo.EmployeeCalendar ec WITH (NOLOCK) ON e.EmpId = ec.EmpId
		INNER JOIN dbo.EmployeeSummary e_s WITH (NOLOCK) ON e_s.EmpId = e.EmpId
	WHERE (
			@isLocal IS NULL
			OR e_s.IsLocal = @isLocal
			)
		AND (
			ec.WorkDate BETWEEN @fromtime
				AND @totime
			)
		AND (ISNULL(ec.ShiftId, 0) <> 0)
		AND ISNULL(ec.LeaveDay, 0) <> 1
		AND e.GroupId IN (
			SELECT GroupId
			FROM #GroupsList
			)
	GROUP BY ISNULL(ec.ShiftId, 0)
		,DATEPART(dw, ec.WorkDate)

	SELECT ShiftId
		,Color
		,Code
		,[2] AS [Monday]
		,[3] AS [Tuesday]
		,[4] AS [Wednesday]
		,[5] AS [Thursday]
		,[6] AS [Friday]
		,[7] AS [Saturday]
		,[1] AS [Sunday]
	FROM (
		SELECT tmp.ShiftId
			,s.Color
			,s.Code
			,tmp.[Weekday]
			,tmp.ToTal
		FROM #tmpSchedule tmp
			LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON tmp.GroupID = g.GroupID
			LEFT JOIN dbo.[Shift] s WITH (NOLOCK) ON s.ShiftId = tmp.ShiftId
		) TEMP
	PIVOT(MAX(ToTal) FOR [Weekday] IN ([2], [3], [4], [5], [6], [7], [1])) AS pvt

	DROP TABLE #GroupsList

	DROP TABLE #tmpSchedule

	DROP TABLE #tmpEmployee
END
