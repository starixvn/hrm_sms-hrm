/****** Object:  Stored Procedure dbo.Department_Insert    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Insert    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[Paternity_Delete] 
	@PaternityId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Delete Paternity.
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
				FROM dbo.EmployeePaternity WITH (NOLOCK)
				WHERE PaternityId = @PaternityId
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The paternity of this employee does not exists'

		RETURN
	END

	DELETE FROM [dbo].[EmployeePaternity]
	WHERE @PaternityId = @PaternityId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert paternity failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END