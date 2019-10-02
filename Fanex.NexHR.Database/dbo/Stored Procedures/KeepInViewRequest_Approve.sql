
CREATE PROCEDURE [dbo].[KeepInViewRequest_Approve] @RequestId BIGINT
	,@RoleId TINYINT
	,@ActorId INT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190522@Quinn.Vo
Task: approve keep in view request
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @OnDate DATE
		,@TotalMinutes INT
		,@EmpId INT
		,@StatusId TINYINT
		,@VerifierId INT
		,@ApproverId INT
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
		,@EmpId = kiv.EmpId
		,@StatusId = wkr.CurrentStatus
		,@VerifierId = wkr.VerifierId
		,@ApproverId = wkr.ApproverId
	FROM dbo.KeepInViewRequest AS kiv WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wkr WITH (NOLOCK) ON kiv.RequestId = wkr.RequestId
		AND wkr.RequestId = @RequestId

	IF @RoleId = @VerifierRoleId
		AND (
			@StatusId <> @PendingStatusId
			OR @VerifierId <> @ActorId
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'You can not verify this request. Please contact Administrator.'

		RETURN
	END

	IF @RoleId = @ApproverRoleId
		AND (
			@StatusId NOT IN (@PendingStatusId, @VerifiedStatusId)
			OR @ApproverId <> @ActorId
			)
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = 'You can not approve this request. Please contact Administrator.'

		RETURN
	END

	IF @RoleId = @HRRoleId
		AND (
			@StatusId <> @ApprovedStatusId
			OR NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole WITH (NOLOCK)
				WHERE EmployeeId = @ActorId
					AND RoleId IN (@HRRoleId, @SystemAdminRoleId)
				)
			)
	BEGIN
		SET @ErrCode = 102
		SET @ErrMsg = 'You can not process this request. Please contact Administrator.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
				INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON wkr.RequestId = kiv.RequestId
				AND kiv.OnDate = @OnDate
			WHERE wkr.EmpId = @EmpId
				AND wkr.CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'Another approved request in the same date has existed.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC dbo.EmployeeCalendar_Insert @empid = @empid
			,@fromdate = @OnDate
			,@todate = @OnDate
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
			SET CurrentStatus = @VerifiedStatusId
				,VerifiedStatus = @VerifiedStatusId
				,VerifierId = @ActorId
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId
		END
		ELSE IF @RoleId = @ApproverRoleId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @ApprovedStatusId
				,ApprovedStatus = @VerifiedStatusId
				,ApproverComment = @Comment
				,ApproverId = @ActorId
				,ApprovedDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId

			UPDATE empca
			SET empca.IsKeepInView = 1
				,empca.ModifiedBy = @ActorId
				,empca.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON empca.WorkDate = kiv.OnDate
				AND empca.EmpId = kiv.EmpId
				AND kiv.RequestId = @RequestId
			WHERE empca.EmpId = @EmpId

			EXEC dbo.KeepInViewRequest_InsertTrans @RequestId = @RequestId

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@OnDate
				,@OnDate
				,1
		END
		ELSE IF @RoleId = @HRRoleId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @ProcessedStatusId
				,HRStatus = @VerifiedStatusId
				,HRId = @ActorId
				,HRDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId
		END

		EXEC dbo.KeepInViewRequest_WriteLog @RequestId
			,@Comment

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 104
			,@ErrMsg = 'Aprrove unsuccessfully.'

		ROLLBACK TRANSACTION;
	END CATCH;
END
