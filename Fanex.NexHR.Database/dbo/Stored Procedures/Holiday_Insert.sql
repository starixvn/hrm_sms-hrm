
CREATE PROCEDURE [dbo].[Holiday_Insert] @HolidayName NVARCHAR(50)
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@ModifiedBy INT
	,@ModifiedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: Insert Holidate
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
DECLARE @id INT

IF (@fromdate < GETDATE())
BEGIN
	SELECT @ErrCode = 1
		,@ErrMsg = N'Can not create Holiday in the Past.'
		,@ID = 0
END
ELSE
BEGIN
	INSERT INTO [dbo].[Holiday] (
		HolidayName
		,FromDate
		,ToDate
		)
	VALUES (
		@HolidayName
		,@FromDate
		,@ToDate
		)

	UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
	SET IsHoliday = 1
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @ModifiedDate
	WHERE WorkDate BETWEEN @FromDate
			AND @ToDate

	IF @@ERROR <> 0
	BEGIN
		SET @ID = 0
		SELECT @ErrCode = 1
			,@ErrMsg = N'Create Holiday failed.'
	END
	ELSE
	BEGIN
		SET @id = cast(SCOPE_IDENTITY() AS INT)

		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END

SELECT @id