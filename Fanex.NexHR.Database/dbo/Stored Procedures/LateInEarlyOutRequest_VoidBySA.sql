
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_VoidBySA] @RequestId BIGINT
	,@ProcessedBy INT
	,@RoleId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151026@Quinn.Vo
Task: Void request late in/early out by SA
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
		,@RequestType TINYINT

	SELECT @EmpId = li.EmpId
		,@OnDate = OnDate
		,@RequestType = wk.RequestType
		,@CurrentStatus = wk.CurrentStatus
	FROM dbo.LateInEarlyOutRequest AS li WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON li.RequestId = wk.RequestId
	WHERE li.RequestID = @RequestID

	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC dbo.WorkingTimeRequest_VoidRequest_ResetIgnoreDiscipline @RequestId = @RequestId
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
			SET EC.LateIn = CASE 
					WHEN @RequestType = 3 THEN 0
					ELSE EC.LateIn
					END
				,EC.EarlyOut = CASE 
					WHEN @RequestType = 4 THEN 0
					ELSE EC.EarlyOut
					END
				,EC.ModifiedDate = @ProcessedDate
			FROM EmployeeCalendar EC WITH (ROWLOCK, UPDLOCK)
			WHERE WorkDate = @OnDate
				AND EC.EmpID = @EmpId

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
				,@OnDate
				,@OnDate
				,1
		END

		EXECUTE dbo.LateInEarlyOutRequest_WriteLog @RequestId = @RequestId

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Void request failed'

		ROLLBACK TRANSACTION
	END CATCH;
END