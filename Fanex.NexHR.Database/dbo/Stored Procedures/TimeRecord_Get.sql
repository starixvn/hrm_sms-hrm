
--[TimeRecord_Get] null, '2016-12-20', '2016-12-20'
CREATE PROCEDURE [dbo].[TimeRecord_Get] @EmpId INT = NULL
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@GroupIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
	,@PageIndex INT = NULL
	,@ItemsPerPage INT = NULL
	,@TotalItem INT = NULL OUT
WITH RECOMPILE AS 
/*
Created: Quinn.Vo@2014-01-24
Task: Get Time Record
DB: HRM_SMS
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT OFF;
	set arithabort on;
set concat_null_yields_null on;
set ansi_nulls on;
set cursor_close_on_commit off;
set ansi_null_dflt_on on;
set implicit_transactions off;
set ansi_padding on;
set ansi_warnings on;
set quoted_identifier on;

	IF OBJECT_ID('tempdb..#tblTimeRecord') IS NOT NULL
	BEGIN
		DROP TABLE #tblTimeRecord
	END

	CREATE TABLE #tblTimeRecord (
		RecordTimeId INT
		,RecordTime DATETIME
		,InOutType TINYINT
		,EmpId INT
		,Code VARCHAR(20)
		,NickName NVARCHAR(20)
		,FullName NVARCHAR(50)
		,GroupId INT
		,GroupCode VARCHAR(20)
		,[Name] NVARCHAR(50)
		)

	DECLARE @GroupsList TABLE (GroupId INT)

	INSERT INTO @GroupsList (GroupId)
	SELECT val
	FROM fnc_strSplit(@GroupIds, ',')

	INSERT INTO #tblTimeRecord
	SELECT t.[Id]
		,t.RecordTime
		,t.InOutType
		,e.EmpId
		,e.Code
		,e.NickName
		,e.FullName
		,g.GroupId
		,g.Code
		,g.[Name]
	FROM dbo.TimeRecord AS t WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON t.EmpID = e.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
	WHERE CONVERT(VARCHAR(10), RecordTime, 111) BETWEEN CONVERT(VARCHAR(10), @FromDate, 111)
			AND CONVERT(VARCHAR(10), @ToDate, 111)
		AND (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
			)
		AND (
			ISNULL(@GroupIds, '') = ''
			OR e.GroupId IN (
				SELECT GroupId
				FROM @GroupsList
				)
			)
		AND (
			@IsLocal IS NULL
			OR es.IsLocal = @IsLocal
			)
	ORDER BY e.EmpId
		,t.RecordTime DESC

	IF @PageIndex IS NOT NULL
		AND @ItemsPerPage IS NOT NULL
	BEGIN
		SELECT RecordTimeId
		,RecordTime
		,InOutType
		,EmpId 
		,Code 
		,NickName 
		,FullName 
		,GroupId 
		,GroupCode
		,[Name]
		FROM #tblTimeRecord AS t
		ORDER BY t.RecordTime DESC OFFSET(@PageIndex * @ItemsPerPage) ROWS

		FETCH NEXT @ItemsPerPage ROWS ONLY;
	END
	ELSE
	BEGIN
		SELECT RecordTimeId
		,RecordTime
		,InOutType
		,EmpId 
		,Code 
		,NickName 
		,FullName 
		,GroupId 
		,GroupCode
		,[Name]
		FROM #tblTimeRecord AS t
		ORDER BY t.RecordTime DESC
	END

	SELECT @TotalItem = COUNT(1)
	FROM #tblTimeRecord

	IF OBJECT_ID('tempdb..#tblTimeRecord') IS NOT NULL
	BEGIN
		DROP TABLE #tblTimeRecord
	END
END