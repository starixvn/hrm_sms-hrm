
CREATE PROCEDURE [dbo].[CheckInOutRequest_Approve] @RequestId BIGINT
	,@ActorId INT
	,@RoleId TINYINT
	,@Comment NVARCHAR(500) = ''
	,@DisciplineXml NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190624
Task: approve check in/out request 
DB: HRM
Original:

Revisions:
Param's Explanation:
    @DisciplineXml: 
        <Root>
            <Row WorkDate='' EmployeeId='' DisciplineTypeId='' IsIgnored='' IsConfirmed='' />
        </Root>
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @EmpId INT
		,@Status TINYINT
		,@OnDate DATE
		,@RequestType SMALLINT
		,@RequestTime DATETIME
		,@ProcessedDate DATETIME = GETDATE()
		,@VerifierId INT
		,@ApproverId INT
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
		,@Status = wk.CurrentStatus
		,@VerifierId = wk.VerifierId
		,@ApproverId = wk.ApproverId
	FROM dbo.CheckInOutRequest AS mis WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON mis.RequestId = wk.RequestId
	WHERE mis.RequestId = @RequestId

	IF @RoleId = @VerifierRoleId
		AND (
			@Status <> @PendingStatusId
			OR @VerifierId <> @ActorId
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'You can not verify this request. Please contact Administrator.'

		RETURN
	END

	IF @RoleId = @ApproverRoleId
		AND (
			@Status NOT IN (@PendingStatusId, @VerifiedStatusId)
			OR @ApproverId <> @ActorId
			)
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = 'You can not approve this request. Please contact Administrator.'

		RETURN
	END

	IF @RoleId = @HRRoleId
		AND (
			@Status <> @ApprovedStatusId
			OR NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole
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
			FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
				INNER JOIN dbo.CheckInOutRequest AS mis WITH (NOLOCK) ON r.RequestId = mis.RequestId
				AND mis.OnDate = @OnDate
			WHERE r.EmpId = @EmpId
				AND r.RequestType = @RequestType
				AND r.CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'Another approved request in the same date has existed.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleId = @ApproverRoleId
			AND ISNULL(@DisciplineXml, '') <> ''
		BEGIN
			EXEC dbo.WorkingTimeRequest_ApproveRequest_IgnoreDiscipline @RequestId = @RequestId
				,@RoleId = @RoleId
				,@ModifiedBy = @ActorId
				,@IgnoredDetails = @DisciplineXml
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END

		IF @ErrCode <> 0
		BEGIN
			ROLLBACK TRANSACTION;

			RETURN
		END

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

			UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
			SET RequestedCheckinTime = CASE 
					WHEN @RequestType = @CheckInRequestTypeId THEN @RequestTime
					ELSE RequestedCheckinTime
					END
				,RequestedCheckoutTime = CASE 
					WHEN @RequestType = @CheckOutRequestTypeId THEN @RequestTime
					ELSE RequestedCheckoutTime
					END
				,ModifiedDate = @ProcessedDate
			WHERE EmpId = @EmpId
				AND DATEDIFF(d, WorkDate, @OnDate) = 0

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

		EXEC dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId
			,@Comment = @Comment

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 100
			,@ErrMsg = 'Aprrove unsuccessfully.'
	END CATCH;
END
