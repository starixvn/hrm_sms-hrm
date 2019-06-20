/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Delete_TmpLeaveBalance    Script Date: 7/20/2016 10:19:21 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Delete_TmpLeaveBalance    Script Date: 7/19/2016 5:22:21 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[WorkingTimeRequest_Delete_TmpLeaveBalance]
	@Requestid bigint,
	@EmpID INT,
	@Status INT=1
	
AS
/*
Created: Tom.Vo@2013-11-05
Task: Delete_TmpLeaveBalance
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
	
*/
SET NOCOUNT ON;


UPDATE dbo.tmpLeaveBalance WITH(ROWLOCK,UPDLOCK)
SET [Status] = @Status 
WHERE Requestid = @Requestid


