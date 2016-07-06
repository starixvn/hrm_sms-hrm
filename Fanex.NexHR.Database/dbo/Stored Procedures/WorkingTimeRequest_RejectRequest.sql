
CREATE PROCEDURE [dbo].[WorkingTimeRequest_RejectRequest] @RequestId BIGINT
	,@ProcessedBy INT
	,@RoleId INT
	,@Comment NVARCHAR(500) = 'No@Comment!'
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-08
Task: reject a request form (main SP)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@RequestType SMALLINT
		,@Status TINYINT
		,@VerifierId INT
		,@ApproverId INT
		,@HRId INT
		,@VerifiedStatus INT
		,@ApprovedStatus INT
		,@HRStatus INT
		,@ProcessedDate DATETIME

	-- if status is not pending, show error
	IF @ProcessedBy IS NULL
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = '@ProcessedBy parameter is missing.'

		RETURN
	END

	-- get employee and requestype of the request		
	SELECT TOP 1 @EmpId = EmpId
		,@Status = [CurrentStatus]
		,@VerifierId = VerifierId
		,@VerifiedStatus = VerifiedStatus
		,@ApproverId = ApproverId
		,@ApprovedStatus = ApprovedStatus
		,@HRId = HRId
		,@HRStatus = HRStatus
		,@RequestType = RequestType
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestID = @RequestId

	IF @RoleId = 6
		AND @Status <> 4
	BEGIN
		SET @ErrCode = 104
		SET @ErrMsg = 'This request is not pending so it cannot be rejected.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		-- reset ignore
		EXEC dbo.WorkingTimeRequest_RejectRequest_ResetIgnoreDiscipline @RequestId = @RequestId
			,@RoleId = @RoleId
			,@ModifiedBy = @ProcessedBy
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		SET @ProcessedDate = GETDATE()

		IF @RequestType = 1
			EXEC [dbo].[WorkingTimeRequest_RejectRequest_1] @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		ELSE IF @RequestType = 2
			EXEC [dbo].[WorkingTimeRequest_RejectRequest_2] @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		ELSE IF @RequestType = 3
			EXEC [dbo].[WorkingTimeRequest_RejectRequest_3] @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		ELSE IF @RequestType = 4
			EXEC [dbo].[WorkingTimeRequest_RejectRequest_4] @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		ELSE IF @RequestType = 5
			EXEC [dbo].[WorkingTimeRequest_RejectRequest_5] @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		ELSE IF @RequestType = 6
			EXEC [dbo].[WorkingTimeRequest_RejectRequest_6] @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		ELSE IF @RequestType = 7
			EXEC [dbo].[WorkingTimeRequest_RejectRequest_7] @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT

		-- write to history
		EXEC [WorkingTimeRequest_WriteHistory] @RequestId
			,@Comment

		-- write log
		DECLARE @LogSPData VARCHAR(4000)
			,@now DATETIME
			,@Description VARCHAR(200)

		SET @now = getdate()

		--Log general info
		SELECT @LogSPData = '<WorkingTimeRequest_RejectRequest RequestID="' + coalesce(cast(@RequestId AS VARCHAR(50)), space(1)) + '" ' + 'RoleID="' + coalesce(cast(@RoleId AS VARCHAR(100)), space(1)) + '" ' + 'ProcessedBy="' + coalesce(cast(@ProcessedBy AS VARCHAR(100)), space(1)) + '" ' + 'ProcessedDate="' + coalesce(convert(VARCHAR(10), @ProcessedDate, 111), space(1)) + '" ' + 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' + 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

		SELECT @Description = 'Reject Request:' + coalesce(cast(@RequestId AS VARCHAR(50)), space(1))

		EXEC dbo.Log_WriteLog @LogActionID = 2
			,@LogDate = @now
			,@Description = @Description
			,@TrackingValue = @LogSPData
			,@UserID = @ProcessedBy
			,@EmpID = @EmpID

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject process failed'
	END CATCH;
END