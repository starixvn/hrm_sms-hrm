

CREATE PROCEDURE [dbo].[Employee_Information_Insert] 
	-- Add the parameters for the stored procedure here
	@fullName nvarchar(50),
	@nickName nvarchar(20),
	@domainUserName varchar(50) = '',
	@code varchar(20),

	@monShift int=NULL,
	@tueShift int=NULL,
	@wedShift int=NULL,
	@thuShift int=NULL,
	@friShift int=NULL,
	@satShift int=NULL,
	@sunShift int=NULL,
	@mainGroupID int,
	@createdDate datetime,
	@createBy int,
	@password nvarchar(100),

	@IsDefaultSchedule BIT,
	
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT	
AS
/*
Created: TOM
Task: Insert an employee Information 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

BEGIN

	SET NOCOUNT ON;

	DECLARE @EmpID_exist INT,@NickName_exist nvarchar(50),@FullName_exist nvarchar(50),@Code_exist nvarchar(50),@IsDelete BIT,@Enabled_exist BIT

	select TOP 1 @EmpID_exist = EmpID, @NickName_exist = NickName,@Code_exist=Code,@IsDelete = isDeleted , @Enabled_exist  = Enabled, @FullName_exist  = FullName
		from dbo.Employee with(nolock) where (Code = @code) -- AND (isDeleted = 0 OR Enabled =1)

	if ISNULL(@EmpID_exist,0)<>0 
	begin
			SELECT @ErrCode = 100, @ErrMsg = N'Code ' + @Code+' has been assigned to ' + @FullName_exist
			RETURN
	end	

	select TOP 1 @EmpID_exist = EmpID, @NickName_exist = NickName,@Code_exist=Code,@IsDelete = isDeleted , @Enabled_exist  = Enabled, @FullName_exist  = FullName
		from dbo.Employee with(nolock) where (nickName = @nickName) AND (isDeleted = 0 OR Enabled =1)

	if ISNULL(@EmpID_exist,0)<>0 
	begin
			SELECT @ErrCode = 100, @ErrMsg = N'Nick Name  ' + @NickName_exist+'  has been assigned to ' + @FullName_exist
			RETURN
	end	

	insert into dbo.Employee(Code,DomainUsername,FullName,NickName,DOB,
					[Enabled],GroupID, CreatedDate, CreatedBy,  Password)
	values(@code,@domainUsername,@fullName,@nickName,cast('1-1-1900' as smalldatetime),
					1,@mainGroupID, @CreatedDate, @createBy, @password)
		
	declare @id int
	IF @@ERROR <> 0
	begin
		SELECT @ErrCode = 101, @ErrMsg = N'Create member code "' + @code + '" failed.'
		return
	end
	else
		set @id = cast(SCOPE_IDENTITY() as int)

	INSERT INTO [dbo].[EmployeeSummary] ([EmpId],IsDefaultSchedule)
	VALUES(@id,@IsDefaultSchedule)

	INSERT INTO [dbo].[EmployeeContract] ([EmpId])
	VALUES(@id)

	--if @IsDefaultSchedule = 1 
	BEGIN 	
		insert into dbo.EmployeeDefaultShift(
				[EmpID]
				,[MonShift]
				,[TueShift]
				,[WedShift]
				,[ThuShift]
				,[FriShift]
				,[SatShift]
				,[SunShift])
		values(@id,@monShift,@tueShift,@wedShift,@thuShift,@friShift,@satShift,@sunShift)
	END

	--	insert main group for employee
	INSERT INTO dbo.EmployeeGroup(GroupId, EmpID, isMainGroup)
	VALUES (@mainGroupID, @id, 1)
				
	--insert working calendar of current month + next month
	declare @fromdate datetime,	@todate datetime
	set @fromdate = getdate()
	set @todate = DATEADD(DAY, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, @fromdate) + 2, 0))

	exec EmployeeCalendar_Insert @Empid=@id, @fromdate=@fromdate,@todate=@todate,@ErrCode=@ErrCode output,@ErrMsg=@ErrMsg output
	
	IF @ErrCode <> 0
		BEGIN
			SELECT @ErrCode = 102, @ErrMsg = N'Create member code "' + @code + '" failed.'
			
			DELETE dbo.EmployeeDefaultShift WHERE EmpID = @id
			DELETE dbo.EmployeeGroup WHERE EmpID = @id
			DELETE dbo.[EmployeeSummary] WHERE EmpID = @id
			DELETE [dbo].[EmployeeContract] WHERE EmpID = @id
			DELETE dbo.Employee WHERE EmpID = @id
		END
	ELSE 
		SELECT @ErrCode=0,@ErrCode=''		
	

END
