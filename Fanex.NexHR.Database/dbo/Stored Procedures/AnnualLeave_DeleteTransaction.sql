
CREATE PROCEDURE [dbo].[AnnualLeave_DeleteTransaction] @LeaveTransId INT
	,@ModifiedDate DATETIME
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013.10.29
Task: soft delete leave transaction and calculate annual leave balance for employee
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @OldDeductDays NUMERIC(18, 1)
		,@OldCreditDays NUMERIC(18, 1)
		,@EmpId INT
		,@isDeleted BIT

	SELECT @EmpId = EmpId
		,@OldDeductDays = ISNULL(DeductDays, 0)
		,@OldCreditDays = ISNULL(CreditDays, 0)
		,@isDeleted = Deleted
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE LeaveTransId = @LeaveTransId

	IF ISNULL(@isDeleted, 0) <> 1
	BEGIN
		UPDATE dbo.LeaveTrans WITH (ROWLOCK, UPDLOCK)
		SET Deleted = 1
			,ModifiedDate = @ModifiedDate
			,ModifiedBy = @ModifiedBy
		WHERE LeaveTransId = @LeaveTransId

		UPDATE dbo.EmployeeSummary WITH (ROWLOCK, UPDLOCK)
		SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance, 0) + @OldDeductDays - @OldCreditDays
		WHERE EmpId = @EmpId
	END
END
