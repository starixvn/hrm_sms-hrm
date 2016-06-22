CREATE PROCEDURE [dbo].[Employee_GetEmployeeByID] --652
	@Empid INT
AS
/*
Created: Tom.Vo@2013-10-21
Task: get employee by code
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	select
		e.EmpId AS ID,
		e.Code,
		e.DomainUsername,
		e.FullName,
		e.NickName,
		e.EmployeeCard,
		e.DOB AS DateOfBirth,
		e.Email,
		e.JobTitleId,
		e.Enabled,
		e.Phone,
		e.Gender,
		e.GroupId,
		g.DepartmentId,
		e.Roles,
		e.ManagerId,
		e.JoinedDate,
		e.ConfirmedDate,
		e.ResignedDate,
		e_s.CompanyDate,
		e_s.WorkingDays,
		e_s.WorkingHours,
		e_s.CountryCode,
		e.Skype,
		e.Yahoo,
		e.Marriage,
		e.LastLogin,
		e_s.AnnualLeaveBalance,
		e_s.KeepInViewBalance,
		e_s.CanOT,
		e_s.IsLocal,
		e_s.IsExpatUseHoliday,
		e.CreatedDate,
		e.CreatedBy,
		e.ModifiedDate,
		e.ModifiedBy,
		e.isDeleted,
		e.PermanentAddress as Address,
		e.CurrentAddress,
		e.IdCardNumber as CardNo,
		e.PassportNo,
		e.PassportExp,
		e.Password,
		e.ChineseName,
		e_s.IsDefaultSchedule,
		e_s.IsTopManager,
		e_s.OffDaysbyWeek,
		e_s.Unit,
		e.PlaceOfbirth,
		e.Ethnic,
		e.[Native],
		e.Religion,
		e.DateOfIDCard,
		e.PlaceOfIDCard,
		e.HomePhone,
		e.Qualification,
		e.Major,
        e_cs.[BankAccount],
        e_cs.[TaxCode],
        e_cs.[SocialInsuranceNo],
        e_cs.[RegisteredHospital],
           
		e_cs.[Contract1],
        e_cs.[FromDate1],
        e_cs.[ToDate1],
           
		e_cs.[Contract2],
        e_cs.[FromDate2],
        e_cs.[ToDate2],
           
		e_cs.[Contract3],
        e_cs.[FromDate3],
        e_cs.[ToDate3],
           
		e_cs.[Contract4],
        e_cs.[FromDate4],
        e_cs.[ToDate4],

        e_cs.[Contract5],
        e_cs.[FromDate5],
        e_cs.[ToDate5],
		e_cs.History ,
		e.PasswordReset,
		isnull(e.GroupID, -1) as [TeamID], isnull(g.Name,'') as [Team]
		
	from dbo.Employee e WITH (NOLOCK) 
		LEFT JOIN dbo.EmployeeSummary e_s WITH (NOLOCK) ON e_s.EmpId = e.EmpId
		LEFT JOIN dbo.EmployeeContract e_cs WITH (NOLOCK) ON e_cs.EmpId = e.EmpId 
		LEFT JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
	where e.EmpId = @Empid
END