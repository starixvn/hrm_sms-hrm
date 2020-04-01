
CREATE PROCEDURE [dbo].[Overtime_RejectRequest] @ActorId INT
	,@RoleId TINYINT
	,@RequestIds VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-07-05
Task: Reject Overtime request
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
		SET req.CurrentStatus = @NotVerifiedStatusId
			,req.VerifierId = @ActorId
			,req.VerifiedDate = @ModifiedDate
			,req.VerifiedStatus = @NotVerifiedStatusId
		FROM dbo.OTMonthlyRequest AS req WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblRequestIds AS tmp WITH (NOLOCK) ON req.RequestId = tmp.RequestId
	END
	ELSE IF @RoleId = @ApproverRoleId
	BEGIN
		DELETE tmp
		FROM #tblRequestIds AS tmp
			INNER JOIN dbo.OTMonthlyRequest AS req WITH (NOLOCK) ON req.RequestId = tmp.RequestId
			AND req.CurrentStatus <> @VerifiedStatusId

		UPDATE req
		SET req.CurrentStatus = @RejectedStatusId
			,req.ApproverId = @ActorId
			,req.ApprovedDate = @ModifiedDate
			,req.ApprovedStatus = @NotVerifiedStatusId
		FROM dbo.OTMonthlyRequest AS req WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblRequestIds AS tmp WITH (NOLOCK) ON req.RequestId = tmp.RequestId
	END
	ELSE IF @RoleId = @HRRoleId
	BEGIN
		DELETE tmp
		FROM #tblRequestIds AS tmp
			INNER JOIN dbo.OTMonthlyRequest AS req WITH (NOLOCK) ON req.RequestId = tmp.RequestId
			AND req.CurrentStatus <> @ApprovedStatusId

		UPDATE req
		SET req.CurrentStatus = @VoidStatusId
			,req.HRId = @ActorId
			,req.HRDate = @ModifiedDate
			,req.HRStatus = @NotVerifiedStatusId
		FROM dbo.OTMonthlyRequest AS req WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblRequestIds AS tmp WITH (NOLOCK) ON req.RequestId = tmp.RequestId
	END

	UPDATE req
	SET req.[Status] = CASE 
			WHEN @RoleId = @VerifierRoleId THEN @NotVerifiedStatusId
			WHEN @RoleId = @ApproverRoleId THEN @RejectedStatusId
			ELSE @VoidStatusId
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
		GROUP by EmpId

		EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @FromDate
			,@toDate = @ToDate
			,@empIds = @EmpId
			,@isUpdateRequestData = 1

		DELETE
		FROM #tblRequestIds
		WHERE RequestId = @RequestId
	END

	DROP TABLE #tblRequestIds

	-- write log
	DECLARE @LogSPData VARCHAR(4000) = '<Overtime_RejectRequest RequestId="' + coalesce(@RequestIds, space(1)) + '" ' + 'RoleId="' + coalesce(cast(@RoleId AS VARCHAR(10)), space(1)) + '" ' + 'ActorId="' + coalesce(cast(@ActorId AS VARCHAR(100)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 12
		,@LogDate = @ModifiedDate
		,@Description = 'Reject overtime request'
		,@TrackingValue = @LogSPData
		,@UserID = @ActorId
		,@EmpID = NULL
END
