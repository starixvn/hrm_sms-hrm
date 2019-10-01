
CREATE PROCEDURE [dbo].[CheckInOutRequest_Reset] @RequestId BIGINT
	,@ActorId INT
	,@RoleId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190522@Quinn.Vo
Task: reset check in/out request
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
		,@RequestType SMALLINT
		,@RequestTime DATETIME
		,@ProcessedDate DATETIME = GETDATE()
		,@VerifierId INT
		,@ApproverId INT
		,@CurrentStatus TINYINT
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
		,@RequestTime = RequestTime
		,@OnDate = OnDate
		,@RequestType = wk.RequestType
		,@VerifierId = wk.VerifierId
		,@ApproverId = wk.ApproverId
		,@CurrentStatus = wk.CurrentStatus
	FROM dbo.CheckInOutRequest AS mis WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON mis.RequestId = wk.RequestId
	WHERE mis.RequestId = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC dbo.WorkingTimeRequest_ResetRequest_ResetIgnoreDiscipline @RequestId = @RequestId
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
			SET CurrentStatus = @PendingStatusId
				,VerifiedStatus = @PendingStatusId
				,VerifierId = @ActorId
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId
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
				WHERE RequestId = @RequestId
			END

			IF @CurrentStatus = @ApprovedStatusId
			BEGIN
				UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
				SET RequestedCheckinTime = CASE 
						WHEN @RequestType = @CheckInRequestTypeId THEN NULL
						ELSE RequestedCheckinTime
						END
					,RequestedCheckoutTime = CASE 
						WHEN @RequestType = @CheckOutRequestTypeId THEN NULL
						ELSE RequestedCheckoutTime
						END
					,ModifiedBy = @ActorId
					,ModifiedDate = @ProcessedDate
				WHERE datediff(d, WorkDate, @OnDate) = 0
					AND EmpId = @EmpId

				EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
					,@OnDate
					,@OnDate
					,1
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
				UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
				SET RequestedCheckinTime = CASE 
						WHEN @RequestType = @CheckInRequestTypeId THEN @RequestTime
						ELSE RequestedCheckinTime
						END
					,RequestedCheckoutTime = CASE 
						WHEN @RequestType = @CheckOutRequestTypeId THEN @RequestTime
						ELSE RequestedCheckoutTime
						END
					,ModifiedBy = @ActorId
					,ModifiedDate = @ProcessedDate
				WHERE EmpId = @EmpId
					AND datediff(d, WorkDate, @OnDate) = 0

				EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
					,@OnDate
					,@OnDate
					,1
			END
		END

		EXEC dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId
			,@Comment = ''

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset unsuccessfully.'

		ROLLBACK TRANSACTION;
	END CATCH;
END
