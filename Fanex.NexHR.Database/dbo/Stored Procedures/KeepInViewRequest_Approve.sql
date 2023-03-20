


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
		,@ProcessedDate DATETIME = GETDATE()
		-- system role
		,@VerifierRoleId TINYINT = 2
		,@ApproverRoleId TINYINT = 3
		,@HRRoleId TINYINT = 6
		-- request status
		,@VerifiedStatusId TINYINT = 2
		,@ApprovedStatusId TINYINT = 4
		,@ProcessedStatusId TINYINT = 6

	SELECT @OnDate = OnDate
		,@EmpId = EmpId
	FROM dbo.KeepInViewRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
				INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON r.RequestId = kiv.RequestId
				AND kiv.OnDate = @OnDate
			WHERE r.RequestId <> @RequestId
				AND r.EmpId = @EmpId
				AND r.CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'Another approved request in the same date has existed.'

		RETURN
	END

	EXEC dbo.EmployeeCalendar_Insert @empid = @empid
		,@fromdate = @OnDate
		,@todate = @OnDate
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	IF @RoleId = @VerifierRoleId
	BEGIN
		UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
		SET CurrentStatus = @VerifiedStatusId
			,VerifiedStatus = @VerifiedStatusId
			--,VerifierId = @ActorId
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
			--,ApproverId = @ActorId
			,ApprovedDate = @ProcessedDate
			,ModifiedBy = @ActorId
			,ModifiedDate = @ProcessedDate
		WHERE RequestId = @RequestId

		UPDATE empca
		SET empca.IsKeepInView = 1
			,empca.ModifiedBy = @ActorId
			,empca.ModifiedDate = @ProcessedDate
			,empca.ShiftId = ISNULL(kiv.ShiftId, empca.ShiftId)
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

	EXEC dbo.KeepInViewRequest_WriteLog @RequestId = @RequestId
		,@Comment = @Comment
END



