ALTER PROCEDURE WorkingTimeRequest_ApproveRequest
	@RequestID bigint,		
	@ApprovedBy int,
	@ApprovedDate datetime,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
Created: 2012/04/20@Dan
Task: approve request form (main SP)
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/

DECLARE @EmpId int, @RequestType smallint, @Status tinyint, @FromDate datetime, @ToDate datetime

-- get employee and requestype of the request		
SELECT @EmpId = EmpId, @RequestType = RequestType, @Status = [status], 
		@FromDate = FromDate, @ToDate = ToDate
FROM dbo.WorkingTimeRequest
WHERE Id = @RequestID

set @FromDate = convert(varchar(10),@FromDate,111) 
set @ToDate = convert(varchar(10),@ToDate,111) 

-- if status is not pending, show error
IF (@Status<>1)
BEGIN
	SET @ErrCode = 100
	SET @ErrMsg = 'This request is not pending so it cannot be approved'

	RETURN
END

IF (@RequestType IN (1,2,3,4,5)
	AND EXISTS(SELECT * FROM WorkingTimeRequest 
		WHERE empid= @EmpId 
			AND RequestType=@RequestType 
			and status = 2
			and FromDate = @FromDate	
	))
BEGIN
	SET @ErrCode = 101
	SET @ErrMsg = 'Another approved request in the same date has existed'

	RETURN
END	

-- if not exist calendar

IF (@RequestType IN (1,2,3,4,5)
	AND NOT EXISTS(SELECT *
				FROM dbo.EmployeeCalendar WITH (NOLOCK)
				WHERE empid=@empid
					AND workdate = @fromdate					
					)
	)
BEGIN
	SET @ErrCode = 102
	SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'
	RETURN
END
ELSE IF (@RequestType = 6)
BEGIN	
	DECLARE	@maxDate datetime, @minDate datetime, @result bit

	SELECT @maxDate=max(workdate),@minDate = min(workdate)
	FROM dbo.EmployeeCalendar WITH (NOLOCK)
	WHERE empid=@empid
		AND workdate BETWEEN @fromdate AND @todate	

	IF (@minDate is null or @maxDate is null 
		or @fromdate <> @minDate OR @todate <> @maxDate)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'

		RETURN
	END
END

-- call appropriate SP for each request form
IF (@RequestType=1)
BEGIN
	EXEC WorkingTimeRequest_ApproveRequest_1 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END
ELSE IF (@RequestType=2)
BEGIN
	EXEC WorkingTimeRequest_ApproveRequest_2 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END
ELSE IF (@RequestType=3)
BEGIN
	EXEC WorkingTimeRequest_ApproveRequest_3 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END
ELSE IF (@RequestType=4)
BEGIN
	EXEC WorkingTimeRequest_ApproveRequest_4 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END
ELSE IF (@RequestType=5)
BEGIN
	EXEC WorkingTimeRequest_ApproveRequest_5 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END
ELSE IF (@RequestType=6)
BEGIN
	EXEC WorkingTimeRequest_ApproveRequest_6 @RequestID, @EmpId, @ApprovedBy, @ApprovedDate
END

-- write to history
EXEC [WorkingTimeRequest_WriteHistory] @RequestID

-- write log
Declare @LogSPData varchar(4000), @now datetime
set @now=getdate()
--Log general info
Select @LogSPData = '<WorkingTimeRequest_ApproveRequest RequestID="'+ coalesce(cast(@RequestID as varchar(50)),space(1)) + '" ' +
'ApprovedBy="'+ coalesce(cast(@ApprovedBy as varchar(100)),space(1)) + '" ' +
'ApprovedDate="'+ coalesce(convert(varchar(10),@ApprovedDate, 111 ),space(1)) + '" ' +
'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'	

EXEC dbo.Log_WriteLog
	@LogActionID = 1,
	@LogDate = @now,
	@Description = 'Approve Request',
	@TrackingValue = @LogSPData,
	@UserID = @ApprovedBy

set @ErrCode = 0