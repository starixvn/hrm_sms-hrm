
CREATE PROCEDURE [dbo].[Message_Update] @MessageId INT
	,@Subject VARCHAR(100)
	,@Contents NVARCHAR(MAX)
	,@Category TINYINT
	,@FromDate DATETIME = NULL
	,@ToDate DATETIME = NULL
	,@EmployeeIds VARCHAR(500) = ''
	,@GroupIds VARCHAR(500) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140714@Quinn.Vo
Task: Update Message.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF @FromDate IS NOT NULL
		AND @ToDate IS NOT NULL
		AND @FromDate > @ToDate
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The From Date must be less than To Date.'

		RETURN
	END

	DECLARE @tblEmployee TABLE (EmployeeId INT)
	DECLARE @tblGroup TABLE (GroupId INT)

	UPDATE dbo.[Message] WITH (ROWLOCK, UPDLOCK)
	SET [Subject] = @Subject
		,Contents = @Contents
		,Category = @Category
		,FromDate = @FromDate
		,ToDate = @ToDate
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE MessageId = @MessageId

	DELETE dbo.MessageGroup WITH (ROWLOCK)
	WHERE MessageId = @MessageId

	IF @GroupIds <> ''
	BEGIN
		INSERT INTO @tblGroup (GroupId)
		SELECT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')

		INSERT INTO dbo.MessageGroup (
			MessageId
			,GroupId
			,Email
			)
		SELECT @MessageId
			,g.GroupId
			,gr.MailGroup
		FROM @tblGroup AS g
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON g.GroupId = gr.GroupId
	END

	DELETE dbo.MessageEmployee WITH (ROWLOCK)
	WHERE MessageId = @MessageId

	IF @EmployeeIds <> ''
	BEGIN
		INSERT INTO @tblEmployee (EmployeeId)
		SELECT val
		FROM dbo.fnc_strSplit(@EmployeeIds, ',')

		INSERT INTO dbo.MessageEmployee (
			MessageId
			,EmployeeId
			,Email
			,IsDisabled
			,IsRead
			)
		SELECT @MessageId
			,e.EmployeeId
			,ee.Email
			,0
			,0
		FROM @tblEmployee AS e
			INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON e.EmployeeId = ee.EmpId
	END
END
