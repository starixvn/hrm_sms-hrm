
CREATE PROCEDURE [dbo].[ShiftMng_RptShiftSchedule] @SupervisorID INT = NULL
	,@fromtime DATETIME
	,@totime DATETIME
	,@groupIds VARCHAR(max) = ''
	,@isShowWholeCompany BIT = 0
	,@isLocal BIT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
	,@isDebug BIT = 0
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

	DECLARE @fromDate NVARCHAR(100)
		,@toDate NVARCHAR(100)

	SELECT @fromDate = CONVERT(VARCHAR(10), @fromtime, 111) + ' 00:00:00'
		,@toDate = CONVERT(VARCHAR(10), @totime, 111) + ' 00:00:00'

	IF (
			DATEPART(dw, @fromDate) <> 2
			OR DATEPART(dw, @toDate) <> 1
			OR DATEDIFF(dd, @fromdate, @toDate) <> 6
			OR @SupervisorID IS NULL
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Parameter not correct.'

		RETURN
	END

	IF @SupervisorID IS NOT NULL
		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM dbo.Employee ee(NOLOCK)
				WHERE EmpId = @SupervisorID
					AND (
						Roles LIKE '%4%'
						OR Roles LIKE '%6%'
						)
				)
		BEGIN
			SELECT @ErrCode = 102
				,@ErrMsg = N'You don''t have permission on the funcion.'

			RETURN
		END

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

	DECLARE @SQL NVARCHAR(4000)
		,@sWhere NVARCHAR(4000)
		,@bWhere BIT

	SELECT @sWhere = ''
		,@bWhere = 0

	IF ISNULL(@groupids, '') <> ''
		SELECT @sWhere = @sWhere + CASE 
				WHEN @bWhere = 1 THEN ' AND '
				ELSE ' WHERE '
				END + ' ( e.GroupId IN (SELECT GroupId FROM #GroupsList) )' + CHAR(13)
			,@bWhere = 1

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
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON gsp.GroupId = gsp.GroupId
		WHERE gsp.OwnerId = @SupervisorID
			AND gsp.ActionType = 4
			AND e.Enabled = 1
			AND ISNULL(gsp.IsDeleted, 0) = 0
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

	SELECT @SQL = 'INSERT INTO #tmpSchedule (GroupId,ShiftId,[Weekday],[Total])
				SELECT 1,ISNULL(ec.ShiftId,0),DATEPART(dw,ec.WorkDate) [Weekday], Count(1) ToTal ' + CHAR(13) + 'FROM dbo.Employee e WITH (NOLOCK)  ' + CHAR(13) + '	INNER JOIN #tmpEmployee AS es  ' + CHAR(13) + '		ON e.EmpId = es.EmpId ' + CHAR(13) + '	INNER JOIN dbo.EmployeeCalendar ec WITH (NOLOCK)  ' + CHAR(13) + '		ON e.EmpId = ec.EmpId  ' + CHAR(13) + CASE 
			WHEN @isLocal IS NOT NULL THEN 'LEFT JOIN dbo.EmployeeSummary e_s WITH (NOLOCK) ON e_s.EmpId = e.EmpId  '
			ELSE ''
			END + 'WHERE (ec.WorkDate BETWEEN ''' + @fromDate + ''' AND ''' + @toDate + ''' ) ' + CHAR(13) + CASE 
			WHEN @bWhere = 1 THEN REPLACE(@sWhere, 'WHERE', 'AND')
			ELSE ''
			END + '	AND (ISNULL(ec.ShiftId,0) <> 0 ) AND ISNULL(ec.LeaveDay,0) <> 1' + CHAR(13) + CASE 
			WHEN @isLocal IS NOT NULL THEN ' AND  e_s.IsLocal = ' + CONVERT(NVARCHAR(10), @isLocal) + CHAR(13)
			ELSE ''
			END + 'GROUP BY ISNULL(ec.ShiftId,0),DATEPART(dw,ec.WorkDate)' + CHAR(13)

	EXEC sp_executesql @SQL

	IF @isDebug = 1
		PRINT @SQL

	SET @SQL = 'SELECT 
			ShiftId, Color ,Code,[2] AS [Monday],[3] AS [Tuesday],[4] AS [Wednesday],[5] AS [Thursday],[6] AS [Friday],[7] AS [Saturday],[1] AS [Sunday] ' + CHAR(13) + ' FROM (' + CHAR(13) + ' 	SELECT 
			tmp.ShiftId, s.Color,s.Code,tmp.[Weekday],tmp.ToTal ' + CHAR(13) + ' 	FROM #tmpSchedule tmp ' + CHAR(13) + ' 		LEFT JOIN dbo.[Group] g WITH (NOLOCK)' + CHAR(13) + '			ON tmp.GroupID = g.GroupID ' + CHAR(13) + ' 		LEFT JOIN dbo.[Shift] s WITH (NOLOCK)' + CHAR(13) + '			ON s.ShiftId = tmp.ShiftId ' + CHAR(13) + '		) tmp ' + CHAR(13) + 'PIVOT' + CHAR(13) + '(
		MAX(ToTal) FOR [Weekday] IN ([2],[3],[4],[5],[6],[7],[1])' + ') AS pvt'

	IF @isDebug = 1
		PRINT @SQL

	EXEC sp_executesql @SQL

	DROP TABLE #GroupsList

	DROP TABLE #tmpSchedule

	DROP TABLE #tmpEmployee

	SELECT @ErrCode = 0
END
