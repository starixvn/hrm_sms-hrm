

CREATE PROCEDURE [dbo].[JobLevel_Insert] @Name NVARCHAR(150)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210420@Lilac.Nguyen
Task: Insert Job Level.
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
				AND IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Job Level ' + @Name + ' has already exists'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.JobLevel WITH (NOLOCK)
			WHERE [Name] = @Name
				AND IsDeleted = 1
			)
	BEGIN
		UPDATE dbo.JobLevel WITH (ROWLOCK, UPDLOCK)
		SET IsDeleted = 0
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		WHERE [Name] = @Name
			AND IsDeleted = 1
	END
	ELSE
	BEGIN
		INSERT INTO dbo.JobLevel (
			[Name]
			,[IsDeleted]
			,[ModifiedBy]
			,[ModifiedDate]
			)
		VALUES (
			@Name
			,0
			,@ModifiedBy
			,GETDATE()
			)
	END
END