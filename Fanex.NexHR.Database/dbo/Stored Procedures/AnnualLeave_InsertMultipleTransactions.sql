/****** Object:  Stored Procedure dbo.AnnualLeave_InsertMultipleTransactions    Script Date: 7/20/2016 10:18:59 AM ******/
/****** Object:  Stored Procedure dbo.AnnualLeave_InsertMultipleTransactions    Script Date: 7/19/2016 5:22:10 PM ******/
CREATE PROCEDURE [dbo].[AnnualLeave_InsertMultipleTransactions] @EmpIds VARCHAR(4000) = NULL
	,@Description NVARCHAR(4000) = ''
	,@FromDate DATETIME
	,@DeductDays NUMERIC(18, 1)
	,@CreditDays NUMERIC(18, 1)
	,@CreatedDate DATETIME
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013.10.29
Task: record multiple annual leave transaction and calculate annual leave balance for employee
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Emp TABLE (Id INT)

	INSERT INTO @Emp (Id)
	SELECT val
	FROM dbo.[fnc_strSplit](@EmpIds, ',')

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary AS es WITH (NOLOCK)
				INNER JOIN @Emp AS e ON es.EmpId = e.Id
			WHERE es.IsLocal = 1
				AND @CreditDays > 13
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Employees - Local: 12. '

		RETURN
	END
	ELSE IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary AS es WITH (NOLOCK)
				INNER JOIN @Emp AS e ON es.EmpId = e.Id
			WHERE es.IsLocal = 0
				AND @CreditDays > 42
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Employees - Expat: no rules (only 42 ALs a day).'

		RETURN
	END
	ELSE IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS es WITH (NOLOCK)
				INNER JOIN @Emp AS e ON es.EmpId = e.Id
			WHERE ISNULL(es.ConfirmedDate, GETDATE() - 365) > @FromDate
				AND @CreditDays > 0
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Probation: no annual days.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO dbo.LeaveTrans (
			EmpId
			,[Description]
			,FromDate
			,DeductDays
			,CreditDays
			,CreatedDate
			,CreatedBy
			)
		SELECT emp.EmpId
			,@Description
			,@FromDate
			,@DeductDays
			,@CreditDays
			,@CreatedDate
			,@CreatedBy
		FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN @Emp AS temp ON emp.EmpId = temp.ID
		WHERE (
				@EmpIds IS NOT NULL
				AND ISNULL(isdeleted, 0) = 0
				)

		UPDATE dbo.EmployeeSummary WITH (ROWLOCK, UPDLOCK)
		SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance, 0) + ISNULL(@CreditDays, 0) - ISNULL(@DeductDays, 0)
		WHERE EmpId IN (
				SELECT id
				FROM @Emp
				)

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrCode = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 100
			,@ErrMsg = N'Reset anual balancefailed.'
	END CATCH;
END