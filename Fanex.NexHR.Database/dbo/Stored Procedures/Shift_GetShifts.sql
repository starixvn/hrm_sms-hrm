---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[Shift_GetShifts]
AS
/*
Created: Tom.Vo@2013-10-23
Task: retrieve data on shift
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
	  ,IsDisabled
FROM [dbo].[Shift] WITH(NOLOCK)
order by [Code]

END


