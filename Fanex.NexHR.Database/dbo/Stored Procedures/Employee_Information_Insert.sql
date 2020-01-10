
CREATE PROCEDURE [dbo].[Employee_Information_Insert] @fullName NVARCHAR(50)
	,@Nickname NVARCHAR(20)
	,@domainUserName VARCHAR(50) = ''
	,@code VARCHAR(20)
	,@GroupId INT
	,@createBy INT
	,@password NVARCHAR(100)
	,@EmpId INT = 0 OUTPUT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160705@Quinn.Vo
Task: Insert an employee Information 
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @EmpId_exist INT
		,@Nickname_exist NVARCHAR(20)
		,@FullName_exist NVARCHAR(50)
		,@MemberRole NVARCHAR(10) = 1

	SELECT TOP 1 @EmpId_exist = EmpId
		,@FullName_exist = FullName
		,@Nickname_exist = NickName
	FROM dbo.Employee WITH (NOLOCK)
	WHERE Code = @code

	IF ISNULL(@EmpId_exist, 0) <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Code ' + @Code + ' has been assigned to ' + @FullName_exist

		RETURN
	END

	SELECT TOP 1 @EmpId_exist = EmpId
		,@FullName_exist = FullName
		,@Nickname_exist = NickName
	FROM dbo.Employee WITH (NOLOCK)
	WHERE NickName = @Nickname
		AND (
			isDeleted = 0
			OR [Enabled] = 1
			)

	IF ISNULL(@EmpId_exist, 0) <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Nickname  ' + @Nickname_exist + '  has been assigned to ' + @FullName_exist

		RETURN
	END

	INSERT INTO dbo.Employee (
		Code
		,DomainUsername
		,FullName
		,NickName
		,DOB
		,[Enabled]
		,GroupId
		,CreatedDate
		,CreatedBy
		,[Password]
		)
	VALUES (
		@code
		,@domainUsername
		,@fullName
		,@Nickname
		,CAST('1-1-1900' AS SMALLDATETIME)
		,1
		,@GroupId
		,GETDATE()
		,@createBy
		,@password
		)

	SET @EmpId = CAST(SCOPE_IDENTITY() AS INT)

	INSERT INTO dbo.EmployeeSummary (
		EmpId
		,IsDefaultSchedule
		)
	VALUES (
		@EmpId
		,0
		)

	INSERT INTO dbo.EmployeeContract (EmpId)
	VALUES (@EmpId)

	EXEC dbo.EmployeeRole_Insert @EmployeeId = @EmpId
		,@RoleIds = @MemberRole
		,@ModifiedBy = @createBy

	INSERT INTO dbo.EmployeeDefaultShift (EmpId)
	VALUES (@EmpId)

	INSERT INTO dbo.EmployeeGroup (
		GroupId
		,EmpId
		,isMainGroup
		)
	VALUES (
		@GroupId
		,@EmpId
		,1
		)
END
