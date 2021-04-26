CREATE PROCEDURE [dbo].[JobLevel_Insert]
	@Name nvarchar(150)
	,@IsActive BIT
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
			WHERE Name = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Job Level ' + @Name + ' has already exists'

		RETURN
	END

	INSERT INTO dbo.JobLevel(
		[Name]
		,[IsActive]
		,[ModifiedBy]
		,[ModifiedDate]

		)
	VALUES (
		@Name
		,@IsActive
		,@ModifiedBy
		,GETDATE()
		)
END