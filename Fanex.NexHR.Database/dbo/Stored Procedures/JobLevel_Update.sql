
CREATE PROCEDURE [dbo].[JobLevel_Update]
	@JobLevelId INT
	,@Name nvarchar(150)
	,@IsActive BIT
	,@CreatedBy INT
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
			WHERE Name = @Name
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The Job Level has already exists'

		RETURN
	END


	UPDATE dbo.JobLevel WITH (ROWLOCK, UPDLOCK)
	SET Name = @Name
		,IsActive = @IsActive
		,CreatedBy = @CreatedBy
		,CreatedDate = GETDATE()
	WHERE JobLevelId = @JobLevelId
END