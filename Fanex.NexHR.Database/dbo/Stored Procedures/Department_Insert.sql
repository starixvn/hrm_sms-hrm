CREATE PROCEDURE [dbo].[Department_Insert] @Code VARCHAR(50)
	,@Name NVARCHAR(200)
	,@UnitId INT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200521@Lilac.Nguyen
Task: Insert Department
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
			,@ErrMsg = N'Department  ' + @Code + ' has already exists'

		RETURN
	END

	INSERT INTO dbo.Department(
		[Code]
		,[Name]
		,[UnitId]
		,[IsActive]
		,[ModifiedBy]
		,[ModifiedDate]
		)
	VALUES (
		@Code
		,@Name
		,@UnitId
		,@IsActive
		,@ModifiedBy
		,GETDATE()
		)
END