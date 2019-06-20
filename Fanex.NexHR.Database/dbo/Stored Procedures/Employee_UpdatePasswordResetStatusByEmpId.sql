/****** Object:  Stored Procedure dbo.Employee_UpdatePasswordResetStatusByEmpId    Script Date: 7/20/2016 10:19:03 AM ******/
/****** Object:  Stored Procedure dbo.Employee_UpdatePasswordResetStatusByEmpId    Script Date: 7/19/2016 5:22:13 PM ******/

CREATE PROCEDURE [dbo].Employee_UpdatePasswordResetStatusByEmpId 
	@empId int,
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
	SET PasswordReset=@PasswordReset
	WHERE empId = @empId

	IF @@ERROR <> 0
		SELECT @ErrCode = 102, @ErrMsg = N'Update Password Reset Employee failed.'
	ELSE 
		SELECT @ErrCode=0,@ErrCode=''	
END