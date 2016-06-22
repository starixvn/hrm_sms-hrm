
CREATE PROCEDURE [dbo].[Employee_Shift_Update]
	@Empid int,
	@monShift int = null,
	@tueShift int = null,
	@wedShift int = null,
	@thuShift int = null,
	@friShift int = null,
	@satShift int = null,
	@sunShift int = null,
	
	@ErrCode int = 0 OUTPUT,
	@ErrMsg varchar(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: Update Shift of Employee
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	update EmployeeDefaultShift WITH(RowLock,UPDLock)
	SET MonShift=@monShift,
		TueShift=@tueShift,
		WedShift=@wedShift,
		ThuShift=@thuShift,
		FriShift=@friShift,
		SatShift=@satShift,
		SunShift=@sunShift
	where EmpID=@Empid

	exec EmployeeCalendar_Update_byDefaultShift @EmpId = @Empid

	IF @@ERROR <> 0
		SELECT @ErrCode = 102, @ErrMsg = N'Update Shift of Employee failed.'
	ELSE 
		SELECT @ErrCode=0,@ErrCode=''		
END
