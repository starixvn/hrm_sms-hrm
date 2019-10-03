
CREATE PROCEDURE [dbo].[LeaveRequest_VoidBySA] @RequestId BIGINT
	,@ActorId INT
	,@Comment NVARCHAR(500) = ''
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
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @CurrentStatus INT
		,@ProcessedDate DATETIME = GETDATE()
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@RequestType SMALLINT
		,@EmpId INT
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	SELECT @CurrentStatus = wk.CurrentStatus
		,@FromDate = lr.FromDate
		,@RequestType = wk.RequestType
		,@EmpId = wk.EmpId
	FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wk.RequestId = lr.RequestId
		AND lr.RequestId = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
		SET CurrentStatus = @VoidStatusId
			,HRId = @ActorId
			,HRDate = @ProcessedDate
			,ModifiedBy = @ActorId
			,ModifiedDate = @ProcessedDate
		WHERE RequestID = @RequestID

		IF @CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId)
		BEGIN
			EXEC dbo.DisciplineRequestTrans_Delete @RequestId = @RequestId

			EXEC dbo.WorkingTimeRequest_LeaveReject_RollbackCalendar @RequestId
				,@EmpId
				,@ActorId

			EXEC dbo.AnnualLeave_DeleteTransactionByRequestId @RequestId
				,@ProcessedDate
				,@ActorId

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
		END

		EXEC dbo.WorkingTimeRequest_Update_TmpLeaveBalance @RequestId = @RequestId
			,@EmpId = @EmpId
			,@Status = 2

		EXEC dbo.WorkingTimeRequest_Update_TmpKIVLeaveBalance @RequestId = @RequestId
			,@EmpId = @EmpId
			,@Status = 2

		EXEC dbo.LeaveRequest_WriteLog @RequestId = @RequestId
			,@Comment = @Comment

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Void unsuccessfully.'

		ROLLBACK TRANSACTION
	END CATCH;
END
