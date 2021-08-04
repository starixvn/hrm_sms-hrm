
CREATE PROCEDURE [dbo].[EmployeeWorkFromHome_GetById] 
	@EmployeeWorkFromHomeId INT
AS
/*
Created: 20210528@Quinn.Vo
Task: Get Employee Work From Home by Id.
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
		,modifier.NickName AS Modifier
		,ew.Remark
	FROM dbo.EmployeeWorkFromHome AS ew WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ew.EmpId = e.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN dbo.Employee AS modifier WITH (NOLOCK) ON ew.ModifiedBy = modifier.EmpId
	WHERE ew.EmployeeWorkFromHomeId = @EmployeeWorkFromHomeId
END