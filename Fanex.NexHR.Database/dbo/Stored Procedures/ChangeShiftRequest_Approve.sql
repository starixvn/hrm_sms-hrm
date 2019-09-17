
CREATE PROCEDURE [dbo].[ChangeShiftRequest_Approve] @RequestId BIGINT
	,@RoleId INT
	,@ProcessedBy INT
	,@Comment NVARCHAR(500) = ''
	,@IgnoredDetails NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190628
Task: approve change shift request 
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @TypeId TINYINT
		,@FromDate DATE
		,@ToDate DATE
		,@EmpId INT
		,@ProcessedDate DATETIME = GETDATE()
		,@StatusId TINYINT
		,@VerifierId INT
		,@ApproverId INT
		-- change shift type
		,@ChangeShift TINYINT = 1
		,@MoveOffDay TINYINT = 2
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

	SELECT @EmpId = cs.EmpId
		,@FromDate = cs.FromDate
		,@ToDate = cs.ToDate
		,@TypeId = cs.TypeId
		,@StatusId = wk.CurrentStatus
		,@VerifierId = wk.VerifierId
		,@ApproverId = wk.ApproverId
	FROM dbo.ChangeShiftRequest AS cs WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON cs.RequestId = wk.RequestId
	WHERE cs.RequestId = @RequestId

	IF @RoleId = @VerifierRoleId
		AND (
			@StatusId <> @PendingStatusId
			OR @VerifierId <> @ProcessedBy
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'You can not verify this request. Please contact Administrator.'

		RETURN
	END

	IF @RoleId = @ApproverRoleId
		AND (
			@StatusId NOT IN (@PendingStatusId, @VerifiedStatusId)
			OR @ApproverId <> @ProcessedBy
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
				WHERE EmployeeId = @ProcessedBy
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
			FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
				INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON wk.RequestId = cs.RequestId
				AND cs.FromDate = @FromDate
			WHERE wk.EmpId = @EmpId
				AND wk.CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'Another approved request in the same date has existed'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleId = @ApproverRoleId
			AND ISNULL(@IgnoredDetails, '') <> ''
		BEGIN
			EXECUTE dbo.WorkingTimeRequest_ApproveRequest_IgnoreDiscipline @RequestId = @RequestId
				,@RoleId = @RoleId
				,@ModifiedBy = @ProcessedBy
				,@IgnoredDetails = @IgnoredDetails
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT

			IF @ErrCode <> 0
			BEGIN
				ROLLBACK TRANSACTION;

				RETURN
			END
		END

		EXECUTE dbo.EmployeeCalendar_Insert @empid = @empid
			,@fromdate = @FromDate
			,@todate = @ToDate
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
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId
		END
		ELSE IF @RoleId = @ApproverRoleId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @ApprovedStatusId
				,ApprovedStatus = @VerifiedStatusId
				,ApproverComment = @Comment
				,ApproverId = @ProcessedBy
				,ApprovedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId
			
			IF @TypeId = @ChangeShift
			BEGIN
				UPDATE empca
				SET empca.IsOffDay = 1
					,empca.ChangeShiftId = csdt.FromShift
					,empca.ShiftId = NULL
					,empca.ModifiedBy = @ProcessedBy
					,empca.ModifiedDate = @ProcessedDate
					,empca.IsPublic = 1
				FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
					AND empca.EmpId = csdt.EmpId
					AND csdt.ToShift = - 1
					AND csdt.RequestId = @RequestId
				WHERE empca.EmpId = @EmpId

				UPDATE empca
				SET empca.ChangeShiftId = NULL
					,empca.ShiftId = csdt.ToShift
					,empca.IsOffDay = 0
					,empca.ModifiedBy = @ProcessedBy
					,empca.ModifiedDate = @ProcessedDate
					,empca.IsPublic = 1
				FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
					AND empca.EmpId = csdt.EmpId
					AND csdt.RequestId = @RequestId
					AND csdt.ToShift <> - 1
				WHERE empca.EmpId = @EmpId
					AND empca.IsOffDay = 1

				UPDATE empca
				SET empca.ChangeShiftId = csdt.FromShift
					,empca.ShiftId = csdt.ToShift
					,empca.ModifiedBy = @ProcessedBy
					,empca.ModifiedDate = @ProcessedDate
					,empca.IsPublic = 1
				FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
					AND empca.EmpId = csdt.EmpId
					AND csdt.RequestId = @RequestId
					AND csdt.ToShift <> - 1
				WHERE empca.EmpId = @EmpId
					AND empca.IsOffDay = 0
			END
			ELSE
			BEGIN
				UPDATE empca
				SET empca.IsOffDay = 0
					,empca.ModifiedBy = @ProcessedBy
					,empca.ModifiedDate = @ProcessedDate
					,empca.IsPublic = 1
				FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.OnDate
					AND empca.EmpId = csdt.EmpId
					AND csdt.RequestId = @RequestId
				WHERE empca.EmpId = @EmpId
					AND empca.IsOffDay = 1

				UPDATE empca
				SET empca.IsOffDay = 1
					,empca.ModifiedBy = @ProcessedBy
					,empca.ModifiedDate = @ProcessedDate
					,empca.IsPublic = 1
				FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS csdt WITH (NOLOCK) ON empca.WorkDate = csdt.ToDate
					AND empca.EmpId = csdt.EmpId
					AND csdt.RequestId = @RequestId
				WHERE empca.EmpId = @EmpId
					AND empca.IsOffDay = 0
			END

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
		END
		ELSE IF @RoleId = @HRRoleId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @ProcessedStatusId
				,HRStatus = @VerifiedStatusId
				,HRId = @ProcessedBy
				,HRDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId
		END

		EXECUTE dbo.ChangeShiftRequest_WriteLog @RequestId = @RequestId
			,@Comment = @Comment

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Aprrove request failed.'

		ROLLBACK TRANSACTION;
	END CATCH;
END
