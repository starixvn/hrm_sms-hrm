
CREATE PROCEDURE [dbo].[CheckInOutRequest_Reject]  @RequestId BIGINT
	,@ProcessedBy INT
	,@RoleId INT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190522@Quinn.Vo
Task: reject check in/out request
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @OnDate DATETIME
		,@EmpId INT
		,@ProcessedDate DATETIME = GETDATE()
		,@RequestType SMALLINT

	SELECT @EmpId = mis.EmpId
		,@OnDate = OnDate
		,@RequestType = wk.RequestType
	FROM dbo.CheckInOutRequest AS mis WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON mis.RequestId = wk.RequestId
	WHERE mis.RequestID = @RequestId

	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		-- reset discipline ignore
		EXECUTE dbo.WorkingTimeRequest_RejectRequest_ResetIgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ProcessedBy
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		IF (@ErrCode <> 0)
		BEGIN
			ROLLBACK TRANSACTION;

			RETURN
		END

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
			SET EC.RequestedCheckinTime = CASE WHEN @RequestType = 1 THEN 0 ELSE ec.RequestedCheckinTime END
				,EC.RequestedCheckoutTime = CASE WHEN @RequestType = 2 THEN 0 ELSE ec.RequestedCheckoutTime END
				,EC.ModifiedDate = @ProcessedDate
			FROM EmployeeCalendar EC WITH (ROWLOCK, UPDLOCK)
			WHERE WorkDate = @OnDate
				AND EC.EmpID = @EmpId

			EXECUTE dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@OnDate
				,@OnDate
				,1
		END

		EXECUTE dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId, @Comment = @Comment

		COMMIT TRANSACTION;

		SET @ErrCode = 0
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject request failed'

		ROLLBACK TRANSACTION;
	END CATCH;
END