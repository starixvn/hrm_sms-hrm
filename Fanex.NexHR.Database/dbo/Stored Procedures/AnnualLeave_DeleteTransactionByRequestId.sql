
CREATE PROCEDURE [dbo].[AnnualLeave_DeleteTransactionByRequestId] @RequestId INT
	,@ModifiedDate DATETIME
	,@ModifiedBy INT
AS
/*
Created: Quinn.Vo@20150928
Task: Delete leave transaction and calculate annual leave balance for employee
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @OldDeductDays NUMERIC(18, 1)
		,@OldCreditDays NUMERIC(18, 1)
		,@EmpId INT
		,@isDeleted BIT

	SELECT @EmpId = EmpId
		,@OldDeductDays = ISNULL(DeductDays, 0)
		,@OldCreditDays = ISNULL(CreditDays, 0)
		,@isDeleted = Deleted
	FROM dbo.LeaveTrans WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF ISNULL(@isDeleted, 0) = 1 RETURN

	UPDATE dbo.LeaveTrans WITH (ROWLOCK, UPDLOCK)
	SET Deleted = 1
		,ModifiedDate = @ModifiedDate
		,ModifiedBy = @ModifiedBy
	WHERE RequestId = @RequestId

	UPDATE dbo.EmployeeSummary WITH (ROWLOCK, UPDLOCK)
	SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance, 0) + @OldDeductDays - @OldCreditDays
	WHERE EmpId = @EmpId
END
