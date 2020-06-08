
CREATE PROCEDURE [dbo].[Group_Update] @TeamId INT
	,@Code VARCHAR(20)
	,@Name NVARCHAR(150)
	,@SectionId INT
	,@MailGroup VARCHAR(50) = ''
	,@HeadId INT
	,@AssistantHeadIds VARCHAR(MAX) = ''
	,@LeaderIds VARCHAR(MAX) = ''
	,@AssistantLeaderIds VARCHAR(MAX) = ''
	,@BasicAnnualLeaveBalance TINYINT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: Update group 
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.[Group] WITH (NOLOCK)
			WHERE GroupId <> @TeamId
				AND Code = @code
				AND IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Code "' + @code + '" have been used by other group.'

		RETURN
	END

	DECLARE @xml XML
		,@logData NVARCHAR(MAX)
		,@logOldData NVARCHAR(MAX)
		,@OldHeadId INT
		,@OldAssistantHeadIds VARCHAR(MAX) = ''
		,@OldLeaderIds VARCHAR(MAX)
		,@OldAssistantLeaderIds VARCHAR(MAX)

	SELECT @OldHeadId = EmpId
	FROM dbo.GroupManager WITH (NOLOCK)
	WHERE GroupId = @TeamId
		AND RoleId = 1

	SELECT @OldAssistantHeadIds = COALESCE(@OldAssistantHeadIds + ',', '') + CAST(EmpId AS VARCHAR(10))
	FROM dbo.GroupManager WITH (NOLOCK)
	WHERE GroupId = @TeamId
		AND RoleId = 2

	SELECT @OldLeaderIds = COALESCE(@OldLeaderIds + ',', '') + CAST(EmpId AS VARCHAR(10))
	FROM dbo.GroupManager WITH (NOLOCK)
	WHERE GroupId = @TeamId
		AND RoleId = 3

	SELECT @OldAssistantLeaderIds = COALESCE(@OldAssistantLeaderIds + ',', '') + CAST(EmpId AS VARCHAR(10))
	FROM dbo.GroupManager WITH (NOLOCK)
	WHERE GroupId = @TeamId
		AND RoleId = 4

	SET @xml = (
			SELECT Code AS '@Code'
				,[Name] AS '@Name'
				,SectionId AS '@SectionId'
				,@OldHeadId AS '@HeadId'
				,@OldAssistantHeadIds AS '@AssistantHeadIds'
				,@OldLeaderIds AS '@LeaderIds'
				,@OldAssistantLeaderIds AS '@AssistantLeaderIds'
				,MailGroup AS '@MailGroup'
				,BasicAnnualLeaveBalance AS '@BasicAnnualLeaveBalance'
			FROM dbo.[Group] WITH (NOLOCK)
			WHERE GroupId = @TeamId
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	DELETE
	FROM dbo.GroupManager WITH (ROWLOCK, UPDLOCK)
	WHERE GroupId = @TeamId

	INSERT INTO dbo.GroupManager (
		GroupId
		,EmpId
		,RoleId
		)
	VALUES (
		@TeamId
		,@HeadId
		,1
		)

	IF ISNULL(@AssistantHeadIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupManager (
			GroupId
			,EmpId
			,RoleId
			)
		SELECT @TeamId
			,val
			,2
		FROM dbo.fnc_strSplit(@AssistantHeadIds, ',')
	END

	IF ISNULL(@LeaderIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupManager (
			GroupId
			,EmpId
			,RoleId
			)
		SELECT @TeamId
			,val
			,3
		FROM dbo.fnc_strSplit(@LeaderIds, ',')
	END

	IF ISNULL(@AssistantLeaderIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupManager (
			GroupId
			,EmpId
			,RoleId
			)
		SELECT @TeamId
			,val
			,4
		FROM dbo.fnc_strSplit(@AssistantLeaderIds, ',')
	END

	UPDATE dbo.[Group] WITH (ROWLOCK, UPDLOCK)
	SET Code = @Code
		,[Name] = @Name
		,SectionId = @SectionId
		,[MailGroup] = @MailGroup
		,ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
		,BasicAnnualLeaveBalance = @BasicAnnualLeaveBalance
	WHERE GroupId = @TeamId

	SET @xml = (
			SELECT Code AS '@Code'
				,[Name] AS '@Name'
				,SectionId AS '@SectionId'
				,@HeadId AS '@HeadId'
				,@AssistantHeadIds AS '@AssistantHeadIds'
				,@LeaderIds AS '@LeaderIds'
				,@AssistantLeaderIds AS '@AssistantLeaderIds'
				,MailGroup AS '@MailGroup'
				,BasicAnnualLeaveBalance AS '@BasicAnnualLeaveBalance'
			FROM dbo.[Group] WITH (NOLOCK)
			WHERE GroupId = @TeamId
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.ActivityLog_Insert @UserId = @ModifiedBy
		,@ObjectTypeId = 2
		,@ObjectId = @TeamId
		,@OldData = @logOldData
		,@Data = @logData
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT
END
