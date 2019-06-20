/****** Object:  Stored Procedure dbo.WorkingTimeRequest_AvaibleBalanceLeave    Script Date: 7/20/2016 10:19:20 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_AvaibleBalanceLeave    Script Date: 7/19/2016 5:22:21 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_AvaibleBalanceLeave] 
	@EmpId INT
	,@RequestID BIGINT = NULL
	,@EffectedDay DATETIME = NULL
AS
/*
Created: Tom.Vo@2013-11-05
Task: Get  Avaible Balance Leave From Request ID
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF @EffectedDay IS NULL
	BEGIN
		SELECT @EffectedDay = CONVERT(DATETIME, CONVERT(NVARCHAR(10), YEAR(GETDATE())) + '/01/01')
	END

	DECLARE @KIVBalance NUMERIC(18, 2)
		,@LeaveBalanceDay NUMERIC(18, 2)
		,@CurrenLeaveDay NUMERIC(18, 2)

	SELECT @KIVBalance = dbo.fnc_GetKIVBalance(@EmpId, @RequestID)
		,@LeaveBalanceDay = dbo.fnc_GetAnnualLeaveBalanceByYear(@EmpId, YEAR(@EffectedDay), @RequestID)
		,@CurrenLeaveDay = dbo.fnc_GetAnnualLeaveBalanceByDate(@EmpId, @EffectedDay, @RequestID)

	SELECT @EmpId AS EmployeeId
		,@LeaveBalanceDay AS AnnualBalance
		,@KIVBalance AS KeepInViewBalance
		,@CurrenLeaveDay AS CurrentLeaveBalanceDay
END
