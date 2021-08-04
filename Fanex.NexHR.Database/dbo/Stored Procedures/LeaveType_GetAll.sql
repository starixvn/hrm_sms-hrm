
CREATE PROCEDURE [dbo].[LeaveType_GetAll]
	@ExcludeNotShown bit = null
AS
/*
Created: Tom.Vo@2013-10-23
Task: Get Leave Type of by ID 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select * from LeaveType WITH (NOLOCK)
	where (@ExcludeNotShown is null or isShown=1)
	order by OrderNo , [name] 
END


