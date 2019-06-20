/****** Object:  Stored Procedure dbo.Shift_GetShiftsByShiftID    Script Date: 7/20/2016 10:19:18 AM ******/
/****** Object:  Stored Procedure dbo.Shift_GetShiftsByShiftID    Script Date: 7/19/2016 5:22:20 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[Shift_GetShiftsByShiftID]
	@ShiftID INT
AS
/*
Created: Tom.Vo@2013-10-23
Task: retrieve data on shift by ShiftID
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
SET NOCOUNT ON;

SELECT [ShiftID]
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
	  ,CrossDay
	  ,Color
	  ,TotalHours
FROM [dbo].[Shift] WITH(NOLOCK)
WHERE ShiftID  = @ShiftID

END


