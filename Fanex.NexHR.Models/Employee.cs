namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Core.Enums;

#pragma warning disable S109 // Magic numbers should not be used
    public class Employee : EntityBase<int>
#pragma warning disable S109 // Magic numbers should not be used
    {
        public string DomainUsername { get; set; }

        [Required]
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string EmployeeCard { get; set; }

        public string ChineseName { get; set; }

        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; }

        public string Ethnic { get; set; }

        public string Native { get; set; }

        public string Religion { get; set; }

        public DateTime? DateOfIdCard { get; set; }

        public string PlaceOfIdCard { get; set; }

        public string Qualification { get; set; }

        public string Major { get; set; }

        public string HomePhone { get; set; }

        [Email]
        public string Email { get; set; }

        public int? JobTitleId { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public string Phone { get; set; }

        public byte? Gender { get; set; }

        public Team Team { get; set; } ////remove

        [DataType(DataType.Date)]
        public DateTime? SectionDate { get; set; }

        public Employee Manager { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JoinedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ConfirmedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ResignedDate { get; set; }

        public string Skype { get; set; }

        public string LetsTalk { get; set; }

        public byte Marriage { get; set; }

        public string MarriageName
        {
            get
            {
                return ((MarriageStatus)Marriage).ToString();
            }
        }

        [DataType(DataType.Date)]
        public DateTime? MilitaryFromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? MilitaryToDate { get; set; }

        public string Remark { get; set; }

        public DateTime? LastLogin { get; set; }

        public decimal AnnualLeaveBalance { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee Creator { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Employee Modifier { get; set; }

        public string JobTitle { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PromotionDate { get; set; }

        [Display(Name = "Permanent Address")]
        public string Address { get; set; }

        public string CurrentAddress { get; set; }

        public string CardNo { get; set; }

        public string PassportNo { get; set; }

        public DateTime? PassportExp { get; set; }

        ////Add new field
        public string Verifier { get; set; }

        public string Approver { get; set; }

        public string Disciplinarian { get; set; }

        [Display(Name = "Emp. Shift List")]
        public string EmpShiftList { get; set; }

        [Display(Name = "Acting Emp. List")]
        public string ActingEmpList { get; set; }

        public bool IsLocal { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompanyDate { get; set; }

        [Range(1, 31, ErrorMessage = "Working Days must be between 1 and 31")]
        public int WorkingDays { get; set; }

        [Range(1, 24, ErrorMessage = "Working Days must be between 1 and 24")]
        public int WorkingHours { get; set; }

        public decimal KeepInViewBalance { get; set; }

        public bool CanOT { get; set; }

        [Display(Name = "Special User")]
        public bool IsTopManager { get; set; }

        [Display(Name = "Default Calendar")]
        public bool IsDefaultSchedule { get; set; }

        public IList<Country> Coutries { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public bool IsShowLeftNavigation { get; set; }

        [Range(0, 5, ErrorMessage = "Number of Offdays must be between 0 and 5")]
        public decimal OffDaysbyWeek { get; set; }

        public string Unit { get; set; }

        [Required]
        public override string Code { get; set; }

        public bool AllowEditPersonalInformation { get; set; }

        public bool IsExpatUseHoliday { get; set; }

        public bool PasswordReset { get; set; }

        public List<SystemRoleModel> Roles { get; set; }

        public string[] Roles2Array
        {
            get
            {
                return GetRoles2Array();
            }

            set
            {
                {
                    Roles = value != null ? value.Select(i => new SystemRoleModel { RoleId = Convert.ToInt32(i) }).ToList() : new List<SystemRoleModel>();
                }
            }
        }

        public string Roles2String
        {
            get
            {
                return Roles == null ? string.Empty : string.Join(", ", Roles.Select(i => i.Name));
            }
        }

        #region Contract

        public string BankAccount { get; set; }

        public string TaxCode { get; set; }

        public string SocialInsuranceNo { get; set; }

        public string RegisteredHospital { get; set; }

        public string ExtensionNo { get; set; }

        #endregion Contract

        public string DisplayName
        {
            get
            {
                return (string.IsNullOrEmpty(NickName) || string.IsNullOrEmpty(FullName))
                ? string.Empty
                : NickName + " (" + FullName + ")";
            }
        }

        [Display(Name = "Pickup Remark")]
        public string PickupRemark { get; set; }

        private string[] GetRoles2Array()
        {
            return Roles?.Select(i => i.RoleId.ToString()).ToArray();
        }
    }
}