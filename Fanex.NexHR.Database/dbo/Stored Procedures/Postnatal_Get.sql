
CREATE PROCEDURE [dbo].[Postnatal_Get] @Year INT
AS
/*
Created: 20190429@Oliver.Dang
Task: Get all employee postnatal on year
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT ep.PostnatalId
		,ep.FromDate
		,ep.ToDate
		,ep.[Minute]
		,e.EmpId
		,e.NickName
		,e.FullName
		,ep.CreatedDate
		,ep.ModifiedDate
		,creator.EmpId
		,creator.NickName AS Creator
		,modifier.EmpId
		,modifier.NickName AS Modifier
		,ep.Remark
	FROM dbo.EmployeePostnatal AS ep WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ep.EmpId = e.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON ep.CreatedBy = creator.EmpId
		LEFT JOIN dbo.Employee AS modifier WITH (NOLOCK) ON ep.ModifiedBy = modifier.EmpId
	WHERE YEAR(ep.FromDate) = @Year
		OR YEAR(ep.ToDate) = @Year
	ORDER BY ep.FromDate
		,e.NickName
END
