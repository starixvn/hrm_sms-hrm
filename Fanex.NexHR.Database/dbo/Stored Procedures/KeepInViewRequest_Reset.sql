
CREATE PROCEDURE [dbo].[KeepInViewRequest_Reset] @RequestId BIGINT
	,@EmpId INT
	,@RoleID INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@CurrentStatus INT = NULL
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

	DECLARE @OnDate DATETIME
		,@IsHalfDay BIT
		,@VerifierId INT
		,@ApproverId INT
		,@TotalMinutes INT

	SELECT @OnDate = OnDate
		,@IsHalfDay = IsHalfDay
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
		,@CurrentStatus = CurrentStatus
		,@TotalMinutes = DATEDIFF(MI, FromTime, ToTime)
	FROM dbo.KeepInViewRequest AS rq WITH (NOLOCK)
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
				UPDATE cl WITH (ROWLOCK, UPDLOCK)
				SET cl.IsKeepInView = 0
					,cl.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar cl
					INNER JOIN dbo.KeepInViewRequest csd WITH (NOLOCK) ON cl.WorkDate = csd.OnDate
					AND cl.EmpId = csd.EmpId
				WHERE cl.EmpID = @EmpId
					AND csd.RequestID = @RequestID

				EXEC dbo.KeepInViewRequest_DeleteTrans @RequestId = @RequestId

				-- process the calendar into attendance
				EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
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
				-- update the working calendar to record the check-in time
				UPDATE cl WITH (ROWLOCK, UPDLOCK)
				SET cl.IsKeepInView = 1
					,cl.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar cl
					INNER JOIN dbo.KeepInViewRequest csd WITH (NOLOCK) ON cl.WorkDate = csd.OnDate
					AND cl.EmpId = csd.EmpId
				WHERE cl.EmpID = @EmpId
					AND csd.RequestID = @RequestID

				EXEC dbo.KeepInViewRequest_InsertTrans @RequestId = @RequestId

				-- process the calendar into attendance
				EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
					,@OnDate
					,@OnDate
					,1
			END
		END

		EXEC dbo.KeepInViewRequest_WriteLog @RequestId

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset request failed'

		ROLLBACK TRANSACTION;
	END CATCH;
END