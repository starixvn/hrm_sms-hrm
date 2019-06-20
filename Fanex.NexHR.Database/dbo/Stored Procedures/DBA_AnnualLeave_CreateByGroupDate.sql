
CREATE PROCEDURE [dbo].[DBA_AnnualLeave_CreateByGroupDate] -- 2015, null, '2014-11-10'
	@Year INT
	,@EmpID INT = NULL
	,@CreatedDate DATETIME
	,@Description NVARCHAR(1000) = ''
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

	IF LEN(ISNULL(@Description, '')) = 0
		SET @Description = 'Add annual balance leave for 5 year in ' + CONVERT(NVARCHAR(10), @Year) + '.'

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
	DECLARE @EmployeeBalance TABLE (
		EmpId INT
		,Amount DECIMAL(18, 2)
		)

	SELECT @FromDate = CONVERT(NVARCHAR(10), @YEAR) + '/01/01'

	SELECT @ToDate = DATEADD(DD, - 1, DATEADD(YY, 1, @FromDate))

	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO @EmployeeBalance (
			EmpId
			,Amount
			)
		SELECT e.EmpId
			,CASE 
				WHEN es.WorkingDays = 15 THEN (
						((DATEDIFF(YY, es.CompanyDate, @FromDate) - 1) / 5) + CASE 
						WHEN (
								((DATEDIFF(YY, es.CompanyDate, @FromDate) - 1) % 5) = 4
								AND MONTH(es.CompanyDate) < 7
								) THEN 1
						ELSE 0 END
						) / 2.0
				ELSE (
						((DATEDIFF(YY, es.CompanyDate, @FromDate) - 1) / 5) + CASE 
						WHEN (
								((DATEDIFF(YY, es.CompanyDate, @FromDate) - 1) % 5) = 4
								AND MONTH(es.CompanyDate) < 7
								) THEN 1
						ELSE 0 END
						)
				END
		FROM dbo.EmployeeSummary es WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.EmpId = es.EmpId
		WHERE ISNULL(isdeleted, 0) = 0
			AND e.[Enabled] = 1
			-- AND e.CompanyDate <= @FromDate
			AND es.CompanyDate IS NOT NULL 
			AND es.CompanyDate <= @FromDate
			AND e.ResignedDate IS NULL
			AND (
				e.EmpId = @EmpID
				OR @EmpID IS NULL
				)
			AND es.IsLocal = 1

		DELETE
		FROM @EmployeeBalance
		WHERE Amount = 0

		UPDATE es
		SET es.AnnualLeaveBalance = es.AnnualLeaveBalance + e.Amount
		FROM dbo.EmployeeSummary es WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @EmployeeBalance e ON e.EmpId = es.EmpId

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
			,@FromDate FromDate
			,@ToDate ToDate
			,0 AS DeductDays
			,e.Amount AS CreditDays
			,@CreatedDate CreatedDate
			,NULL CreatedBy
			,0 Deleted
		FROM @EmployeeBalance e
			INNER JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrCode = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 100
			,@ErrMsg = N'Add annual balance leave for 5 year failed.'

		RETURN
	END CATCH;

	INSERT INTO [dbo].[Log] (
		[LogActionID]
		,[LogDate]
		,[Description]
		,[TrackingValue]
		,[UserID]
		)
	SELECT 100
		,CONVERT(NVARCHAR(10), @YEAR) + '/01/01' AS [LogDate]
		,'Add annual balance leave for 5 year in ' + CONVERT(NVARCHAR(10), @Year) + '.'
		,'<LogDate: ' + CONVERT(NVARCHAR(25), getdate(), 101)
		  + ',ModifiedDate: ' + CONVERT(NVARCHAR(25), ISNULL(@CreatedDate, getdate()))
		  + ',ModifiedBy:' + CONVERT(NVARCHAR(25), ISNULL(NULL, '-')) + '>' AS [TrackingValue]
		,1000 AS [UserID]
END