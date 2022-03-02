

CREATE PROCEDURE [dbo].[JobLevel_Update] @JobLevelId INT
	,@Name NVARCHAR(150)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210422@Lilac.Nguyen
Task: Update Job Level.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.JobLevel WITH (NOLOCK)
			WHERE [Name] = @Name
				AND JobLevelId <> @JobLevelId
				AND IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Job Level ' + @Name + ' has already exists'

		RETURN
	END

	UPDATE dbo.JobLevel WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @Name
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE JobLevelId = @JobLevelId
END