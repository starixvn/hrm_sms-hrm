
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest_7] @RequestID BIGINT
	,@EmpId INT
	,@RoleID INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-13
Task: approve keep in view request form (type 7)
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

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
		,@IsHalfDay = IsHalfDay
	FROM dbo.KeepInViewRequest WITH (NOLOCK)
	WHERE RequestID = @RequestID

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC [dbo].[EmployeeCalendar_Insert]
			@empid = @empid,
			@fromdate = @FromDate, 
			@todate = @ToDate,
			@ErrCode = @ErrCode  OUTPUT,
			@ErrMsg = @ErrMsg  OUTPUT
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