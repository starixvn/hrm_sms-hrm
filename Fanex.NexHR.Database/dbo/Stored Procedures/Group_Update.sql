
CREATE PROCEDURE [dbo].[Group_Update] @TeamId INT
	,@Code VARCHAR(20)
	,@Name NVARCHAR(150)
	,@SectionId INT
	,@MailGroup VARCHAR(50) = ''
	,@HeadId INT
	,@ManagerIds VARCHAR(MAX) = ''
	,@LeaderIds VARCHAR(MAX) = ''
	,@SupervisorIds VARCHAR(MAX) = ''
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
		,@OldManagerIds VARCHAR(MAX) = ''
		,@OldLeaderIds VARCHAR(MAX)
		,@OldSupervisorIds VARCHAR(MAX)

	SELECT @OldHeadId = EmpId
	FROM dbo.GroupManager WITH (NOLOCK)
	WHERE GroupId = @TeamId
		AND RoleId = 1

	SELECT @OldManagerIds = COALESCE(@OldManagerIds + ',', '') + CAST(EmpId AS VARCHAR(10))
	FROM dbo.GroupManager WITH (NOLOCK)
	WHERE GroupId = @TeamId
		AND RoleId = 2

	SELECT @OldLeaderIds = COALESCE(@OldLeaderIds + ',', '') + CAST(EmpId AS VARCHAR(10))
	FROM dbo.GroupManager WITH (NOLOCK)
	WHERE GroupId = @TeamId
		AND RoleId = 3

	SELECT @OldSupervisorIds = COALESCE(@OldSupervisorIds + ',', '') + CAST(EmpId AS VARCHAR(10))
	FROM dbo.GroupManager WITH (NOLOCK)
	WHERE GroupId = @TeamId
		AND RoleId = 4

	SET @xml = (
			SELECT Code AS '@Code'
				,[Name] AS '@Name'
				,SectionId AS '@SectionId'
				,@OldHeadId AS '@HeadId'
				,@OldManagerIds AS '@ManagerIds'
				,@OldLeaderIds AS '@LeaderIds'
				,@OldSupervisorIds AS '@SupervisorIds'
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

	IF ISNULL(@ManagerIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupManager (
			GroupId
			,EmpId
			,RoleId
			)
		SELECT @TeamId
			,val
			,2
		FROM dbo.fnc_strSplit(@ManagerIds, ',')
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

	IF ISNULL(@SupervisorIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupManager (
			GroupId
			,EmpId
			,RoleId
			)
		SELECT @TeamId
			,val
			,4
		FROM dbo.fnc_strSplit(@SupervisorIds, ',')
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
				,@ManagerIds AS '@ManagerIds'
				,@LeaderIds AS '@LeaderIds'
				,@SupervisorIds AS '@SupervisorIds'
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
