/****** Object:  Stored Procedure dbo.OverTime_RejectSingleRequest    Script Date: 7/20/2016 10:19:15 AM ******/
/****** Object:  Stored Procedure dbo.OverTime_RejectSingleRequest    Script Date: 7/19/2016 5:22:19 PM ******/

--DROP PROCEDURE [dbo].OverTime_RejectSingleRequest
CREATE PROCEDURE [dbo].[OverTime_RejectSingleRequest] @RequestID BIGINT
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@RoleID INT
	,@bOnbehalf BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-13
Task: Reject Overtime request form (type 8)
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

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

	-- get all request detail 
	DECLARE @EmpId INT
		,@RequestType SMALLINT
		,@Status TINYINT
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@TotalDay INT
	DECLARE @VerifierId INT
		,@ApproverId INT
		,@HRId INT
	DECLARE @VerifiedStatus INT
		,@ApprovedStatus INT
		,@HRStatus INT
	DECLARE @Month DATETIME

	-- get employee and requestype of the request		
	SELECT TOP 1 @EmpId = EmpId
		,@Status = [CurrentStatus]
		,@VerifierId = VerifierId
		,@VerifiedStatus = VerifiedStatus
		,@ApproverId = ApproverId
		,@ApprovedStatus = ApprovedStatus
		,@HRId = HRId
		,@HRStatus = HRStatus
		,@Month = [Month]
	FROM dbo.[OTMonthlyRequest] WITH (NOLOCK)
	WHERE RequestID = @RequestID

	IF @RoleID = 2
		AND (
			(
				@VerifierId <> @ProcessedBy
				AND @bOnbehalf = 1
				)
			OR @Status <> 1
			)
	BEGIN
		SET @ErrCode = 102
		SET @ErrMsg = 'This request is not pending so it cannot be rejected.'

		RETURN
	END
	ELSE IF @RoleID = 3
		AND (
			(
				@ApproverId <> @ProcessedBy
				AND @bOnbehalf = 1
				)
			OR @Status <> 2
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'This request is not pending so it cannot be rejected.'

		RETURN
	END
	ELSE IF @RoleID = 6
		AND @Status <> 4
	BEGIN
		SET @ErrCode = 104
		SET @ErrMsg = 'This request is not pending so it cannot be rejected.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION AAA;

		IF @RoleID = 2 --Verifier
		BEGIN
			UPDATE OT
			SET OT.[Status] = 3
			FROM [OTDailyRequest] OT WITH (ROWLOCK, UPDLOCK)
			WHERE RequestID = @RequestID

			UPDATE dbo.[OTMonthlyRequest] WITH (ROWLOCK, UPDLOCK)
			SET [CurrentStatus] = 3 -- approved
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,VerifiedStatus = 3
			WHERE RequestID = @RequestID
		END
		ELSE IF @RoleID = 3 --Approver
		BEGIN
			UPDATE OT
			SET OT.[Status] = 5
			FROM [OTDailyRequest] OT WITH (ROWLOCK, UPDLOCK)
			WHERE RequestID = @RequestID

			UPDATE dbo.[OTMonthlyRequest] WITH (ROWLOCK, UPDLOCK)
			SET [CurrentStatus] = 5 -- approved
				,ApproverId = @ProcessedBy
				,ApprovedDate = @ProcessedDate
				,ApprovedStatus = 3
			WHERE RequestID = @RequestID
		END
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE OT
			SET OT.[Status] = 7
			FROM [OTDailyRequest] OT WITH (ROWLOCK, UPDLOCK)
			WHERE RequestID = @RequestID

			UPDATE dbo.[OTMonthlyRequest] WITH (ROWLOCK, UPDLOCK)
			SET [CurrentStatus] = 7 -- approved
				,HRId = @ProcessedBy
				,HRDate = @ProcessedDate
				,HRStatus = 3
			WHERE RequestID = @RequestID

			EXEC [dbo].[Overtime_SET_OTSum] @EmpId = @EmpId
				,@Month = @Month
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END

		COMMIT TRANSACTION AAA;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Aprrove process failed'

		ROLLBACK TRANSACTION AAA;
	END CATCH;

	-- write log
	DECLARE @LogSPData VARCHAR(4000)
		,@now DATETIME
		,@Description VARCHAR(200)

	SET @now = getdate()

	--Log general info
	SELECT @LogSPData = '<OverTimeRequest_RejectRequest RequestID="' + coalesce(cast(@RequestID AS VARCHAR(1000)), space(1)) + '" ' + 'RoleID="' + coalesce(cast(@RoleID AS VARCHAR(100)), space(1)) + '" ' + 'ProcessedBy="' + coalesce(cast(@ProcessedBy AS VARCHAR(100)), space(1)) + '" ' + 'ProcessedDate="' + coalesce(convert(VARCHAR(10), @ProcessedDate, 111), space(1)) + '" ' + 'ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' + 'ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

	SELECT @Description = 'Reject OT Single Request:' + coalesce(cast(@RequestID AS VARCHAR(1000)), space(1))

	EXEC dbo.Log_WriteLog @LogActionID = 15
		,@LogDate = @now
		,@Description = @Description
		,@TrackingValue = @LogSPData
		,@UserID = @ProcessedBy
		,@EmpID = @EmpId
END