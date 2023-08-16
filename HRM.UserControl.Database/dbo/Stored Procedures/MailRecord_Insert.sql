
CREATE PROCEDURE [dbo].[MailRecord_Insert] @MailTo NVARCHAR(50)
	,@Subject NVARCHAR(250)
	,@Body NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230714@Tim.Trinh
Task: insert mail record
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @now DATETIME = GETUTCDATE()

	INSERT INTO dbo.MailRecord (
		MailTo
		,[Subject]
		,Body
		,CreatedDate
		,LastSentTime
		,SentSuccess
		,SentFail
		)
	VALUES (
		@MailTo
		,@Subject
		,@Body
		,@now
		,@now
		,1
		,0
		)

	IF @@Error <> 0
	BEGIN
		SELECT @ErrMsg = ERROR_MESSAGE()
			,@ErrCode = 101

		RETURN
	END
END