
CREATE PROCEDURE [dbo].[Employee_Delete] @Empid INT
	,@ActorId INT
	,@ResignedDate DATE
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: delete an employee
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	UPDATE dbo.Employee WITH (ROWLOCK, UPDLOCK)
	SET ResignedDate = @ResignedDate
		,isDeleted = 1
		,[Enabled] = 0
		,ModifiedBy = @ActorId
		,ModifiedDate = GETDATE()
	WHERE Empid = @Empid

	-- unused calendar
	DELETE dbo.EmployeeCalendar WITH (ROWLOCK)
	WHERE WorkDate >= @ResignedDate
		AND EmpId = @Empid

	DELETE dbo.EmployeeAttendance WITH (ROWLOCK)
	WHERE AttendanceDate >= @ResignedDate
		AND EmpId = @Empid

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'delete an employee failed.'
	END
END
