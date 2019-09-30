
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ResetRequest] @RequestId BIGINT
	,@ProcessedBy INT
	,@RoleId INT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-06
Task: Reset Status request form (main SP)
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@RequestType SMALLINT
		,@CurrentStatus TINYINT
		,@VerifierId INT
		,@ApproverId INT
		,@ProcessedDate DATETIME

	SELECT TOP 1 @EmpId = EmpId
		,@CurrentStatus = CurrentStatus
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
		,@RequestType = RequestType
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestID = @RequestId

	IF @RoleId = 2
		AND (
			@VerifierId <> @ProcessedBy
			OR @CurrentStatus NOT IN (2, 3)
			)
	BEGIN
		SET @ErrCode = 101

		IF @CurrentStatus NOT IN (2, 3)
			SET @ErrMsg = 'You could not reset the request.'
		ELSE IF @VerifierId <> @ProcessedBy
			SET @ErrMsg = 'You don''t have permission on this request.'

		RETURN
	END

	IF @RoleId = 3
		AND (
			@ApproverId <> @ProcessedBy
			OR @CurrentStatus NOT IN (4, 5)
			)
	BEGIN
		SET @ErrCode = 102

		IF @CurrentStatus NOT IN (4, 5)
			SET @ErrMsg = 'You could not reset the request.'
		ELSE IF @ApproverId <> @ProcessedBy
			SET @ErrMsg = 'You don''t have permission on this request.'

		RETURN
	END

	IF @RoleId = 6
		AND (@CurrentStatus NOT IN (6, 7))
	BEGIN
		SELECT @ErrCode = 103
			,@ErrMsg = 'You could not reset the request.'

		RETURN
	END

	-- reset ignore
	EXEC dbo.WorkingTimeRequest_ResetRequest_ResetIgnoreDiscipline @RequestId = @RequestId
		,@RoleId = @RoleId
		,@ModifiedBy = @ProcessedBy
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	IF @ErrCode = 0
	BEGIN
		SET @ProcessedDate = GETDATE()

		-- call appropriate SP for each request form
		IF (@RequestType = 1) -- check in
		BEGIN
			EXEC dbo.WorkingTimeRequest_ResetRequest_1 @RequestId = @RequestId
				,@EmpId = @EmpId
				,@RoleId = @RoleId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@CurrentStatus = @CurrentStatus
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 2) -- check out
		BEGIN
			EXEC dbo.WorkingTimeRequest_ResetRequest_2 @RequestId = @RequestId
				,@EmpId = @EmpId
				,@RoleId = @RoleId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@CurrentStatus = @CurrentStatus
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 6) -- leave
		BEGIN
			EXEC dbo.WorkingTimeRequest_ResetRequest_6 @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@CurrentStatus = @CurrentStatus
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 7) -- keep in view
		BEGIN
			EXEC dbo.WorkingTimeRequest_ResetRequest_7 @RequestId = @RequestId
				,@EmpId = @EmpId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@RoleId = @RoleId
				,@CurrentStatus = @CurrentStatus
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

		-- write log
		DECLARE @LogSPData VARCHAR(4000)
			,@now DATETIME
			,@Description VARCHAR(200)

		SET @now = GETDATE()

		--Log general info
		SELECT @LogSPData = '<WorkingTimeRequest_ResetRequest RequestID="' + COALESCE(CAST(@RequestId AS VARCHAR(50)), SPACE(1)) + '" ' + 'ApprovedBy="' + COALESCE(CAST(@ProcessedBy AS VARCHAR(100)), SPACE(1)) + '" ' + 'ApprovedDate="' + COALESCE(CONVERT(VARCHAR(10), @ProcessedDate, 111), SPACE(1)) + '" ' + 'RoleID="' + COALESCE(CAST(@RoleId AS VARCHAR(100)), SPACE(1)) + '" ' + 'ErrCode="' + COALESCE(CAST(@ErrCode AS VARCHAR(50)), SPACE(1)) + '" ' + 'ErrMsg="' + COALESCE(CAST(@ErrMsg AS VARCHAR(4000)), SPACE(1)) + '" />'

		SELECT @Description = 'Reset Status Request:' + COALESCE(CAST(@RequestId AS VARCHAR(50)), SPACE(1))

		EXEC dbo.Log_WriteLog @LogActionID = 3
			,@LogDate = @now
			,@Description = @Description
			,@TrackingValue = @LogSPData
			,@UserID = @ProcessedBy
			,@EmpID = @EmpID
	END
END
