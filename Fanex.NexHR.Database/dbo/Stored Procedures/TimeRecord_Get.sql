

CREATE PROCEDURE [dbo].[TimeRecord_Get] @EmpId INT = NULL
	,@SupervisorId INT = NULL
	,@FromDate DATE
	,@ToDate DATE
	,@TeamIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
	,@PageIndex INT = NULL
	,@ItemsPerPage INT = NULL
	,@TotalItem INT = 0 OUT
AS

/*
Created: Quinn.Vo@2014-01-24
Task: Get Time Record
DB: HRM_SMS
Original:

Revisions:
	- 20200714@Merlin.Le: Get section code and department code
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblEmployeeId (EmpId INT)

	CREATE TABLE #tblTimeRecord (
		RecordTimeId INT
		,RecordTime DATETIME
		,InOutType TINYINT
		,EmpId INT
		,Code VARCHAR(20)
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Team NVARCHAR(50)
		,SectionCode NVARCHAR(50)
		,DepartmentCode NVARCHAR(50)
		,IsCheckInOutOnline BIT
		)

	DECLARE @tblGroupId TABLE (GroupId INT)

	INSERT INTO @tblGroupId (GroupId)
	SELECT val
	FROM fnc_strSplit(@TeamIds, ',')

	IF @EmpId IS NULL
	BEGIN
		INSERT INTO #tblEmployeeId (EmpId)
		SELECT EmpId
		FROM dbo.fnc_GetEmployeeByActorId(@SupervisorId, 1, 0)
	END
	ELSE
	BEGIN
		INSERT INTO #tblEmployeeId (EmpId)
		VALUES (@EmpId)
	END

	INSERT INTO #tblTimeRecord
	SELECT t.Id
		,t.RecordTime
		,t.InOutType
		,e.EmpId
		,e.Code
		,e.NickName
		,e.FullName
		,g.[Name]
		,sec.Code SectionCode
		,dep.Code DepartmentCode
		,CASE
			WHEN tex.ID IS NOT NULL THEN 1
			ELSE 0
		END AS IsInOutOnline
	FROM dbo.TimeRecord AS t WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON t.EmpID = e.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN @tblGroupId AS tblgr ON g.GroupId = tblgr.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON g.SectionId = sec.SectionId
		INNER JOIN dbo.Department AS dep WITH (NOLOCK) ON sec.DepartmentId = dep.DepartmentId
		LEFT JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN dbo.TimeRecordExt AS tex WITH (NOLOCK) ON t.ID = tex.ID
	WHERE CAST(RecordTime AS DATE) BETWEEN @FromDate
			AND @ToDate
		AND (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
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
			,Nickname
			,FullName
			,Team
			,SectionCode
			,DepartmentCode
			,IsCheckInOutOnline
		FROM #tblTimeRecord WITH (NOLOCK)
		ORDER BY RecordTime DESC OFFSET(@PageIndex * @ItemsPerPage) ROWS

		FETCH NEXT @ItemsPerPage ROWS ONLY;
	END
	ELSE
	BEGIN
		SELECT RecordTimeId
			,RecordTime
			,InOutType
			,EmpId
			,Code
			,Nickname
			,FullName
			,Team
			,SectionCode
			,DepartmentCode
			,IsCheckInOutOnline
		FROM #tblTimeRecord WITH (NOLOCK)
		ORDER BY RecordTime DESC
	END

	SELECT @TotalItem = COUNT(1)
	FROM #tblTimeRecord WITH (NOLOCK)

	DROP TABLE #tblEmployeeId

	DROP TABLE #tblTimeRecord
END