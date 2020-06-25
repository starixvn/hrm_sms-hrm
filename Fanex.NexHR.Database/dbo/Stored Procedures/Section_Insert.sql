
CREATE PROCEDURE [dbo].[Section_Insert] @Code VARCHAR(50)
	,@Name NVARCHAR(200)
	,@DepartmentId INT
	,@IsActive BIT = 1
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200520@Quinn.Vo
Task: Insert Section.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.Section WITH (NOLOCK)
			WHERE Code = @Code AND IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Section code ' + @Code + ' has already exists'

		RETURN
	END

	INSERT INTO dbo.Section(
		[Code]
		,[Name]
		,[DepartmentId]
		,[IsActive]
		,[ModifiedBy]
		,[ModifiedDate]
		,[IsDeleted]
		)
	VALUES (
		@Code
		,@Name
		,@DepartmentId
		,@IsActive
		,@ModifiedBy
		,GETDATE()
		,0
		)
END