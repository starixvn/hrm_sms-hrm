CREATE PROCEDURE [dbo].[AnnualLeave_TransferAnnualBalance] @Year INT
	,@CreatedDate DATETIME
	,@CreatedBy INT
	,@Description NVARCHAR(1000) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013.10.29
Task: Transfer annual balance leave.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	IF LEN(ISNULL(@Description, '')) = 0
		SET @Description = 'Transfer annual balance leave in ' + CONVERT(NVARCHAR(10), @Year) + '.'

	DECLARE @DeductDays_local NUMERIC(18, 1)
		,@CreditDays_local NUMERIC(18, 1)
	DECLARE @DeductDays_expat NUMERIC(18, 1)
		,@CreditDays_expat NUMERIC(18, 1)
	DECLARE @FromDate DATETIME
		,@LoS_Annual DATETIME
	DECLARE @MaxTranferAnnualLeaveBalance NUMERIC(18, 1)

	SELECT @FromDate = CONVERT(NVARCHAR(10), @YEAR) + '/01/01'
		,@LoS_Annual = CONVERT(NVARCHAR(10), @YEAR) + '/01/15'

	SELECT @DeductDays_local = 0
		,@CreditDays_local = 12

	SELECT @DeductDays_expat = 0
		,@CreditDays_expat = 42

	SELECT @MaxTranferAnnualLeaveBalance = 3

	--DECLARE @EmpID INT = 1
	IF EXISTS (
			SELECT 1
			FROM dbo.LeaveTrans LT WITH (NOLOCK)
				INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.EmpId = LT.EmpId
				INNER JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
			WHERE (
					ISNULL(isdeleted, 0) = 0
					OR (ISNULL(ResignedDate, GETDATE()) + 3650) < GETDATE()
					)
				AND ISNULL(e.ConfirmedDate, GETDATE() - 365) < @FromDate
				AND LT.FromDate = @FromDate
				AND es.IsLocal = 0
				AND [Description] = 'Reset  annual balance leave in ' + CONVERT(NVARCHAR(10), @Year - 1)
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'This process has been run last time.'

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
		SELECT e.EmpId
			,'Reset  annual balance leave in ' + CONVERT(NVARCHAR(10), @Year - 1) + '.' [Description]
			,DATEADD(DD, - 1, @FromDate) FromDate
			,es.AnnualLeaveBalance DeductDays
			,0 CreditDays
			,@CreatedDate
			,@CreatedBy
		FROM dbo.Employee e WITH (NOLOCK)
			INNER JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
			INNER JOIN dbo.LeaveTrans lt ON lt.EmpId = es.EmpId
			AND YEAR(lt.FromDate) = YEAR(DATEADD(DD, - 1, @FromDate))
			AND lt.CreditDays >= 0
		WHERE (
				ISNULL(isdeleted, 0) = 0
				OR (ISNULL(ResignedDate, GETDATE()) + 3650) < GETDATE()
				)
			AND ISNULL(e.ConfirmedDate, GETDATE() - 365) < @FromDate
			AND e.EmpId NOT IN (16, 139)

		INSERT INTO dbo.LeaveTrans (
			EmpId
			,[Description]
			,FromDate
			,DeductDays
			,CreditDays
			,CreatedDate
			,CreatedBy
			)
		SELECT e.EmpId
			,@Description
			,@FromDate
			,CASE 
				WHEN es.IsLocal = 1 THEN @DeductDays_local
				ELSE @DeductDays_local
				END DeductDays
			,CASE 
				WHEN es.IsLocal = 1 THEN (CASE 
						WHEN ISNULL(es.AnnualLeaveBalance, 0) > @MaxTranferAnnualLeaveBalance THEN @MaxTranferAnnualLeaveBalance
						ELSE ISNULL(es.AnnualLeaveBalance, 0) END
						) + @CreditDays_local + (CASE 
						WHEN (DATEDIFF(DD, e.JoinedDate, @LoS_Annual) / (365 * 5.00)) > 1 THEN (CASE 
								WHEN DATEPART(DD, e.JoinedDate) <= 15
									AND DATEPART(MM, e.JoinedDate) = 1 THEN DATEDIFF(DD, e.JoinedDate, @LoS_Annual) / (365 * 5)
								ELSE DATEDIFF(DD, e.JoinedDate, @LoS_Annual) / (365 * 5) - 1 END
								)
						ELSE 0 END
						)
				ELSE @CreditDays_expat
				END CreditDays
			,@CreatedDate
			,@CreatedBy
		FROM dbo.Employee e WITH (NOLOCK)
			INNER JOIN dbo.EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
		WHERE (
				ISNULL(isdeleted, 0) = 0
				OR (ISNULL(ResignedDate, GETDATE()) + 3650) < GETDATE()
				)
			AND ISNULL(e.ConfirmedDate, GETDATE() - 365) < @FromDate
			AND e.EmpId NOT IN (16, 139)

		UPDATE es
		SET es.AnnualLeaveBalance = (CASE 
				WHEN es.IsLocal = 1 THEN (CASE 
						WHEN ISNULL(es.AnnualLeaveBalance, 0) > @MaxTranferAnnualLeaveBalance THEN @MaxTranferAnnualLeaveBalance
						ELSE ISNULL(es.AnnualLeaveBalance, 0) END
						) + (CASE 
						WHEN (DATEDIFF(DD, e.JoinedDate, @LoS_Annual) / (365 * 5.00)) > 1 THEN (CASE 
								WHEN DATEPART(DD, e.JoinedDate) <= 15
									AND DATEPART(MM, e.JoinedDate) = 1 THEN DATEDIFF(DD, e.JoinedDate, @LoS_Annual) / (365 * 5)
								ELSE DATEDIFF(DD, e.JoinedDate, @LoS_Annual) / (365 * 5) - 1 END
								)
						ELSE 0 END
						)
				ELSE 0 END
				) + (CASE 
				WHEN es.IsLocal = 1 THEN @CreditDays_local
				ELSE @CreditDays_expat END
				) - (CASE 
				WHEN es.IsLocal = 1 THEN @DeductDays_local
				ELSE @DeductDays_expat END
				)
		FROM dbo.EmployeeSummary es WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.EmpId = es.EmpId
		WHERE (
				ISNULL(isdeleted, 0) = 0
				OR (ISNULL(ResignedDate, GETDATE()) + 3650) < GETDATE()
				)
			AND ISNULL(e.ConfirmedDate, GETDATE() - 365) < @FromDate
			AND e.EmpId NOT IN (16, 139)

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrCode = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 100
			,@ErrMsg = N'Tranfers anual balance failed.'

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
		,'Transfer annual balance leave in ' + CONVERT(NVARCHAR(10), @Year) + '.'
		,'<LogDate: ' + CONVERT(NVARCHAR(25), getdate(), 101)
		  + ',DeductDays_local: '+ CONVERT(NVARCHAR(25), @DeductDays_local)
		  + ',DeductDays_expat: ' + CONVERT(NVARCHAR(25), @DeductDays_expat)
		  + ',CreditDays_local: ' + CONVERT(NVARCHAR(25), @CreditDays_local)
		  + ',CreditDays_expat: ' + CONVERT(NVARCHAR(25), @CreditDays_expat)
		  + ',ModifiedDate: ' + CONVERT(NVARCHAR(25), ISNULL(@CreatedDate, getdate()))
		  + ',ModifiedBy:' + CONVERT(NVARCHAR(25), ISNULL(@CreatedBy, '-')) + '>' AS [TrackingValue]
		,1000 AS [UserID]
END