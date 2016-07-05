CREATE PROCEDURE [dbo].[EmployeeCalendar_Update] @EmpId INT
	,@date DATETIME
	,@ShiftID INT
	,@ModifiedBy INT
	,@ModifiedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*  
Created: Tom.Vo@2013-10-29
Task: Update working calendar for an employee in a day  
DB: HRM_SMS  
Original:   
  
Revisions:      
Param's Explanation:  
 @EmpId: employee ID  
 @date: the date to update  
 @ShiftID: shift id (null if not work)  
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromdate DATETIME
		,@todate DATETIME
		,@dw INT
		,@isHoliday BIT

	SET @date = convert(VARCHAR(10), @date, 111)

	UPDATE dbo.EmployeeCalendar
	SET @dw = datepart(dw, WorkDate)
		,@isHoliday = (CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.Holiday
					WHERE WorkDate BETWEEN FromDate
							AND ToDate
					) THEN 1
			ELSE 0 END
			)
		,IsHoliday = (CASE 
			WHEN @ShiftID IS NULL THEN @isHoliday
			ELSE 0 END
			)
		,ShiftId = @ShiftID
		,isOffDay = (CASE 
			WHEN @ShiftID IS NULL
				AND @isHoliday = 0
				AND LeaveTypeId IS NULL THEN 1
			ELSE 0 END
			)
		,IsLeaveDay = (CASE 
			WHEN @ShiftId IS NULL
				AND LeaveTypeId IS NOT NULL THEN 1
			ELSE 0 END
			)
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE datediff(d, WorkDate, @date) = 0
		AND empId = @EmpId

	-- update attendance record 
	EXEC EmployeeAttendance_ProcessingAndInsert @empId = @empId
		,@fromTime = @date
		,@toTime = @date
		,@onlyUpdate = 1

	IF @ErrCode <> 0
		SELECT @ErrCode = 100
			,@ErrMsg = N'working calendar for an employee failed.'
	ELSE
		SELECT @ErrCode = 0
			,@ErrCode = ''
END