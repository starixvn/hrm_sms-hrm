
CREATE PROCEDURE [dbo].[Department_Insert] @Code VARCHAR(20)
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
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.Department WITH (NOLOCK)
			WHERE Code = @Code
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Department code ' + @Code + ' has already exists'

		RETURN
	END

	INSERT INTO dbo.Department (
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
END

