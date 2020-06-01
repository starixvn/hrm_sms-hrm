/****** Object:  Stored Procedure dbo.Message_Insert    Script Date: 7/20/2016 10:19:13 AM ******/
/****** Object:  Stored Procedure dbo.Message_Insert    Script Date: 7/19/2016 5:22:17 PM ******/

CREATE PROCEDURE [dbo].[Message_Insert] 
	@Subject VARCHAR(100)
	,@Contents NVARCHAR(MAX)
	,@Category TINYINT
	,@FromDate DATETIME = NULL
	,@ToDate DATETIME = NULL
	,@EmployeeIds VARCHAR(500) = ''
	,@TeamId VARCHAR(500) = ''
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20140714@Quinn.Vo
Task: Insert Message.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	IF(@FromDate IS NOT NULL AND @ToDate IS NOT NULL AND @FromDate > @ToDate)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'.The From Date must be less than To Date.'

		RETURN
	END

	DECLARE @MessageId INT
	DECLARE @Employees TABLE(
		EmployeeId INT
	)
	DECLARE @Groups TABLE(
		GroupId INT
	)

	INSERT INTO dbo.[Message] (
		[Subject]
		,[Contents]
		,[Category]
		,[FromDate]
		,[ToDate]
		,[IsDisabled]
		,[CreatedBy]
		,[CreatedDate]
		)
	VALUES (
		@Subject
		,@Contents
		,@Category
		,@FromDate
		,@ToDate
		,0
		,@CreatedBy
		,GETDATE()
		)

	SELECT @MessageId = SCOPE_IDENTITY();

	IF(@TeamId <> '')
	BEGIN
		INSERT INTO @Groups (GroupId)
		SELECT val FROM fnc_strSplit (@TeamId, ',')

		INSERT INTO dbo.MessageGroup(MessageId, GroupId, Email)
		SELECT @MessageId, g.GroupId, gr.MailGroup
		FROM @Groups AS g INNER JOIN
		dbo.[Group] AS gr ON g.GroupId = gr.GroupId
	END

	IF(@EmployeeIds <> '')
	BEGIN
		INSERT INTO @Employees (EmployeeId)
		SELECT val FROM fnc_strSplit (@EmployeeIds, ',')

		INSERT INTO dbo.MessageEmployee(MessageId, EmployeeId, Email, IsDisabled, IsRead)
		SELECT @MessageId, e.EmployeeId, ee.Email, 0, 0
		FROM @Employees AS e INNER JOIN
		dbo.Employee AS ee ON e.EmployeeId = ee.EmpId
	END

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert Message failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END
