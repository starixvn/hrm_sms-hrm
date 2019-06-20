/****** Object:  Stored Procedure dbo.WorkingTimeRequest_GetChangeShiftTypeDetailByRequestID    Script Date: 7/20/2016 10:19:21 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_GetChangeShiftTypeDetailByRequestID    Script Date: 7/19/2016 5:22:22 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetChangeShiftTypeDetailByRequestID] 
	@RequestID bigint
AS
/*
Created: Tom.Vo@2013-11-05
Task:Get Request ChangeShiftType Detail By RequestID
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT 
		rd.OnDate,
		rd.FromShift as FromShiftID, 
		s1.Code as FromShift,
		rd.ToShift as ToShiftID, 
		s2.Code as ToShift,
		rd.Remark
	FROM  dbo.ChangeShiftRequestDetail rd WITH (NOLOCK) 
		LEFT JOIN dbo.Shift s1 WITH (NOLOCK) ON s1.ShiftID= rd.FromShift
		LEFT JOIN dbo.Shift s2 WITH (NOLOCK) ON s2.ShiftID= rd.ToShift
	WHERE rd.RequestID = @RequestID
	ORDER BY rd.OnDate 
END


