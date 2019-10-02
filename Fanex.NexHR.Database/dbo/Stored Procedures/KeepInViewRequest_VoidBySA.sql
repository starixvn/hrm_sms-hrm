
CREATE PROCEDURE [dbo].[KeepInViewRequest_VoidBySA] @RequestId BIGINT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190522@Quinn.Vo
Task: void keep in view request by SA.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @CurrentStatus TINYINT
		,@ProcessedDate DATETIME = GETDATE()
		,@OnDate DATE
		,@EmpId INT
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	SELECT @CurrentStatus = wkr.CurrentStatus
		,@OnDate = kiv.OnDate
		,@EmpId = wkr.EmpId
	FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
		INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON wkr.RequestId = kiv.RequestId
		AND kiv.RequestId = @RequestId

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
		SET CurrentStatus = @VoidStatusId
			,ModifiedBy = @ActorId
			,ModifiedDate = @ProcessedDate
		WHERE RequestId = @RequestId

		IF @CurrentStatus IN (@ApprovedStatusId, @ProcessedStatusId)
		BEGIN
			UPDATE empca
			SET empca.IsKeepInView = 0
				,empca.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON empca.WorkDate = kiv.OnDate
				AND empca.EmpId = kiv.EmpId
				AND kiv.RequestId = @RequestId

			EXEC dbo.KeepInViewRequest_DeleteTrans @RequestId = @RequestId

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@OnDate
				,@OnDate
				,1
		END

		EXEC dbo.KeepInViewRequest_WriteLog @RequestId = @RequestId
			,@Comment = ''

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Void unsuccessfully.'

		ROLLBACK TRANSACTION
	END CATCH;
END
