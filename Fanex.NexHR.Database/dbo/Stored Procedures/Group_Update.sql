
CREATE PROCEDURE [dbo].[Group_Update] @GroupId INT
	,@Code VARCHAR(20)
	,@Name NVARCHAR(150)
	,@DepartmentId INT
	,@MailGroup VARCHAR(50) = ''
	,@LeaderIds VARCHAR(MAX)
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

	IF EXISTS (
			SELECT 1
			FROM dbo.[Group] WITH (NOLOCK)
			WHERE GroupId <> @GroupId
				AND Code = @code
				AND IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Code "' + @code + '" have been used by other Group.'

		RETURN
	END

	DECLARE @xml XML
		,@logData NVARCHAR(MAX)
		,@logOldData NVARCHAR(MAX)
		,@OldLeaderIds VARCHAR(MAX)
		,@OldAssistantLeaderIds VARCHAR(MAX)

	SELECT @OldLeaderIds = COALESCE(@OldLeaderIds + ',', '') + CAST(EmployeeId AS VARCHAR(10))
	FROM dbo.GroupLeader WITH (NOLOCK)
	WHERE GroupId = @GroupId
		AND IsLeader = 1

	SELECT @OldAssistantLeaderIds = COALESCE(@OldAssistantLeaderIds + ',', '') + CAST(EmployeeId AS VARCHAR(10))
	FROM dbo.GroupLeader WITH (NOLOCK)
	WHERE GroupId = @GroupId
		AND ISNULL(IsLeader, 0) = 0

	SET @xml = (
			SELECT Code AS '@Code'
				,[Name] AS '@Name'
				,DepartmentId AS '@DepartmentId'
				,@OldLeaderIds AS '@LeaderIds'
				,@OldAssistantLeaderIds AS '@AssistantLeaderIds'
				,MailGroup AS '@MailGroup'
				,BasicAnnualLeaveBalance AS '@BasicAnnualLeaveBalance'
			FROM dbo.[Group] WITH (NOLOCK)
			WHERE GroupId = @GroupId
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	DELETE
	FROM dbo.GroupLeader WITH (ROWLOCK, UPDLOCK)
	WHERE GroupId = @GroupId

	IF ISNULL(@LeaderIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupLeader (
			GroupId
			,EmployeeId
			,IsLeader
			)
		SELECT @GroupId
			,val
			,1
		FROM fnc_strSplit(@LeaderIds, ',')
	END

	IF ISNULL(@AssistantLeaderIds, '') <> ''
	BEGIN
		INSERT INTO [dbo].[GroupLeader] (
			GroupId
			,EmployeeId
			,IsLeader
			)
		SELECT @GroupId
			,val
			,0
		FROM fnc_strSplit(@AssistantLeaderIds, ',')
	END

	UPDATE dbo.[Group] WITH (ROWLOCK, UPDLOCK)
	SET Code = @Code
		,[Name] = @Name
		,DepartmentId = @DepartmentId
		,[MailGroup] = @MailGroup
		,ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
		,BasicAnnualLeaveBalance = @BasicAnnualLeaveBalance
	WHERE GroupId = @GroupId

	SET @xml = (
			SELECT Code AS '@Code'
				,[Name] AS '@Name'
				,DepartmentId AS '@DepartmentId'
				,@LeaderIds AS '@LeaderIds'
				,@AssistantLeaderIds AS '@AssistantLeaderIds'
				,MailGroup AS '@MailGroup'
				,BasicAnnualLeaveBalance AS '@BasicAnnualLeaveBalance'
			FROM dbo.[Group] WITH (NOLOCK)
			WHERE GroupId = @GroupId
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.ActivityLog_Insert @UserId = @ModifiedBy
		,@ObjectTypeId = 2
		,@ObjectId = @GroupId
		,@OldData = @logOldData
		,@Data = @logData
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT
END
