
CREATE PROCEDURE [dbo].[Auto_AnnualLeaveBalance_UpdateForNewEmployee] @Year INT
	,@EmpId INT = NULL
	,@IsDelete BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2020-01-03
Task: Update annual leave balance for new employee
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Description NVARCHAR(100) = 'Create annual balance leave in ' + CONVERT(NVARCHAR(10), @Year) + '.'
	DECLARE @CreatedDate DATETIME = GETDATE()
		,@Yesterday DATE = DATEADD(DAY, - 1, GETDATE())
		,@FromDate DATE = CONVERT(NVARCHAR(10), @Year) + '/01/01'
		,@ToDate DATE

	SELECT @ToDate = DATEADD(DAY, - 1, DATEADD(YEAR, 1, @FromDate))

	DECLARE @tblEmployeeBalance TABLE (
		EmpId INT
		,Amount DECIMAL(5, 2)
		)

	INSERT INTO @tblEmployeeBalance (
		EmpId
		,Amount
		)
	SELECT emp.EmpId
		,ROUND((ISNULL(gr.BasicAnnualLeaveBalance, 0) / 12.0) * (CASE 
				WHEN DAY(emp.JoinedDate) <= 15 THEN DATEDIFF(MONTH, emp.JoinedDate, CONVERT(NVARCHAR(10), YEAR(emp.ConfirmedDate)) + '/12/01')
				ELSE DATEDIFF(MONTH, emp.JoinedDate, CONVERT(NVARCHAR(10), YEAR(emp.ConfirmedDate)) + '/12/01') - 1 END
				), 0)
	FROM dbo.EmployeeSummary AS empsum WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		AND ISNULL(emp.isDeleted, 0) = 0
		AND emp.[Enabled] = 1
		AND emp.ResignedDate IS NULL
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE emp.ConfirmedDate IS NOT NULL
		AND emp.ConfirmedDate = @Yesterday
		AND (
			@EmpId IS NULL
			OR emp.EmpId = @EmpId
			)

	DELETE
	FROM @tblEmployeeBalance
	WHERE Amount = 0

	IF @IsDelete = 1
	BEGIN
		UPDATE empsum
		SET AnnualLeaveBalance = empsum.AnnualLeaveBalance - lt.CreditDays
		FROM dbo.EmployeeSummary AS empsum WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.LeaveTrans AS lt WITH (NOLOCK) ON empsum.EmpId = lt.EmpId
			AND lt.FromDate = @FromDate
			AND ISNULL(lt.Deleted, 0) = 0
			AND lt.RequestId IS NULL
		WHERE @EmpId IS NULL
			OR empsum.EmpId = @EmpId

		DELETE dbo.LeaveTrans
		WHERE (
				@EmpId IS NULL
				OR EmpId = @EmpId
				)
			AND FromDate = @FromDate
			AND CreditDays > 0
			AND RequestId IS NULL
	END

	UPDATE empsum
	SET empsum.AnnualLeaveBalance = empsum.AnnualLeaveBalance + emp.Amount
	FROM dbo.EmployeeSummary AS empsum WITH (ROWLOCK, UPDLOCK)
		INNER JOIN @tblEmployeeBalance AS emp ON emp.EmpId = empsum.EmpId

	INSERT INTO dbo.LeaveTrans (
		EmpId
		,[Description]
		,FromDate
		,ToDate
		,DeductDays
		,CreditDays
		,CreatedDate
		,CreatedBy
		,Deleted
		)
	SELECT EmpId
		,@Description
		,@FromDate
		,@ToDate
		,0 AS DeductDays
		,Amount AS CreditDays
		,@CreatedDate
		,NULL AS CreatedBy
		,0 AS Deleted
	FROM @tblEmployeeBalance
END
