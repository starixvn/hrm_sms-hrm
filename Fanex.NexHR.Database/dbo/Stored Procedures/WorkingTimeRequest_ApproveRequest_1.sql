
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_1] @RequestId BIGINT
	,@EmpId INT
	,@RoleId INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-06
Task: approve check-in request form (type 1)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON

	DECLARE @OnDate DATETIME
		,@CheckinTime DATETIME

	SELECT @CheckinTime = [CheckTime]
		,@OnDate = OnDate
	FROM dbo.[InoutMissRequest] WITH (NOLOCK)
	WHERE RequestID = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC [dbo].[EmployeeCalendar_Insert] @empid = @empid
			,@fromdate = @CheckinTime
			,@todate = @CheckinTime
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

			EXEC [WorkingTimeRequest_WriteHistory] @RequestId
				,@Comment
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

			EXEC [WorkingTimeRequest_WriteHistory] @RequestId
				,@Comment

			UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
			SET RequestedCheckinTime = @CheckinTime
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

			EXEC [WorkingTimeRequest_WriteHistory] @RequestId
				,@Comment
		END

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 100
			,@ErrMsg = 'Aprrove request failed'
	END CATCH;
END
