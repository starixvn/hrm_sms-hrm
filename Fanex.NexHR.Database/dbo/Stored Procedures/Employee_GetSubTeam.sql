---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[Employee_GetSubTeam]
	@EmpId int
AS
/*
Created: Tom.Vo@2013-10-23
Task: Get Memo by ID
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	select g.* 
	from dbo.EmployeeGroup eg WITH (NOLOCK)
		JOIN dbo.[Group] g WITH (NOLOCK) ON eg.GroupID = g.GroupID AND isMaingroup=0
	where eg.EmpID=@empId 
END


