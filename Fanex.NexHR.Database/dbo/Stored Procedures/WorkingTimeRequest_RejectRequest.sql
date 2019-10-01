
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

	IF @RoleId NOT IN (2, 3, 6)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'You don''t have permission on this function.'

		RETURN
	END

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

	-- reset ignore
	EXEC dbo.WorkingTimeRequest_RejectRequest_ResetIgnoreDiscipline @RequestId = @RequestId
		,@RoleId = @RoleId
		,@ModifiedBy = @ProcessedBy
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	IF @ErrCode = 0
	BEGIN
		SET @ProcessedDate = GETDATE()

		IF @RequestType = 6
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_RejectRequest_6] @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END
		ELSE IF @RequestType = 7
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_RejectRequest_7] @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END
	END
	ELSE
	BEGIN
		RETURN
	END

	IF @ErrCode = 0
	BEGIN
		-- write to history
		EXEC [WorkingTimeRequest_WriteHistory] @RequestId
			,@Comment

		-- write log
		DECLARE @LogSPData VARCHAR(4000)
			,@now DATETIME
			,@Description VARCHAR(200)

		SET @now = GETDATE()

		--Log general info
		SELECT @LogSPData = '<WorkingTimeRequest_RejectRequest RequestID="' + COALESCE(CAST(@RequestId AS VARCHAR(50)), SPACE(1)) + '" ' + 'RoleID="' + COALESCE(CAST(@RoleId AS VARCHAR(100)), SPACE(1)) + '" ' + 'ProcessedBy="' + COALESCE(CAST(@ProcessedBy AS VARCHAR(100)), SPACE(1)) + '" ' + 'ProcessedDate="' + COALESCE(CONVERT(VARCHAR(10), @ProcessedDate, 111), SPACE(1)) + '" ' + 'ErrCode="' + COALESCE(CAST(@ErrCode AS VARCHAR(50)), SPACE(1)) + '" ' + 'ErrMsg="' + COALESCE(CAST(@ErrMsg AS VARCHAR(4000)), SPACE(1)) + '" />'

		SELECT @Description = 'Reject Request:' + COALESCE(CAST(@RequestId AS VARCHAR(50)), SPACE(1))

		EXEC dbo.Log_WriteLog @LogActionID = 2
			,@LogDate = @now
			,@Description = @Description
			,@TrackingValue = @LogSPData
			,@UserID = @ProcessedBy
			,@EmpID = @EmpID
	END
END
