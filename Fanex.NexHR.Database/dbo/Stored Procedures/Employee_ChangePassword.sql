CREATE PROCEDURE [dbo].[Employee_ChangePassword] 
	@empId int,
	@password nvarchar(100),
	@ModifiedBy INT,
	@PasswordReset BIT=0,

	@ErrCode int = 0 OUTPUT,
	@ErrMsg varchar(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23 
Task: Update passowrd Employee
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	UPDATE Employee WITH (UPDLOCK,ROWLOCK)
	SET Password = @password,
		PasswordReset=@PasswordReset,
		ModifiedDate = GETDATE()
	WHERE empId = @empId

	IF @@ERROR <> 0
		SELECT @ErrCode = 102, @ErrMsg = N'Update password Employee failed.'
	ELSE 
		SELECT @ErrCode=0,@ErrCode=''	

	DECLARE @Now DATETIME=GETDATE()
	SET @password = '<Password>' + @password + '<PasswordReset>' + CONVERT(VARCHAR(10),@PasswordReset)
	EXEC dbo.Log_WriteLog
		@LogActionID = 501,
		@LogDate = @Now,
		@Description = 'Employee change password.',
		@TrackingValue = @password,
		@UserID = @ModifiedBy,
		@EmpId=@EmpId

END

