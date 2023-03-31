
CREATE PROCEDURE [dbo].[Group_Insert] @Code VARCHAR(20)
	,@Name VARCHAR(150)
	,@SectionId INT
	,@MailGroup VARCHAR(50) = ''
	,@HeadId INT
	,@ManagerIds VARCHAR(MAX) = ''
	,@LeaderIds VARCHAR(MAX) = ''
	,@SupervisorIds VARCHAR(MAX) = ''
	,@LocalBasicAnnualLeaveBalance TINYINT = NULL
	,@LocalWorkingDays TINYINT = NULL
	,@LocalWorkingHours TINYINT = NULL
	,@LocalOffDaysbyWeek TINYINT = NULL
	,@LocalWorkingHoursByWeek TINYINT = NULL
	,@ExpatBasicAnnualLeaveBalance TINYINT = NULL
	,@ExpatWorkingDays TINYINT = NULL
	,@ExpatWorkingHours TINYINT = NULL
	,@ExpatOffDaysbyWeek TINYINT = NULL
	,@ExpatWorkingHoursByWeek TINYINT = NULL
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
		,SectionId
		,MailGroup
		,CreatedDate
		,CreatedBy
		,LocalBasicAnnualLeaveBalance
		,LocalWorkingDays
		,LocalWorkingHours
		,LocalOffDaysbyWeek
		,LocalWorkingHoursByWeek
		,ExpatBasicAnnualLeaveBalance
		,ExpatWorkingDays
		,ExpatWorkingHours
		,ExpatOffDaysbyWeek
		,ExpatWorkingHoursByWeek
		)
	VALUES (
		@Code
		,@Name
		,@SectionId
		,@MailGroup
		,GETDATE()
		,@CreatedBy
		,@LocalBasicAnnualLeaveBalance
		,@LocalWorkingDays
		,@LocalWorkingHours
		,@LocalOffDaysbyWeek
		,@LocalWorkingHoursByWeek
		,@ExpatBasicAnnualLeaveBalance
		,@ExpatWorkingDays
		,@ExpatWorkingHours
		,@ExpatOffDaysbyWeek
		,@ExpatWorkingHoursByWeek
		)

	DECLARE @id INT = cast(SCOPE_IDENTITY() AS INT)

	INSERT INTO dbo.GroupManager (GroupId, EmpId, RoleId)
	VALUES (@id, @HeadId, 1)

	IF ISNULL(@ManagerIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupManager (GroupId, EmpId, RoleId)
		SELECT @id, val, 2
		FROM dbo.fnc_strSplit(@ManagerIds, ',')
	END

	IF ISNULL(@LeaderIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupManager (GroupId, EmpId, RoleId)
		SELECT @id, val, 3
		FROM dbo.fnc_strSplit(@LeaderIds, ',')
	END

	IF ISNULL(@SupervisorIds, '') <> ''
	BEGIN
		INSERT INTO dbo.GroupManager (GroupId, EmpId, RoleId)
		SELECT @id, val, 4
		FROM dbo.fnc_strSplit(@SupervisorIds, ',')
	END

	DECLARE @xml XML
		,@logData NVARCHAR(MAX)

	SET @xml = (
			SELECT Code AS '@Code'
				,[Name] AS '@Name'
				,SectionId AS '@SectionId'
				,@HeadId AS '@HeadId'
				,@ManagerIds AS '@ManagerIds'
				,@LeaderIds AS '@LeaderIds'
				,@SupervisorIds AS '@SupervisorIds'
				,MailGroup AS '@MailGroup'
				,LocalBasicAnnualLeaveBalance AS '@LocalBasicAnnualLeaveBalance'
				,LocalWorkingDays AS '@LocalWorkingDays'
				,LocalWorkingHours AS '@LocalWorkingHours'
				,LocalOffDaysbyWeek AS '@LocalOffDaysbyWeek'
				,LocalWorkingHoursByWeek AS '@LocalWorkingHoursByWeek'
				,ExpatBasicAnnualLeaveBalance AS '@ExpatBasicAnnualLeaveBalance'
				,ExpatWorkingDays AS '@ExpatWorkingDays'
				,ExpatWorkingHours AS '@ExpatWorkingHours'
				,ExpatOffDaysbyWeek AS '@ExpatOffDaysbyWeek'
				,ExpatWorkingHoursByWeek AS '@ExpatWorkingHoursByWeek'
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
