
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_Reject] @RequestId BIGINT
	,@RoleId INT
	,@Comment NVARCHAR(500) = ''
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190625
Task: reject late in/early out request 
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @EmpId INT
		,@OnDate DATE
		,@RequestType TINYINT
		,@ProcessedDate DATETIME = GETDATE()
		-- request type
		,@LateInRequestTypeId TINYINT = 3
		,@EarlyOutRequestTypeId TINYINT = 4
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

	SELECT @EmpId = li.EmpId
		,@OnDate = OnDate
		,@RequestType = wk.RequestType
	FROM dbo.LateInEarlyOutRequest AS li WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON li.RequestId = wk.RequestId
	WHERE li.RequestId = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC dbo.WorkingTimeRequest_RejectRequest_ResetIgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ActorId
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

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

			UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
			SET LateIn = CASE 
					WHEN @RequestType = @LateInRequestTypeId THEN 0
					ELSE LateIn
					END
				,EarlyOut = CASE 
					WHEN @RequestType = @EarlyOutRequestTypeId THEN 0
					ELSE EarlyOut
					END
				,ModifiedDate = @ProcessedDate
			WHERE WorkDate = @OnDate
				AND EmpId = @EmpId

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@OnDate
				,@OnDate
				,1
		END

		EXEC dbo.LateInEarlyOutRequest_WriteLog @RequestId = @RequestId
			,@Comment = @Comment

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject unsuccessfully.'

		ROLLBACK TRANSACTION;
	END CATCH;
END
