---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  
CREATE PROCEDURE [dbo].[EmployeeCalendar_Update]   
	@EmpId int,   
	@date datetime,  
	@ShiftID int,
	@ModifiedBy int,
	@ModifiedDate datetime,

	@ErrCode int = 0 OUTPUT,
	@ErrMsg varchar(200) = '' OUTPUT
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
 SET NOCOUNT ON;  
  
 DECLARE @fromdate datetime,  
   @todate datetime,  
   @dw int,  
   @isHoliday bit  
  
 set @date = convert(varchar(10),@date,111)  
  
 UPDATE dbo.EmployeeCalendar  
 SET @dw=datepart(dw, WorkDate),  
	@isHoliday =  case when EXISTS(select * from dbo.Holiday   
		 where WorkDate between FromDate and ToDate)  
	   THEN 1 ELSE 0 END,  
	IsHoliday = Case WHEN @ShiftID IS NULL THEN @isHoliday ELSE 0 end ,  
	ShiftId =  @ShiftID,  
	isOffDay = case when @ShiftID is null and @isHoliday=0 and LeaveTypeId is null then 1  
	  else 0 end,  
	IsLeaveDay = case when @ShiftId is null and LeaveTypeId is not null then 1  
	  else 0 end ,
	ModifiedBy = @ModifiedBy ,
	ModifiedDate = GETDATE()  
 WHERE datediff(d, WorkDate, @date) = 0   AND empId = @EmpId  
  
	-- update attendance record 
EXEC EmployeeAttendance_ProcessingAndInsert @empId = @empId, @fromtime=@date, @totime=@date, @onlyUpdate=1

IF @ErrCode <> 0
	SELECT @ErrCode = 100, @ErrMsg = N'working calendar for an employee failed.'
ELSE 
	SELECT @ErrCode=0,@ErrCode=''


