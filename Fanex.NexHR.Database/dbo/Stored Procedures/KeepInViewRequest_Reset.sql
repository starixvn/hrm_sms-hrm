
CREATE PROCEDURE [dbo].[KeepInViewRequest_Reset] @RequestId BIGINT
	,@RoleId TINYINT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190522@Quinn.Vo
Task: reset keep in view request
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
		,@CurrentStatus TINYINT
		,@VerifierId INT
		,@ApproverId INT
		,@TotalMinutes INT
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

	SELECT @OnDate = kiv.OnDate
		,@EmpId = wkr.EmpId
		,@VerifierId = wkr.VerifierId
		,@ApproverId = wkr.ApproverId
		,@CurrentStatus = wkr.CurrentStatus
		,@TotalMinutes = DATEDIFF(MI, kiv.FromTime, kiv.ToTime)
	FROM dbo.KeepInViewRequest AS kiv WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wkr WITH (NOLOCK) ON kiv.RequestId = wkr.RequestId
		AND wkr.RequestId = @RequestId

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
				UPDATE empca
				SET empca.IsKeepInView = 0
					,empca.ModifiedBy = @ActorId
					,empca.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON empca.WorkDate = kiv.OnDate
					AND empca.EmpId = kiv.EmpId
					AND kiv.RequestId = @RequestId

				EXEC dbo.KeepInViewRequest_DeleteTrans @RequestId = @RequestId

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
				UPDATE empca
				SET empca.IsKeepInView = 1
					,empca.ModifiedBy = @ActorId
					,empca.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON empca.WorkDate = kiv.OnDate
					AND empca.EmpId = kiv.EmpId
					AND kiv.RequestId = @RequestId

				EXEC dbo.KeepInViewRequest_InsertTrans @RequestId = @RequestId

				EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
					,@OnDate
					,@OnDate
					,1
			END
		END

		EXEC dbo.KeepInViewRequest_WriteLog @RequestId = @RequestId
			,@Comment = ''

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset unsuccessfully.'

		ROLLBACK TRANSACTION;
	END CATCH;
END
