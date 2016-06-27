CREATE PROCEDURE [dbo].[OverTime_ResetMultiRequest]
	--@Month datetime,
	@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@RoleID INT
	,@bOnbehalf BIT = 0
	,@ListRequestID VARCHAR(8000)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-13
Task: Reset Status Overtime request form (type 8)
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	--SELECT @Month = CONVERT(NVARCHAR(10),YEAR(@Month)) + '/' + CONVERT(NVARCHAR(10),MONTH(@Month)) + '/01'
	IF 1 = 0
		AND NOT (
			DAY(GETDATE()) BETWEEN 15
				AND 25
			)
	BEGIN
		SET @ErrCode = 105
		SET @ErrMsg = 'You could only reject on arrange day 15 to 25 every month.'

		RETURN;
	END

	IF OBJECT_ID('tempdb..#RequestID') IS NOT NULL
		DROP TABLE #RequestID

	CREATE TABLE #RequestID (RequestID INT)

	IF ISNULL(@ListRequestID, '') <> ''
		INSERT INTO #RequestID (RequestID)
		SELECT val
		FROM fnc_strSplit(@ListRequestID, ',')

	IF NOT EXISTS (
			SELECT TOP 1 1
			FROM #RequestID
			)
	BEGIN
		SET @ErrCode = 104
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	IF (@RoleID = 2)
	BEGIN
		DELETE tmp
		FROM #RequestID tmp
			INNER JOIN dbo.[OTMonthlyRequest] OT WITH (NOLOCK) ON OT.RequestID = tmp.RequestID
			AND OT.CurrentStatus IN (1, 4, 5, 6, 7)
	END
	ELSE IF (@RoleID = 3)
	BEGIN
		DELETE tmp
		FROM #RequestID tmp
			INNER JOIN dbo.[OTMonthlyRequest] OT WITH (NOLOCK) ON OT.RequestID = tmp.RequestID
			AND OT.CurrentStatus IN (1, 2, 3, 6, 7)
	END
	ELSE IF (@RoleID = 3)
	BEGIN
		DELETE tmp
		FROM #RequestID tmp
			INNER JOIN dbo.[OTMonthlyRequest] OT WITH (NOLOCK) ON OT.RequestID = tmp.RequestID
			AND OT.CurrentStatus IN (1, 2, 3, 4, 5)
	END

	DECLARE @Month DATETIME
		,@sEmpID VARCHAR(8000) = ''

	IF @RoleID IN (3, 6)
	BEGIN
		SELECT TOP 1 @Month = OT.[Month]
		FROM dbo.OTMonthlyRequest OT WITH (NOLOCK)
			INNER JOIN #RequestID tmp ON OT.RequestID = tmp.RequestID

		SELECT @sEmpID = COALESCE(@sEmpID + ',', '') + convert(NVARCHAR(50), ISNULL(OT.EmpId, ''))
		FROM dbo.OTMonthlyRequest OT WITH (NOLOCK)
			INNER JOIN #RequestID tmp ON OT.RequestID = tmp.RequestID
	END

	BEGIN TRY
		BEGIN TRANSACTION AAA;

		UPDATE OT
		SET OT.[Status] = CASE 
				WHEN @RoleID = 2 THEN 1
				WHEN @RoleID = 3 THEN 2
				ELSE (CASE 
						WHEN OT.[Status] = 7 THEN 1
						ELSE 4 END
						)
				END
		FROM [OTDailyRequest] OT WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #RequestID tmp ON OT.RequestID = tmp.RequestID

		IF @RoleID = 2 --Verifier
		BEGIN
			UPDATE OT
			SET OT.[CurrentStatus] = 1
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,VerifiedStatus = 1
			FROM [OTMonthlyRequest] OT WITH (ROWLOCK, UPDLOCK)
				INNER JOIN #RequestID tmp ON OT.RequestID = tmp.RequestID
		END
		ELSE IF @RoleID = 3 --Approver
		BEGIN
			UPDATE OT
			SET [CurrentStatus] = 2
				,-- approved
				ApproverId = @ProcessedBy
				,ApprovedDate = @ProcessedDate
				,ApprovedStatus = 1
			FROM [OTMonthlyRequest] OT WITH (ROWLOCK, UPDLOCK)
				INNER JOIN #RequestID tmp ON OT.RequestID = tmp.RequestID

			EXEC [dbo].[Overtime_SET_OTSum] @EmpId = @sEmpId
				,@Month = @Month
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE OT
			SET [CurrentStatus] = CASE 
					WHEN OT.CurrentStatus = 7 THEN 1
					ELSE 4
					END
				,-- approved
				ApproverId = @ProcessedBy
				,HRDate = @ProcessedDate
				,HRStatus = 0
			FROM [OTMonthlyRequest] OT WITH (ROWLOCK, UPDLOCK)
				INNER JOIN #RequestID tmp ON OT.RequestID = tmp.RequestID

			EXEC [dbo].[Overtime_SET_OTSum] @EmpId = @sEmpId
				,@Month = @Month
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END

		COMMIT TRANSACTION AAA;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reset Status process failed'

		ROLLBACK TRANSACTION AAA;
	END CATCH;

	DROP TABLE #RequestID

	-- write log
	DECLARE @LogSPData VARCHAR(4000)
		,@now DATETIME
		,@Description VARCHAR(200)

	SET @now = getdate()

	--Log general info
	SELECT @LogSPData = '<OverTimeRequest_ResetRequest RequestID="' + coalesce(cast(@ListRequestID AS VARCHAR(1000)), space(1)) + '" '
		+ 'RoleID="' + coalesce(cast(@RoleID AS VARCHAR(100)), space(1)) + '" '
		+ 'ProcessedBy="' + coalesce(cast(@ProcessedBy AS VARCHAR(100)), space(1)) + '" '
		+ 'ProcessedDate="' + coalesce(convert(VARCHAR(10), @ProcessedDate, 111), space(1)) + '" '
		+ 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" '
		+ 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

	SELECT @Description = 'Reset OT Request:' + coalesce(cast(@ListRequestID AS VARCHAR(1000)), space(1))

	EXEC dbo.Log_WriteLog @LogActionID = 13
		,@LogDate = @now
		,@Description = @Description
		,@TrackingValue = @LogSPData
		,@UserID = @ProcessedBy
		,@EmpID = NULL
END