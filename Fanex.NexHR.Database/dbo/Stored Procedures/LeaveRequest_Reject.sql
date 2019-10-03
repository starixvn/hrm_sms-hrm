
CREATE PROCEDURE [dbo].[LeaveRequest_Reject] @RequestId BIGINT
	,@ActorId INT
	,@RoleId TINYINT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20150929
Task: reject leave request
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @EmpId INT
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@ProcessedDate DATETIME = GETDATE()
		-- system role
		,@VerifierRoleId TINYINT = 2
		,@ApproverRoleId TINYINT = 3
		,@HRRoleId TINYINT = 6
		,@SystemAdminRoleId TINYINT = 7
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	SELECT @EmpId = lr.EmpId
		,@FromDate = lr.FromDate
		,@ToDate = lr.ToDate
	FROM dbo.LeaveRequest AS lr WITH (NOLOCK)
		LEFT JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON lr.RequestId = lrd.RequestId
	WHERE lr.RequestId = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleId = @VerifierRoleId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @NotVerifiedStatusId
				,VerifiedStatus = @NotVerifiedStatusId
				,VerifierId = @ActorId
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId
		END
		ELSE IF @RoleId = @ApproverRoleId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @RejectedStatusId
				,ApprovedStatus = @NotVerifiedStatusId
				,ApproverId = @ActorId
				,ApprovedDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId
		END
		ELSE IF @RoleId = @HRRoleId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @VoidStatusId
				,HRStatus = @NotVerifiedStatusId
				,HRId = @ActorId
				,HRDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId

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

		EXEC dbo.LeaveRequest_DeleteWeekendLeaveTrans @RequestId = @RequestId

		EXEC dbo.LeaveRequest_DeleteKIVLeaveTrans @RequestId = @RequestId

		EXEC dbo.WorkingTimeRequest_Update_TmpLeaveBalance @RequestId = @RequestId
			,@EmpId = @EmpId
			,@Status = 2

		EXEC dbo.LeaveRequest_WriteLog @RequestId = @RequestId
			,@Comment = @Comment

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject unsuccessfully.'

		ROLLBACK TRANSACTION;
	END CATCH;
END
