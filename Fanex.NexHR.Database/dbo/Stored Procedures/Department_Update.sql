
CREATE PROCEDURE [dbo].[Department_Update] @DepartmentId INT
	,@Code VARCHAR(20)
	,@Name VARCHAR(100)
	,@ParentId INT = NULL
	,@ManagerId INT = NULL
	,@IsActive BIT = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151222@Quinn.Vo
Task: Update Department.
DB: HRM
Original:

Revisions:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.Department WITH (NOLOCK)
			WHERE Code = @Code
				AND DepartmentId <> @DepartmentId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Department code ' + @Code + ' has already exists'

		RETURN
	END

	IF EXISTS (
		SELECT 1
		FROM dbo.Department WITH (NOLOCK)
		WHERE @IsActive = 0 -- Disable department
			AND IsActive != @IsActive
			AND DepartmentId = @DepartmentId
		)
	BEGIN
		IF EXISTS (
			SELECT 1
			FROM dbo.Department WITH (NOLOCK)
			WHERE [ParentId] = @DepartmentId
			)
		BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Cant disable the department having child departments'

		RETURN
		END

		IF EXISTS (
			SELECT 1
			FROM dbo.Department AS dep WITH (NOLOCK)
				INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON dep.DepartmentId = g.DepartmentId
			WHERE dep.DepartmentId = @DepartmentId
				AND g.IsDeleted != 1
			)
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Cant disable the department having teams'

			RETURN
		END
	END

	UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
	SET [Code] = @Code
		,[Name] = @Name
		,[ParentId] = @ParentId
		,[ManagerId] = @ManagerId
		,[IsActive] = @IsActive
		,[ModifiedBy] = @ModifiedBy
		,[ModifiedDate] = GETDATE()
	WHERE DepartmentId = @DepartmentId
END
