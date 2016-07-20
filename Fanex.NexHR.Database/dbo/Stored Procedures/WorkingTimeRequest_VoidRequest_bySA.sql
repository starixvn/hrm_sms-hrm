CREATE PROCEDURE [dbo].[WorkingTimeRequest_VoidRequest_bySA] @RequestId BIGINT
	,@ProcessedBy INT
	,@RoleId INT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151026@Quinn.Vo
Task: Void request by SA
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @RequestType SMALLINT
		,@ProcessedDate DATETIME

	SELECT @RequestType = wk.RequestType
		,@ProcessedDate = GETDATE()
	FROM dbo.WorkingTimeRequest wk WITH (NOLOCK)
	WHERE wk.RequestID = @RequestID

	-- reset ignore
	EXEC dbo.WorkingTimeRequest_VoidRequest_ResetIgnoreDiscipline @RequestId = @RequestId
		,@RoleId = @RoleId
		,@ModifiedBy = @ProcessedBy
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	IF @ErrCode = 0
	BEGIN
		IF (@RequestType = 1) -- check in
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_VoidRequest_bySA_1] @RequestId
				,@ProcessedBy
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 2) -- check out
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_VoidRequest_bySA_2] @RequestId
				,@ProcessedBy
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 3) -- late in
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_VoidRequest_bySA_3] @RequestId
				,@ProcessedBy
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 4) -- early out
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_VoidRequest_bySA_4] @RequestId
				,@ProcessedBy
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 5) -- change shift
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_VoidRequest_bySA_5] @RequestId
				,@ProcessedBy
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 6) -- leave
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_VoidRequest_bySA_6] @RequestId
				,@ProcessedBy
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 7) -- keep in view
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_VoidRequest_bySA_7] @RequestId
				,@ProcessedBy
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT
		END
	END

	IF @ErrCode = 0
	BEGIN
		-- update IsConfirmed
		EXEC dbo.WorkingTimeRequest_UpdateIsConfirmedDiscipline

		-- write to history
		INSERT dbo.WorkingTimeRequestHistory (
			EmpId
			,RequestID
			,RequestType
			,Remark
			,CurrentStatus
			,VerifierId
			,ApproverId
			,HRId
			,VerifiedDate
			,ApprovedDate
			,HRDate
			,VerifiedStatus
			,ApprovedStatus
			,HRStatus
			,CreatedDate
			,CreatedBy
			,ModifiedDate
			,ModifiedBy
			,ReworkCount
			,InsertedDate
			)
		SELECT EmpId
			,@RequestID
			,RequestType
			,@Comment
			,CurrentStatus
			,VerifierId
			,ApproverId
			,@ProcessedBy
			,VerifiedDate
			,ApprovedDate
			,@ProcessedDate
			,VerifiedStatus
			,ApprovedStatus
			,HRStatus
			,CreatedDate
			,CreatedBy
			,@ProcessedDate
			,@ProcessedBy
			,ReworkCount
			,@ProcessedDate
		FROM dbo.WorkingTimeRequest WITH (NOLOCK)
		WHERE RequestID = @RequestID

		-- write log
		DECLARE @LogSPData VARCHAR(4000)
			,@now DATETIME
			,@Description VARCHAR(200)

		SET @now = getdate()

		--Log general info
		SELECT @LogSPData = '<WorkingTimeRequest_VoidRequest_bySA RequestID="' + coalesce(cast(@RequestID AS VARCHAR(50)), space(1)) + '" '
			+ 'ProcessedBy="' + coalesce(cast(@ProcessedBy AS VARCHAR(100)), space(1)) + '" '
			+ 'ApprovedDate="' + coalesce(convert(VARCHAR(10), @ProcessedDate, 111), space(1)) + '" '
			+ 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" '
			+ 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

		SELECT @Description = 'Void Status Request by SA:' + coalesce(cast(@RequestID AS VARCHAR(50)), space(1))

		EXEC dbo.Log_WriteLog @LogActionID = 4
			,@LogDate = @now
			,@Description = @Description
			,@TrackingValue = @LogSPData
			,@UserID = @ProcessedBy
			,@EmpID = @ProcessedBy
	END
END