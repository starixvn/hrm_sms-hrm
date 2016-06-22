---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[LeaveType_GetById]
	@LeaveTypeID int
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
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select * from LeaveType with(nolock)
	where LeaveTypeID=@LeaveTypeID
END


