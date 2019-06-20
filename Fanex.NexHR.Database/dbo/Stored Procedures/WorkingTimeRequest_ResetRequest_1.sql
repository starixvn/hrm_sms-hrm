
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_ResetRequest_1    Script Date: 7/20/2016 10:19:24 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_ResetRequest_1    Script Date: 7/19/2016 5:22:23 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_ResetRequest_1] @RequestID BIGINT
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
Task: Reset Status check-in request form (type 1)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @OnDate DATETIME
		,@CheckInTime DATETIME
		,@VerifierId INT
		,@ApproverId INT

	SELECT @CheckInTime = [CheckTime]
		,@OnDate = OnDate
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
	FROM dbo.InoutMissRequest AS rq WITH (NOLOCK)
	INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON rq.RequestId = wk.RequestId
	WHERE rq.RequestId = @RequestID

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleID = 2 --Verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
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
				SET CurrentStatus = CASE WHEN VerifiedStatus = 2 THEN 2 ELSE 1 END,
					ApprovedStatus = 1,
					ApproverId= @ProcessedBy,
					ApprovedDate= @ProcessedDate,
					ModifiedBy = @ProcessedBy,
					ModifiedDate = @ProcessedDate
				WHERE RequestID = @RequestID
			END

			IF @CurrentStatus = 4
			BEGIN
				UPDATE EC
				SET EC.RequestedCheckinTime = NULL
					,EC.ModifiedDate = @ProcessedDate
				FROM EmployeeCalendar EC WITH (ROWLOCK, UPDLOCK)
				WHERE datediff(d, WorkDate, @OnDate) = 0
					AND EC.EmpID = @EmpId

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
				SET RequestedCheckinTime = @CheckinTime
					,ModifiedDate = @ProcessedDate
				WHERE EmpID = @EmpId
					AND datediff(d, WorkDate, @OnDate) = 0

				EXEC EmployeeAttendance_ProcessingAndInsert @EmpId, @OnDate, @OnDate, 1
			END
		END

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset request failed'
	END CATCH;
END