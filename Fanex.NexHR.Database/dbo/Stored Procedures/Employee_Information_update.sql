
CREATE PROCEDURE [dbo].[Employee_Information_update] @EmpId INT
	,@Code VARCHAR(20)
	,@DomainUsername VARCHAR(50) = ''
	,@FullName NVARCHAR(50) = ''
	,@NickName NVARCHAR(20) = ''
	,@DOB SMALLDATETIME = NULL
	,@Email VARCHAR(50) = NULL
	,@JobTitleId INT = NULL
	,@Enabled BIT
	,@Phone VARCHAR(50) = NULL
	,@Gender TINYINT = NULL
	,@GroupId INT
	,@Roles VARCHAR(20)
	,@ManagerId INT = NULL
	,@JoinedDate SMALLDATETIME = NULL
	,@ConfirmedDate SMALLDATETIME = NULL
	,@ResignedDate SMALLDATETIME = NULL
	,@Skype VARCHAR(100) = NULL
	,@Yahoo VARCHAR(100) = NULL
	,@Marriage TINYINT = 1
	,@LastLogin DATETIME = NULL
	,@CreatedBy INT = NULL
	,@ModifiedDate DATETIME
	,@ModifiedBy INT
	,@isDeleted BIT = 0
	,@PermanentAddress NVARCHAR(250) = ''
	,@CurrentAddress NVARCHAR(250) = ''
	,@IdCardNumber VARCHAR(20) = ''
	,@PassportNo VARCHAR(35) = ''
	,@PassportExp DATETIME = NULL
	,@Password NVARCHAR(100) = ''
	,@PasswordReset BIT = 0
	,@ChineseName NVARCHAR(200) = NULL
	,@Unit NVARCHAR(2000) = NULL
	,@AnnualLeaveBalance NUMERIC(18, 1) = 0
	,@KeepInViewBalance NUMERIC(18, 1) = 0
	,@CanOT BIT
	,@SpecialOT DECIMAL(3, 1) = NULL
	,@IsLocal BIT
	,@WorkingDays INT
	,@WorkingHours INT
	,@CompanyDate DATETIME = NULL
	,@CountryCode VARCHAR(5) = NULL
	,@IsTopManager BIT = 0
	,@OffDaysbyWeek SMALLINT = 0
	,@IsExpatUseHoliday BIT = 0
	,@PlaceOfbirth NVARCHAR(250) = ''
	,@Ethnic NVARCHAR(250) = ''
	,@Native NVARCHAR(250) = ''
	,@Religion NVARCHAR(250) = ''
	,@DateOfIDCard DATETIME = NULL
	,@PlaceOfIDCard NVARCHAR(250) = ''
	,@HomePhone NVARCHAR(250) = ''
	,@Qualification NVARCHAR(300) = ''
	,@Major NVARCHAR(300) = ''
	,@BankAccount NVARCHAR(100) = NULL
	,@TaxCode NVARCHAR(150) = NULL
	,@SocialInsuranceNo NVARCHAR(150) = NULL
	,@RegisteredHospital NVARCHAR(250) = NULL
	,@Contract1 TINYINT = NULL
	,@FromDate1 DATETIME = NULL
	,@ToDate1 DATETIME = NULL
	,@Contract2 TINYINT = NULL
	,@FromDate2 DATETIME = NULL
	,@ToDate2 DATETIME = NULL
	,@Contract3 TINYINT = NULL
	,@FromDate3 DATETIME = NULL
	,@ToDate3 DATETIME = NULL
	,@Contract4 TINYINT = NULL
	,@FromDate4 DATETIME = NULL
	,@ToDate4 DATETIME = NULL
	,@Contract5 TINYINT = NULL
	,@FromDate5 DATETIME = NULL
	,@ToDate5 DATETIME = NULL
	,@History NVARCHAR(500) = NULL
	,@EmployeeCard VARCHAR(10) = ''
	,@PickupRemark NVARCHAR(200) = NULL
	,@PromotionDate DATETIME = NULL
	,@SectionDate DATETIME = NULL
	,@MilitaryFromDate DATETIME = NULL
	,@MilitaryToDate DATETIME = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: TOM
Task: UPDATE an employee Information 
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

	SELECT TOP 1 @EmpID_exist = EmpID
		,@NickName_exist = NickName
		,@Code_exist = Code
		,@IsDelete = isDeleted
		,@Enabled_exist = Enabled
		,@FullName_exist = FullName
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmpID <> @EmpID
		AND Code = @code

	IF ISNULL(@EmpID_exist, 0) <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Code ' + @Code + ' has been assigned to ' + @FullName_exist

		RETURN
	END

	SELECT TOP 1 @EmpID_exist = EmpID
		,@NickName_exist = NickName
		,@Code_exist = Code
		,@IsDelete = isDeleted
		,@Enabled_exist = [Enabled]
		,@FullName_exist = FullName
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmpID <> @EmpID
		AND nickName = @nickName
		AND (
			isDeleted = 0
			OR [Enabled] = 1
			)

	IF ISNULL(@EmpID_exist, 0) <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Nick Name  ' + @NickName_exist + '  has been assigned to ' + @FullName_exist

		RETURN
	END

	DECLARE @Code_old VARCHAR(20)
		,@DomainUsername_old VARCHAR(50) = ''
		,@FullName_old NVARCHAR(50) = ''
		,@NickName_old NVARCHAR(20) = ''
		,@DOB_old SMALLDATETIME = NULL
		,@Email_old VARCHAR(50) = NULL
		,@JobTitleId_old INT = NULL
		,@Enabled_old BIT
		,@Phone_old VARCHAR(50) = NULL
		,@Gender_old TINYINT = NULL
		,@GroupId_old INT
		,@Roles_old VARCHAR(20)
		,@ManagerId_old INT = NULL
		,@JoinedDate_old SMALLDATETIME
		,@ConfirmedDate_old SMALLDATETIME
		,@ResignedDate_old SMALLDATETIME
		,@Skype_old VARCHAR(100)
		,@Yahoo_old VARCHAR(100)
		,@Marriage_old TINYINT = 1
		,@LastLogin_old DATETIME = NULL
		,@CreatedBy_old INT = NULL
		,@ModifiedDate_old DATETIME
		,@ModifiedBy_old INT
		,@isDeleted_old BIT = 0
		,@PermanentAddress_old NVARCHAR(250) = ''
		,@CurrentAddress_old NVARCHAR(250) = ''
		,@IdCardNumber_old VARCHAR(20) = ''
		,@PassportNo_old VARCHAR(35) = ''
		,@PassportExp_old DATETIME = NULL
		,@Password_old NVARCHAR(100)
		,@ChineseName_old NVARCHAR(200) = NULL
		,@Unit_old NVARCHAR(2000) = NULL
		,@AnnualLeaveBalance_old NUMERIC(18, 1) = 0
		,@KeepInViewBalance_old NUMERIC(18, 1) = 0
		,@CanOT_old BIT
		,@SpecialOT_old DECIMAL(3, 1) = NULL
		,@IsLocal_old BIT
		,@WorkingDays_old INT
		,@WorkingHours_old INT
		,@CompanyDate_old DATETIME = NULL
		,@CountryCode_old VARCHAR(5) = NULL
		,@IsDefaultSchedule_old BIT
		,@IsTopManager_old BIT = 0
		,@OffDaysbyWeek_old SMALLINT = 0
		,@IsExpatUseHoliday_old BIT = 0
		,@PlaceOfbirth_old NVARCHAR(250) = ''
		,@Ethnic_old NVARCHAR(250) = ''
		,@Native_old NVARCHAR(250) = ''
		,@Religion_old NVARCHAR(250) = ''
		,@DateOfIDCard_old DATETIME = NULL
		,@PlaceOfIDCard_old NVARCHAR(250) = ''
		,@HomePhone_old NVARCHAR(250) = ''
		,@Qualification_old NVARCHAR(300) = ''
		,@Major_old NVARCHAR(300) = ''
		,@BankAccount_old NVARCHAR(100) = NULL
		,@TaxCode_old NVARCHAR(150) = NULL
		,@SocialInsuranceNo_old NVARCHAR(150) = NULL
		,@RegisteredHospital_old NVARCHAR(250) = NULL
		,@Contract1_old TINYINT = NULL
		,@FromDate1_old DATETIME = NULL
		,@ToDate1_old DATETIME = NULL
		,@Contract2_old TINYINT = NULL
		,@FromDate2_old DATETIME = NULL
		,@ToDate2_old DATETIME = NULL
		,@Contract3_old TINYINT = NULL
		,@FromDate3_old DATETIME = NULL
		,@ToDate3_old DATETIME = NULL
		,@Contract4_old TINYINT = NULL
		,@FromDate4_old DATETIME = NULL
		,@ToDate4_old DATETIME = NULL
		,@Contract5_old TINYINT = NULL
		,@FromDate5_old DATETIME = NULL
		,@ToDate5_old DATETIME = NULL
		,@History_old NVARCHAR(500) = NULL
		,@EmployeeCard_old VARCHAR(10) = ''
		,@PickupRemark_old NVARCHAR(200) = NULL
		,@PromotionDate_old DATETIME = NULL
		,@SectionDate_old DATETIME = NULL
		,@MilitaryFromDate_old DATETIME = NULL
		,@MilitaryToDate_old DATETIME = NULL

	SELECT TOP 1 @Code_Old = Code
		,@DomainUsername_Old = DomainUsername
		,@FullName_Old = FullName
		,@NickName_Old = NickName
		,@DOB_Old = DOB
		,@Email_Old = Email
		,@JobTitleId_Old = JobTitleId
		,@Enabled_Old = Enabled
		,@Phone_Old = Phone
		,@Gender_Old = Gender
		,@GroupId_Old = GroupId
		,@ManagerId_Old = ManagerId
		,@JoinedDate_Old = JoinedDate
		,@ConfirmedDate_Old = ConfirmedDate
		,@ResignedDate_Old = ResignedDate
		,@Skype_Old = Skype
		,@Yahoo_Old = Yahoo
		,@Marriage_Old = Marriage
		,@LastLogin_Old = LastLogin
		,@ModifiedDate_Old = ModifiedDate
		,@ModifiedBy_Old = ModifiedBy
		,@isDeleted_Old = isDeleted
		,@PermanentAddress_Old = PermanentAddress
		,@CurrentAddress_Old = CurrentAddress
		,@IdCardNumber_Old = IdCardNumber
		,@PassportNo_Old = PassportNo
		,@PassportExp_Old = PassportExp
		,@ChineseName_Old = ChineseName
		,@PlaceOfbirth_Old = PlaceOfbirth
		,@Ethnic_Old = Ethnic
		,@Native_Old = [Native]
		,@Religion_Old = Religion
		,@DateOfIDCard_Old = DateOfIDCard
		,@PlaceOfIDCard_Old = PlaceOfIDCard
		,@HomePhone_Old = HomePhone
		,@Qualification_Old = Qualification
		,@Major_Old = Major
		,@EmployeeCard_old = EmployeeCard
		,@PickupRemark_old = PickupRemark
		,@PromotionDate_old = PromotionDate
		,@SectionDate_old = SectionDate
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmpId = @EmpId

	SELECT @Roles_old = COALESCE(@Roles_old + ',', '') + CONVERT(NVARCHAR(2), RoleId)
	FROM dbo.EmployeeRole WITH (NOLOCK)
	WHERE EmployeeId = @EmpId

	SELECT TOP 1 @CanOT_old = [CanOT]
		,@SpecialOT_old = SpecialOT
		,@IsLocal_old = [IsLocal]
		,@WorkingDays_old = [WorkingDays]
		,@WorkingHours_old = [WorkingHours]
		,@IsDefaultSchedule_old = [IsDefaultSchedule]
		,@IsTopManager_old = IsTopManager
		,@CompanyDate_old = [CompanyDate]
		,@CountryCode_old = CountryCode
		,@Unit_old = Unit
		,@IsExpatUseHoliday_old = IsExpatUseHoliday
		,@OffDaysbyWeek_old = OffDaysbyWeek
		,@MilitaryFromDate_old = MilitaryFromDate
		,@MilitaryToDate_old = MilitaryToDate
	FROM dbo.[EmployeeSummary] WITH (NOLOCK)
	WHERE EmpId = @EmpId

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE dbo.Employee WITH (ROWLOCK)
		SET Code = @Code
			,DomainUsername = @DomainUsername
			,FullName = @FullName
			,NickName = @NickName
			,DOB = @DOB
			,Email = REPLACE(@Email, ' ', '')
			,JobTitleId = @JobTitleId
			,[Enabled] = @Enabled
			,Phone = @Phone
			,Gender = @Gender
			,GroupId = @GroupId
			,ManagerId = @ManagerId
			,JoinedDate = @JoinedDate
			,ConfirmedDate = @ConfirmedDate
			,ResignedDate = @ResignedDate
			,Skype = @Skype
			,Yahoo = @Yahoo
			,Marriage = @Marriage
			,LastLogin = @LastLogin
			,ModifiedDate = @ModifiedDate
			,ModifiedBy = @ModifiedBy
			,isDeleted = @isDeleted
			,PermanentAddress = @PermanentAddress
			,CurrentAddress = @CurrentAddress
			,IdCardNumber = @IdCardNumber
			,PassportNo = @PassportNo
			,PassportExp = @PassportExp
			,ChineseName = @ChineseName
			,PlaceOfbirth = @PlaceOfbirth
			,Ethnic = @Ethnic
			,[Native] = @Native
			,Religion = @Religion
			,DateOfIDCard = @DateOfIDCard
			,PlaceOfIDCard = @PlaceOfIDCard
			,HomePhone = @HomePhone
			,Qualification = @Qualification
			,Major = @Major
			,EmployeeCard = @EmployeeCard
			,PickupRemark = @PickupRemark
			,PromotionDate = @PromotionDate
			,SectionDate = @SectionDate
		WHERE EmpId = @EmpId

		EXEC [dbo].[EmployeeRole_Update] @EmployeeId = @EmpId
			,@RoleIds = @Roles
			,@ModifiedBy = @ModifiedBy;

		EXEC [dbo].[Employee_Summary_Update] @EmpID = @EmpID
			,@AnnualLeaveBalance = @AnnualLeaveBalance
			,@KeepInViewBalance = @KeepInViewBalance
			,@CanOT = @CanOT
			,@SpecialOT = @SpecialOT
			,@IsLocal = @IsLocal
			,@WorkingDays = @WorkingDays
			,@WorkingHours = @WorkingHours
			,@CompanyDate = @CompanyDate
			,@CountryCode = @CountryCode
			,@Unit = @Unit
			,@IsDefaultSchedule = @IsDefaultSchedule_old
			,@IsTopManager = @IsTopManager
			,@OffDaysbyWeek = @OffDaysbyWeek
			,@IsExpatUseHoliday = @IsExpatUseHoliday
			,@MilitaryFromDate = @MilitaryFromDate
			,@MilitaryToDate = @MilitaryToDate
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		EXEC [dbo].[Employee_Contract_Update] @EmpID = @EmpID
			,@BankAccount = @BankAccount
			,@TaxCode = @TaxCode
			,@SocialInsuranceNo = @SocialInsuranceNo
			,@RegisteredHospital = @RegisteredHospital
			,@Contract1 = @Contract1
			,@FromDate1 = @FromDate1
			,@ToDate1 = @ToDate1
			,@Contract2 = @Contract2
			,@FromDate2 = @FromDate2
			,@ToDate2 = @ToDate2
			,@Contract3 = @Contract3
			,@FromDate3 = @FromDate3
			,@ToDate3 = @ToDate3
			,@Contract4 = @Contract4
			,@FromDate4 = @FromDate4
			,@ToDate4 = @ToDate4
			,@Contract5 = @Contract5
			,@FromDate5 = @FromDate5
			,@ToDate5 = @ToDate5
			,@History = @History
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0
			,@ErrCode = ''
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 100
			,@ErrMsg = N'Update failed';
	END CATCH
	
	DECLARE @LogSPData NVARCHAR(4000)
		,@now DATETIME

	SET @now = getdate()

	--Log general info
	SELECT @LogSPData = '<Employee_Information_update EmpId="' + coalesce(cast(@EmpId AS VARCHAR(50)), space(1)) + '" ' 
		+ '@DomainUsername="' + coalesce(cast(@DomainUsername_Old AS VARCHAR(100)), space(1)) + '"#@#' + coalesce(cast(@DomainUsername AS VARCHAR(100)), space(1)) + '" '
		+ '@FullName="' + coalesce(cast(@FullName_Old AS NVARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@FullName AS NVARCHAR(100)), space(1)) + '" '
		+ '@NickName="' + coalesce(cast(@NickName_Old AS NVARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@NickName AS NVARCHAR(100)), space(1)) + '" '
		+ '@EmployeeCard="' + coalesce(cast(@EmployeeCard_old AS NVARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@EmployeeCard AS NVARCHAR(100)), space(1)) + '" '
		+ '@DOB="' + coalesce(cast(@DOB_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@DOB AS VARCHAR(100)), space(1)) + '" '
		+ '@Email="' + coalesce(cast(@Email_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Email AS VARCHAR(100)), space(1)) + '" '
		+ '@JobTitleId="' + coalesce(cast(@JobTitleId_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@JobTitleId AS VARCHAR(100)), space(1)) + '" '
		+ '@Enabled="' + coalesce(cast(@Enabled_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Enabled AS VARCHAR(100)), space(1)) + '" '
		+ '@Phone="' + coalesce(cast(@Phone_Old AS VARCHAR(100)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Phone AS VARCHAR(100)), space(1)) + '" '
		+ '@Gender="' + coalesce(cast(@Gender_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Gender AS VARCHAR(100)), space(1)) + '" '
		+ '@GroupId="' + coalesce(cast(@GroupId_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@GroupId AS VARCHAR(100)), space(1)) + '" '
		+ '@Roles="' + coalesce(cast(@Roles_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Roles AS VARCHAR(100)), space(1)) + '" '
		+ '@ManagerId="' + coalesce(cast(@ManagerId_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@ManagerId AS VARCHAR(100)), space(1)) + '" '
		+ '@JoinedDate="' + coalesce(cast(@JoinedDate_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@JoinedDate AS VARCHAR(100)), space(1)) + '" '
		+ '@ConfirmedDate="' + coalesce(cast(@ConfirmedDate_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@ConfirmedDate AS VARCHAR(100)), space(1)) + '" '
		+ '@ResignedDate="' + coalesce(cast(@ResignedDate_Old AS VARCHAR(100)), space(1)) + '" ' + '"#@#' + coalesce(cast(@ResignedDate AS VARCHAR(100)), space(1)) + '" '
		+ '@Skype="' + coalesce(cast(@Skype_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Skype AS VARCHAR(100)), space(1)) + '" '
		+ '@Yahoo="' + coalesce(cast(@Yahoo_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Yahoo AS VARCHAR(100)), space(1)) + '" '
		+ '@Marriage="' + coalesce(cast(@Marriage_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Marriage AS VARCHAR(100)), space(1)) + '" '
		+ '@LastLogin="' + coalesce(cast(@LastLogin_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@LastLogin AS VARCHAR(100)), space(1)) + '" '
		+ '@ModifiedBy="' + coalesce(cast(@ModifiedBy_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@ModifiedBy AS VARCHAR(100)), space(1)) + '" '
		+ '@isDeleted="' + coalesce(cast(@isDeleted_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@isDeleted AS VARCHAR(100)), space(1)) + '" '
		+ '@PermanentAddress="' + coalesce(cast(@PermanentAddress_Old AS NVARCHAR(250)), space(1)) + '" ' + '"#@#' + coalesce(cast(@PermanentAddress AS NVARCHAR(250)), space(1)) + '" '
		+ '@CurrentAddress="' + coalesce(cast(@CurrentAddress_Old AS NVARCHAR(250)), space(1)) + '" ' + '"#@#' + coalesce(cast(@CurrentAddress AS NVARCHAR(250)), space(1)) + '" '
		+ '@IdCardNumber="' + coalesce(cast(@IdCardNumber_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@IdCardNumber AS VARCHAR(100)), space(1)) + '" '
		+ '@PassportNo="' + coalesce(cast(@PassportNo_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@PassportNo AS VARCHAR(100)), space(1)) + '" '
		+ '@PassportExp="' + coalesce(cast(@PassportExp_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@PassportExp AS VARCHAR(100)), space(1)) + '" '
		+ '@ChineseName="' + coalesce(cast(@ChineseName_Old AS NVARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@ChineseName AS NVARCHAR(100)), space(1)) + '" '
		+ '@PlaceOfbirth="' + coalesce(cast(@PlaceOfbirth_Old AS NVARCHAR(250)), space(1)) + '" ' + '"#@#' + coalesce(cast(@PlaceOfbirth AS NVARCHAR(250)), space(1)) + '" '
		+ '@Native="' + coalesce(cast(@Native_Old AS VARCHAR(100)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Native AS VARCHAR(100)), space(1)) + '" '
		+ '@Ethnic="' + coalesce(cast(@Ethnic_Old AS NVARCHAR(250)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Ethnic AS NVARCHAR(250)), space(1)) + '" '
		+ '@Religion="' + coalesce(cast(@Religion_Old AS NVARCHAR(250)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Religion AS NVARCHAR(250)), space(1)) + '" '
		+ '@DateOfIDCard="' + coalesce(cast(@DateOfIDCard_Old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@DateOfIDCard AS VARCHAR(100)), space(1)) + '" '
		+ '@PlaceOfIDCard="' + coalesce(cast(@PlaceOfIDCard_Old AS NVARCHAR(250)), space(1)) + '" ' + '"#@#' + coalesce(cast(@PlaceOfIDCard AS NVARCHAR(250)), space(1)) + '" '
		+ '@HomePhone="' + coalesce(cast(@HomePhone_Old AS NVARCHAR(250)), space(1)) + '" ' + '"#@#' + coalesce(cast(@PlaceOfIDCard AS NVARCHAR(250)), space(1)) + '" '
		+ '@Qualification="' + coalesce(cast(@Qualification_Old AS NVARCHAR(250)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Qualification AS NVARCHAR(250)), space(1)) + '" '
		+ '@Major="' + coalesce(cast(@Major_Old AS NVARCHAR(100)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Major AS NVARCHAR(100)), space(1)) + '" '
		+ '@CanOT="' + coalesce(cast(@CanOT_old AS VARCHAR(100)), space(1)) + '" ' + '"#@#' + coalesce(cast(@CanOT AS VARCHAR(100)), space(1)) + '" '
		+ '@SpecialOT="' + coalesce(cast(@SpecialOT_old AS VARCHAR(100)), space(1)) + '" ' + '"#@#' + coalesce(cast(@SpecialOT AS VARCHAR(100)), space(1)) + '" '
		+ '@IsLocal="' + coalesce(cast(@IsLocal_old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@IsLocal AS VARCHAR(100)), space(1)) + '" '
		+ '@WorkingDays="' + coalesce(cast(@WorkingDays_old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@WorkingDays AS VARCHAR(100)), space(1)) + '" '
		+ '@WorkingHours="' + coalesce(cast(@WorkingHours_old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@WorkingHours AS VARCHAR(100)), space(1)) + '" '
		+ '@IsTopManager="' + coalesce(cast(@IsTopManager_old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@IsTopManager AS VARCHAR(100)), space(1)) + '" '
		+ '@CompanyDate="' + coalesce(cast(@CompanyDate_old AS VARCHAR(1000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@CompanyDate AS VARCHAR(100)), space(1)) + '" '
		+ '@CountryCode="' + coalesce(cast(@CountryCode_old AS NVARCHAR(100)), space(1)) + '" ' + '"#@#' + coalesce(cast(@CountryCode AS VARCHAR(100)), space(1)) + '" '
		+ '@Unit="' + coalesce(cast(@Unit_old AS NVARCHAR(2000)), space(1)) + '" ' + '"#@#' + coalesce(cast(@Unit AS NVARCHAR(2000)), space(1)) + '" '
		+ '@IsExpatUseHoliday="' + coalesce(cast(@IsExpatUseHoliday_old AS NVARCHAR(100)), space(1)) + '" ' + '"#@#' + coalesce(cast(@IsExpatUseHoliday AS VARCHAR(100)), space(1)) + '" '
		+ '@PickupRemark="' + coalesce(cast(@PickupRemark_old AS NVARCHAR(200)), space(1)) + '" ' + '"#@#' + coalesce(cast(@PickupRemark AS NVARCHAR(200)), space(1)) + '" />'

	EXEC dbo.Log_WriteLog @LogActionID = 500
		,@LogDate = @now
		,@Description = 'Update Employee Information'
		,@TrackingValue = @LogSPData
		,@UserID = @ModifiedBy
		,@EmpId = @EmpId
END
