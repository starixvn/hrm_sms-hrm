CREATE PROCEDURE [dbo].[JobLevel_Delete] @JobLevelId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210420@Lilac.Nguyen
Task: Delete Job Level 
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.JobLevel AS j WITH (NOLOCK)
				INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON j.JobLevelId = emp.JobLevelId
			WHERE emp.isDeleted = 0
				AND emp.[Enabled] = 1
				AND j.JobLevelId = @JobLevelId
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'Delete unsuccessful, it has been already used.'

		RETURN
	END

	UPDATE dbo.JobLevel WITH (ROWLOCK, UPDLOCK)
	SET IsDeleted = 1
		,ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
	WHERE JobLevelId = @JobLevelId
END