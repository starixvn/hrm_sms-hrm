/****** Object:  Stored Procedure dbo.DBA_AnnualLeave_DeleteAnnualBalance    Script Date: 7/20/2016 10:19:00 AM ******/
/****** Object:  Stored Procedure dbo.DBA_AnnualLeave_DeleteAnnualBalance    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[DBA_AnnualLeave_DeleteAnnualBalance] @Year INT
	,@EmpID INT = NULL
	,@CreatedDate DATETIME
	,@CreatedBy INT
	,@Description NVARCHAR(1000) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013.10.29
Task: create annual balance leave.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	IF LEN(ISNULL(@Description, '')) = 0
		SET @Description = 'Delete annual balance leave in ' + CONVERT(NVARCHAR(10), @Year) + '.'

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@LoS_Annual DATETIME

	SELECT @FromDate = CONVERT(NVARCHAR(10), @YEAR) + '/01/01'
		,@LoS_Annual = CONVERT(NVARCHAR(10), @YEAR) + '/01/15'

	SELECT @ToDate = DATEADD(DD, - 1, DATEADD(YY, 1, @FromDate))

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE ES
		SET AnnualLeaveBalance = es.AnnualLeaveBalance - lt.CreditDays
		FROM dbo.EmployeeSummary ES
			INNER JOIN LeaveTrans lt ON ES.EmpId = lt.EmpId
			AND FromDate = @FromDate
			AND ISNULL(lt.Deleted, 0) = 0
			AND RequestId IS NULL
		WHERE ES.EmpId = @EmpID
			OR @EmpID IS NULL

		DELETE dbo.LeaveTrans
		WHERE (
				EmpId = @EmpID
				OR @EmpID IS NULL
				)
			AND FromDate = @FromDate
			AND CreditDays > 0
			AND RequestId IS NULL

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
		,'Delete annual balance leave in ' + CONVERT(NVARCHAR(10), @Year) + '.'
		,'<LogDate: ' + CONVERT(NVARCHAR(25), getdate(), 101)
		  + ',ModifiedDate: ' + CONVERT(NVARCHAR(25), ISNULL(@CreatedDate, getdate()))
		  + ',ModifiedBy:' + CONVERT(NVARCHAR(25), ISNULL(@CreatedBy, '-')) + '>' AS [TrackingValue]
		,1000 AS [UserID]
END