/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Update_TmpLeaveBalance    Script Date: 7/20/2016 10:19:25 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_Update_TmpLeaveBalance    Script Date: 7/19/2016 5:22:24 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_Update_TmpLeaveBalance] 
	@Requestid BIGINT
	,@EmpID INT
	,@Status INT = 1
AS
/*
Created: Quinn.Vo@20150925
Task: update temp al leave balance.
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.tmpLeaveBalance WITH (ROWLOCK, UPDLOCK)
	SET [Status] = @Status
	WHERE Requestid = @Requestid
END
