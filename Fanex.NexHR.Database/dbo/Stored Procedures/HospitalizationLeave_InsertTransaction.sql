
CREATE PROCEDURE [dbo].[HospitalizationLeave_InsertTransaction] @EmpId INT
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
Created: Tim.Trinh@2021-08-20
Task: insert transaction
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	INSERT INTO dbo.HospitalizationLeaveTrans (
		EmpId
		,RequestId
		,[Description]
		,FromDate
		,ToDate
		,DeductDays
		,CreditDays
		,CreatedDate
		,CreatedBy
		,IsDeleted
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
		,0
		)

	UPDATE dbo.EmployeeSummary WITH (ROWLOCK, UPDLOCK)
	SET HospitalizationLeaveBalance = ISNULL(HospitalizationLeaveBalance, 0) + ISNULL(@CreditDays, 0) - ISNULL(@DeductDays, 0)
	WHERE EmpId = @EmpId
END