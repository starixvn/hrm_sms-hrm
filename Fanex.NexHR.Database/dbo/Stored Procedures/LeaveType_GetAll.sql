/****** Object:  Stored Procedure dbo.LeaveType_GetAll    Script Date: 7/20/2016 10:19:11 AM ******/
/****** Object:  Stored Procedure dbo.LeaveType_GetAll    Script Date: 7/19/2016 5:22:16 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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


