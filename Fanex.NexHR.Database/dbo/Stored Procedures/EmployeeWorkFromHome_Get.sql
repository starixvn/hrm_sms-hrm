
CREATE PROCEDURE [dbo].[EmployeeWorkFromHome_Get] 
	@FromDate DATETIME
	,@ToDate DATETIME
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

	SELECT ew.EmployeeWorkFromHomeId
		,ew.FromDate
		,ew.ToDate
		,e.EmpId
		,e.NickName
		,e.FullName
		,ew.ModifiedDate
		,modifier.EmpId
		,modifier.NickName AS Modifier
		,ew.Remark
	FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ew.EmpId = e.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN dbo.Employee AS modifier WITH (NOLOCK) ON ew.ModifiedBy = modifier.EmpId
	WHERE (ew.FromDate BETWEEN @FromDate AND @ToDate)
		OR (ew.ToDate BETWEEN @FromDate AND @ToDate)
		OR (@FromDate BETWEEN ew.FromDate AND ew.ToDate)
		OR (@ToDate BETWEEN ew.FromDate AND ew.ToDate) 
	ORDER BY ew.FromDate
		,e.NickName
END