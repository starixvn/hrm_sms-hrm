
CREATE PROCEDURE [dbo].[DBA_AnnualLeave_CreateAnnualBalance] @Year INT
	,@EmpID INT = NULL
	,@CreatedDate DATETIME
	,@Description NVARCHAR(1000) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013.10.29
Task: DELETE annual balance leave.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	IF LEN(ISNULL(@Description, '')) = 0
		SET @Description = 'Create annual balance leave in ' + CONVERT(NVARCHAR(10), @Year) + '.'

	DECLARE @DeductDays_local NUMERIC(18, 1)
		,@CreditDays_local NUMERIC(18, 1)
		,@CreditDays_DriverAndHouseGuard NUMERIC(18, 1)
	DECLARE @DeductDays_expat NUMERIC(18, 1)
		,@CreditDays_expat NUMERIC(18, 1)
	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@LoS_Annual DATETIME
	DECLARE @MaxTranferAnnualLeaveBalance NUMERIC(18, 1)

	SELECT @FromDate = CONVERT(NVARCHAR(10), @YEAR) + '/01/01'
		,@LoS_Annual = CONVERT(NVARCHAR(10), @YEAR) + '/01/07'

	SELECT @ToDate = DATEADD(DD, - 1, DATEADD(YY, 1, @FromDate))

	SELECT @DeductDays_local = 0
		,@CreditDays_local = 12
		,@CreditDays_DriverAndHouseGuard = 6

	SELECT @DeductDays_expat = 0
		,@CreditDays_expat = 42

	SELECT @MaxTranferAnnualLeaveBalance = 0

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE es
		SET es.AnnualLeaveBalance = (
				es.AnnualLeaveBalance + (CASE 
					WHEN es.IsLocal = 1 THEN (CASE 
							WHEN (es.WorkingDays = 15) THEN @CreditDays_DriverAndHouseGuard
							ELSE @CreditDays_local END
							)
					ELSE @CreditDays_expat END
					)
				)
		FROM dbo.EmployeeSummary es WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.EmpId = es.EmpId
			LEFT JOIN (
			SELECT MAX(FromDate) FromDate
				,EmpId
			FROM dbo.LeaveTrans lt
			WHERE lt.FromDate >= @FromDate
				AND lt.CreditDays > 0
				AND ISNULL(Deleted, 0) = 0
			GROUP BY EmpId
			) lt ON e.EmpId = lt.EmpId
		WHERE ISNULL(isdeleted, 0) = 0
			AND e.[Enabled] = 1
			AND ((es.CompanyDate IS NOT NULL AND es.CompanyDate <= @FromDate)
				OR es.IsLocal = 0
			)
			AND e.ResignedDate IS NULL
			AND (
				e.EmpId = @EmpID
				OR @EmpID IS NULL
				)
			AND lt.EmpId IS NULL

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
			,@Description Description
			,@FromDate FromDate
			,@ToDate ToDate
			,0 AS DeductDays
			,(CASE 
				WHEN es.IsLocal = 1 THEN (CASE 
						WHEN (es.WorkingDays = 15) THEN @CreditDays_DriverAndHouseGuard
						ELSE @CreditDays_local END
						)
				ELSE @CreditDays_expat END
				) AS CreditDays
			,@CreatedDate CreatedDate
			,NULL CreatedBy
			,0 Deleted
		FROM dbo.Employee e WITH (NOLOCK)
			INNER JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
			LEFT JOIN (
			SELECT MAX(FromDate) FromDate
				,EmpId
			FROM dbo.LeaveTrans lt
			WHERE lt.FromDate >= @FromDate
				AND lt.CreditDays > 0
				AND ISNULL(Deleted, 0) = 0
			GROUP BY EmpId
			) lt ON e.EmpId = lt.EmpId
		WHERE ISNULL(isdeleted, 0) = 0
			AND e.[Enabled] = 1
			AND ((es.CompanyDate IS NOT NULL AND es.CompanyDate <= @FromDate)
				OR es.IsLocal = 0
			)
			AND e.ResignedDate IS NULL
			AND (
				e.EmpId = @EmpID
				OR @EmpID IS NULL
				)
			AND lt.EmpId IS NULL

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrCode = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 100
			,@ErrMsg = N'Create anual balance failed.'

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
		,'Create annual balance leave in ' + CONVERT(NVARCHAR(10), @Year) + '.'
		,'<LogDate: ' + CONVERT(NVARCHAR(25), getdate(), 101)
		  + ',DeductDays_local: ' + CONVERT(NVARCHAR(25), @DeductDays_local)
		  + ',DeductDays_expat: ' + CONVERT(NVARCHAR(25), @DeductDays_expat)
		  + ',CreditDays_local: ' + CONVERT(NVARCHAR(25), @CreditDays_local)
		  + ',CreditDays_expat: ' + CONVERT(NVARCHAR(25), @CreditDays_expat)
		  + ',ModifiedDate: ' + CONVERT(NVARCHAR(25), ISNULL(@CreatedDate, getdate()))
		  + ',ModifiedBy:' + CONVERT(NVARCHAR(25), ISNULL(NULL, '-')) + '>' AS [TrackingValue]
		,1000 AS [UserID]
END