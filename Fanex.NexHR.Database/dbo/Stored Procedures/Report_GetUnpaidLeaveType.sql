CREATE PROCEDURE [dbo].[Report_GetUnpaidLeaveType]
AS
/*
Created: Lilac.Nguyen@2020-06-19
Task: Get Leave Type for Unpaid Leave
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SELECT LeaveTypeId AS Id
		,Code
		,Name
	FROM dbo.LeaveType WITH (NOLOCK)
	WHERE Code IN ('UPL','UPLI','PL','ML')
END
