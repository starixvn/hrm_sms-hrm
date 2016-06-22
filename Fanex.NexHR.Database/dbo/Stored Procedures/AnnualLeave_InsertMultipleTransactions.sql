---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[AnnualLeave_InsertMultipleTransactions]
	@EmpIds varchar(4000) = null, 	
	@Description nvarchar(4000)='',
	@FromDate datetime,
	@DeductDays numeric(18,1),
	@CreditDays numeric(18,1),
	@CreatedDate datetime,
	@CreatedBy int,

	@ErrCode int = 0 OUTPUT
	,@ErrMsg varchar(200) = '' OUTPUT	
AS
/*
Created: Tom.Vo@2013.10.29
Task: record multiple annual leave transaction and calculate annual leave balance for employee
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE	@Emp TABLE(ID int) 

INSERT INTO @Emp (ID)
SELECT val 
FROM dbo.[fnc_strSplit](@EmpIDs,',')

IF EXISTS(SELECT TOP 1 1 
			FROM EmployeeSummary es WITH(NOLOCK) 
				INNER JOIN @Emp e ON es.EmpId= e.ID
					WHERE es.IsLocal= 1 AND @CreditDays >13)
	BEGIN
		SELECT @ErrCode = 100, @ErrMsg = N'Employees - Local: 12. '
		RETURN
	END
ELSE IF EXISTS(SELECT TOP 1 1 
			FROM EmployeeSummary es WITH(NOLOCK) 
				INNER JOIN @Emp e ON es.EmpId= e.ID
					WHERE es.IsLocal= 0 AND @CreditDays >42)
	BEGIN
		SELECT @ErrCode = 100, @ErrMsg =  N'Employees - Expat: no rules (only 42 ALs a day).'
		RETURN
	END
ELSE IF EXISTS(SELECT TOP 1 1 
			FROM Employee es WITH(NOLOCK) 
				INNER JOIN @Emp e ON es.EmpId= e.ID
					WHERE ISNULL(es.ConfirmedDate,GETDATE()-365) > @FromDate AND @CreditDays>0)
	BEGIN
		SELECT @ErrCode = 100, @ErrMsg = N'Probation: no annual days.'
		RETURN
	END
BEGIN TRY
	BEGIN TRANSACTION;
		INSERT INTO dbo.LeaveTrans(EmpId, Description, FromDate, DeductDays,CreditDays, CreatedDate, CreatedBy)	
		SELECT EmpId,  @Description, @FromDate, @DeductDays,@CreditDays, @CreatedDate, @CreatedBy
		from dbo.Employee WITH(NOLOCK)
		WHERE ((@EmpIds IS NULL AND ISNULL(isdeleted,0)=0) OR  
				EmpId in (select id from @Emp))
 
		UPDATE dbo.EmployeeSummary WITH(ROWLOCK,UPDLOCK)
		SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance,0) + ISNULL(@CreditDays,0) - ISNULL(@DeductDays,0)
		WHERE EmpId in (select id from @Emp)
    COMMIT TRANSACTION;
	SELECT @ErrCode=0,@ErrCode=''
END TRY
BEGIN CATCH
			ROLLBACK TRANSACTION;
			SELECT @ErrCode = 100, @ErrMsg = N'Reset anual balancefailed.'
END CATCH;


