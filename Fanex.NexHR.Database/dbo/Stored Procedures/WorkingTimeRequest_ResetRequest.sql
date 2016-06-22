
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

	IF @RoleId NOT IN (2, 3, 6)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'You don''t have permission on this request.'

		RETURN
	END

	BEGIN TRY
		SELECT TOP 1 @EmpId = EmpId
			,@CurrentStatus = CurrentStatus
			,@VerifierId = VerifierId
			,@ApproverId = ApproverId
			,@RequestType = RequestType
		FROM dbo.WorkingTimeRequest WITH (NOLOCK)
		WHERE RequestID = @RequestId

		IF (
				@RoleId = 2
				AND (
					(@VerifierId <> @ProcessedBy)
					OR (@CurrentStatus NOT IN (2, 3))
					)
				)
		BEGIN
			SET @ErrCode = 101

			IF @CurrentStatus NOT IN (2, 3)
				SET @ErrMsg = 'You could not reset the request.'
			ELSE IF @VerifierId <> @ProcessedBy
				SET @ErrMsg = 'You don''t have permission on this request.'

			RETURN
		END

		IF (
				@RoleId = 3
				AND (
					(@ApproverId <> @ProcessedBy)
					OR (@CurrentStatus NOT IN (4, 5))
					)
				)
		BEGIN
			SET @ErrCode = 102

			IF @CurrentStatus NOT IN (4, 5)
				SET @ErrMsg = 'You could not reset the request.'
			ELSE IF @ApproverId <> @ProcessedBy
				SET @ErrMsg = 'You don''t have permission on this request.'

			RETURN
		END

		IF (
				@RoleId = 6
				AND ((@CurrentStatus NOT IN (6, 7)))
				)
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
			ELSE IF (@RequestType = 3) -- late in
			BEGIN
				EXEC dbo.WorkingTimeRequest_ResetRequest_3 @RequestId = @RequestId
					,@EmpId = @EmpId
					,@ProcessedBy = @ProcessedBy
					,@ProcessedDate = @ProcessedDate
					,@RoleId = @RoleId
					,@CurrentStatus = @CurrentStatus
					,@ErrCode = @ErrCode OUTPUT
					,@ErrMsg = @ErrMsg OUTPUT
			END
			ELSE IF (@RequestType = 4) -- early out
			BEGIN
				EXEC dbo.WorkingTimeRequest_ResetRequest_4 @RequestId = @RequestId
					,@EmpId = @EmpId
					,@ProcessedBy = @ProcessedBy
					,@ProcessedDate = @ProcessedDate
					,@RoleId = @RoleId
					,@CurrentStatus = @CurrentStatus
					,@ErrCode = @ErrCode OUTPUT
					,@ErrMsg = @ErrMsg OUTPUT
			END
			ELSE IF (@RequestType = 5) -- change shift
			BEGIN
				EXEC dbo.WorkingTimeRequest_ResetRequest_5 @RequestId = @RequestId
					,@EmpId = @EmpId
					,@ProcessedBy = @ProcessedBy
					,@ProcessedDate = @ProcessedDate
					,@RoleId = @RoleId
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

		IF @ErrCode = 0
		BEGIN
			-- write to history
			EXEC [WorkingTimeRequest_WriteHistory] @RequestId

			-- write log
			DECLARE @LogSPData VARCHAR(4000)
				,@now DATETIME
				,@Description VARCHAR(200)

			SET @now = getdate()

			--Log general info
			SELECT @LogSPData = '<WorkingTimeRequest_ResetRequest RequestID="' + coalesce(cast(@RequestId AS VARCHAR(50)), space(1)) + '" ' + 'ApprovedBy="' + coalesce(cast(@ProcessedBy AS VARCHAR(100)), space(1)) + '" ' + 'ApprovedDate="' + coalesce(convert(VARCHAR(10), @ProcessedDate, 111), space(1)) + '" ' + 'RoleID="' + coalesce(cast(@RoleId AS VARCHAR(100)), space(1)) + '" ' + 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' + 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

			SELECT @Description = 'Reset Status Request:' + coalesce(cast(@RequestId AS VARCHAR(50)), space(1))

			EXEC dbo.Log_WriteLog @LogActionID = 3
				,@LogDate = @now
				,@Description = @Description
				,@TrackingValue = @LogSPData
				,@UserID = @ProcessedBy
				,@EmpID = @EmpID
		END
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset Request failed'
	END CATCH;
END
