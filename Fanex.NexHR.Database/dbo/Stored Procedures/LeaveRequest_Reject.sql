
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
		-- request status
		,@NotVerifiedStatusId TINYINT = 3
		,@RejectedStatusId TINYINT = 5
		,@VoidStatusId TINYINT = 7

	SELECT @EmpId = EmpId
		,@FromDate = FromDate
		,@ToDate = ToDate
	FROM dbo.LeaveRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

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

		EXEC dbo.LeaveRequest_Reject_RollbackCalendar @RequestId
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

	EXEC dbo.LeaveRequest_UpdateTmpAnnualLeaveTrans @RequestId = @RequestId
		,@EmpId = @EmpId
		,@Status = 2

	EXEC dbo.LeaveRequest_WriteLog @RequestId = @RequestId
		,@Comment = @Comment
END
