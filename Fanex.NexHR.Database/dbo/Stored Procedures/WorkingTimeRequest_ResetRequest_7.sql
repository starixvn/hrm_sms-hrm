
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ResetRequest_7] @RequestID BIGINT
	,@EmpId INT
	,@RoleID INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@CurrentStatus INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-13
Task: Reset Status keep in view request form (type 7)
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@IsHalfDay BIT
		,@VerifierId INT
		,@ApproverId INT

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
		,@IsHalfDay = IsHalfDay
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
		,@CurrentStatus = CurrentStatus
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
				UPDATE EmployeeSummary WITH (ROWLOCK, UPDLOCK)
				SET KeepInViewBalance = ISNULL(KeepInViewBalance, 0) - CASE 
						WHEN @IsHalfDay = 1 THEN 0.5
						ELSE 1
						END
				WHERE EmpId = @EmpId

				UPDATE cl WITH (ROWLOCK, UPDLOCK)
				SET cl.IsKeepInView = 0
					--,cl.LeaveDay = CASE 
					--	WHEN csd.IsHalfDay = 1 THEN 0.5
					--	ELSE 0
					--	END
					,cl.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar cl
					INNER JOIN dbo.KeepInViewRequest csd WITH (NOLOCK) ON (
						cl.WorkDate BETWEEN csd.FROMDate
							AND csd.ToDate
						)
					AND cl.EmpId = csd.EmpId
				WHERE cl.EmpID = @EmpId
					AND csd.RequestID = @RequestID

				-- process the calendar into attendance
				EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
					,@FromDate
					,@ToDate
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
				UPDATE EmployeeSummary WITH (ROWLOCK, UPDLOCK)
				SET KeepInViewBalance = ISNULL(KeepInViewBalance, 0) + CASE 
						WHEN @IsHalfDay = 1 THEN 0.5
						ELSE 1
						END
				WHERE EmpId = @EmpId

				-- update the working calendar to record the check-in time
				UPDATE cl WITH (ROWLOCK, UPDLOCK)
				SET cl.IsKeepInView = 1
					,cl.LeaveDay = CASE 
						WHEN csd.IsHalfDay = 1 THEN 0.5
						ELSE 0
						END
					,cl.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar cl
					INNER JOIN dbo.KeepInViewRequest csd WITH (NOLOCK) ON (
						cl.WorkDate BETWEEN csd.FROMDate
							AND csd.ToDate
						)
					AND cl.EmpId = csd.EmpId
				WHERE cl.EmpID = @EmpId
					AND csd.RequestID = @RequestID

				-- process the calendar into attendance
				EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
					,@FromDate
					,@ToDate
					,1
			END
		END

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset request failed'

		ROLLBACK TRANSACTION;
	END CATCH;
END