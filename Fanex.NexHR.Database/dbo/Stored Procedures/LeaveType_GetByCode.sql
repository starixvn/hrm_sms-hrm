/****** Object:  Stored Procedure dbo.LeaveType_GetByCode    Script Date: 7/20/2016 10:19:11 AM ******/
/****** Object:  Stored Procedure dbo.LeaveType_GetByCode    Script Date: 7/19/2016 5:22:16 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[LeaveType_GetByCode]
	@Code NVARCHAR(100),
	@bLike Bit = 0
AS
/*
Created: Tom.Vo@2013-10-23
Task: Get Leave Type of by Code
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF @bLike = 0
		select * from LeaveType with(nolock)
		where Code=@Code
	ELSE
		select * from LeaveType with(nolock)
		where Code like @Code + '%'

END


