
CREATE PROCEDURE [dbo].[CheckInOutRequest_VoidBySA] @RequestId BIGINT
	,@ProcessedBy INT
	,@RoleId INT
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
	SET NOCOUNT ON

	DECLARE @CurrentStatus INT
		,@ProcessedDate DATETIME = GETDATE()
		,@OnDate DATETIME
		,@EmpId INT
		,@RequestType SMALLINT
	
	SELECT @EmpId = mis.EmpId
		,@OnDate = mis.OnDate
		,@CurrentStatus = wk.CurrentStatus
		,@RequestType = wk.RequestType
	FROM dbo.CheckInOutRequest AS mis WITH (NOLOCK)
	INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON mis.RequestId = wk.RequestId
	WHERE mis.RequestId = @RequestID

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;

		EXECUTE dbo.WorkingTimeRequest_VoidRequest_ResetIgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ProcessedBy
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		IF (@ErrCode <> 0)
		BEGIN
			ROLLBACK TRANSACTION;

			RETURN
		END

		UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 7
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID

		IF @CurrentStatus IN (4, 6)
		BEGIN
			UPDATE EC
				SET EC.RequestedCheckinTime = CASE WHEN @RequestType = 1 THEN 0 ELSE RequestedCheckinTime END
					,EC.RequestedCheckoutTime = CASE WHEN @RequestType = 2 THEN 0 ELSE RequestedCheckoutTime END
					,EC.ModifiedDate = @ProcessedDate
				FROM EmployeeCalendar EC WITH (ROWLOCK, UPDLOCK)
				WHERE WorkDate = @OnDate
					AND EC.EmpID = @EmpId

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
				,@OnDate
				,@OnDate
				,1
		END

		EXECUTE dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Void request failed'

		ROLLBACK TRANSACTION
	END CATCH;
END