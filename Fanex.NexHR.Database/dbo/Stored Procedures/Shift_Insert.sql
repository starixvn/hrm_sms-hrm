
CREATE PROCEDURE [dbo].[Shift_Insert] 
	@Name VARCHAR(20)
	,@CheckInTime VARCHAR(5)
	,@CheckOutTime VARCHAR(5)
	,@IsInAfterDay BIT
	,@IsOutAfterDay BIT
	,@CreatedBy INT
	,@CreatedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140630@Quinn.Vo
Task: Insert Shift.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @FromDate DATETIME,
		@ToDate DATETIME,
		@TotalHours DECIMAL(10,2),
		@IsCrossDay BIT

	SET @FromDate = CAST((CONVERT(VARCHAR(10), GETDATE(), 111) + ' ' + @CheckInTime) AS DATETIME)
	SET @ToDate = CAST((CONVERT(VARCHAR(10), GETDATE(), 111) + ' ' + @CheckOutTime) AS DATETIME)

	IF(@IsInAfterDay = 1)
	BEGIN
		SET @FromDate = DATEADD(d, 1, @FromDate)
	END

	IF(@IsOutAfterDay = 1)
	BEGIN
		SET @ToDate = DATEADD(d, 1, @ToDate)
	END

	IF(@IsInAfterDay = 0 AND @IsOutAfterDay = 1)
	BEGIN
		SET @IsCrossDay = 1
	END
	ELSE
	BEGIN
		SET @IsCrossDay = 0
	END

	SET @TotalHours = DATEDIFF(MI, @FromDate, @ToDate) / 60.0

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.[Shift] WITH (NOLOCK)
				WHERE Name = @Name
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Shift ' + @Name + ' has already exists'

		RETURN
	END

	IF (@FromDate >= @ToDate)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Time in & time out is invalid.'

		RETURN
	END

	INSERT INTO dbo.[Shift] (
		[Code]
      ,[Name]
      ,[BeginCheckInTime]
      ,[EndCheckInTime]
      ,[BeginCheckOutTime]
      ,[EndCheckOutTime]
      ,[BreakTimeMins]
      ,[OrderNo]
      ,[CrossDay]
      ,[TotalHours]
      ,[IsInAfterDay]
      ,[IsOutAfterDay]
	  ,IsDisabled
	  ,CreatedBy
	  ,CreatedDate
		)
	VALUES (
		@Name
		,@Name
		,@CheckInTime
		,@CheckInTime
		,@CheckOutTime
		,@CheckOutTime
		,0
		,100
		,@IsCrossDay
		,@TotalHours
		,@IsInAfterDay
		,@IsOutAfterDay
		,0
		,@CreatedBy
		,@CreatedDate
		)

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert Shift failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END
