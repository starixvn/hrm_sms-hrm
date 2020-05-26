CREATE PROCEDURE [dbo].[Unit_Insert] @Code VARCHAR(20)
	,@Name VARCHAR(100)
	,@IsActive BIT = 1
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200521@Patsy.Doan
Task: Insert Unit.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.Unit WITH (NOLOCK)
			WHERE Code = @Code
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Unit code ' + @Code + ' has already exists'

		RETURN
	END

	INSERT INTO dbo.Unit(
		[Code]
		,[Name]
		,[ModifiedBy]
		,[ModifiedDate]
		,[IsActive]
		)
	VALUES (
		@Code
		,@Name
		,@CreatedBy
		,GETDATE()
		,@IsActive
		)
END



