
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_VoidBySA] @RequestId BIGINT
	,@ActorId INT
	,@RoleId TINYINT
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
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @CurrentStatus INT
		,@ProcessedDate DATETIME = GETDATE()
		,@OnDate DATE
		,@EmpId INT
		,@RequestType TINYINT
		-- request type
		,@LateInRequestTypeId TINYINT = 3
		,@EarlyOutRequestTypeId TINYINT = 4
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	SELECT @EmpId = li.EmpId
		,@OnDate = OnDate
		,@RequestType = wk.RequestType
		,@CurrentStatus = wk.CurrentStatus
	FROM dbo.LateInEarlyOutRequest AS li WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON li.RequestId = wk.RequestId
	WHERE li.RequestId = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC dbo.WorkingTimeRequest_VoidRequest_ResetIgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ActorId
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
		SET CurrentStatus = @VoidStatusId
			,ModifiedBy = @ActorId
			,ModifiedDate = @ProcessedDate
		WHERE RequestId = @RequestId

		IF @CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId)
		BEGIN
			UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
			SET LateIn = CASE 
					WHEN @RequestType = @LateInRequestTypeId THEN 0
					ELSE LateIn
					END
				,EarlyOut = CASE 
					WHEN @RequestType = @EarlyOutRequestTypeId THEN 0
					ELSE EarlyOut
					END
				,ModifiedDate = @ProcessedDate
			WHERE WorkDate = @OnDate
				AND EmpId = @EmpId

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@OnDate
				,@OnDate
				,1
		END

		EXEC dbo.LateInEarlyOutRequest_WriteLog @RequestId = @RequestId
			,@Comment = ''

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Void unsuccessfully.'

		ROLLBACK TRANSACTION
	END CATCH;
END
