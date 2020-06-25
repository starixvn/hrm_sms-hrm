CREATE PROCEDURE [dbo].[Section_Delete] @SectionId INT
	,@ModifiedBy INT
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

	IF EXISTS (
			SELECT 1
			FROM dbo.Section AS sec WITH (NOLOCK)
				INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON sec.SectionId = g.SectionId
			WHERE sec.SectionId = @SectionId
				AND g.IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The section exists has teams'

		RETURN
	END

	UPDATE dbo.[Section] WITH (ROWLOCK, UPDLOCK)
	SET IsDeleted = 1
		,ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
	WHERE SectionId = @SectionId
END