/****** Object:  Stored Procedure dbo.WorkingTimeRequest_GetLeaveRequestDetailByRequestID    Script Date: 7/20/2016 10:19:21 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_GetLeaveRequestDetailByRequestID    Script Date: 7/19/2016 5:22:22 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetLeaveRequestDetailByRequestID] 
	@RequestID bigint
AS
/*
Created: Tom.Vo@2013-11-05
Task:Get Leave Request Detail By RequestID
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT
		rd.OnDate,
		s.ShiftID as ShiftID,
		s.Code as ShiftCode,
		s.BeginCheckInTime,
		s.EndCheckOutTime,
		rd.WorkType,
		rd.[LeaveType] as [LeaveType],
		lt.[Code] as [LeaveCode],
		lt.Name as [LeaveName],
		rd.[IsHalfDay] as [IsHalfDay],
		rd.[FromTime],
		rd.[ToTime],
		lt.IsAnnualQuota,
		lt.IsDefault,
		lt.IsSalaryDeduction,
		lt.IsHoliday,
		lt.isShown,
		lt.IsLeaveDuringWorking,
		lt.IsCalculateWoking
	FROM  dbo.[LeaveRequestDetail] rd WITH (NOLOCK) 
		LEFT JOIN [dbo].[Shift] s WITH (NOLOCK) ON s.ShiftID= rd.[ShiftId]
		LEFT JOIN [dbo].[LeaveType] lt WITH (NOLOCK)  ON lt.[LeaveTypeId]= rd.[LeaveType]
	WHERE rd.RequestID = @RequestID
	ORDER BY rd.OnDate 
END


