/****** Object:  Stored Procedure dbo.Department_Insert    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Insert    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[Department_Insert] 
	@Code VARCHAR(20)
	,@Name VARCHAR(100)
	,@IsActive BIT = 1
	,@ParentId INT = NULL
	,@ManagerId INT = NULL
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151222@Quinn.Vo
Task: Insert Department.
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
			EXISTS (
				SELECT 1
				FROM dbo.Department WITH (NOLOCK)
				WHERE Code = @Code
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Department code' + @Code + ' has already exists'

		RETURN
	END

	INSERT INTO dbo.Department(
		[Code]
		,[Name]
		,[ParentId]
		,[ManagerId]
		,[CreatedBy]
		,[CreatedDate]
		,[IsActive]
		)
	VALUES (
		@Code
		,@Name
		,@ParentId
		,@ManagerId
		,@CreatedBy
		,GETDATE()
		,1
		)

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert department failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END
