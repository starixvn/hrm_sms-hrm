/****** Object:  Stored Procedure dbo.WorkingTimeRequest_ApproveRequest_5    Script Date: 7/20/2016 10:19:20 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_ApproveRequest_5    Script Date: 7/19/2016 5:22:21 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_5] @RequestId BIGINT
	,@EmpId INT
	,@RoleID INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-06
Task: approve changing working shift request form (type 5)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ChangeShiftId INT
		,@FromDate DATETIME
		,@ToDate DATETIME

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
	FROM dbo.ChangeShiftRequest WITH (NOLOCK)
	WHERE RequestID = @RequestID

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC [dbo].[EmployeeCalendar_Insert] @empid = @empid
			,@fromdate = @FromDate
			,@todate = @ToDate
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		IF @ErrCode <> 0
		BEGIN
			ROLLBACK TRANSACTION;

			RETURN
		END

		IF @RoleID = 2 --Verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 2
				,VerifiedStatus = 2
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID

			EXEC [WorkingTimeRequest_WriteHistory] @RequestID, @Comment
		END
		ELSE IF @RoleID = 3 --Approver
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET [CurrentStatus] = 4
				,ApprovedStatus = 2
				,ApproverComment = @Comment
				,ApproverId = @ProcessedBy
				,ApprovedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID

			EXEC [WorkingTimeRequest_WriteHistory] @RequestID, @Comment

			-- update the working calendar to record the check-in time
			UPDATE EC
			SET EC.IsOffDay = 1
				,EC.ChangeShiftID = dt.FromShift
				,EC.ShiftId = NULL
				,EC.ModifiedDate = @ProcessedDate
				,EC.IsPublic = 1
			FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK) 
					ON EC.WorkDate = dt.OnDate
					AND EC.EmpId = dt.EmpId
			WHERE EC.EmpID = @EmpId
				AND dt.ToShift = - 1
				AND dt.RequestId = @RequestID
				AND EC.IsOffDay = 0

			UPDATE EC
			SET EC.ModifiedDate = @ProcessedDate
				,EC.IsPublic = 1
			FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK)
					ON EC.WorkDate = dt.OnDate
					AND EC.EmpId = dt.EmpId
			WHERE EC.EmpID = @EmpId
				AND dt.ToShift = - 1
				AND dt.RequestId = @RequestID
				AND EC.IsOffDay = 1

			UPDATE EC
			SET EC.ChangeShiftID = dt.FromShift
				,EC.ShiftId = dt.ToShift
				,EC.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK)
					ON EC.WorkDate = dt.OnDate
						AND EC.EmpId = dt.EmpId
			WHERE EC.EmpID = @EmpId
				AND dt.RequestID = @RequestID
				AND dt.ToShift <> - 1
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

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId, @FromDate, @ToDate, 1
		END
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 6
				,HRStatus = 2
				,HRId = @ProcessedBy
				,HRDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID

			EXEC [WorkingTimeRequest_WriteHistory] @RequestID, @Comment
		END

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Aprrove request failed'

		ROLLBACK TRANSACTION;
	END CATCH;
END