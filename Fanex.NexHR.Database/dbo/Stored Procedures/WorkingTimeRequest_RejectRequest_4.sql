/****** Object:  Stored Procedure dbo.WorkingTimeRequest_RejectRequest_4    Script Date: 7/20/2016 10:19:23 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_RejectRequest_4    Script Date: 7/19/2016 5:22:23 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_RejectRequest_4] @RequestId BIGINT
	,@EmpId INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@RoleID INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-07
Task: disapprove Early Out request form (type 4)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
	FROM dbo.InOutRequest WITH (NOLOCK)
	WHERE RequestID = @RequestID

	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleID = 2 --Verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 3
				,VerifiedStatus = 3
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID
		END
		ELSE IF @RoleID = 3 --Approver
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
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 7
				,HRStatus = 3
				,HRId = @ProcessedBy
				,HRDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId

			UPDATE EC
			SET EarlyOut = 0
				,EC.ModifiedDate = @ProcessedDate
			FROM EmployeeCalendar EC WITH (ROWLOCK, UPDLOCK)
			WHERE WorkDate BETWEEN @FromDate
					AND @ToDate
				AND EC.EmpID = @EmpId

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
		END

		COMMIT TRANSACTION;

		SET @ErrCode = 0
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject request failed'

		ROLLBACK TRANSACTION;
	END CATCH;
END
