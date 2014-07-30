// --------------------------------------------------------------------------
// <copyright file="UpdateEmployeeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Employee Criteria.
    /// </summary>
    public class UpdateEmployeeCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public string Code { get; set; }

        public string DomainUsername { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        [SpParam(SqlParamName = "DOB")]
        public DateTime? DateOfBirth { get; set; }

        public string Email { get; set; }

        public int? JobTitleId { get; set; }

        public bool Enabled { get; set; }

        public string Phone { get; set; }

        public byte? Gender { get; set; }

        public int GroupId { get; set; }

        public string Roles { get; set; }

        public int ManagerId { get; set; }

        public DateTime? JoinedDate { get; set; }

        public DateTime? ConfirmedDate { get; set; }

        public DateTime? ResignedDate { get; set; }

        public string Skype { get; set; }

        public string Yahoo { get; set; }

        public byte Marriage { get; set; }

        public DateTime? LastLogin { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string PermanentAddress { get; set; }

        public string CurrentAddress { get; set; }

        public string IdCardNumber { get; set; }

        public string PassportNo { get; set; }

        public DateTime? PassportExp { get; set; }

        public string Password { get; set; }

        public bool PasswordReset { get; set; }

        public string ChineseName { get; set; }

        public string Unit { get; set; }

        public decimal AnnualLeaveBalance { get; set; }

        public decimal KeepInViewBalance { get; set; }

        public bool CanOT { get; set; }

        public bool IsLocal { get; set; }

        public int WorkingDays { get; set; }

        public int WorkingHours { get; set; }

        public DateTime? CompanyDate { get; set; }

        public string CountryCode { get; set; }

        public bool IsDefaultSchedule { get; set; }

        public bool IsTopManager { get; set; }

        public decimal? OffDaysbyWeek { get; set; }

        public bool IsExpatUseHoliday { get; set; }

        public string PlaceOfbirth { get; set; }

        public string Ethnic { get; set; }

        public string Native { get; set; }

        public string Religion { get; set; }

        public DateTime? DateOfIDCard { get; set; }

        public string PlaceOfIDCard { get; set; }

        public string HomePhone { get; set; }

        public string Qualification { get; set; }

        public string Major { get; set; }

        public string BankAccount { get; set; }

        public string TaxCode { get; set; }

        public string SocialInsuranceNo { get; set; }

        public string RegisteredHospital { get; set; }

        public byte Contract1 { get; set; }

        public DateTime? FromDate1 { get; set; }

        public DateTime? ToDate1 { get; set; }

        public byte Contract2 { get; set; }

        public DateTime? FromDate2 { get; set; }

        public DateTime? ToDate2 { get; set; }

        public byte Contract3 { get; set; }

        public DateTime? FromDate3 { get; set; }

        public DateTime? ToDate3 { get; set; }

        public byte Contract4 { get; set; }

        public DateTime? FromDate4 { get; set; }

        public DateTime? ToDate4 { get; set; }

        public byte Contract5 { get; set; }

        public DateTime? FromDate5 { get; set; }

        public DateTime? ToDate5 { get; set; }

        public string History { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_UpdateEmployee";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
