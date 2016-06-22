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


