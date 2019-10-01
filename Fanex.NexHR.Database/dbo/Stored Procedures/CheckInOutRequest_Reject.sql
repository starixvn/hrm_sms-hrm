
CREATE PROCEDURE [dbo].[CheckInOutRequest_Reject] @RequestId BIGINT
	,@ActorId INT
	,@RoleId TINYINT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190522@Quinn.Vo
Task: reject check in/out request
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @OnDate DATE
		,@EmpId INT
		,@ProcessedDate DATETIME = GETDATE()
		,@RequestType SMALLINT
		-- request type
		,@CheckInRequestTypeId TINYINT = 1
		,@CheckOutRequestTypeId TINYINT = 2
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

	SELECT @EmpId = mis.EmpId
		,@OnDate = OnDate
		,@RequestType = wk.RequestType
	FROM dbo.CheckInOutRequest AS mis WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON mis.RequestId = wk.RequestId
	WHERE mis.RequestId = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

		EXECUTE dbo.WorkingTimeRequest_RejectRequest_ResetIgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ActorId
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		IF @ErrCode <> 0
		BEGIN
			ROLLBACK TRANSACTION;

			RETURN
		END

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
			WHERE RequestID = @RequestId

			UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
			SET RequestedCheckinTime = CASE 
					WHEN @RequestType = @CheckInRequestTypeId THEN 0
					ELSE RequestedCheckinTime
					END
				,RequestedCheckoutTime = CASE 
					WHEN @RequestType = @CheckOutRequestTypeId THEN 0
					ELSE RequestedCheckoutTime
					END
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE WorkDate = @OnDate
				AND EmpId = @EmpId

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@OnDate
				,@OnDate
				,1
		END

		EXEC dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId
			,@Comment = @Comment

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject unsuccessfully.'

		ROLLBACK TRANSACTION;
	END CATCH;
END
