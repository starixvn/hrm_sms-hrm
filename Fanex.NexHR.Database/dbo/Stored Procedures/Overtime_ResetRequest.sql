
CREATE PROCEDURE [dbo].[Overtime_ResetRequest] @ActorId INT
	,@RoleId INT
	,@RequestIds VARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-07-05
Task: Reset overtime request
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()
		-- system role
		,@VerifierRoleId TINYINT = 2
		,@ApproverRoleId TINYINT = 3
		,@HRRoleId TINYINT = 6
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	CREATE TABLE #tblRequestIds (
		RequestId BIGINT
		,StatusId TINYINT
		)

	INSERT INTO #tblRequestIds (RequestId)
	SELECT val
	FROM dbo.fnc_strSplit(@RequestIds, ',')

	UPDATE tbl
	SET tbl.StatusId = req.CurrentStatus
	FROM #tblRequestIds AS tbl WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.OTMonthlyRequest AS req WITH (NOLOCK) ON tbl.RequestId = req.RequestId

	IF @RoleId = @VerifierRoleId
	BEGIN
		DELETE #tblRequestIds WITH (ROWLOCK, UPDLOCK)
		WHERE StatusId IN (@PendingStatusId, @ApprovedStatusId, @RejectedStatusId, @ProcessedStatusId, @VoidStatusId)

		UPDATE req
		SET CurrentStatus = @PendingStatusId
			--,VerifierId = @ActorId
			,VerifiedDate = @ModifiedDate
			,VerifiedStatus = @PendingStatusId
		FROM dbo.OTMonthlyRequest AS req WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblRequestIds AS tmp WITH (NOLOCK) ON req.RequestId = tmp.RequestId
	END
	ELSE IF @RoleId = @ApproverRoleId
	BEGIN
		DELETE #tblRequestIds WITH (ROWLOCK, UPDLOCK)
		WHERE StatusId IN (@PendingStatusId, @VerifiedStatusId, @NotVerifiedStatusId, @ProcessedStatusId, @VoidStatusId)

		UPDATE req
		SET CurrentStatus = @PendingStatusId
			--,ApproverId = @ActorId
			,ApprovedDate = @ModifiedDate
			,ApprovedStatus = @PendingStatusId
		FROM dbo.OTMonthlyRequest AS req WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblRequestIds AS tmp WITH (NOLOCK) ON req.RequestId = tmp.RequestId
	END
	ELSE IF @RoleId = @HRRoleId
	BEGIN
		DELETE #tblRequestIds WITH (ROWLOCK, UPDLOCK)
		WHERE StatusId IN (@PendingStatusId, @VerifiedStatusId, @NotVerifiedStatusId, @RejectedStatusId)

		UPDATE req
		SET CurrentStatus = CASE 
				WHEN req.CurrentStatus = @VoidStatusId THEN @PendingStatusId
				ELSE @ApprovedStatusId
				END
			,HRId = @ActorId
			,HRDate = @ModifiedDate
			,HRStatus = 0
		FROM dbo.OTMonthlyRequest AS req WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblRequestIds AS tmp WITH (NOLOCK) ON req.RequestId = tmp.RequestId
			AND tmp.StatusId <> @ApprovedStatusId

		UPDATE req
		SET CurrentStatus = @PendingStatusId
			--,ApproverId = @ActorId
			,ApprovedDate = @ModifiedDate
			,ApprovedStatus = @PendingStatusId
		FROM dbo.OTMonthlyRequest AS req WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblRequestIds AS tmp WITH (NOLOCK) ON req.RequestId = tmp.RequestId
			AND tmp.StatusId = @ApprovedStatusId

		DELETE #tblRequestIds WITH (ROWLOCK, UPDLOCK)
		WHERE StatusId <> @ApprovedStatusId
	END

	UPDATE req
	SET [Status] = CASE 
			WHEN @RoleId = @VerifierRoleId
				OR @RoleId = @ApproverRoleId
				OR (
					@RoleId = @HRRoleId
					AND tmp.StatusId = @ApprovedStatusId
					) THEN @PendingStatusId
			ELSE (CASE 
					WHEN [Status] = @VoidStatusId THEN @PendingStatusId
					ELSE @ApprovedStatusId END
					)
			END
	FROM dbo.OTDailyRequest AS req WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblRequestIds AS tmp WITH (NOLOCK) ON req.RequestId = tmp.RequestId

	DECLARE @RequestId BIGINT
		,@EmpId INT
		,@FromDate DATE
		,@ToDate DATE

	WHILE EXISTS (
			SELECT 1
			FROM #tblRequestIds WITH (NOLOCK)
			)
	BEGIN
		SELECT TOP 1 @RequestId = RequestId
		FROM #tblRequestIds WITH (NOLOCK)

		SELECT @EmpId = EmpId
			,@FromDate = MIN(OvertimeDate)
			,@ToDate = MAX(OvertimeDate)
		FROM dbo.OTDailyRequest WITH (NOLOCK)
		WHERE RequestId = @RequestId
		GROUP BY EmpId

		EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @FromDate
			,@toDate = @ToDate
			,@empIds = @EmpId
			,@isUpdateRequestData = 1

		DELETE
		FROM #tblRequestIds WITH (ROWLOCK, UPDLOCK)
		WHERE RequestId = @RequestId
	END

	DROP TABLE #tblRequestIds

	-- write log
	DECLARE @LogSPData VARCHAR(4000) = '<Overtime_ResetRequest RequestId="' + coalesce(@RequestIds, space(1)) + '" ' + 'RoleId="' + coalesce(cast(@RoleId AS VARCHAR(100)), space(1)) + '" ' + 'ActorId="' + coalesce(cast(@ActorId AS VARCHAR(100)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 13
		,@LogDate = @ModifiedDate
		,@Description = 'Reset overtime request'
		,@TrackingValue = @LogSPData
		,@UserID = @ActorId
		,@EmpID = NULL
END
