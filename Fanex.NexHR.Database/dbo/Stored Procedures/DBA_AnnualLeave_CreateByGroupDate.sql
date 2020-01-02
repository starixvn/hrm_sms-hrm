
CREATE PROCEDURE [dbo].[DBA_AnnualLeave_CreateByGroupDate] @Year INT
	,@EmpId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@2014.11.10
Task: create annual balance leave additional by joined date.
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Description NVARCHAR(100) = 'Add annual balance leave for 5 year in ' + CONVERT(NVARCHAR(10), @Year) + '.'
	DECLARE @CreatedDate DATETIME = GETDATE()
		,@FromDate DATE = CONVERT(NVARCHAR(10), @Year) + '/01/01'
		,@ToDate DATE
	DECLARE @tblEmployeeBalance TABLE (
		EmpId INT
		,Amount DECIMAL(18, 2)
		)

	SELECT @ToDate = DATEADD(DD, - 1, DATEADD(YY, 1, @FromDate))

	INSERT INTO @tblEmployeeBalance (
		EmpId
		,Amount
		)
	SELECT e.EmpId
		,CASE 
			WHEN es.WorkingDays = 15 THEN (
					((DATEDIFF(YY, es.CompanyDate, @FromDate) - 1) / 5) + CASE 
					WHEN ((DATEDIFF(YY, es.CompanyDate, @FromDate) - 1) % 5) = 4
						AND MONTH(es.CompanyDate) < 7 THEN 1
					ELSE 0 END
					) / 2.0
			ELSE (
					((DATEDIFF(YY, es.CompanyDate, @FromDate) - 1) / 5) + CASE 
					WHEN ((DATEDIFF(YY, es.CompanyDate, @FromDate) - 1) % 5) = 4
						AND MONTH(es.CompanyDate) < 7 THEN 1
					ELSE 0 END
					)
			END
	FROM dbo.EmployeeSummary AS es WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = es.EmpId
	WHERE ISNULL(isdeleted, 0) = 0
		AND e.[Enabled] = 1
		-- AND e.CompanyDate <= @FromDate
		AND es.CompanyDate IS NOT NULL
		AND es.CompanyDate <= @FromDate
		AND e.ResignedDate IS NULL
		AND (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
			)
		AND es.IsLocal = 1

	DELETE
	FROM @tblEmployeeBalance
	WHERE Amount = 0

	UPDATE es
	SET es.AnnualLeaveBalance = es.AnnualLeaveBalance + e.Amount
	FROM dbo.EmployeeSummary AS es WITH (ROWLOCK, UPDLOCK)
		INNER JOIN @tblEmployeeBalance AS e ON e.EmpId = es.EmpId

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
	SELECT e.EmpId
		,@Description
		,@FromDate
		,@ToDate
		,0 AS DeductDays
		,e.Amount AS CreditDays
		,@CreatedDate
		,NULL AS CreatedBy
		,0 AS Deleted
	FROM @tblEmployeeBalance AS e
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
END
