
CREATE PROCEDURE [dbo].[EmployeeWorkFromHome_Get] @FromDate DATE
	,@ToDate DATE
	,@TeamIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
AS
/*
Created: 20210528@Quinn.Vo
Task: Get Employee Work From Home by date.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @tblGroupIds TABLE (GroupId INT)

	IF ISNULL(@TeamIds, '') <> ''
	BEGIN
		INSERT INTO @tblGroupIds (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@TeamIds, ',')
	END

	SELECT ew.EmployeeWorkFromHomeId
		,ew.FromDate
		,ew.ToDate
		,e.EmpId
		,e.Code
		,e.NickName
		,e.FullName
		,g.[Name] AS Team
		,sec.[Name] AS Section
		,dep.[Name] AS Department
		,ew.ModifiedDate
		,ew.ModifiedBy
		,modifier.NickName AS Modifier
		,ew.Remark
	FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ew.EmpId = e.EmpId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON e.EmpId = empsum.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN @tblGroupIds AS tblgr ON g.GroupId = tblgr.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON sec.SectionId = g.SectionId
		INNER JOIN dbo.Department AS dep WITH (NOLOCK) ON dep.DepartmentId = sec.DepartmentId
		LEFT JOIN dbo.Employee AS modifier WITH (NOLOCK) ON ew.ModifiedBy = modifier.EmpId
	WHERE (
			(
				ew.FromDate BETWEEN @FromDate
					AND @ToDate
				)
			OR (
				ew.ToDate BETWEEN @FromDate
					AND @ToDate
				)
			OR (
				@FromDate BETWEEN ew.FromDate
					AND ew.ToDate
				)
			OR (
				@ToDate BETWEEN ew.FromDate
					AND ew.ToDate
				)
			)
		AND (
			@IsLocal IS NULL
			OR empsum.IsLocal = @IsLocal
			)
	ORDER BY ew.FromDate
		,g.[Name]
END
