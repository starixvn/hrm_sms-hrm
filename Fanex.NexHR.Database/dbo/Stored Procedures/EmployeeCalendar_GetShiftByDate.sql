/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetShiftByDate    Script Date: 7/20/2016 10:19:05 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeCalendar_GetShiftByDate    Script Date: 7/19/2016 5:22:13 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[EmployeeCalendar_GetShiftByDate]
	@empId int,
	@date datetime
AS
/*
Created: Tom.Vo@2013-10-29
Task: GET Shift of Employee on date
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @ShiftId INT,@IsDefaultSchedule BIT
	 
	select top(1) @ShiftId  = ISNULL(ShiftId,0) from EmployeeCalendar WITH(NOLOCK)
	where DATEDIFF(day,WorkDate,@date)=0 AND EmpId = @empId

	IF @ShiftId IS NULL
		BEGIN
			select top(1) @IsDefaultSchedule  = ISNULL(IsDefaultSchedule,0) 
			from EmployeeSummary WITH(NOLOCK)
			where EmpId = @empId
			
			IF @IsDefaultSchedule =1 
				SELECT @ShiftId = CASE WHEN DATEPART(dw,@date) = 1 THEN SunShift 
										WHEN DATEPART(dw,@date) = 2 THEN MonShift 
										WHEN DATEPART(dw,@date) = 3 THEN TueShift 
										WHEN DATEPART(dw,@date) = 4 THEN WedShift 
										WHEN DATEPART(dw,@date) = 5 THEN ThuShift 
										WHEN DATEPART(dw,@date) = 6 THEN FriShift 
										WHEN DATEPART(dw,@date) = 7 THEN SatShift
									END
				from EmployeeDefaultShift WITH(NOLOCK)
				where EmpId = @empId
		END
		PRINT DATEPART(WEEK,@date)
		SELECT TOP 1 * from dbo.[Shift] WITH(NOLOCK)
		where ShiftId  = @ShiftId 

END



