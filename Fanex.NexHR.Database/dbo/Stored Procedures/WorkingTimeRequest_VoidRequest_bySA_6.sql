
CREATE PROCEDURE [dbo].[WorkingTimeRequest_VoidRequest_bySA_6] @RequestID BIGINT
	,@ProcessedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151026@Quinn.Vo
Task: Void request by SA
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @CurrentStatus INT
	DECLARE 
		@ProcessedDate DATETIME
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@RequestType SMALLINT
		,@EmpId INT

	SELECT TOP 1 @CurrentStatus = wk.[CurrentStatus]
		,@ProcessedDate = GETDATE()
		,@FromDate = lr.FromDate
		,@RequestType = wk.RequestType
		,@EmpId = wk.EmpId
	FROM dbo.WorkingTimeRequest wk WITH (NOLOCK)
		LEFT JOIN dbo.LeaveRequest lr WITH (NOLOCK) ON wk.RequestId = lr.RequestId
	WHERE wk.RequestID = @RequestID

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;
		UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
		SET CurrentStatus = 7
			,HRId = @ProcessedBy
			,HRDate = @ProcessedDate
			,ModifiedBy = @ProcessedBy
			,ModifiedDate = @ProcessedDate
		WHERE RequestID = @RequestID

		IF @CurrentStatus IN (4, 6)
		BEGIN
			EXECUTE [dbo].[WorkingTimeRequest_LeaveReject_RollbackCalendar]
				@RequestID
				,@EmpId
				,@ProcessedBy

			EXECUTE [dbo].[AnnualLeave_DeleteTransactionByRequestId] 
				@RequestId
				,@ProcessedDate
				,@ProcessedBy

			EXECUTE EmployeeAttendance_ProcessingAndInsert @EmpId, @FromDate, @ToDate, 1
		END

		EXECUTE [dbo].WorkingTimeRequest_Update_TmpLeaveBalance
			@Requestid = @Requestid,
			@EmpID = @EmpId,
			@Status = 2

		EXECUTE [dbo].WorkingTimeRequest_Update_TmpKIVLeaveBalance
			@Requestid = @Requestid,
			@EmpID = @EmpId,
			@Status = 2

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Void request failed'

		ROLLBACK TRANSACTION
	END CATCH;
END