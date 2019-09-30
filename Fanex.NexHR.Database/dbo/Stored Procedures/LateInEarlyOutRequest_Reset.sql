
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_Reset] @RequestId BIGINT
	,@RoleId TINYINT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190625
Task: reset late in/early out request 
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
		,@LateIn INT
		,@EarlyOut INT
		,@RequestType TINYINT
		,@ProcessedDate DATETIME = GETDATE()
		,@VerifierId INT
		,@ApproverId INT
		,@CurrentStatus TINYINT
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
		,@LateIn = LateInMins
		,@EarlyOut = EarlyOutMins
		,@RequestType = wk.RequestType
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
		,@CurrentStatus = wk.CurrentStatus
	FROM dbo.LateInEarlyOutRequest AS li WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON li.RequestId = wk.RequestId
	WHERE li.RequestId = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC dbo.WorkingTimeRequest_ResetRequest_ResetIgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ActorId
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

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
				SET LateIn = CASE 
						WHEN @RequestType = @LateInRequestTypeId THEN 0
						ELSE LateIn
						END
					,EarlyOut = CASE 
						WHEN @RequestType = @EarlyOutRequestTypeId THEN 0
						ELSE EarlyOut
						END
					,ModifiedDate = @ProcessedDate
				WHERE EmpId = @EmpId
					AND WorkDate = @OnDate

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
				SET LateIn = CASE 
						WHEN @RequestType = @LateInRequestTypeId THEN @LateIn
						ELSE LateIn
						END
					,EarlyOut = CASE 
						WHEN @RequestType = @EarlyOutRequestTypeId THEN @EarlyOut
						ELSE EarlyOut
						END
					,ModifiedDate = @ProcessedDate
				WHERE EmpId = @EmpId
					AND WorkDate = @OnDate

				EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
					,@OnDate
					,@OnDate
					,1
			END
		END

		EXEC dbo.LateInEarlyOutRequest_WriteLog @RequestId = @RequestId
			,@Comment = ''

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset unsuccessfully.'

		ROLLBACK TRANSACTION
	END CATCH;
END
