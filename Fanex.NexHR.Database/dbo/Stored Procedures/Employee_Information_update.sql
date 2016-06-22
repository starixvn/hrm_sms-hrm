

CREATE PROCEDURE [dbo].[Employee_Information_update] 
	-- Add the parameters for the stored procedure here
	@EmpId int
	,@Code varchar(20)
	,@DomainUsername varchar(50)=''
	,@FullName nvarchar(50)=''
	,@NickName nvarchar(20)=''
	,@DOB smalldatetime=NULL
	,@Email varchar(50)=NULL
	,@JobTitleId int=NULL
	,@Enabled bit
	,@Phone varchar(50)=NULL
	,@Gender tinyint=NULL
	,@GroupId int
	,@Roles varchar(20)
	,@ManagerId int=NULL
	,@JoinedDate smalldatetime = null
	,@ConfirmedDate smalldatetime = null
	,@ResignedDate smalldatetime = null
	,@Skype varchar(100) = null
	,@Yahoo varchar(100) = null
	,@Marriage tinyint = 1
	,@LastLogin datetime=NULL
	,@CreatedBy int=NULL
	,@ModifiedDate datetime
	,@ModifiedBy int
	,@isDeleted bit=0
	,@PermanentAddress nvarchar(250)=''
	,@CurrentAddress nvarchar(250)=''
	,@IdCardNumber varchar(20)=''
	,@PassportNo varchar(35)=''
	,@PassportExp datetime=null
	,@Password nvarchar(100)=''
	,@PasswordReset BIT=0
	,@ChineseName nvarchar(200)=NULL
	,@Unit nvarchar(2000)=NULL
	
	,@AnnualLeaveBalance numeric(18,1)= 0
    ,@KeepInViewBalance numeric(18,1)= 0
    ,@CanOT bit
    ,@IsLocal bit
    ,@WorkingDays int
    ,@WorkingHours int
    ,@CompanyDate datetime=NULL
	,@CountryCode varchar(5)=NULL
	,@IsDefaultSchedule BIT
	,@IsTopManager BIT=0
	,@OffDaysbyWeek SMALLINT=0
	,@IsExpatUseHoliday BIT=0

	,@PlaceOfbirth nvarchar(250)=''
	,@Ethnic nvarchar(250)=''
	,@Native nvarchar(250)=''
	,@Religion nvarchar(250)=''
	,@DateOfIDCard datetime=NULL
	,@PlaceOfIDCard nvarchar(250)=''
	,@HomePhone nvarchar(250)=''
	,@Qualification nvarchar(300)=''
	,@Major nvarchar(300)=''

    ,@BankAccount nvarchar(100)=NULL
    ,@TaxCode nvarchar(150)=NULL
    ,@SocialInsuranceNo nvarchar(150)=NULL
    ,@RegisteredHospital nvarchar(250)=NULL
    ,@Contract1 tinyint=NULL
    ,@FromDate1 datetime=NULL
    ,@ToDate1 datetime=NULL
    ,@Contract2 tinyint=NULL
    ,@FromDate2 datetime=NULL
    ,@ToDate2 datetime=NULL
    ,@Contract3 tinyint=NULL
    ,@FromDate3 datetime=NULL
    ,@ToDate3 datetime=NULL
    ,@Contract4 tinyint=NULL
    ,@FromDate4 datetime=NULL
    ,@ToDate4 datetime=NULL
    ,@Contract5 tinyint=NULL
    ,@FromDate5 datetime=NULL
    ,@ToDate5 datetime=NULL
	,@History nvarchar(500)=NULL
	,@EmployeeCard VARCHAR(10) = ''

	,@ErrCode int = 0 OUTPUT
	,@ErrMsg nvarchar(200) = '' OUTPUT	
AS
/*
Created: TOM
Task: UPDATE an employee Information 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

BEGIN

	SET NOCOUNT ON;

	DECLARE @EmpID_exist INT,@NickName_exist nvarchar(50),@FullName_exist nvarchar(50),@Code_exist nvarchar(50),@IsDelete BIT,@Enabled_exist BIT

	select TOP 1 @EmpID_exist = EmpID, @NickName_exist = NickName,@Code_exist=Code,@IsDelete = isDeleted , @Enabled_exist  = Enabled, @FullName_exist  = FullName
		from dbo.Employee with(nolock) where EmpID <> @EmpID AND(Code = @code) -- AND (isDeleted = 0 OR Enabled =1)

	if ISNULL(@EmpID_exist,0)<>0 
	begin
			SELECT @ErrCode = 100, @ErrMsg = N'Code ' + @Code+' has been assigned to ' + @FullName_exist
			RETURN
	end	

	select TOP 1 @EmpID_exist = EmpID, @NickName_exist = NickName,@Code_exist=Code,@IsDelete = isDeleted , @Enabled_exist  = Enabled, @FullName_exist  = FullName
		from dbo.Employee with(nolock) where EmpID <> @EmpID AND (nickName = @nickName) AND (isDeleted = 0 OR Enabled =1)

	if ISNULL(@EmpID_exist,0)<>0 
	begin
			SELECT @ErrCode = 100, @ErrMsg = N'Nick Name  ' + @NickName_exist+'  has been assigned to ' + @FullName_exist
			RETURN
	end	

	DECLARE 	
		@Code_old varchar(20)
		,@DomainUsername_old varchar(50)=''
		,@FullName_old nvarchar(50)=''
		,@NickName_old nvarchar(20)=''
		,@DOB_old smalldatetime=NULL
		,@Email_old varchar(50)=NULL
		,@JobTitleId_old int=NULL
		,@Enabled_old bit
		,@Phone_old varchar(50)=NULL
		,@Gender_old tinyint=NULL
		,@GroupId_old int
		,@Roles_old varchar(20)
		,@ManagerId_old int=NULL
		,@JoinedDate_old smalldatetime
		,@ConfirmedDate_old smalldatetime
		,@ResignedDate_old smalldatetime
		,@Skype_old varchar(100)
		,@Yahoo_old varchar(100)
		,@Marriage_old tinyint = 1
		,@LastLogin_old datetime=NULL
		,@CreatedBy_old int=NULL
		,@ModifiedDate_old datetime
		,@ModifiedBy_old int
		,@isDeleted_old bit=0
		,@PermanentAddress_old nvarchar(250)=''
		,@CurrentAddress_old nvarchar(250)=''
		,@IdCardNumber_old varchar(20)=''
		,@PassportNo_old varchar(35)=''
		,@PassportExp_old datetime=null
		,@Password_old nvarchar(100)
		,@ChineseName_old nvarchar(200)=NULL
		,@Unit_old nvarchar(2000)=NULL
	
		,@AnnualLeaveBalance_old numeric(18,1)= 0
		,@KeepInViewBalance_old numeric(18,1)= 0
		,@CanOT_old bit
		,@IsLocal_old bit
		,@WorkingDays_old int
		,@WorkingHours_old int
		,@CompanyDate_old datetime=NULL
		,@CountryCode_old varchar(5)=NULL
		,@IsDefaultSchedule_old BIT
		,@IsTopManager_old BIT=0
		,@OffDaysbyWeek_old SMALLINT=0
		,@IsExpatUseHoliday_old BIT=0

		,@PlaceOfbirth_old nvarchar(250)=''
		,@Ethnic_old nvarchar(250)=''
		,@Native_old nvarchar(250)=''
		,@Religion_old nvarchar(250)=''
		,@DateOfIDCard_old datetime=NULL
		,@PlaceOfIDCard_old nvarchar(250)=''
		,@HomePhone_old nvarchar(250)=''
		,@Qualification_old nvarchar(300)=''
		,@Major_old nvarchar(300)=''

		,@BankAccount_old nvarchar(100)=NULL
		,@TaxCode_old nvarchar(150)=NULL
		,@SocialInsuranceNo_old nvarchar(150)=NULL
		,@RegisteredHospital_old nvarchar(250)=NULL
		,@Contract1_old tinyint=NULL
		,@FromDate1_old datetime=NULL
		,@ToDate1_old datetime=NULL
		,@Contract2_old tinyint=NULL
		,@FromDate2_old datetime=NULL
		,@ToDate2_old datetime=NULL
		,@Contract3_old tinyint=NULL
		,@FromDate3_old datetime=NULL
		,@ToDate3_old datetime=NULL
		,@Contract4_old tinyint=NULL
		,@FromDate4_old datetime=NULL
		,@ToDate4_old datetime=NULL
		,@Contract5_old tinyint=NULL
		,@FromDate5_old datetime=NULL
		,@ToDate5_old datetime=NULL
		,@History_old nvarchar(500)=NULL
		,@EmployeeCard_old VARCHAR(10) = ''

	SELECT  TOP 1
		   @Code_Old = Code 
				,@DomainUsername_Old=DomainUsername 
				,@FullName_Old=FullName 
				,@NickName_Old=NickName
				,@DOB_Old=DOB
				,@Email_Old=Email 
				,@JobTitleId_Old=JobTitleId
				,@Enabled_Old=Enabled
				,@Phone_Old=Phone
				,@Gender_Old=Gender
				,@GroupId_Old=GroupId
				,@Roles_Old=Roles
				,@ManagerId_Old=ManagerId
				,@JoinedDate_Old=JoinedDate
				,@ConfirmedDate_Old=ConfirmedDate
				,@ResignedDate_Old=ResignedDate
				,@Skype_Old=Skype
				,@Yahoo_Old=Yahoo
				,@Marriage_Old=Marriage
				,@LastLogin_Old=LastLogin
				,@ModifiedDate_Old=ModifiedDate
				,@ModifiedBy_Old=ModifiedBy
				,@isDeleted_Old=isDeleted
				,@PermanentAddress_Old=PermanentAddress
				,@CurrentAddress_Old=CurrentAddress
				,@IdCardNumber_Old=IdCardNumber
				,@PassportNo_Old=PassportNo
				,@PassportExp_Old=PassportExp
				,@ChineseName_Old=ChineseName

				,@PlaceOfbirth_Old=PlaceOfbirth
				,@Ethnic_Old=Ethnic
				,@Native_Old=[Native]
				,@Religion_Old=Religion
				,@DateOfIDCard_Old=DateOfIDCard
				,@PlaceOfIDCard_Old=PlaceOfIDCard
				,@HomePhone_Old=HomePhone
				,@Qualification_Old=Qualification
				,@Major_Old=Major
				,@EmployeeCard_old = EmployeeCard
		FROM dbo.Employee WITH(NOLOCK)
			WHERE EmpId =@EmpId


		SELECT TOP 1
				   @CanOT_old=[CanOT] 
				  ,@IsLocal_old=[IsLocal] 
				  ,@WorkingDays_old=[WorkingDays] 
				  ,@WorkingHours_old=[WorkingHours] 
				  ,@IsDefaultSchedule_old=[IsDefaultSchedule] 
				  ,@IsTopManager_old=IsTopManager 
				  ,@CompanyDate_old=[CompanyDate] 
				  ,@CountryCode_old=CountryCode 
				  ,@Unit_old=Unit 
				  ,@IsExpatUseHoliday_old=IsExpatUseHoliday

				  ,@OffDaysbyWeek_old=OffDaysbyWeek
		FROM dbo.[EmployeeSummary] WITH(NOLOCK)
			WHERE EmpId =@EmpId

	BEGIN TRY
		BEGIN TRANSACTION;	
			UPDATE dbo.Employee WITH(ROWLOCK)
		   SET Code = @Code 
				,DomainUsername =@DomainUsername 
				,FullName =@FullName 
				,NickName  =@NickName 
				,DOB =@DOB 
				,Email =REPLACE(@Email ,' ' , '')
				,JobTitleId  =@JobTitleId 
				,Enabled = @Enabled
				,Phone =@Phone 
				,Gender = @Gender 
				,GroupId =@GroupId 
				,Roles =@Roles 
				,ManagerId =@ManagerId 
				,JoinedDate =@JoinedDate 
				,ConfirmedDate =@ConfirmedDate 
				,ResignedDate =@ResignedDate 
				,Skype =@Skype 
				,Yahoo =@Yahoo 
				,Marriage = @Marriage 
				,LastLogin= @LastLogin 
				,ModifiedDate =@ModifiedDate 
				,ModifiedBy =@ModifiedBy 
				,isDeleted =@isDeleted 
				,PermanentAddress =@PermanentAddress 
				,CurrentAddress =@CurrentAddress 
				,IdCardNumber =@IdCardNumber 
				,PassportNo =@PassportNo 
				,PassportExp =@PassportExp
				,ChineseName=@ChineseName

				,PlaceOfbirth =@PlaceOfbirth
				,Ethnic =@Ethnic 
				,[Native] =@Native 
				,Religion =@Religion 
				,DateOfIDCard =@DateOfIDCard 
				,PlaceOfIDCard =@PlaceOfIDCard 
				,HomePhone =@HomePhone 
				,Qualification =@Qualification 
				,Major =@Major
				,EmployeeCard = @EmployeeCard
			WHERE EmpId =@EmpId

			-- update password
			IF (@password IS NOT NULL AND @password <> '')
			BEGIN
				EXEC [dbo].Employee_ChangePassword @EmpId, @password,@PasswordReset
			END

			--
			EXEC [dbo].[Employee_Summary_Update] @EmpID =@EmpID,
				@AnnualLeaveBalance =@AnnualLeaveBalance,
				@KeepInViewBalance =@KeepInViewBalance ,
				@CanOT =@CanOT ,
				@IsLocal =@IsLocal ,
				@WorkingDays =@WorkingDays ,
				@WorkingHours =@WorkingHours ,
				@CompanyDate =@CompanyDate ,
				@CountryCode = @CountryCode,
				@Unit = @Unit,
	
				@IsDefaultSchedule =@IsDefaultSchedule,
				@IsTopManager=@IsTopManager,
				@OffDaysbyWeek=@OffDaysbyWeek,
				@IsExpatUseHoliday=@IsExpatUseHoliday,

				@ErrCode = @ErrCode OUTPUT,
				@ErrMsg = @ErrMsg OUTPUT

			EXEC [dbo].[Employee_Contract_Update] 
				@EmpID =@EmpID 
				,@BankAccount =@BankAccount 
				,@TaxCode =@TaxCode 
				,@SocialInsuranceNo =@SocialInsuranceNo 
				,@RegisteredHospital =@RegisteredHospital 
				
				,@Contract1 =@Contract1 
				,@FromDate1 =@FromDate1 
				,@ToDate1 =@ToDate1 
				
				,@Contract2 =@Contract2 
				,@FromDate2 =@FromDate2 
				,@ToDate2 =@ToDate2 
				
				,@Contract3 =@Contract3 
				,@FromDate3 =@FromDate3 
				,@ToDate3 =@ToDate3 

				,@Contract4 =@Contract4 
				,@FromDate4 =@FromDate4 
				,@ToDate4 =@ToDate4 

				,@Contract5 =@Contract5 
				,@FromDate5 =@FromDate5 
				,@ToDate5 =@ToDate5 
				,@History=@History
	
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT

			COMMIT TRANSACTION;
		SELECT @ErrCode=0,@ErrCode=''
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SELECT @ErrCode=100,@ErrMsg=N'Update failed';
	END CATCH
END



Declare @LogSPData nvarchar(4000), @now datetime
	set @now=getdate()
	--Log general info
	Select @LogSPData = '<Employee_Information_update EmpId="'+ coalesce(cast(@EmpId as varchar(50)),space(1)) + '" ' +
		'@DomainUsername="'+ coalesce(cast(@DomainUsername_Old as varchar(100)),space(1)) + '"#@#' +coalesce(cast(@DomainUsername as varchar(100)),space(1)) + '" ' +
		'@FullName="'+ coalesce(cast(@FullName_Old as nvarchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@FullName as nvarchar(100)),space(1)) + '" ' +
		'@NickName="'+ coalesce(cast(@NickName_Old as nvarchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@NickName as nvarchar(100)),space(1)) + '" ' +
		'@EmployeeCard="'+ coalesce(cast(@EmployeeCard_old as nvarchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@EmployeeCard as nvarchar(100)),space(1)) + '" ' +
		'@DOB="'+ coalesce(cast(@DOB_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@DOB as varchar(100)),space(1)) + '" ' +
		'@Email="'+ coalesce(cast(@Email_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Email as varchar(100)),space(1)) + '" ' +
		'@JobTitleId="'+ coalesce(cast(@JobTitleId_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@JobTitleId as varchar(100)),space(1)) + '" ' +
		'@Enabled="'+ coalesce(cast(@Enabled_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Enabled as varchar(100)),space(1)) + '" ' +

		'@Phone="'+ coalesce(cast(@Phone_Old as varchar(100)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Phone as varchar(100)),space(1)) + '" ' +
		'@Gender="'+ coalesce(cast(@Gender_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Gender as varchar(100)),space(1)) + '" ' +
		'@GroupId="'+ coalesce(cast(@GroupId_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@GroupId as varchar(100)),space(1)) + '" ' +
		'@Roles="'+ coalesce(cast(@Roles_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Roles as varchar(100)),space(1)) + '" ' +
		'@ManagerId="'+ coalesce(cast(@ManagerId_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@ManagerId as varchar(100)),space(1)) + '" ' +
		'@JoinedDate="'+ coalesce(cast(@JoinedDate_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@JoinedDate as varchar(100)),space(1)) + '" ' +
		'@ConfirmedDate="'+ coalesce(cast(@ConfirmedDate_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@ConfirmedDate as varchar(100)),space(1)) + '" ' +
		
		'@ResignedDate="'+ coalesce(cast(@ResignedDate_Old as varchar(100)),space(1)) + '" ' +'"#@#' +coalesce(cast(@ResignedDate as varchar(100)),space(1)) + '" ' +
		'@Skype="'+ coalesce(cast(@Skype_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Skype as varchar(100)),space(1)) + '" ' +
		'@Yahoo="'+ coalesce(cast(@Yahoo_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Yahoo as varchar(100)),space(1)) + '" ' +
		'@Marriage="'+ coalesce(cast(@Marriage_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Marriage as varchar(100)),space(1)) + '" ' +
		'@LastLogin="'+ coalesce(cast(@LastLogin_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@LastLogin as varchar(100)),space(1)) + '" ' +
		'@ModifiedBy="'+ coalesce(cast(@ModifiedBy_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@ModifiedBy as varchar(100)),space(1)) + '" ' +
		'@isDeleted="'+ coalesce(cast(@isDeleted_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@isDeleted as varchar(100)),space(1)) + '" ' +

		'@PermanentAddress="'+ coalesce(cast(@PermanentAddress_Old as varchar(100)),space(1)) + '" ' +'"#@#' +coalesce(cast(@PermanentAddress as varchar(100)),space(1)) + '" ' +
		'@CurrentAddress="'+ coalesce(cast(@CurrentAddress_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@CurrentAddress as varchar(100)),space(1)) + '" ' +
		'@IdCardNumber="'+ coalesce(cast(@IdCardNumber_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@IdCardNumber as varchar(100)),space(1)) + '" ' +
		'@PassportNo="'+ coalesce(cast(@PassportNo_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@PassportNo as varchar(100)),space(1)) + '" ' +
		'@PassportExp="'+ coalesce(cast(@PassportExp_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@PassportExp as varchar(100)),space(1)) + '" ' +
		'@ChineseName="'+ coalesce(cast(@ChineseName_Old as nvarchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@ChineseName as nvarchar(100)),space(1)) + '" ' +
		'@PlaceOfbirth="'+ coalesce(cast(@PlaceOfbirth_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@PlaceOfbirth as varchar(100)),space(1)) + '" ' +

		'@Native="'+ coalesce(cast(@Native_Old as varchar(100)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Native as varchar(100)),space(1)) + '" ' +
		'@Ethnic="'+ coalesce(cast(@Ethnic_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@DomainUsername as varchar(100)),space(1)) + '" ' +
		'@Religion="'+ coalesce(cast(@Religion_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Ethnic as varchar(100)),space(1)) + '" ' +
		'@DateOfIDCard="'+ coalesce(cast(@DateOfIDCard_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@DateOfIDCard as varchar(100)),space(1)) + '" ' +
		'@PlaceOfIDCard="'+ coalesce(cast(@PlaceOfIDCard_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@PlaceOfIDCard as varchar(100)),space(1)) + '" ' +
		'@HomePhone="'+ coalesce(cast(@HomePhone_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@PlaceOfIDCard as varchar(100)),space(1)) + '" ' +
		'@Qualification="'+ coalesce(cast(@PlaceOfIDCard_Old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@PlaceOfIDCard as varchar(100)),space(1)) + '" ' +
		'@Major="'+ coalesce(cast(@Major_Old as nvarchar(100)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Major as nvarchar(100)),space(1)) + '" ' +
		
		'@CanOT="'+ coalesce(cast(@CanOT_old as varchar(100)),space(1)) + '" ' +'"#@#' +coalesce(cast(@CanOT as varchar(100)),space(1)) + '" ' +
		'@IsLocal="'+ coalesce(cast(@IsLocal_old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@IsLocal as varchar(100)),space(1)) + '" ' +
		'@WorkingDays="'+ coalesce(cast(@WorkingDays_old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@WorkingDays as varchar(100)),space(1)) + '" ' +
		'@WorkingHours="'+ coalesce(cast(@WorkingHours_old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@WorkingHours as varchar(100)),space(1)) + '" ' +
		'@IsDefaultSchedule="'+ coalesce(cast(@IsDefaultSchedule_old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@IsDefaultSchedule as varchar(100)),space(1)) + '" ' +
		'@IsTopManager="'+ coalesce(cast(@IsTopManager_old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@IsTopManager as varchar(100)),space(1)) + '" ' +
		'@CompanyDate="'+ coalesce(cast(@CompanyDate_old as varchar(1000)),space(1)) + '" ' +'"#@#' +coalesce(cast(@CompanyDate as varchar(100)),space(1)) + '" ' +
		'@CountryCode="'+ coalesce(cast(@CountryCode_old as nvarchar(100)),space(1)) + '" ' +'"#@#' +coalesce(cast(@CountryCode as varchar(100)),space(1)) + '" ' +
		'@Unit="'+ coalesce(cast(@Unit_old as nvarchar(100)),space(1)) + '" ' +'"#@#' +coalesce(cast(@Unit as varchar(100)),space(1)) + '" ' +
		'@IsExpatUseHoliday="'+ coalesce(cast(@IsExpatUseHoliday_old as nvarchar(100)),space(1)) + '" ' +'"#@#' +coalesce(cast(@IsExpatUseHoliday as varchar(100)),space(1)) + '" ' +

		'ErrCode="'+ coalesce(cast(@ErrCode as varchar(50)),space(1)) + '" ' +
		'ErrMsg="'+ coalesce(cast(@ErrMsg as varchar(4000)),space(1)) + '" />'	


	EXEC dbo.Log_WriteLog
		@LogActionID = 500,
		@LogDate = @now,
		@Description = 'Update Employee Information',
		@TrackingValue = @LogSPData,
		@UserID = @ModifiedBy,
		@EmpId=@EmpId