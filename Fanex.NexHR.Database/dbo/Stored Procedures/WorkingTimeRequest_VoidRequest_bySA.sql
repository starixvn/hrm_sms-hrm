
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
	FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
	WHERE wk.RequestID = @RequestID

	-- reset ignore
	EXEC dbo.WorkingTimeRequest_VoidRequest_ResetIgnoreDiscipline @RequestId = @RequestId
		,@RoleId = @RoleId
		,@ModifiedBy = @ProcessedBy
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	IF @ErrCode = 0
	BEGIN
		IF @RequestType = 6 -- leave
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_VoidRequest_bySA_6] @RequestId
				,@ProcessedBy
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT
		END
		ELSE IF @RequestType = 7 -- keep in view
		BEGIN
			EXEC [dbo].[WorkingTimeRequest_VoidRequest_bySA_7] @RequestId
				,@ProcessedBy
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT
		END
	END
	ELSE
	BEGIN
		RETURN
	END

	IF @ErrCode = 0
	BEGIN
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

		SET @now = GETDATE()

		--Log general info
		SELECT @LogSPData = '<WorkingTimeRequest_VoidRequest_bySA RequestID="' + COALESCE(CAST(@RequestID AS VARCHAR(50)), SPACE(1)) + '" ' + 'ProcessedBy="' + COALESCE(CAST(@ProcessedBy AS VARCHAR(100)), SPACE(1)) + '" ' + 'ApprovedDate="' + COALESCE(CONVERT(VARCHAR(10), @ProcessedDate, 111), SPACE(1)) + '" ' + 'ErrCode="' + COALESCE(CAST(@ErrCode AS VARCHAR(50)), SPACE(1)) + '" ' + 'ErrMsg="' + COALESCE(CAST(@ErrMsg AS VARCHAR(4000)), SPACE(1)) + '" />'

		SELECT @Description = 'Void Status Request by SA:' + COALESCE(CAST(@RequestID AS VARCHAR(50)), SPACE(1))

		EXEC dbo.Log_WriteLog @LogActionID = 4
			,@LogDate = @now
			,@Description = @Description
			,@TrackingValue = @LogSPData
			,@UserID = @ProcessedBy
			,@EmpID = @ProcessedBy
	END
END
