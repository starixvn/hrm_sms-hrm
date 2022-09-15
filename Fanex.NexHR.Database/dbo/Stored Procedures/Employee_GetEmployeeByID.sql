
CREATE PROCEDURE [dbo].[Employee_GetEmployeeByID] @Empid INT
AS
/*
Created: Tom.Vo@2013-10-21
Task: get employee by id
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @KIVLeaveBalance NUMERIC(18, 2)
		,@AnnualLeaveBalance NUMERIC(18, 2)
		,@Today DATE = GETDATE()

	SELECT @KIVLeaveBalance = dbo.fnc_GetKIVByDayBalance(@EmpID, @Today, NULL)
		,@AnnualLeaveBalance = dbo.fnc_GetAnnualLeaveBalanceByYear(@EmpId, YEAR(@Today), NULL)

	SELECT e.EmpId
		,e.Code
		,e.DomainUsername
		,e.FullName
		,e.NickName
		,e.EmployeeCard
		,e.DOB AS DateOfBirth
		,e.Email
		,e.[Enabled]
		,e.Phone
		,e.Gender
		,e.TeamDate
		,e.PromotionDate
		,e.JoinedDate
		,e.ConfirmedDate
		,e.ResignedDate
		,empsum.CompanyDate
		,empsum.WorkingDays
		,empsum.WorkingHours
		,empsum.WorkingHoursbyWeek
		,empsum.MilitaryFromDate
		,empsum.MilitaryToDate
		,e.Skype
		,e.LetsTalk
		,e.Marriage
		,e.LastLogin
		,@AnnualLeaveBalance AS AnnualLeaveBalance
		,@KIVLeaveBalance AS KeepInViewBalance
		,empsum.CountryCode
		,ct.CountryName
		,empsum.CanOT
		,empsum.SpecialOT
		,empsum.IsLocal
		,empsum.IsExpatUseHoliday
		,e.CreatedDate
		,e.CreatedBy
		,e.ModifiedDate
		,e.ModifiedBy
		,e.isDeleted
		,e.PermanentAddress AS [Address]
		,e.CurrentAddress
		,e.IdCardNumber AS CardNo
		,e.PassportNo
		,e.PassportExp
		,e.[Password]
		,e.ChineseName
		,empsum.IsDefaultSchedule
		,empsum.IsTopManager
		,empsum.OffDaysbyWeek
		,empsum.Unit
		,e.PlaceOfbirth
		,e.Ethnic
		,e.[Native]
		,e.Religion
		,e.DateOfIDCard
		,e.PlaceOfIDCard
		,e.HomePhone
		,e.Qualification
		,e.Major
		,e_cs.[BankAccount]
		,e_cs.[TaxCode]
		,e_cs.[SocialInsuranceNo]
		,e_cs.[RegisteredHospital]
		,e_cs.[Contract1]
		,e_cs.[FromDate1]
		,e_cs.[ToDate1]
		,e_cs.[Contract2]
		,e_cs.[FromDate2]
		,e_cs.[ToDate2]
		,e_cs.[Contract3]
		,e_cs.[FromDate3]
		,e_cs.[ToDate3]
		,e_cs.[Contract4]
		,e_cs.[FromDate4]
		,e_cs.[ToDate4]
		,e_cs.[Contract5]
		,e_cs.[FromDate5]
		,e_cs.[ToDate5]
		,e_cs.History
		,e.PasswordReset
		,e.PickupRemark
		,e.JobTitleId
		,e.JobLevelId
		,lk.[Name] AS JobTitle
		,ISNULL(e.GroupId, - 1) AS [TeamId]
		,g.Code
		,g.[Name]
	FROM dbo.Employee e WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeSummary empsum WITH (NOLOCK) ON empsum.EmpId = e.EmpId
		LEFT JOIN dbo.EmployeeContract e_cs WITH (NOLOCK) ON e_cs.EmpId = e.EmpId
		LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		LEFT JOIN dbo.Country ct WITH (NOLOCK) ON empsum.CountryCode = ct.CountryCode
		LEFT JOIN dbo.LookupData lk WITH (NOLOCK) ON e.JobTitleId = lk.Id
		AND lk.LookupId = 1
	WHERE e.EmpId = @Empid
END
