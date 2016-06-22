
CREATE PROCEDURE [dbo].[Group_Insert] @Code VARCHAR(20)
	,@Name VARCHAR(50)
	,@DepartmentId INT
	,@MailGroup VARCHAR(50) = ''
	,@LeaderId INT
	,@AssistantLeaderId INT = NULL
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: INSERT group 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.[Group] WITH (NOLOCK)
			WHERE Code = @Code
				AND IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Code "' + @code + '" exists.'

		RETURN
	END

	INSERT INTO dbo.[Group] (
		Code
		,[Name]
		,DepartmentId
		,MailGroup
		,LeaderId
		,AssistantLeaderId
		,CreatedDate
		,CreatedBy
		)
	VALUES (
		@Code
		,@Name
		,@DepartmentId
		,@MailGroup
		,@LeaderId
		,@AssistantLeaderId
		,GETDATE()
		,@CreatedBy
		)

	SELECT CAST(SCOPE_IDENTITY() AS INT)

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
