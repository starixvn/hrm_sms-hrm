
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ResetRequest_byMember_6] @RequestID BIGINT
	,@EmpId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-07
Task: Reset Status  leave request form (type 6)
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:
*/
SET NOCOUNT ON

DECLARE @CurrentStatus INT
DECLARE 
	@ProcessedDate DATETIME
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@ReworkCount INT

SELECT TOP 1 @CurrentStatus = wk.[CurrentStatus]
	,@ProcessedDate = GETDATE()
	,@FromDate = lr.FromDate
FROM dbo.WorkingTimeRequest wk WITH (NOLOCK)
	LEFT JOIN dbo.LeaveRequest lr WITH (NOLOCK) ON wk.RequestId = lr.RequestId
WHERE wk.RequestID = @RequestID
	AND wk.EmpId = @EmpId

IF (@CurrentStatus > 1)
BEGIN
	SELECT @ReworkCount = COUNT(wkr.CurrentStatus)
	FROM [dbo].WorkingTimeRequestHistory wkr WITH (NOLOCK)
		INNER JOIN [dbo].[LeaveRequestHistory] lr WITH (NOLOCK) ON wkr.RequestId = lr.RequestId
		AND wkr.InsertedDate < lr.InsertedDate
	WHERE lr.RequestId = @RequestID
		AND wkr.CurrentStatus > 1

	IF (@ReworkCount > 1)
	BEGIN
		SET @ErrCode = 111
		SET @ErrMsg = 'This request was reedited already. You cannot reedit anymore.'

		RETURN;
	END

	IF @CurrentStatus <> 1
		AND CONVERT(DATETIME, CONVERT(NVARCHAR(25), @FromDate, 111)) <= CONVERT(DATETIME, CONVERT(NVARCHAR(25), GETDATE(), 111))
	BEGIN
		SET @ErrCode = 112
		SET @ErrMsg = 'Process awaiting approval. You could not edit this request.'

		RETURN;
	END
END
ELSE
BEGIN
	SET @ErrCode = 113
	SET @ErrMsg = 'Your request is pending. Could not reset this request.'

	RETURN;
END

SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
	SET CurrentStatus = 1
		,VerifiedStatus = 1
		,ApprovedStatus = 1
		,HRStatus = 1
		,ModifiedBy = @EmpId
		,ModifiedDate = @ProcessedDate
	WHERE RequestID = @RequestID

	IF @CurrentStatus IN (4, 6)
	BEGIN
		EXECUTE [dbo].[WorkingTimeRequest_LeaveReject_RollbackCalendar]
			@RequestID
			,@EmpId
			,@EmpId

		EXECUTE [dbo].[AnnualLeave_DeleteTransactionByRequestId] 
			@RequestId
			,@ProcessedDate
			,@EmpId

		EXECUTE EmployeeAttendance_ProcessingAndInsert @EmpId, @FromDate, @ToDate, 1
	END

	EXECUTE [dbo].WorkingTimeRequest_Update_TmpLeaveBalance
		@Requestid = @Requestid,
		@EmpID = @EmpID,
		@Status = 0

	EXECUTE [dbo].WorkingTimeRequest_Update_TmpKIVLeaveBalance
		@Requestid = @Requestid,
		@EmpID = @EmpID,
		@Status = 0

	EXEC [WorkingTimeRequest_WriteHistory] @RequestId

	COMMIT TRANSACTION;

	SELECT @ErrCode = 0;
END TRY

BEGIN CATCH
	SELECT @ErrCode = 100
		,@ErrMsg = 'Reset request failed'

	ROLLBACK TRANSACTION
END CATCH;
