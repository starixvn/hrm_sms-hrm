
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ResetRequest_2] @RequestID BIGINT
	,@EmpId INT
	,@RoleID INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@CurrentStatus INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-06
Task: Reset Status check-in request form (type 2)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @OnDate DATETIME
		,@CheckOutTime DATETIME
		,@VerifierId INT
		,@ApproverId INT

	SELECT @CheckOutTime = CheckTime
		,@OnDate = OnDate
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
	FROM dbo.InoutMissRequest AS rq WITH (NOLOCK)
	INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON rq.RequestId = wk.RequestId
	WHERE rq.RequestId = @RequestID

	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleID = 2 --Verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest
			SET CurrentStatus = 1
				,VerifiedStatus = 1
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID
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
				SET CurrentStatus = 2,
					ApprovedStatus = 1,
					ApproverId= @ProcessedBy,
					ApprovedDate= @ProcessedDate,
					ModifiedBy = @ProcessedBy,
					ModifiedDate = @ProcessedDate
				WHERE RequestID = @RequestID
			END

			IF @CurrentStatus = 4
			BEGIN
				UPDATE dbo.EmployeeCalendar
				SET RequestedCheckoutTime = NULL
					,ModifiedDate = @ProcessedDate
				WHERE EmpID = @EmpId
					AND datediff(d, WorkDate, @OnDate) = 0

				EXEC EmployeeAttendance_ProcessingAndInsert @EmpId, @OnDate, @OnDate, 1
			END
		END
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 4
				,HRStatus = 1
				,HRId = @ProcessedBy
				,HRDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID

			IF @CurrentStatus = 7
			BEGIN
				UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
				SET RequestedCheckoutTime = @CheckoutTime
					,ModifiedDate = @ProcessedDate
				WHERE EmpID = @EmpId
					AND datediff(d, WorkDate, @OnDate) = 0

				EXEC EmployeeAttendance_ProcessingAndInsert @EmpId, @OnDate, @OnDate, 1
			END
		END

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = ' Reset request failed'

		ROLLBACK TRANSACTION
	END CATCH;
END