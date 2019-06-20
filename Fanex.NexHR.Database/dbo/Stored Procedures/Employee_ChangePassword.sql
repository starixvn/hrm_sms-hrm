/****** Object:  Stored Procedure dbo.Employee_ChangePassword    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Employee_ChangePassword    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[Employee_ChangePassword] @empId INT
	,@password NVARCHAR(100)
	,@ModifiedBy INT
	,@PasswordReset BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
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

	UPDATE Employee WITH (UPDLOCK, ROWLOCK)
	SET Password = @password
		,PasswordReset = @PasswordReset
		,ModifiedDate = GETDATE()
	WHERE empId = @empId

	IF @@ERROR <> 0
		SELECT @ErrCode = 102
			,@ErrMsg = N'Update password Employee failed.'
	ELSE
		SELECT @ErrCode = 0
			,@ErrCode = ''

	DECLARE @Now DATETIME = GETDATE()

	SET @password = '<Password>' + @password + '<PasswordReset>' + CONVERT(VARCHAR(10), @PasswordReset)

	EXEC dbo.Log_WriteLog @LogActionID = 501
		,@LogDate = @Now
		,@Description = 'Employee change password.'
		,@TrackingValue = @password
		,@UserID = @ModifiedBy
		,@EmpId = @EmpId
END