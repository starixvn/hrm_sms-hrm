
CREATE PROCEDURE [dbo].[Postnatal_GetById] @PostnatalId INT
AS
/*
Created: 20190429@Oliver.Dang
Task: Get postnatal by Id.
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
		,ep.EmpId
		,ep.Remark
		,e.NickName
		,e.FullName
	FROM dbo.EmployeePostnatal AS ep WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ep.EmpId = e.EmpId
	WHERE ep.PostnatalId = @PostnatalId
END
