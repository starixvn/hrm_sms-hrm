/****** Object:  Stored Procedure dbo.Employee_Contract_Update    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Employee_Contract_Update    Script Date: 7/19/2016 5:22:12 PM ******/

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Employee_Contract_Update] @EmpID INT
	,@BankAccount NVARCHAR(100) = NULL
	,@TaxCode NVARCHAR(150) = NULL
	,@SocialInsuranceNo NVARCHAR(150) = NULL
	,@RegisteredHospital NVARCHAR(250) = NULL
	,@Contract1 TINYINT = NULL
	,@FromDate1 DATETIME = NULL
	,@ToDate1 DATETIME = NULL
	,@Contract2 TINYINT = NULL
	,@FromDate2 DATETIME = NULL
	,@ToDate2 DATETIME = NULL
	,@Contract3 TINYINT = NULL
	,@FromDate3 DATETIME = NULL
	,@ToDate3 DATETIME = NULL
	,@Contract4 TINYINT = NULL
	,@FromDate4 DATETIME = NULL
	,@ToDate4 DATETIME = NULL
	,@Contract5 TINYINT = NULL
	,@FromDate5 DATETIME = NULL
	,@ToDate5 DATETIME = NULL
	,@History NVARCHAR(500) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: TOM@2013-10-01
Task: Update data of an employee Contract 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE [dbo].[EmployeeContract] WITH (ROWLOCK)
		SET [BankAccount] = @BankAccount
			,[TaxCode] = @TaxCode
			,[SocialInsuranceNo] = @SocialInsuranceNo
			,[RegisteredHospital] = @RegisteredHospital
			,[Contract1] = @Contract1
			,[FromDate1] = @FromDate1
			,[ToDate1] = @ToDate1
			,[Contract2] = @Contract2
			,[FromDate2] = @FromDate2
			,[ToDate2] = @ToDate2
			,[Contract3] = @Contract3
			,[FromDate3] = @FromDate3
			,[ToDate3] = @ToDate3
			,[Contract4] = @Contract4
			,[FromDate4] = @FromDate4
			,[ToDate4] = @ToDate4
			,[Contract5] = @Contract5
			,[FromDate5] = @FromDate5
			,[ToDate5] = @ToDate5
			,[History] = @History
		WHERE [EmpId] = @EmpId

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrCode = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 100
			,@ErrMsg = N'Update failed';
	END CATCH
END