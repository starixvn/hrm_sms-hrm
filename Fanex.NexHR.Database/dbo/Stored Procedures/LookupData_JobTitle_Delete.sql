
CREATE PROCEDURE [dbo].[LookupData_JobTitle_Delete] @Id INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-29
Task: Delete Job Title
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee WITH (NOLOCK)
			WHERE JobTitleId = @Id
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = 'Delete unsuccessful, it has been already used.'

		RETURN
	END

	DELETE dbo.LookupData WITH (ROWLOCK, UPDLOCK)
	WHERE Id = @Id
END




