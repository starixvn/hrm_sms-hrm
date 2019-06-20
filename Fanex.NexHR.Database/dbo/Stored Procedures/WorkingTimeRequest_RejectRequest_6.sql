/****** Object:  Stored Procedure dbo.WorkingTimeRequest_RejectRequest_6    Script Date: 7/20/2016 10:19:23 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_RejectRequest_6    Script Date: 7/19/2016 5:22:23 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_RejectRequest_6] @RequestId BIGINT
	,@EmpId INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@RoleID INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20150929
Task: disapprove leave request
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME

	SELECT @FromDate = lr.FromDate
		,@ToDate = lr.ToDate
	FROM dbo.LeaveRequest lr WITH (NOLOCK)
		LEFT JOIN dbo.LeaveRequestDetail lrd WITH (NOLOCK) ON lr.RequestId = lrd.RequestId
	WHERE lr.RequestID = @RequestID

	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleID = 2 -- Not Verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 3
				,VerifiedStatus = 3
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId
		END
		ELSE IF @RoleID = 3 --Reject
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 5
				,ApprovedStatus = 3
				,ApproverId = @ProcessedBy
				,ApprovedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId
		END
		ELSE IF @RoleID = 6 --Void
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 7
				,HRStatus = 3
				,HRId = @ProcessedBy
				,HRDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId

			EXECUTE [dbo].[WorkingTimeRequest_LeaveReject_RollbackCalendar] @RequestID
				,@EmpId
				,@ProcessedBy

			EXECUTE [dbo].[AnnualLeave_DeleteTransactionByRequestId] @RequestId
				,@ProcessedDate
				,@ProcessedBy

			EXECUTE [dbo].EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
		END

		EXECUTE [dbo].WorkingTimeRequest_Update_TmpLeaveBalance @Requestid = @Requestid
			,@EmpID = @EmpID
			,@Status = 2

		EXECUTE [dbo].WorkingTimeRequest_Update_TmpKIVLeaveBalance @Requestid = @Requestid
			,@EmpID = @EmpID
			,@Status = 2

		COMMIT TRANSACTION;

		SET @ErrCode = 0
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject request failed'

		ROLLBACK TRANSACTION;
	END CATCH;
END
