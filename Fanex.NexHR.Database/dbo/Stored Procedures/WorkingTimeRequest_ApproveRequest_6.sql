
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_ApproveRequest_6    Script Date: 7/20/2016 10:19:20 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_ApproveRequest_6    Script Date: 7/19/2016 5:22:21 PM ******/
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_6]
	@RequestID BIGINT
	,@EmpId INT
	,@RoleID INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20150925
Task: approve leave request form (type 6)
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:
*/

BEGIN
	SET NOCOUNT ON

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME

	SELECT @FromDate = FromDate, 
		@ToDate = ToDate
	FROM dbo.LeaveRequest lr WITH(NOLOCK)
	WHERE lr.RequestID = @RequestID

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;
		IF @RoleID = 2  --Verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH(ROWLOCK,UPDLOCK)
			SET CurrentStatus = 2,
				VerifiedStatus = 2,
				VerifierId= @ProcessedBy,
				VerifiedDate= @ProcessedDate,
				ModifiedBy = @ProcessedBy,
				ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID

			EXEC [WorkingTimeRequest_WriteHistory] @RequestID, @Comment
		END
		ELSE IF @RoleID = 3 --Approver
		BEGIN
				UPDATE dbo.WorkingTimeRequest WITH(ROWLOCK,UPDLOCK)
				SET CurrentStatus = 4,
					ApprovedStatus = 2,
					ApproverComment = @Comment,
					ApproverId= @ProcessedBy,
					ApprovedDate= @ProcessedDate,
					ModifiedBy = @ProcessedBy,
					ModifiedDate = @ProcessedDate
				WHERE RequestID = @RequestID

				EXEC [WorkingTimeRequest_WriteHistory] @RequestID, @Comment
				
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

				EXECUTE [dbo].EmployeeAttendance_ProcessingAndInsert @EmpId, @FromDate, @ToDate, 1
		END
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH(ROWLOCK,UPDLOCK)
			SET CurrentStatus = 6,
				HRStatus = 2,
				HRId= @ProcessedBy,
				HRDate= @ProcessedDate,
				ModifiedBy = @ProcessedBy,
				ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID

			EXEC [WorkingTimeRequest_WriteHistory] @RequestID, @Comment
		END

		COMMIT TRANSACTION;
		SELECT @ErrCode = 0;
	END TRY
	BEGIN CATCH
	
		SELECT @ErrCode = 100, @ErrMsg = 'Approve process failed'
		ROLLBACK TRANSACTION 
	END CATCH
END