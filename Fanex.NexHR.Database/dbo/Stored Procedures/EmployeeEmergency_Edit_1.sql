
/****** Object:  Stored Procedure dbo.Department_Update    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Update    Script Date: 7/19/2016 5:22:11 PM ******/
CREATE PROCEDURE [dbo].[EmployeeEmergency_Edit] @EmergencyId INT
	,@FullName NVARCHAR(200)
	,@Phone NVARCHAR(100)
	,@DateOfBirth DATE
	,@Gender BIT
	,@Relationship NVARCHAR(200) = ''
	,@Modifier INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Update emergency.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	IF (
			NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeEmergency WITH (NOLOCK)
				WHERE Id = @EmergencyId
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The emergency not found'

		RETURN
	END

	UPDATE dbo.EmployeeEmergency
	WITH (
			ROWLOCK
			,UPDLOCK
			)

	SET FullName = @FullName
		,Phone = @Phone
		,DateOfBirth = @DateOfBirth
		,Gender = @Gender
		,Relationship = @Relationship
		,ModifiedBy = @Modifier
		,ModifiedDate = GETDATE()
	WHERE Id = @EmergencyId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update emergency failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END