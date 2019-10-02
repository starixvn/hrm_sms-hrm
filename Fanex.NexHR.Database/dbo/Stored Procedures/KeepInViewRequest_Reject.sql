
CREATE PROCEDURE [dbo].[KeepInViewRequest_Reject] @RequestId BIGINT
	,@ActorId INT
	,@RoleId TINYINT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190522@Quinn.Vo
Task: reject keep in view request
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

	SELECT @OnDate = OnDate
		,@EmpId = EmpId
	FROM dbo.KeepInViewRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

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
			WHERE RequestId = @RequestId

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

		EXEC dbo.KeepInViewRequest_WriteLog @RequestId
			,@Comment

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject unsuccessfully.'

		ROLLBACK TRANSACTION;
	END CATCH;
END
