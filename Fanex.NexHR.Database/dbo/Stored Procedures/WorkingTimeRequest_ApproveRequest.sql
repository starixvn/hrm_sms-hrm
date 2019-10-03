
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest] @RequestId BIGINT
	,@ActorId INT
	,@RoleId TINYINT
	,@Comment NVARCHAR(500) = ''
	,@DisciplineXml NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-06
Task: approve request
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
    @IgnoredDetails:
        <ROOT>
            <Discipline WorkDate='' EmployeeId='' DisciplineTypeId='' IsIgnored='' IsConfirmed=''></Discipline>
        </ROOT>
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @EmpId INT
		,@RequestTypeId SMALLINT
		,@StatusId TINYINT
		,@VerifierId INT
		,@ApproverId INT
		-- system role
		,@VerifierRoleId TINYINT = 2
		,@ApproverRoleId TINYINT = 3
		,@HRRoleId TINYINT = 6
		,@SystemAdminRoleId TINYINT = 7
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@ApprovedStatusId TINYINT = 4
		-- request type
		,@CheckInRequestTypeId TINYINT = 1
		,@CheckOutRequestTypeId TINYINT = 2
		,@LateInRequestTypeId TINYINT = 3
		,@EarlyOutRequestTypeId TINYINT = 4
		,@ChangeShiftRequestTypeId TINYINT = 5
		,@LeaveRequestTypeId TINYINT = 6
		,@KeepInViewRequestTypeId TINYINT = 7

	SELECT @EmpId = EmpId
		,@RequestTypeId = RequestType
		,@StatusId = CurrentStatus
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF @RoleId = @VerifierRoleId
		AND (
			@StatusId <> @PendingStatusId
			OR @VerifierId <> @ActorId
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'You can not verify this request.'

		RETURN
	END

	IF @RoleId = @ApproverRoleId
		AND (
			@StatusId NOT IN (@PendingStatusId, @VerifiedStatusId)
			OR @ApproverId <> @ActorId
			)
	BEGIN
		SELECT @ErrCode = 101
			,@ErrMsg = 'You can not approve this request.'

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
		SELECT @ErrCode = 102
			,@ErrMsg = 'You can not process this request.'

		RETURN
	END

	IF @RequestTypeId = @CheckInRequestTypeId
		OR @RequestTypeId = @CheckOutRequestTypeId
	BEGIN
		EXEC dbo.CheckInOutRequest_Approve @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ActorId = @ActorId
			,@DisciplineXml = @DisciplineXml
			,@Comment = @Comment
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT
	END
	ELSE IF @RequestTypeId = @LateInRequestTypeId
		OR @RequestTypeId = @EarlyOutRequestTypeId
	BEGIN
		EXEC dbo.LateInEarlyOutRequest_Approve @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ActorId = @ActorId
			,@DisciplineXml = @DisciplineXml
			,@Comment = @Comment
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT
	END
	ELSE IF @RequestTypeId = @ChangeShiftRequestTypeId
	BEGIN
		EXEC dbo.ChangeShiftRequest_Approve @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ActorId = @ActorId
			,@DisciplineXml = @DisciplineXml
			,@Comment = @Comment
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT
	END
	ELSE IF @RequestTypeId = @LeaveRequestTypeId
	BEGIN
		EXEC dbo.LeaveRequest_Approve @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ActorId = @ActorId
			,@DisciplineXml = @DisciplineXml
			,@Comment = @Comment
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT
	END
	ELSE IF @RequestTypeId = @KeepInViewRequestTypeId
	BEGIN
		EXEC dbo.KeepInViewRequest_Approve @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ActorId = @ActorId
			,@Comment = @Comment
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT
	END
END
