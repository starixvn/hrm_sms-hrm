
CREATE PROCEDURE [dbo].[Employee_Summary_Update] @EmpID INT
	,@AnnualLeaveBalance NUMERIC(18, 1) = 0
	,@KeepInViewBalance NUMERIC(18, 1) = 0
	,@CanOT BIT = 0
	,@SpecialOT DECIMAL(3, 1) = NULL
	,@IsLocal BIT = 0
	,@WorkingDays INT = 0
	,@WorkingHours INT = 0
	,@WorkingHoursbyWeek INT = 0
	,@CompanyDate DATETIME = NULL
	,@CountryCode NVARCHAR(5) = NULL
	,@Unit NVARCHAR(2000) = NULL
	,@IsDefaultSchedule BIT = 0
	,@IsTopManager BIT = 0
	,@OffDaysbyWeek SMALLINT = 0
	,@IsExpatUseHoliday BIT = 0
	,@MilitaryFromDate DATETIME = NULL
	,@MilitaryToDate DATETIME = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
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

		UPDATE [dbo].[EmployeeSummary] WITH (ROWLOCK)
		SET [CanOT] = @CanOT
			,SpecialOT = @SpecialOT
			,[IsLocal] = @IsLocal
			,[WorkingDays] = @WorkingDays
			,[WorkingHours] = @WorkingHours
			,[WorkingHoursbyWeek] = @WorkingHoursbyWeek
			,[IsDefaultSchedule] = @IsDefaultSchedule
			,IsTopManager = @IsTopManager
			,[CompanyDate] = @CompanyDate
			,CountryCode = @CountryCode
			,Unit = @Unit
			,IsExpatUseHoliday = @IsExpatUseHoliday
			,OffDaysbyWeek = @OffDaysbyWeek
			,MilitaryFromDate = @MilitaryFromDate
			,MilitaryToDate = @MilitaryToDate
		WHERE [EmpId] = @EmpId

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 100
			,@ErrMsg = N'Update failed'
	END CATCH
END
