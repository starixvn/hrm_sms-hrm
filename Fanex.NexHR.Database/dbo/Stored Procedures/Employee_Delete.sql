---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Employee_Delete] 
	@Empid int,
	@ResignedDate smalldatetime,

	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT
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

	IF (@ResignedDate is null)
	BEGIN
		SELECT @ErrCode = 100 , @ErrMsg=N'The user cannot be deleted without resigned date'
		RETURN
	END
	
	BEGIN TRY
		BEGIN TRANSACTION;
			UPDATE dbo.Employee WITH(ROWLOCK,UPDLOCK)
			SET ResignedDate = @ResignedDate,
				isDeleted=1,
				Enabled=0,
				ModifiedDate = GETDATE()
			WHERE  Empid=@Empid
    	
			-- delete unused calendar
			DELETE dbo.EmployeeCalendar WITH(ROWLOCK)
			WHERE WorkDate>=@ResignedDate 
				and empid=@Empid
		
			DELETE dbo.EmployeeAttendance WITH(ROWLOCK)
			WHERE AttendanceDate >= @ResignedDate				
				and empid=@Empid
	    COMMIT TRANSACTION;
		SELECT @ErrCode=0,@ErrCode=''
	END TRY
	BEGIN CATCH
				ROLLBACK TRANSACTION;
				SELECT @ErrCode = 100, @ErrMsg = N'delete an employee failed.'
	END CATCH;
END


