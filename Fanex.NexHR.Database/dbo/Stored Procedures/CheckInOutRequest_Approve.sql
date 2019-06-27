
CREATE PROCEDURE [dbo].[CheckInOutRequest_Approve] @RequestId BIGINT
	,@ProcessedBy INT
	,@RoleId INT
	,@Comment NVARCHAR(500) = ''
	,@IgnoredDetails NVARCHAR(MAX) = NULL
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
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON

	DECLARE @EmpId INT
		,@Status TINYINT
		,@OnDate DATETIME
		,@RequestType SMALLINT
		,@RequestTime DATETIME
		,@ProcessedDate DATETIME = GETDATE()
		,@VerifierId INT
		,@ApproverId INT

	SELECT @EmpId = mis.EmpId
		,@RequestTime = RequestTime
		,@OnDate = OnDate
		,@RequestType = wk.RequestType
		,@Status = wk.CurrentStatus
		,@VerifierId = wk.VerifierId
		,@ApproverId = wk.ApproverId
	FROM dbo.CheckInOutRequest AS mis WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON mis.RequestId = wk.RequestId
	WHERE mis.RequestID = @RequestId

	IF (
			@RoleId = 2
			AND (
				@Status <> 1
				OR @VerifierId <> @ProcessedBy
				)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'You can not verify this request. Please contact Administrator.'

		RETURN
	END

	IF (
			@RoleId = 3
			AND (
				@Status NOT IN (1, 2)
				OR @ApproverId <> @ProcessedBy
				)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'You can not approve this request. Please contact Administrator.'

		RETURN
	END

	IF (
			@RoleId = 6
			AND (
				@Status <> 4
				OR NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeRole
					WHERE EmployeeId = @ProcessedBy
						AND RoleId IN (6, 7)
					)
				)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'You can not process this request. Please contact Administrator.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
				INNER JOIN dbo.CheckInOutRequest AS mis WITH (NOLOCK) ON r.RequestId = mis.RequestId
			WHERE r.empid = @EmpId
				AND r.RequestType = @RequestType
				AND r.CurrentStatus = 6
				AND mis.OnDate = @OnDate
			)
	BEGIN
		SET @ErrCode = 104
		SET @ErrMsg = 'Another approved request in the same date has existed'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		EXECUTE dbo.WorkingTimeRequest_ApproveRequest_IgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ProcessedBy
			,@IgnoredDetails = @IgnoredDetails
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		EXECUTE [dbo].[EmployeeCalendar_Insert] @empid = @empid
			,@fromdate = @OnDate
			,@todate = @OnDate
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		IF @ErrCode <> 0
		BEGIN
			ROLLBACK TRANSACTION;

			RETURN
		END

		IF @RoleId = 2 -- verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 2
				,VerifiedStatus = 2
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId
		END
		ELSE IF @RoleId = 3 -- approver
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 4
				,ApprovedStatus = 2
				,ApproverComment = @Comment
				,ApproverId = @ProcessedBy
				,ApprovedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId

			UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
			SET RequestedCheckinTime = CASE WHEN @RequestType = 1 THEN @RequestTime ELSE RequestedCheckinTime END
				,RequestedCheckoutTime = CASE WHEN @RequestType = 2 THEN @RequestTime ELSE RequestedCheckoutTime END
				,ModifiedDate = @ProcessedDate
			WHERE EmpID = @EmpId
				AND DATEDIFF(d, WorkDate, @OnDate) = 0

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
				,@OnDate
				,@OnDate
				,1
		END
		ELSE IF @RoleId = 6 -- hr
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 6
				,HRStatus = 2
				,HRId = @ProcessedBy
				,HRDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId
		END

		EXECUTE dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId, @Comment = @Comment

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 100
			,@ErrMsg = 'Aprrove request failed'
	END CATCH;
END