/****** Object:  Stored Procedure dbo.Shift_Get    Script Date: 7/20/2016 10:19:18 AM ******/
/****** Object:  Stored Procedure dbo.Shift_Get    Script Date: 7/19/2016 5:22:20 PM ******/

CREATE PROCEDURE [dbo].[Shift_Get] 
AS
/*
Created: 20141002@Quinn.Vo
Task: Get all shifts
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT ShiftId
	,Code
	,Name
	,BeginCheckInTime
	,EndCheckInTime
	,BeginCheckOutTime
	,EndCheckOutTime
	,IsInAfterDay
	,IsOutAfterDay
	,IsDisabled
	FROM dbo.[Shift] WITH (NOLOCK)
END
