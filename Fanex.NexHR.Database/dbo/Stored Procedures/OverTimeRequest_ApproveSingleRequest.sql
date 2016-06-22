---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

--DROP PROCEDURE [dbo].[OverTimeRequest_ApproveRequest]

CREATE PROCEDURE [dbo].[OverTimeRequest_ApproveSingleRequest]
	@RequestID bigint,		
	@ProcessedBy int,
	@ProcessedDate datetime,
	@RoleID INT,
	@bOnbehalf bit=0,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-13
Task: approve Overtime request form (type 8)
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;

-- get all request detail 
DECLARE @EmpId int, @RequestType smallint, @Status tinyint, @FromDate datetime, @ToDate datetime,@TotalDay INT
DECLARE @VerifierId int,@ApproverId int,@HRId  int
DECLARE @VerifiedStatus	INT,@ApprovedStatus	INT,@HRStatus INT

-- get employee and requestype of the request		
SELECT TOP 1 @EmpId = EmpId			,@Status = [CurrentStatus]
		,@VerifierId = VerifierId	,@VerifiedStatus = VerifiedStatus
		,@ApproverId = ApproverId	,@ApprovedStatus = ApprovedStatus
		,@HRId = HRId				,@HRStatus = HRStatus
FROM dbo.WorkingTimeRequest WITH(NOLOCK)
WHERE RequestID = @RequestID

IF (@RoleID=2 and ((@VerifierId <> @ProcessedBy and @bOnbehalf=1) or (@Status <>1)))
BEGIN
	SET @ErrCode = 101
	IF  @Status <>1
		SET @ErrMsg = 'This request have been approved or rejected.'
	ELSE IF @VerifierId <> @ProcessedBy
		SET @ErrMsg = 'You don''t have permission on this request.'
	RETURN
END

IF (@RoleID=3 and ((@ApproverId <> @ProcessedBy and @bOnbehalf=1) or (@Status not in (1,2,3) )) )
BEGIN
	SET @ErrCode = 102
	IF  @Status not in (1,2,3)
		SET @ErrMsg = 'This request have been approved or rejected.'
	ELSE IF @ApproverId <> @ProcessedBy
		SET @ErrMsg = 'You don''t have permission on this request.'
	RETURN
END
	
--IF (@RoleID=6 and ((@HRId <> @ProcessedBy and @bOnbehalf=1) or (@Status not in (4) )))
IF (@RoleID=6 and ((@Status not in (4) )))
BEGIN
	SET @ErrCode = 103
	IF  @Status not in (4) 
		SET @ErrMsg = 'This request have been approved or rejected.'
	ELSE IF @HRId <> @ProcessedBy
		SET @ErrMsg = 'You don''t have permission on this request.'
	RETURN
END

BEGIN TRY
    BEGIN TRANSACTION AAA;

     IF @RoleID = 2  --Verifier
		BEGIN
			UPDATE dbo.[OTMonthlyRequest] WITH(ROWLOCK,UPDLOCK)
			SET [CurrentStatus] = 2, -- approved
				VerifierId= @ProcessedBy,
				VerifiedDate= @ProcessedDate,
				VerifiedStatus=2 
			WHERE RequestID = @RequestID

			UPDATE dbo.WorkingTimeRequest WITH(ROWLOCK,UPDLOCK)
			SET  [CurrentStatus] = 2,
				VerifierId= @ProcessedBy,
				VerifiedDate= @ProcessedDate,
				VerifiedStatus=2
			WHERE RequestID = @RequestID
		END
	ELSE IF @RoleID = 3 --Approver
		BEGIN
			UPDATE dbo.[OTMonthlyRequest] WITH(ROWLOCK,UPDLOCK)
			SET [CurrentStatus] = 4, -- approved
				ApproverId= @ProcessedBy,
				ApprovedDate= @ProcessedDate,
				ApprovedStatus=2 
			WHERE RequestID = @RequestID

			-- update status of request
			UPDATE dbo.WorkingTimeRequest WITH(ROWLOCK,UPDLOCK)
			SET [CurrentStatus] = 4, -- approved
				ApproverId= @ProcessedBy,
				ApprovedDate= @ProcessedDate,
				ApprovedStatus=2
				--ModifiedDate = @ApprovedDate,
				--ModifiedBy = @ApprovedBy
			WHERE RequestID = @RequestID
		END
	ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE dbo.[OTMonthlyRequest] WITH(ROWLOCK,UPDLOCK)
			SET [CurrentStatus] = 2, -- approved
				ApproverId= @ProcessedBy,
				HRDate= @ProcessedDate,
				HRStatus=2 
			WHERE RequestID = @RequestID

			UPDATE dbo.WorkingTimeRequest WITH(ROWLOCK,UPDLOCK)
			SET [CurrentStatus] = 6,
				HRId= @ProcessedBy,
				HRDate= @ProcessedDate,
				HRStatus=2
				--ModifiedDate = @ApprovedDate,
				--ModifiedBy = @ApprovedBy
			WHERE RequestID = @RequestID
		END
	COMMIT TRANSACTION AAA;
	SELECT @ErrCode=0;
END TRY
BEGIN CATCH
	SELECT @ErrCode=100,@ErrMsg = 'Aprrove process failed'
	ROLLBACK TRANSACTION AAA;
END CATCH;


