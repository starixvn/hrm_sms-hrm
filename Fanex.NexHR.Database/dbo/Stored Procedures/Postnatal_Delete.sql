/****** Object:  Stored Procedure dbo.Department_Insert    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Insert    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[Postnatal_Delete] 
	@PostnatalId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Delete Postnatal.
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
				FROM dbo.EmployeePostnatal WITH (NOLOCK)
				WHERE PostnatalId = @PostnatalId
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee does not exists'

		RETURN
	END

	DELETE FROM [dbo].[EmployeePostnatal]
	WHERE PostnatalId = @PostnatalId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert postnatal failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END