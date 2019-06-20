
CREATE PROCEDURE [dbo].[KeepInViewRequest_Approve] @RequestID BIGINT
	,@EmpId INT
	,@RoleID INT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190522@Quinn.Vo
Task: approve keep in view request
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @OnDate DATETIME
		,@TotalMinutes INT

	SELECT @OnDate = OnDate
	FROM dbo.KeepInViewRequest WITH (NOLOCK)
	WHERE RequestID = @RequestID

	SET XACT_ABORT ON;
	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC [dbo].[EmployeeCalendar_Insert]
			@empid = @empid,
			@fromdate = @OnDate, 
			@todate = @OnDate,
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

			-- update the working calendar to record the check-in time
			UPDATE cl WITH (ROWLOCK, UPDLOCK)
			SET cl.IsKeepInView = 1
				,cl.ModifiedDate = @ProcessedDate
			FROM dbo.EmployeeCalendar cl
				INNER JOIN dbo.KeepInViewRequest csd WITH (NOLOCK) ON 
					cl.WorkDate = csd.OnDate
				AND cl.EmpId = csd.EmpId
			WHERE cl.EmpID = @EmpId
				AND csd.RequestID = @RequestID

			EXEC dbo.KeepInViewRequest_InsertTrans @RequestId = @RequestId

			EXEC EmployeeAttendance_ProcessingAndInsert @EmpId, @OnDate, @OnDate, 1
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
		END

		EXEC dbo.KeepInViewRequest_WriteLog @RequestId, @Comment

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Aprrove request failed'

		ROLLBACK TRANSACTION;
	END CATCH;
END