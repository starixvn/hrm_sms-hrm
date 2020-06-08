CREATE PROCEDURE [dbo].[Section_Delete] @SectionId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200520@Quinn.Vo
Task: delete Section.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.Section WITH (NOLOCK)
			WHERE SectionId = @SectionId
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The section does not exists'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.Section AS sec WITH (NOLOCK)
				INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON sec.SectionId = g.SectionId
			WHERE sec.SectionId = @SectionId
				AND g.IsDeleted != 1
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The section exists has teams'

		RETURN
	END

    DELETE
	FROM dbo.Section
	WHERE SectionId = @SectionId
END