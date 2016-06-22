---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[AnnualLeave_ResetMultipleTransactions]	
	@EmpValue varchar(4000) = null, 	
	@Description nvarchar(4000),
	@FromDate datetime,
	@ResetDays numeric(18,1),
	@CreatedDate datetime,
	@CreatedBy int,

	@ErrCode int = 0 OUTPUT,
	@ErrMsg varchar(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: reset anual balance
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
	@EmpValue:
		<ROOT><Employee Id="1" LeaveBalance="2"/></ROOT>
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @xml XML
DECLARE @Emp TABLE(EmpID int,LeaveBalance decimal)

SET @xml = CONVERT(XML, @EmpValue)
INSERT INTO @Emp(EmpID,LeaveBalance)
SELECT	T.C.value('./@Id', 'int')
		,T.C.value('./@LeaveBalance', 'numeric(18,1)')		
FROM @xml.nodes('/ROOT/Employee') as [T](C) --T: table, C: column

BEGIN TRY
	BEGIN TRANSACTION;
		INSERT INTO dbo.LeaveTrans(EmpId, Description, FromDate, DeductDays,CreditDays, CreatedDate, CreatedBy)	
		SELECT EmpID,  @Description, @FromDate,LeaveBalance-@ResetDays,0, @CreatedDate, @CreatedBy
		FROM @Emp 
 
		UPDATE e
		SET e.AnnualLeaveBalance = e.AnnualLeaveBalance - (d.LeaveBalance-@ResetDays)
		FROM dbo.EmployeeSummary AS e
		INNER JOIN @Emp AS d ON e.EmpID = d.EmpID
    COMMIT TRANSACTION;
	SELECT @ErrCode=0,@ErrCode=''
END TRY
BEGIN CATCH
			ROLLBACK TRANSACTION;
			SELECT @ErrCode = 100, @ErrMsg = N'Reset anual balance failed.'
END CATCH;


