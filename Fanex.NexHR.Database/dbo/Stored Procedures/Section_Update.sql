CREATE PROCEDURE [dbo].[Section_Update] @SectionId INT
	,@Code VARCHAR(20)
	,@Name VARCHAR(100)
	,@DepartmentId INT
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200520@Quinn.Vo
Task: Update Section.
DB: HRM
Original:

Revisions:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.Section WITH (NOLOCK)
			WHERE Code = @Code
				AND SectionId <> @SectionId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Section code ' + @Code + ' has already exists'

		RETURN
	END

	IF EXISTS (
		SELECT 1
		FROM dbo.Section WITH (NOLOCK)
		WHERE @IsActive = 0 -- Disable department
			AND IsActive != @IsActive
			AND SectionId = @SectionId
		)
	BEGIN

		IF EXISTS (
			SELECT 1
			FROM dbo.Section AS sec WITH (NOLOCK)
				INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON sec.SectionId = g.SectionId
			WHERE sec.SectionId = @SectionId
				AND g.IsDeleted != 1
			)
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Cant disable the section having teams'

			RETURN
		END
	END

	UPDATE dbo.Section WITH (ROWLOCK, UPDLOCK)
	SET [Code] = @Code
		,[Name] = @Name
		,[DepartmentId] = @DepartmentId
		,[IsActive] = @IsActive
		,[ModifiedBy] = @ModifiedBy
		,[ModifiedDate] = GETDATE()
	WHERE SectionId = @SectionId
END



