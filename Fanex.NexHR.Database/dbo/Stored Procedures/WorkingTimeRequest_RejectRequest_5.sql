/****** Object:  Stored Procedure dbo.WorkingTimeRequest_RejectRequest_5    Script Date: 7/20/2016 10:19:23 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_RejectRequest_5    Script Date: 7/19/2016 5:22:23 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_RejectRequest_5] @RequestId BIGINT
	,@EmpId INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@RoleID INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-07
Task: disapprove change shift request form (type 5)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME

	SELECT @FromDate = lr.FromDate
		,@ToDate = lr.ToDate
	FROM dbo.ChangeShiftRequest lr WITH (NOLOCK)
	WHERE lr.RequestID = @RequestID

	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleID = 2 --Verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 3
				,VerifiedStatus = 3
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID
		END
		ELSE IF @RoleID = 3 --Approver
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 5
				,ApprovedStatus = 3
				,ApproverId = @ProcessedBy
				,ApprovedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId
		END
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 7
				,HRStatus = 3
				,HRId = @ProcessedBy
				,HRDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestId

			UPDATE EC
			SET EC.IsOffDay = 0
				,EC.ChangeShiftID = NULL
				,EC.ShiftId = FromShift
				,EC.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS EC WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS dt WITH (NOLOCK) ON EC.WorkDate = dt.OnDate
			WHERE EC.EmpID = @EmpId
				AND dt.ToShift = - 1
				AND dt.RequestId = @RequestID
				AND EC.ChangeShiftId IS NOT NULL

			UPDATE cl
			SET cl.ChangeShiftID = NULL
				,cl.ShiftId = NULL
				,cl.IsOffDay = 1
				,cl.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS cl WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS csd WITH (NOLOCK) ON cl.WorkDate = csd.OnDate
				AND cl.EmpId = csd.EmpId
			WHERE cl.EmpID = @EmpId
				AND csd.RequestID = @RequestID
				AND csd.ToShift <> - 1
				AND cl.ChangeShiftId IS NULL

			UPDATE cl
			SET cl.ChangeShiftID = NULL
				,cl.ShiftId = cl.ChangeShiftID
				,cl.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar AS cl WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.ChangeShiftRequestDetail AS csd WITH (NOLOCK) ON cl.WorkDate = csd.OnDate
				AND cl.EmpId = csd.EmpId
			WHERE cl.EmpID = @EmpId
				AND csd.RequestID = @RequestID
				AND csd.ToShift <> - 1
				AND cl.ChangeShiftId IS NOT NULL

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
		END

		COMMIT TRANSACTION;

		SET @ErrCode = 0
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject request failed'

		ROLLBACK TRANSACTION;
	END CATCH;
END
