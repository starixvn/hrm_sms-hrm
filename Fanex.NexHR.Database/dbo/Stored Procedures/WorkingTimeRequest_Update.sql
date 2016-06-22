---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[WorkingTimeRequest_Update]
	@RequestId bigint,
	@EmpId int,
	@RequestType INT=NULL,
	@IsPlanned bit=NULL,
	@Remark nvarchar(2000)=NULL,
	@FromDate datetime=NULL,
	@FromShift int=NULL,
	@ToDate datetime=NULL,
	@ToShift int=NULL,
	@LeaveTypeID int=NULL,
	@TotalDays numeric(5,1)=NULL, 
	@LateInMins int=NULL,
	@EarlyOutMins int=NULL,
	@OvertimeType smallint=NULL,
	@IsOvertimeHours bit=NULL,
	@ModifiedDate datetime=NULL,
	@ModifiedBy int=NULL,
	@RefNo nvarchar(200)='',

	@VerifierId  int=NULL,
    @ApproverId  int=NULL,
    @HRId  int=NULL,

	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT	
AS
/*
Created: Tom.Vo@2013-11-04
Task: update a request form
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

SET NOCOUNT ON

IF (@RequestType IN (1,2) AND @FromDate IS NULL)
BEGIN
	SET @ErrCode =101
	SET @ErrMsg = 'Missing check in/out request has the invalid parameter'
	RETURN;
END

IF (@RequestType IN (3) AND (@FromDate IS NULL OR isnull(@LateInMins,0) <= 0))
BEGIN
	SET @ErrCode =102
	SET @ErrMsg = 'Late in request has the invalid parameter'
	RETURN;
END

IF (@RequestType IN (4) AND (@FromDate IS NULL OR isnull(@EarlyOutMins,0) <= 0))
BEGIN
	SET @ErrCode =103
	SET @ErrMsg = 'Late out request has the invalid parameter'
	RETURN;
END

IF (@RequestType IN (5) AND 
	 (@FromDate IS NULL OR @FromShift IS NULL OR @ToShift IS NULL)
	)
BEGIN
	SET @ErrCode =104
	SET @ErrMsg = 'Change-shift request has the invalid parameter'
	RETURN;
END

IF (@RequestType IN (6) AND 
	(@FromDate IS NULL OR @ToDate IS NULL OR @LeaveTypeID IS NULL 
		OR ISNULL(@TotalDays,0) < 0 )
	)
BEGIN
	SET @ErrCode =105
	SET @ErrMsg = 'Leave request has the invalid parameter'
	RETURN;
END

UPDATE WorkingTimeRequest
SET 	
		Remark = @Remark,
		VerifierId = @VerifierId,
		ApproverId = @ApproverId,
		HRId = @HRId
WHERE RequestId = @RequestId

IF (@RequestType IN (1,2) )
	BEGIN
		UPDATE [dbo].[LeaveRequest] WITH(ROWLOCK,UPDLOCK)
		SET [EmpId] = @EmpId
			,[FromDate]=@FromDate
			,[ToDate]=@ToDate
			,[LeaveType]=@RequestType
			,[TotalDays]=1
			,[RefNo] = @RefNo
		WHERE RequestId = @RequestId

	END

IF (@RequestType IN (3,4) )
	BEGIN
		UPDATE [dbo].[InOutRequest]
		SET			   
			[EmpId] = @EmpId
			,[LateInMins]=@LateInMins
			,[EarlyOutMins]=@EarlyOutMins
			,[FromDate]=@FromDate
			,[ToDate]=@ToDate
			,[RefNo]=@RefNo
		WHERE [RequestId] = @RequestId
	END



