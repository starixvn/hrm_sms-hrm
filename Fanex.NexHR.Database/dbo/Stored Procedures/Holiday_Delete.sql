CREATE PROCEDURE [dbo].[Holiday_Delete] @Id INT
	,@ModifiedBy INT
	,@ModifiedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: Delete Holidate by ID
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
DECLARE @Fromdate DATETIME
	,@todate DATETIME

SELECT @fromdate = fromdate
	,@todate = todate
FROM dbo.Holiday
WHERE Id = @id

IF (@fromdate < GETDATE())
BEGIN
	SELECT @ErrCode = 1
		,@ErrMsg = N'Can not delete Holiday in the Past.'
		,@ID = 0
END
ELSE
BEGIN
	UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
	SET IsHoliday = 0
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @ModifiedDate
	WHERE WorkDate BETWEEN @FromDate
			AND @ToDate

	DELETE
	FROM Holiday
	WHERE Id = @id

	IF @@ERROR <> 0
		SELECT @ErrCode = 1
			,@ErrMsg = N'Delete Holiday failed.'
	ELSE
		SELECT @ErrCode = 0
			,@ErrMsg = ''
END
