
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
		-- request type
		,@LateInRequestTypeId TINYINT = 3
		,@EarlyOutRequestTypeId TINYINT = 4
		-- system role
		,@VerifierRoleId TINYINT = 2
		,@ApproverRoleId TINYINT = 3
		,@HRRoleId TINYINT = 6
		-- request status
		,@VerifiedStatusId TINYINT = 2
		,@ApprovedStatusId TINYINT = 4
		,@ProcessedStatusId TINYINT = 6

	SELECT @EmpId = lie.EmpId
		,@OnDate = OnDate
		,@LateIn = LateInMins
		,@EarlyOut = EarlyOutMins
		,@RequestType = r.RequestType
	FROM dbo.LateInEarlyOutRequest AS lie WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS r WITH (NOLOCK) ON lie.RequestId = r.RequestId
		AND r.RequestId = @RequestId

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

	EXEC dbo.EmployeeCalendar_Insert @EmpId = @EmpId
		,@fromdate = @OnDate
		,@todate = @OnDate
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

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

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 104
			,@ErrMsg = 'Aprrove unsuccessfully.'
	END
END
