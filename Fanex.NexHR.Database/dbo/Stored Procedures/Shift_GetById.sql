---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[Shift_GetById]
	@ShiftId INT
AS
/*
Created: 20140630@Quinn.Vo
Task: get Shift by id.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
SET NOCOUNT ON;

DECLARE @IsUsed BIT

IF(
	EXISTS(SELECT 1 FROM dbo.EmployeeCalendar WITH (NOLOCK) WHERE ShiftId = @ShiftId)
	OR
	EXISTS(SELECT 1 FROM dbo.EmployeeAttendance WITH (NOLOCK) WHERE ShiftId = @ShiftId)
	OR
	EXISTS(SELECT 1 FROM dbo.ChangeShiftRequestDetail WITH (NOLOCK) WHERE FromShift = @ShiftId OR ToShift = @ShiftId)
	OR
	EXISTS(SELECT 1 FROM dbo.EmployeeScheduleShiftMultiple WITH (NOLOCK) WHERE ShiftId = @ShiftId)
)
BEGIN
	SET @IsUsed = 1
END
ELSE
BEGIN
	SET @IsUsed = 0
END

SELECT [ShiftID] AS Id
      ,[Code]
      ,[Name]
      ,[BeginCheckInTime]
      ,[EndCheckInTime]
      ,[BeginCheckOutTime]
      ,[EndCheckOutTime]
      ,[BreakTimeMins]
      ,[OrderNo]
      ,[BeginBreakTime]
      ,[EndBreakTime]
      ,[CrossDay]
      ,[Color]
      ,[TotalHours]
      ,[IsInAfterDay]
      ,[IsOutAfterDay]
      ,[IsDisabled]
	  ,@IsUsed AS IsUsed
      ,[CreatedBy]
      ,[CreatedDate]
      ,[ModifiedBy]
      ,[ModifiedDate]
FROM [dbo].[Shift] WITH(NOLOCK)
WHERE ShiftID  = @ShiftId

END


