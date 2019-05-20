
/****** Object:  Stored Procedure dbo.Department_Get    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Get    Script Date: 7/19/2016 5:22:11 PM ******/
CREATE PROCEDURE [dbo].[Paternity_Get] @Year INT
AS
/*
Created: 20190429@Oliver.Dang
Task: Get all employee paternity on year
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT ep.PaternityId
		,ep.ChildBornDate
		,ep.LeaveDay
		,e.EmpId
		,e.NickName
		,e.FullName
		,ep.CreatedDate
		,ep.ModifiedDate
		,creator.EmpId
		,creator.NickName as Creator
		,modifier.EmpId
		,modifier.NickName as Modifier
		,ep.Remark
	FROM dbo.EmployeePaternity AS ep WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ep.EmpId = e.EmpId
	INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON ep.CreatedBy = creator.EmpId
	LEFT JOIN dbo.Employee AS modifier WITH (NOLOCK) ON ep.ModifiedBy = modifier.EmpId
	WHERE YEAR(ep.ChildBornDate) = @Year
	ORDER BY ep.ChildBornDate
		,e.NickName
END