CREATE PROCEDURE [dbo].[Unit_Update] @UnitId INT
	,@Code VARCHAR(20)
	,@Name VARCHAR(100)
	,@IsActive BIT = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200521@Patsy.Doan
Task: Update Unit.
DB: HRM
Original:

Revisions:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.Unit WITH (NOLOCK)
			WHERE Code = @Code
				AND UnitId <> @UnitId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Unit code ' + @Code + ' has already exists'

		RETURN
	END

	UPDATE dbo.Unit WITH (ROWLOCK, UPDLOCK)
		SET [Code] = @Code
			,[Name] = @Name
			,[IsActive] = @IsActive
			,[ModifiedBy] = @ModifiedBy
			,[ModifiedDate] = GETDATE()
		WHERE UnitId = @UnitId
	
END
