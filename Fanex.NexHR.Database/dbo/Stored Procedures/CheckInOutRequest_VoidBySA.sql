
CREATE PROCEDURE [dbo].[CheckInOutRequest_VoidBySA] @RequestId BIGINT
	,@ActorId INT
	,@RoleId TINYINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190624@Quinn.Vo
Task: void check in/out request by SA.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @CurrentStatus INT
		,@ProcessedDate DATETIME = GETDATE()
		,@OnDate DATE
		,@EmpId INT
		,@RequestType SMALLINT
		-- request type
		,@CheckInRequestTypeId TINYINT = 1
		,@CheckOutRequestTypeId TINYINT = 2
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	SELECT @EmpId = mis.EmpId
		,@OnDate = mis.OnDate
		,@CurrentStatus = wk.CurrentStatus
		,@RequestType = wk.RequestType
	FROM dbo.CheckInOutRequest AS mis WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON mis.RequestId = wk.RequestId
	WHERE mis.RequestId = @RequestID

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC dbo.WorkingTimeRequest_VoidRequest_ResetIgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ActorId
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		IF @ErrCode <> 0
		BEGIN
			ROLLBACK TRANSACTION;

			RETURN
		END

		UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
		SET CurrentStatus = @VoidStatusId
			,ModifiedBy = @ActorId
			,ModifiedDate = @ProcessedDate
		WHERE RequestId = @RequestId

		IF @CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId)
		BEGIN
			UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
			SET RequestedCheckinTime = CASE 
					WHEN @RequestType = @CheckInRequestTypeId THEN 0
					ELSE RequestedCheckinTime
					END
				,RequestedCheckoutTime = CASE 
					WHEN @RequestType = @CheckOutRequestTypeId THEN 0
					ELSE RequestedCheckoutTime
					END
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE WorkDate = @OnDate
				AND EmpId = @EmpId

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@OnDate
				,@OnDate
				,1
		END

		EXEC dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Void unsuccessfully.'

		ROLLBACK TRANSACTION
	END CATCH;
END
