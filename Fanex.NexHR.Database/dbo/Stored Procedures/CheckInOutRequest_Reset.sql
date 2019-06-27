
CREATE PROCEDURE [dbo].[CheckInOutRequest_Reset] @RequestId BIGINT
	,@ProcessedBy INT
	,@RoleId INT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190522@Quinn.Vo
Task: reset keep in view request
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@Status TINYINT
		,@OnDate DATETIME
		,@RequestType SMALLINT
		,@RequestTime DATETIME
		,@ProcessedDate DATETIME = GETDATE()
		,@VerifierId INT
		,@ApproverId INT
		,@CurrentStatus TINYINT

	SELECT @EmpId = mis.EmpId
		,@RequestTime = RequestTime
		,@OnDate = OnDate
		,@RequestType = wk.RequestType
		,@Status = wk.CurrentStatus
		,@VerifierId = wk.VerifierId
		,@ApproverId = wk.ApproverId
		,@CurrentStatus  = wk.CurrentStatus
	FROM dbo.CheckInOutRequest AS mis WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON mis.RequestId = wk.RequestId
	WHERE mis.RequestID = @RequestId

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;

		-- reset ignore
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
				UPDATE EC
				SET EC.RequestedCheckinTime = NULL
					,EC.ModifiedDate = @ProcessedDate
				FROM EmployeeCalendar EC WITH (ROWLOCK, UPDLOCK)
				WHERE datediff(d, WorkDate, @OnDate) = 0
					AND EC.EmpID = @EmpId

				EXECUTE EmployeeAttendance_ProcessingAndInsert @EmpId, @OnDate, @OnDate, 1
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
				SET RequestedCheckinTime = CASE WHEN @RequestType = 1 THEN @RequestTime ELSE RequestedCheckinTime END
					,RequestedCheckoutTime = CASE WHEN @RequestType = 2 THEN @RequestTime ELSE RequestedCheckoutTime END
					,ModifiedDate = @ProcessedDate
				WHERE EmpID = @EmpId
					AND datediff(d, WorkDate, @OnDate) = 0

				EXECUTE EmployeeAttendance_ProcessingAndInsert @EmpId, @OnDate, @OnDate, 1
			END
		END

		EXECUTE dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId, @Comment = @Comment

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset request failed'

		ROLLBACK TRANSACTION;
	END CATCH;
END