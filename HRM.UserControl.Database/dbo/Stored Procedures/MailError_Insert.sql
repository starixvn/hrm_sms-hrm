
CREATE PROCEDURE [dbo].[MailError_Insert] @MailTo NVARCHAR(250)
	,@Subject NVARCHAR(250)
	,@Body NVARCHAR(MAX)
	,@Message NVARCHAR(MAX) = ''
	,@StackTrace NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230714@Tim.Trinh
Task: insert mail error
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @MailId BIGINT
		,@now DATETIME = GETUTCDATE()

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.MailRecord WITH (NOLOCK)
			WHERE MailTo = @MailTo
				AND [Subject] = @Subject
				AND Body = @Body
			)
	BEGIN
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
			,0
			,0
			)

		SELECT @MailId = @@IDENTITY
	END
	ELSE
	BEGIN
		SELECT @MailId = MailId
		FROM dbo.MailRecord WITH (NOLOCK)
		WHERE MailTo = @MailTo
			AND [Subject] = @Subject
			AND Body = @Body
	END

	INSERT INTO dbo.MailError (
		MailId
		,[Message]
		,StackTrace
		,ErrorTime
		)
	VALUES (
		@MailId
		,@Message
		,@StackTrace
		,@now
		)

	UPDATE dbo.MailRecord WITH (ROWLOCK, UPDLOCK)
	SET SentFail = SentFail + 1
	WHERE MailId = @MailId

	IF @@Error <> 0
	BEGIN
		SELECT @ErrMsg = ERROR_MESSAGE()
			,@ErrCode = 101

		RETURN
	END
END