
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_Reset] @RequestID BIGINT
	,@RoleID INT
	,@ProcessedBy INT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20190625
Task: Reset late in/early out request 
DB: HRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @EmpId INT
		,@OnDate DATETIME
		,@LateIn INT
		,@EarlyOut INT
		,@RequestType TINYINT
		,@ProcessedDate DATETIME = GETDATE()
		,@VerifierId INT
		,@ApproverId INT
		,@CurrentStatus TINYINT

	SELECT @EmpId = li.EmpId
		,@OnDate = OnDate
		,@LateIn = LateInMins
		,@EarlyOut = EarlyOutMins
		,@RequestType = wk.RequestType
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
		,@CurrentStatus = wk.CurrentStatus
	FROM dbo.LateInEarlyOutRequest AS li WITH (NOLOCK)
	INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON li.RequestId = wk.RequestId
	WHERE li.RequestID = @RequestID

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;

		EXECUTE dbo.WorkingTimeRequest_ResetRequest_ResetIgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ProcessedBy
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		IF (@ErrCode <> 0)
		BEGIN
			ROLLBACK TRANSACTION;

			RETURN
		END

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
				UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
				SET LateIn = CASE WHEN @RequestType = 3 THEN 0 ELSE LateIn END
					,EarlyOut = CASE WHEN @RequestType = 4 THEN 0 ELSE EarlyOut END
					,ModifiedDate = @ProcessedDate
				WHERE EmpID = @EmpId
					AND WorkDate = @OnDate

				EXECUTE EmployeeAttendance_ProcessingAndInsert @EmpId
					,@OnDate
					,@OnDate
					,1
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
				SET LateIn = CASE WHEN @RequestType = 3 THEN @LateIn ELSE LateIn END
					,EarlyOut = CASE WHEN @RequestType = 4 THEN @EarlyOut ELSE EarlyOut END
					,ModifiedDate = @ProcessedDate
				WHERE EmpID = @EmpId
					AND WorkDate = @OnDate

				EXECUTE EmployeeAttendance_ProcessingAndInsert @EmpId
					,@OnDate
					,@OnDate
					,1
			END
		END

		EXECUTE dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId, @Comment = @Comment
		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset request failed'

		ROLLBACK TRANSACTION
	END CATCH;
END