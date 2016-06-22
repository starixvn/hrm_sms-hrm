---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/*
AnnualLeave_UpdateTransaction 1, NULL, 'CAP NGAY PHEP DAU NAM', '2012/01/01', '2012/01/01',0,12, '2012/04/20', 86
*/

CREATE PROCEDURE [dbo].[AnnualLeave_UpdateTransaction]	
	@LeaveTransId int
	,@Description nvarchar(4000) = ''
	,@FromDate datetime
	,@DeductDays numeric(18,1)
	,@CreditDays numeric(18,1)
	,@ModifiedDate datetime
	,@ModifiedBy int
		
	,@ErrCode int = 0 OUTPUT
	,@ErrMsg varchar(200) = '' OUTPUT	
AS

/*
Created: Tom.Vo@2013.10.22
Task: update leave transaction and calculate annual leave balance for employee
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
SET NOCOUNT ON

DECLARE @OldDeductDays numeric(18,1), @OldCreditDays numeric(18,1) , @EmpID int

SELECT @EmpID = EmpID
	, @OldDeductDays =  ISNULL(DeductDays,0), @OldCreditDays = ISNULL(CreditDays,0)
FROM dbo.LeaveTrans WITH(NOLOCK)
WHERE LeaveTransId=@LeaveTransId

BEGIN TRY
	BEGIN TRANSACTION
		UPDATE dbo.LeaveTrans WITH (ROWLOCK, UPDLOCK)
		SET Description= @Description,
			FromDate = @FromDate,
			ToDate = @FromDate,
			DeductDays = @DeductDays,
			CreditDays = @CreditDays,
			ModifiedDate = @ModifiedDate,
			ModifiedBy = @ModifiedBy
		WHERE LeaveTransId=@LeaveTransId

		UPDATE dbo.EmployeeSummary WITH (ROWLOCK, UPDLOCK)
		SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance,0) + @OldDeductDays - @OldCreditDays + ISNULL(@CreditDays,0) - ISNULL(@DeductDays,0)
		WHERE EmpID = @EmpId
	 COMMIT TRANSACTION;
	SELECT @ErrCode=0,@ErrCode=''
END TRY
BEGIN CATCH
			ROLLBACK TRANSACTION;
			SELECT @ErrCode = 100, @ErrMsg = N'Update leave transaction and calculate annual leave balance for employee failed.'
END CATCH;


