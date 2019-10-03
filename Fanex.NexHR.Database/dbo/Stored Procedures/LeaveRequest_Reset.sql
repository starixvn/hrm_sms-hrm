
CREATE PROCEDURE [dbo].[LeaveRequest_Reset] @RequestId BIGINT
	,@ActorId INT
	,@RoleId TINYINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20150925
Task: Reset leave request.
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
		,@VerifierId INT
		,@ApproverId INT
		,@CurrentStatus TINYINT
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
		,@FromDate = FromDate
		,@ToDate = ToDate
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
		,@CurrentStatus = wk.CurrentStatus
	FROM dbo.LeaveRequest AS lr WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lr.RequestId = wk.RequestId
		AND wk.RequestId = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleId = @VerifierRoleId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @PendingStatusId
				,VerifiedStatus = @PendingStatusId
				,VerifierId = @ActorId
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId

			IF @CurrentStatus = @NotVerifiedStatusId
			BEGIN
				EXEC dbo.LeaveRequest_UpdateTmpAnnualLeaveTrans @RequestId = @RequestId
					,@EmpId = @EmpId
					,@Status = 0

				EXEC dbo.LeaveRequest_InsertKIVLeaveTrans @RequestId = @RequestId

				EXEC dbo.LeaveRequest_InsertWeekendLeaveTrans @RequestId = @RequestId
			END
		END
		ELSE IF @RoleId = @ApproverRoleId
		BEGIN
			IF @VerifierId = @ApproverId
			BEGIN
				UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
				SET CurrentStatus = @PendingStatusId
					,VerifiedStatus = @PendingStatusId
					,ApprovedStatus = @PendingStatusId
					,VerifierId = @ActorId
					,VerifiedDate = @ProcessedDate
					,ApproverId = @ActorId
					,ApprovedDate = @ProcessedDate
					,ModifiedBy = @ActorId
					,ModifiedDate = @ProcessedDate
				WHERE RequestId = @RequestId
			END
			ELSE
			BEGIN
				UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
				SET CurrentStatus = CASE 
						WHEN VerifiedStatus = @VerifiedStatusId THEN @VerifiedStatusId
						ELSE @PendingStatusId
						END
					,ApprovedStatus = @PendingStatusId
					,ApproverId = @ActorId
					,ApprovedDate = @ProcessedDate
					,ModifiedBy = @ActorId
					,ModifiedDate = @ProcessedDate
				WHERE RequestID = @RequestID
			END

			IF @CurrentStatus = @ApprovedStatusId
			BEGIN
				EXEC dbo.DisciplineRequestTrans_Delete @RequestId = @RequestId

				EXEC dbo.LeaveRequest_Reject_RollbackCalendar @RequestId
					,@EmpId
					,@ActorId

				EXEC dbo.AnnualLeave_DeleteTransactionByRequestId @RequestId
					,@ProcessedDate
					,@ActorId

				EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
					,@FromDate
					,@ToDate
					,1
			END

			IF @CurrentStatus = @RejectedStatusId
			BEGIN
				EXEC dbo.LeaveRequest_UpdateTmpAnnualLeaveTrans @RequestId = @RequestId
					,@EmpId = @EmpId
					,@Status = 0

				EXEC dbo.LeaveRequest_InsertKIVLeaveTrans @RequestId = @RequestId

				EXEC dbo.LeaveRequest_InsertWeekendLeaveTrans @RequestId = @RequestId
			END
		END
		ELSE IF @RoleId = @HRRoleId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @ApprovedStatusId
				,HRStatus = @PendingStatusId
				,HRId = @ActorId
				,HRDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId

			IF @CurrentStatus = @VoidStatusId
			BEGIN
				EXEC dbo.LeaveRequest_InsertKIVLeaveTrans @RequestId = @RequestId

				EXEC dbo.LeaveRequest_InsertWeekendLeaveTrans @RequestId = @RequestId

				EXEC dbo.LeaveRequest_UpdateTmpAnnualLeaveTrans @Requestid = @RequestId
					,@EmpId = @EmpId
					,@Status = 1

				EXEC dbo.LeaveRequest_Approve_UpdateCalendar @RequestId
					,@EmpId
					,@ActorId

				EXEC dbo.AnnualLeave_InsertTransactionByRequestId @RequestId
					,@EmpId
					,@FromDate
					,@ToDate
					,@ActorId

				EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
					,@FromDate
					,@ToDate
					,1
			END
		END

		EXECUTE dbo.LeaveRequest_WriteLog @RequestId = @RequestId
			,@Comment = ''

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset unsuccessfully.'

		ROLLBACK TRANSACTION
	END CATCH;
END
