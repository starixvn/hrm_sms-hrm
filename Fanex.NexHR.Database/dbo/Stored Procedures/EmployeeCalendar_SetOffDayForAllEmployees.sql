CREATE PROCEDURE [dbo].[EmployeeCalendar_SetOffDayForAllEmployees] @date DATETIME
	,@isOffDay BIT
	,@ModifiedBy INT
	,@ModifiedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-29
Task: Turn on/off the off day on working calendar for all employees. 
	It's used to set replacement off day for public holiday when public holiday is on the off day
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF (@isOffDay IS NULL)
		SET @isOffDay = 0

	-- if not exist calendar
	IF (
			NOT EXISTS (
				SELECT TOP 1 1
				FROM dbo.EmployeeCalendar WITH (NOLOCK)
				WHERE workdate = @date
				)
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'

		RETURN
	END

	-- cut off time
	SET @date = convert(VARCHAR(10), @date, 111)

	UPDATE dbo.EmployeeCalendar
	SET isOffDay = CASE 
			WHEN @isOffDay = 0
				AND ShiftID IS NULL
				AND isHoliday = 0
				AND isLeaveday = 0 THEN 1
			ELSE @isOffDay
			END
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @ModifiedDate
	WHERE WorkDate = @date

	-- update attendate record   
	EXEC EmployeeAttendance_ProcessingAndInsert @empId = NULL
		,@fromTime = @date
		,@toTime = @date
		,@onlyUpdate = 1

	-- write log
	DECLARE @LogSPData VARCHAR(4000)
		,@now DATETIME

	SET @now = getdate()

	--Log general info
	SELECT @LogSPData = '<EmployeeCalendar_SetOffDayForAllEmployees @date="' + coalesce(convert(VARCHAR(10), @date, 111), space(1)) + '" ' 
	   + 'isOffDay="' + coalesce(cast(@isOffDay AS VARCHAR(100)), space(1)) + '" ' 
	   + '@ModifiedBy="' + coalesce(cast(@ModifiedBy AS VARCHAR(100)), space(1)) + '" ' 
	   + '@ModifiedDate="' + coalesce(convert(VARCHAR(10), @ModifiedDate, 111), space(1)) 
	   + '@ErrCode="' + coalesce(cast(@ErrCode AS VARCHAR(50)), space(1)) + '" ' 
	   + '@ErrMsg="' + coalesce(cast(@ErrMsg AS VARCHAR(4000)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 4
		,@LogDate = @now
		,@Description = 'Set off day for all employees'
		,@TrackingValue = @LogSPData
		,@UserID = @ModifiedBy

	SET @ErrCode = 0
END