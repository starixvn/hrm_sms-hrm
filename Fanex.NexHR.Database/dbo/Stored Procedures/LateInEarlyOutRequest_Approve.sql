
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_Approve] @RequestId BIGINT
	,@ActorId INT
	,@RoleId TINYINT
	,@Comment NVARCHAR(500) = ''
	,@DisciplineXml NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190625
Task: Approve late in/early out request 
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
		,@OnDate DATE
		,@LateIn INT
		,@EarlyOut INT
		,@RequestType TINYINT
		,@ProcessedDate DATETIME = GETDATE()
		,@StatusId TINYINT
		,@VerifierId INT
		,@ApproverId INT
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
		,@StatusId = wk.CurrentStatus
		,@VerifierId = wk.VerifierId
		,@ApproverId = wk.ApproverId
	FROM dbo.LateInEarlyOutRequest AS li WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON li.RequestId = wk.RequestId
	WHERE li.RequestId = @RequestId
	
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
			FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
				INNER JOIN dbo.LateInEarlyOutRequest AS lie WITH (NOLOCK) ON r.RequestId = lie.RequestId
				AND lie.OnDate = @OnDate
			WHERE r.RequestId <> @RequestId
				AND r.EmpId = @EmpId
				AND r.CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'Another approved request in the same date has existed.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC dbo.EmployeeCalendar_Insert @EmpId = @EmpId
			,@fromdate = @OnDate
			,@todate = @OnDate
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		IF @ErrCode <> 0
		BEGIN
			ROLLBACK TRANSACTION;

			RETURN
		END

		IF @RoleId = @ApproverRoleId
			AND ISNULL(@DisciplineXml, '') <> ''
		BEGIN
			EXEC dbo.WorkingTimeRequest_ApproveRequest_IgnoreDiscipline @RequestId = @RequestId
				,@RoleId = @RoleId
				,@ModifiedBy = @ActorId
				,@IgnoredDetails = @DisciplineXml
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT

			IF @ErrCode <> 0
			BEGIN
				ROLLBACK TRANSACTION;

				RETURN
			END
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
				AND datediff(d, @OnDate, WorkDate) >= 0
				AND datediff(d, WorkDate, @OnDate) >= 0

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

		EXEC dbo.LateInEarlyOutRequest_WriteLog @RequestId = @RequestId
			,@Comment = @Comment

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 104
			,@ErrMsg = 'Aprrove unsuccessfully.'

		ROLLBACK TRANSACTION
	END CATCH;
END
