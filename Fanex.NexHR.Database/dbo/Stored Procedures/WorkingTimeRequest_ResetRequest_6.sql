

/****** Object:  Stored Procedure dbo.WorkingTimeRequest_ResetRequest_6    Script Date: 7/20/2016 10:19:24 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_ResetRequest_6    Script Date: 7/19/2016 5:22:23 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_ResetRequest_6]
	@RequestID BIGINT,
	@EmpId INT,
	@ProcessedBy INT,
	@ProcessedDate DATETIME,
	@RoleID INT,
	@CurrentStatus INT = NULL,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20150925
Task: Reset leave request.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/

BEGIN
	SET NOCOUNT ON
	DECLARE @FromDate DATETIME, @ToDate DATETIME, @VerifierId INT, @ApproverId INT

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
	FROM dbo.LeaveRequest AS lv WITH (NOLOCK)
	INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON lv.RequestId = wk.RequestId
	WHERE lv.RequestId = @RequestID

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;
		IF @RoleID = 2  --Verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH(ROWLOCK,UPDLOCK)
			SET CurrentStatus = 1,
				VerifiedStatus = 1,
				VerifierId= @ProcessedBy,
				VerifiedDate= @ProcessedDate,
				ModifiedBy = @ProcessedBy,
				ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID

			IF (@CurrentStatus = 3)
			BEGIN
				EXECUTE [dbo].WorkingTimeRequest_Update_TmpLeaveBalance
					@Requestid = @Requestid,
					@EmpID = @EmpID,
					@Status = 0

				EXECUTE [dbo].WorkingTimeRequest_Update_TmpKIVLeaveBalance
					@Requestid = @Requestid,
					@EmpID = @EmpID,
					@Status = 0
			END
		END
		ELSE IF @RoleID = 3 --Approver
		BEGIN
			IF(@VerifierId = @ApproverId)
			BEGIN
				UPDATE dbo.WorkingTimeRequest WITH(ROWLOCK,UPDLOCK)
				SET CurrentStatus = 1,
					VerifiedStatus = 1,
					ApprovedStatus = 1,
					VerifierId= @ProcessedBy,
					VerifiedDate= @ProcessedDate,
					ApproverId= @ProcessedBy,
					ApprovedDate= @ProcessedDate,
					ModifiedBy = @ProcessedBy,
					ModifiedDate = @ProcessedDate
				WHERE RequestID = @RequestID
			END
			ELSE
			BEGIN
				UPDATE dbo.WorkingTimeRequest WITH(ROWLOCK,UPDLOCK)
				SET CurrentStatus = CASE WHEN VerifiedStatus = 2 THEN 2 ELSE 1 END,
					ApprovedStatus = 1,
					ApproverId= @ProcessedBy,
					ApprovedDate= @ProcessedDate,
					ModifiedBy = @ProcessedBy,
					ModifiedDate = @ProcessedDate
				WHERE RequestID = @RequestID
			END

			IF (@CurrentStatus = 4)
			BEGIN
				EXECUTE [dbo].[WorkingTimeRequest_LeaveReject_RollbackCalendar]
					@RequestID
					,@EmpId
					,@ProcessedBy

				EXECUTE [AnnualLeave_DeleteTransactionByRequestId] 
					@RequestId
					,@ProcessedDate
					,@ProcessedBy

				EXECUTE EmployeeAttendance_ProcessingAndInsert @EmpId, @FromDate, @ToDate, 1
			END

			EXECUTE [dbo].WorkingTimeRequest_Update_TmpLeaveBalance
				@Requestid = @Requestid,
				@EmpID = @EmpID,
				@Status = 0

			EXECUTE [dbo].WorkingTimeRequest_Update_TmpKIVLeaveBalance
				@Requestid = @Requestid,
				@EmpID = @EmpID,
				@Status = 0
		END
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH(ROWLOCK,UPDLOCK)
			SET CurrentStatus = 4,
				HRStatus = 1,
				HRId = @ProcessedBy,
				HRDate= @ProcessedDate,
				ModifiedBy = @ProcessedBy,
				ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID

			IF (@CurrentStatus = 7)
			BEGIN
				EXECUTE [dbo].[WorkingTimeRequest_LeaveApprove_UpdateCalendar]
					@RequestId
					,@EmpId
					,@ProcessedBy

				EXECUTE [dbo].[AnnualLeave_InsertTransactionByRequestId]
					@RequestId
					,@EmpId
					,@FromDate
					,@ToDate
					,@ProcessedBy

				EXECUTE [dbo].WorkingTimeRequest_Update_TmpLeaveBalance
					@Requestid = @Requestid,
					@EmpID = @EmpID,
					@Status = 1

				EXECUTE [dbo].WorkingTimeRequest_Update_TmpKIVLeaveBalance
					@Requestid = @Requestid,
					@EmpID = @EmpID,
					@Status = 1

				EXEC EmployeeAttendance_ProcessingAndInsert @EmpId, @FromDate, @ToDate, 1
			END
		END

		COMMIT TRANSACTION;
		SELECT @ErrCode = 0;
	END TRY
	BEGIN CATCH
		SELECT @ErrCode = 100, @ErrMsg = 'Reset request failed'
		ROLLBACK TRANSACTION 
	END CATCH;
END