
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--DROP PROCEDURE [dbo].OverTimeRequest_RejectMonthlySingleRequest
CREATE PROCEDURE [dbo].[OverTime_RejectMonthlySingleRequest] @RequestID BIGINT
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

	-- get employee and requestype of the request		
	SELECT TOP 1 @EmpId = EmpId
		,@Status = [CurrentStatus]
		,@VerifierId = VerifierId
		,@VerifiedStatus = VerifiedStatus
		,@ApproverId = ApproverId
		,@ApprovedStatus = ApprovedStatus
		,@HRId = HRId
		,@HRStatus = HRStatus
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestID = @RequestID

	IF (
			@RoleID = 2
			AND (
				(
					@VerifierId <> @ProcessedBy
					AND @bOnbehalf = 1
					)
				OR (@Status < 4)
				)
			)
	BEGIN
		SET @ErrCode = 101

		IF @Status < 4
			SET @ErrMsg = 'This request have been approved or rejected.'
		ELSE IF @VerifierId <> @ProcessedBy
			SET @ErrMsg = 'You don''t have permission on this request.'

		RETURN
	END

	IF (
			@RoleID = 3
			AND (
				(
					@ApproverId <> @ProcessedBy
					AND @bOnbehalf = 1
					)
				OR (@Status < 6)
				)
			)
	BEGIN
		SET @ErrCode = 102

		IF @Status < 6
			SET @ErrMsg = 'This request have been approved or rejected.'
		ELSE IF @ApproverId <> @ProcessedBy
			SET @ErrMsg = 'You don''t have permission on this request.'

		RETURN
	END

	--IF (@RoleID=6 and ((@HRId <> @ProcessedBy and @bOnbehalf=1) or (@Status not in (4) )))
	IF (
			@RoleID = 6
			AND ((@Status NOT IN (4)))
			)
	BEGIN
		SET @ErrCode = 103

		IF @Status NOT IN (4)
			SET @ErrMsg = 'This request have been approved or rejected.'
		ELSE IF @HRId <> @ProcessedBy
			SET @ErrMsg = 'You don''t have permission on this request.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION AAA;

		IF @RoleID = 2 --Verifier
		BEGIN
			UPDATE dbo.[OTMonthlyRequest] WITH (ROWLOCK, UPDLOCK)
			SET [CurrentStatus] = 3
				,-- approved
				VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,VerifiedStatus = 3
			WHERE RequestID = @RequestID
		END
		ELSE IF @RoleID = 3 --Approver
		BEGIN
			UPDATE dbo.[OTMonthlyRequest] WITH (ROWLOCK, UPDLOCK)
			SET [CurrentStatus] = 5
				,-- approved
				ApproverId = @ProcessedBy
				,ApprovedDate = @ProcessedDate
				,ApprovedStatus = 3
			WHERE RequestID = @RequestID
		END
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE dbo.[OTMonthlyRequest] WITH (ROWLOCK, UPDLOCK)
			SET [CurrentStatus] = 7
				,-- approved
				ApproverId = @ProcessedBy
				,HRDate = @ProcessedDate
				,HRStatus = 3
			WHERE RequestID = @RequestID
		END

		COMMIT TRANSACTION AAA;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Reject process failed'

		ROLLBACK TRANSACTION AAA;
	END CATCH;
END