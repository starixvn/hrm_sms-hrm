
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AnnualLeave_InsertTransaction] @EmpId INT
	,@RequestId INT = NULL
	,@Description NVARCHAR(4000) = ''
	,@FromDate DATETIME
	,@ToDate DATETIME = NULL
	,@DeductDays NUMERIC(18, 1)
	,@CreditDays NUMERIC(18, 1)
	,@CreatedDate DATETIME
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-29
Task: record leave transaction and calculate annual leave balance for employee
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON;

	IF EXISTS (
			SELECT 1
			FROM EmployeeSummary es WITH (NOLOCK)
			WHERE es.EmpId = @EmpId
				AND es.IsLocal = 1
				AND @CreditDays > 13
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Employees - Local: 12. '

		RETURN
	END
	ELSE IF EXISTS (
			SELECT 1
			FROM EmployeeSummary es WITH (NOLOCK)
			WHERE es.EmpId = @EmpId
				AND es.IsLocal = 0
				AND @CreditDays > 42
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Employees - Expat: no rules (only 42 ALs a day).'

		RETURN
	END
	ELSE IF EXISTS (
			SELECT 1
			FROM Employee es WITH (NOLOCK)
			WHERE es.EmpId = @EmpId
				AND ISNULL(es.ConfirmedDate, GETDATE() - 365) > @FromDate
				AND @CreditDays > 0
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Probation: no annual days.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO dbo.LeaveTrans (
			EmpId
			,RequestId
			,[Description]
			,FromDate
			,ToDate
			,DeductDays
			,CreditDays
			,CreatedDate
			,CreatedBy
			)
		VALUES (
			@EmpId
			,@RequestId
			,@Description
			,@FromDate
			,@ToDate
			,@DeductDays
			,@CreditDays
			,@CreatedDate
			,@CreatedBy
			)

		UPDATE dbo.EmployeeSummary WITH (ROWLOCK, UPDLOCK)
		SET AnnualLeaveBalance = ISNULL(AnnualLeaveBalance, 0) + ISNULL(@CreditDays, 0) - ISNULL(@DeductDays, 0)
		WHERE EmpId = @EmpId

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrCode = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 100
			,@ErrMsg = N'working calendar for an employee failed.'
	END CATCH;
END