
CREATE PROCEDURE [dbo].[DBA_AnnualLeave_DeleteAnnualBalance] @Year INT
	,@EmpId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013.10.29
Task: delete annual balance leave.
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Description NVARCHAR(100) = 'Delete annual balance leave in ' + CONVERT(NVARCHAR(10), @Year) + '.'
	DECLARE @CreatedDate DATETIME = GETDATE()
		,@FromDate DATE = CONVERT(NVARCHAR(10), @Year) + '/01/01'
		,@ToDate DATE

	SELECT @ToDate = DATEADD(DD, - 1, DATEADD(YY, 1, @FromDate))

	UPDATE es
	SET AnnualLeaveBalance = es.AnnualLeaveBalance - lt.CreditDays
	FROM dbo.EmployeeSummary AS es WITH (ROWLOCK, UPDLOCK)
		INNER JOIN LeaveTrans AS lt WITH (NOLOCK) ON es.EmpId = lt.EmpId
		AND lt.FromDate = @FromDate
		AND ISNULL(lt.Deleted, 0) = 0
		AND lt.RequestId IS NULL
	WHERE @EmpId IS NULL
		OR es.EmpId = @EmpId

	DELETE dbo.LeaveTrans
	WHERE (
			@EmpId IS NULL
			OR EmpId = @EmpId
			)
		AND FromDate = @FromDate
		AND CreditDays > 0
		AND RequestId IS NULL
END
