/****** Object:  Stored Procedure dbo.Department_GetById    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_GetById    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[Paternity_GetById]
	@PaternityId INT
AS
/*
Created: 20190429@Oliver.Dang
Task: Get paternity by Id.
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT ep.PaternityId
		  ,ep.ChildBornDate
		  ,ep.LeaveDay
		  ,ep.EmpId
		  ,ep.Remark
		  ,e.NickName
		  ,e.FullName
	FROM dbo.EmployeePaternity As ep WITH (NOLOCK)
	INNER JOIN dbo.Employee As e WITH (NOLOCK) ON ep.EmpId = e.EmpId
	WHERE ep.PaternityId = @PaternityId

END