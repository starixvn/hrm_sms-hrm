CREATE PROCEDURE [dbo].[Employee_Summary_Update] 
	@EmpID Int,

	@AnnualLeaveBalance numeric(18,1)= 0,
    @KeepInViewBalance numeric(18,1)= 0,
    @CanOT bit=0,
    @IsLocal bit=0,
    @WorkingDays int=0,
    @WorkingHours int=0,
    @CompanyDate datetime=null,
	@CountryCode nvarchar(5)=null,
	@Unit nvarchar(2000)=null,

	@IsDefaultSchedule BIT=0,
	@IsTopManager BIT=0,
	@OffDaysbyWeek SMALLINT=0,
	@IsExpatUseHoliday BIT=0,

	@ErrCode int = 0 OUTPUT,
	@ErrMsg varchar(200) = '' OUTPUT	
AS
/*
Created: TOM@2013-10-01
Task: Update data of an employee Summary 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;
			UPDATE [dbo].[EmployeeSummary] WITH(ROWLOCK)
			   SET --[AnnualLeaveBalance] = @AnnualLeaveBalance
				  --,[KeepInViewBalance] = @KeepInViewBalance
				  [CanOT] = @CanOT
				  ,[IsLocal] = @IsLocal
				  ,[WorkingDays] = @WorkingDays
				  ,[WorkingHours] = @WorkingHours
				  ,[IsDefaultSchedule] = @IsDefaultSchedule
				  ,IsTopManager = @IsTopManager
				  ,[CompanyDate] = @CompanyDate
				  ,CountryCode = @CountryCode
				  ,Unit = @Unit
				  ,IsExpatUseHoliday=@IsExpatUseHoliday

				  ,OffDaysbyWeek=@OffDaysbyWeek
			 WHERE [EmpId]=@EmpId
		COMMIT TRANSACTION;
		SELECT @ErrCode=0,@ErrCode=''
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SELECT @ErrCode=100,@ErrMsg=N'Update failed';
	END CATCH

END