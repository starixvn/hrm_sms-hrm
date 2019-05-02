
CREATE PROCEDURE [dbo].[Employee_Shift_Update] @Empid INT
	,@monShift INT = NULL
	,@tueShift INT = NULL
	,@wedShift INT = NULL
	,@thuShift INT = NULL
	,@friShift INT = NULL
	,@satShift INT = NULL
	,@sunShift INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
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
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	UPDATE dbo.EmployeeDefaultShift WITH (ROWLOCK, UPDLOCK)
	SET MonShift = @monShift
		,TueShift = @tueShift
		,WedShift = @wedShift
		,ThuShift = @thuShift
		,FriShift = @friShift
		,SatShift = @satShift
		,SunShift = @sunShift
	WHERE EmpID = @Empid

	EXEC dbo.EmployeeCalendar_Update_byDefaultShift @EmpId = @Empid

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 102
			,@ErrMsg = N'Update Shift of Employee failed.'
	END
END
