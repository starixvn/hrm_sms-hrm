
CREATE PROCEDURE [dbo].[Overtime_ApproveRequest] @ActorId INT
	,@RoleId INT
	,@RequestIds VARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-07-05
Task: approve overtime request
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

	CREATE TABLE #tblRequestIds (RequestId BIGINT)

	INSERT INTO #tblRequestIds (RequestId)
	SELECT val
	FROM dbo.fnc_strSplit(@RequestIds, ',')

	IF @RoleId = @VerifierRoleId
	BEGIN
		DELETE tmp
		FROM #tblRequestIds AS tmp
			INNER JOIN dbo.OTMonthlyRequest AS req WITH (NOLOCK) ON req.RequestId = tmp.RequestId
			AND req.CurrentStatus <> @PendingStatusId

		UPDATE req
		SET CurrentStatus = @VerifiedStatusId
			,VerifierId = @ActorId
			,VerifiedDate = @ModifiedDate
			,VerifiedStatus = 2
		FROM dbo.OTMonthlyRequest AS req WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblRequestIds AS tmp ON req.RequestId = tmp.RequestId
	END
	ELSE IF @RoleId = @ApproverRoleId
	BEGIN
		DELETE tmp
		FROM #tblRequestIds AS tmp
			INNER JOIN dbo.OTMonthlyRequest AS req WITH (NOLOCK) ON req.RequestId = tmp.RequestId
			AND req.CurrentStatus NOT IN (@PendingStatusId, @VerifiedStatusId)

		UPDATE req
		SET CurrentStatus = @ApprovedStatusId
			,ApproverId = @ActorId
			,ApprovedDate = @ModifiedDate
			,ApprovedStatus = @VerifiedStatusId
		FROM dbo.OTMonthlyRequest AS req WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblRequestIds AS tmp ON req.RequestId = tmp.RequestId
	END
	ELSE IF @RoleId = @HRRoleId
	BEGIN
		DELETE tmp
		FROM #tblRequestIds tmp
			INNER JOIN dbo.OTMonthlyRequest AS req WITH (NOLOCK) ON OT.RequestId = tmp.RequestId
			AND req.CurrentStatus <> @ApprovedStatusId

		UPDATE req
		SET CurrentStatus = @ProcessedStatusId
			,HRId = @ActorId
			,HRDate = @ModifiedDate
			,HRStatus = @VerifiedStatusId
		FROM dbo.OTMonthlyRequest AS req WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblRequestIds AS tmp ON req.RequestId = tmp.RequestId
	END

	UPDATE req
	SET [Status] = CASE 
			WHEN @RoleId = @VerifierRoleId THEN @VerifiedStatusId
			WHEN @RoleId = @ApproverRoleId THEN @ApprovedStatusId
			ELSE @ProcessedStatusId
			END
	FROM dbo.OTDailyRequest AS req WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblRequestIds AS tmp ON req.RequestId = tmp.RequestId

	DROP TABLE #tblRequestIds

	-- write log
	DECLARE @LogSPData VARCHAR(4000) = '<Overtime_ApproveRequest RequestId="' + coalesce(@RequestIds, space(1)) + '" ' + 'RoleId="' + coalesce(cast(@RoleId AS VARCHAR(100)), space(1)) + '" ' + 'ActorId="' + coalesce(cast(@ActorId AS VARCHAR(10)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 11
		,@LogDate = @ModifiedDate
		,@Description = 'Approve overtime request'
		,@TrackingValue = @LogSPData
		,@UserID = @ActorId
		,@EmpID = NULL
END
