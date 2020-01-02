
CREATE PROCEDURE [dbo].[Group_Insert] @Code VARCHAR(20)
	,@Name VARCHAR(150)
	,@DepartmentId INT
	,@MailGroup VARCHAR(50) = ''
	,@LeaderIds VARCHAR(MAX)
	,@AssistantLeaderIds VARCHAR(MAX) = ''
	,@BasicAnnualLeaveBalance TINYINT
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190515@Oliver.Dang
Task: insert group 
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
		,CreatedDate
		,CreatedBy
		,BasicAnnualLeaveBalance
		)
	VALUES (
		@Code
		,@Name
		,@DepartmentId
		,@MailGroup
		,GETDATE()
		,@CreatedBy
		,@BasicAnnualLeaveBalance
		)

	DECLARE @id INT = cast(SCOPE_IDENTITY() AS INT)

	IF ISNULL(@LeaderIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupLeader (
			GroupId
			,EmployeeId
			,IsLeader
			)
		SELECT @id
			,val
			,1
		FROM dbo.fnc_strSplit(@LeaderIds, ',')
	END

	IF ISNULL(@AssistantLeaderIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupLeader (
			GroupId
			,EmployeeId
			,IsLeader
			)
		SELECT @id
			,val
			,0
		FROM dbo.fnc_strSplit(@AssistantLeaderIds, ',')
	END
	
	DECLARE @xml XML
		,@logData NVARCHAR(MAX)
	
	SET @xml = (
			SELECT Code AS '@Code'
				,[Name] AS '@Name'
				,DepartmentId AS '@DepartmentId'
				,@LeaderIds AS '@LeaderIds'
				,@AssistantLeaderIds AS '@AssistantLeaderIds'
				,MailGroup AS '@MailGroup'
				,BasicAnnualLeaveBalance AS '@BasicAnnualLeaveBalance'
			FROM dbo.[Group] WITH (NOLOCK)
			WHERE GroupId = @id
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	
	EXEC dbo.ActivityLog_Insert @UserId = @CreatedBy
		,@ObjectTypeId = 2
		,@ObjectId = @id
		,@OldData = NULL
		,@Data = @logData
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT
END
