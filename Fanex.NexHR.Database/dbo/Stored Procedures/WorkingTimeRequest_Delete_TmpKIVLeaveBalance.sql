---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[WorkingTimeRequest_Delete_TmpKIVLeaveBalance]
	@Requestid bigint,
	@EmpID INT,
	@Status INT=1
	
AS
/*
Created: Tom.Vo@2013-11-05
Task: [tmpKIVLeaveBalance]
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
	
*/
SET NOCOUNT ON;


UPDATE dbo.[tmpKIVLeaveBalance] WITH(ROWLOCK,UPDLOCK)
SET [Status] = @Status 
WHERE Requestid = @Requestid


