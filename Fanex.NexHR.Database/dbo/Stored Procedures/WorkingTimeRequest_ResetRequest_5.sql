
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ResetRequest_5] @RequestID BIGINT
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
Task: Reset Status changing working shift request form (type 5)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@VerifierId INT
		,@ApproverId INT

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
	FROM dbo.ChangeShiftRequest AS rq WITH (NOLOCK)
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
				UPDATE EC
				SET EC.IsOffDay = 0
					,EC.ChangeShiftID = NULL
					,EC.ShiftId = FromShift
					,EC.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK) ON EC.WorkDate = dt.OnDate
					AND EC.EmpId = dt.EmpId
				WHERE EC.EmpID = @EmpId
					AND dt.ToShift = - 1
					AND dt.RequestId = @RequestID
					AND EC.ChangeShiftId IS NOT NULL

				UPDATE EC
				SET EC.ShiftId = NULL
					,EC.IsOffDay = 1
					,EC.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK) ON EC.WorkDate = dt.OnDate
					AND EC.EmpId = dt.EmpId
				WHERE EC.EmpID = @EmpId
					AND dt.RequestID = @RequestID
					AND dt.ToShift <> - 1
					AND EC.ChangeShiftId IS NULL

				UPDATE EC
				SET EC.ChangeShiftID = NULL
					,EC.ShiftId = EC.ChangeShiftID
					,EC.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK) ON EC.WorkDate = dt.OnDate
					AND EC.EmpId = dt.EmpId
				WHERE EC.EmpID = @EmpId
					AND dt.RequestID = @RequestID
					AND dt.ToShift <> - 1
					AND EC.ChangeShiftId IS NOT NULL

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
				UPDATE EC
				SET EC.ModifiedDate = @ProcessedDate
					,EC.IsPublic = 1
				FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK) ON EC.WorkDate = dt.OnDate
					AND EC.EmpId = dt.EmpId
				WHERE EC.EmpID = @EmpId
					AND dt.ToShift = - 1
					AND dt.RequestId = @RequestID
					AND EC.IsOffDay = 1

				UPDATE EC
				SET EC.IsOffDay = 1
					,EC.ChangeShiftID = dt.FromShift
					,EC.ShiftId = NULL
					,EC.ModifiedDate = @ProcessedDate
					,EC.IsPublic = 1
				FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK) ON EC.WorkDate = dt.OnDate
					AND EC.EmpId = dt.EmpId
				WHERE EC.EmpID = @EmpId
					AND dt.ToShift = - 1
					AND dt.RequestId = @RequestID
					AND EC.IsOffDay = 0
				
				UPDATE EC
				SET EC.ChangeShiftID = NULL
					,EC.ShiftId = dt.ToShift
					,EC.IsOffDay = 0
					,EC.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK) ON EC.WorkDate = dt.OnDate
					AND EC.EmpId = dt.EmpId
				WHERE EC.EmpID = @EmpId
					AND dt.RequestID = @RequestID
					AND dt.ToShift <> - 1
					AND EC.IsOffDay = 1

				UPDATE EC
				SET EC.ChangeShiftID = dt.FromShift
					,EC.ShiftId = dt.ToShift
					,EC.ModifiedDate = @ProcessedDate
				FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK) ON EC.WorkDate = dt.OnDate
					AND EC.EmpId = dt.EmpId
				WHERE EC.EmpID = @EmpId
					AND dt.RequestID = @RequestID
					AND dt.ToShift <> - 1
					AND EC.IsOffDay = 0

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