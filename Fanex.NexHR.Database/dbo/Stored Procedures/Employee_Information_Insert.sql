
CREATE PROCEDURE [dbo].[Employee_Information_Insert]
	@fullName NVARCHAR(50)
	,@nickName NVARCHAR(20)
	,@domainUserName VARCHAR(50) = ''
	,@code VARCHAR(20)
	,@monShift INT = NULL
	,@tueShift INT = NULL
	,@wedShift INT = NULL
	,@thuShift INT = NULL
	,@friShift INT = NULL
	,@satShift INT = NULL
	,@sunShift INT = NULL
	,@mainGroupID INT
	,@createdDate DATETIME
	,@createBy INT
	,@password NVARCHAR(100)
	,@IsDefaultSchedule BIT
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

	DECLARE @EmpID_exist INT
		,@NickName_exist NVARCHAR(50)
		,@FullName_exist NVARCHAR(50)
		,@Code_exist NVARCHAR(50)
		,@IsDelete BIT
		,@Enabled_exist BIT
		,@MemberRole NVARCHAR(10);

	SET @MemberRole = 1;

	SELECT TOP 1 @EmpID_exist = EmpID
		,@NickName_exist = NickName
		,@Code_exist = Code
		,@IsDelete = isDeleted
		,@Enabled_exist = Enabled
		,@FullName_exist = FullName
	FROM dbo.Employee WITH (NOLOCK)
	WHERE (Code = @code);-- AND (isDeleted = 0 OR Enabled =1)

	IF ISNULL(@EmpID_exist, 0) <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Code ' + @Code + ' has been assigned to ' + @FullName_exist;

		RETURN;
	END;

	SELECT TOP 1 @EmpID_exist = EmpID
		,@NickName_exist = NickName
		,@Code_exist = Code
		,@IsDelete = isDeleted
		,@Enabled_exist = Enabled
		,@FullName_exist = FullName
	FROM dbo.Employee WITH (NOLOCK)
	WHERE (nickName = @nickName)
		AND (
			isDeleted = 0
			OR Enabled = 1
			);

	IF ISNULL(@EmpID_exist, 0) <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Nick Name  ' + @NickName_exist + '  has been assigned to ' + @FullName_exist;

		RETURN;
	END;

	INSERT INTO dbo.Employee (
		Code
		,DomainUsername
		,FullName
		,NickName
		,DOB
		,[Enabled]
		,GroupID
		,CreatedDate
		,CreatedBy
		,Password
		)
	VALUES (
		@code
		,@domainUsername
		,@fullName
		,@nickName
		,CAST('1-1-1900' AS SMALLDATETIME)
		,1
		,@mainGroupID
		,@CreatedDate
		,@createBy
		,@password
		);

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 101
			,@ErrMsg = N'Create member code "' + @code + '" failed.';

		RETURN;
	END;
	ELSE
	BEGIN
		SET @EmpId = CAST(SCOPE_IDENTITY() AS INT);
	END

	INSERT INTO [dbo].[EmployeeSummary] (
		[EmpId]
		,IsDefaultSchedule
		)
	VALUES (
		@EmpId
		,@IsDefaultSchedule
		);

	INSERT INTO [dbo].[EmployeeContract] ([EmpId])
	VALUES (@EmpId);

	EXEC [EmployeeRole_Insert] @EmployeeId = @EmpId
		,@RoleIds = @MemberRole
		,@ModifiedBy = @createBy

	BEGIN
		INSERT INTO dbo.EmployeeDefaultShift (
			[EmpID]
			,[MonShift]
			,[TueShift]
			,[WedShift]
			,[ThuShift]
			,[FriShift]
			,[SatShift]
			,[SunShift]
			)
		VALUES (
			@EmpId
			,@monShift
			,@tueShift
			,@wedShift
			,@thuShift
			,@friShift
			,@satShift
			,@sunShift
			);
	END;

	INSERT INTO dbo.EmployeeGroup (
		GroupId
		,EmpID
		,isMainGroup
		)
	VALUES (
		@mainGroupID
		,@EmpId
		,1
		);

	--insert working calendar of current month + next month
	DECLARE @fromdate DATETIME
		,@todate DATETIME;

	SET @fromdate = GETDATE();
	SET @todate = DATEADD(DAY, - 1, DATEADD(MONTH, DATEDIFF(MONTH, 0, @fromdate) + 2, 0));

	EXEC EmployeeCalendar_Insert @Empid = @EmpId
		,@fromdate = @fromdate
		,@todate = @todate
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT;

	IF @ErrCode <> 0
	BEGIN
		SELECT @ErrCode = 102
			,@ErrMsg = N'Create member code "' + @code + '" failed.';

		DELETE dbo.EmployeeDefaultShift
		WHERE EmpID = @EmpId;

		DELETE dbo.EmployeeGroup
		WHERE EmpID = @EmpId;

		DELETE dbo.[EmployeeSummary]
		WHERE EmpID = @EmpId;

		DELETE [dbo].[EmployeeContract]
		WHERE EmpID = @EmpId;

		DELETE dbo.Employee
		WHERE EmpID = @EmpId;
	END;
	ELSE
		SELECT @ErrCode = 0
			,@ErrCode = '';
END;