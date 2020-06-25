CREATE PROCEDURE [dbo].[Department_Update] @DepartmentId INT
	,@Code VARCHAR(20)
	,@Name VARCHAR(100)
	,@UnitId INT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200521@Lilac.Nguyen
Task: Update Department
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
				AND UnitId = @UnitId
				AND DepartmentId <> @DepartmentId AND IsDeleted  = 0
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Department code ' + @Code + ' has already exists'

		RETURN
	END

	UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
	SET [Code] = @Code
		,[Name] = @Name
		,[UnitId] = @UnitId
		,[IsActive] = @IsActive
		,[ModifiedBy] = @ModifiedBy
		,[ModifiedDate] = GETDATE()
	WHERE DepartmentId = @DepartmentId
END