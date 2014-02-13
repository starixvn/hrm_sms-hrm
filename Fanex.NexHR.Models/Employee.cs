// --------------------------------------------------------------------------
// <copyright file="Employee.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Attributes;
    using Fanex.NexHR.Core.Enums;

    /// <summary>
    /// Employee Class.
    /// </summary>
    public class Employee : EntityBase<int>
    {
        public int EmpId { get; set; }

        public string DomainUsername { get; set; }

        [Required]
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ChineseName { get; set; }

        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

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

        public string Roles { get; set; }

        public Employee Manager { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JoinedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ConfirmedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ResignedDate { get; set; }

        public string Skype { get; set; }

        public string Yahoo { get; set; }

        public byte Marriage { get; set; }

        public string MarriageName
        {
            get
            {
                return ((MarriageStatus)Marriage).ToString();
            }
        }

        public DateTime? LastLogin { get; set; }

        public decimal AnnualLeaveBalance { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee Creator { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Employee Modifier { get; set; }

        [Display(Name = "Teams' leaders receive requests")]
        public string LeaderGroups { get; set; } ////remove

        public string JobTitle { get; set; }

        [Display(Name = "Permanent Address")]
        public string Address { get; set; }

        public string CurrentAddress { get; set; }

        public string CardNo { get; set; }

        public string PassportNo { get; set; }

        public DateTime? PassportExp { get; set; }

        ////Add new field
        public string Verifier { get; set; }

        public string Approver { get; set; }

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

        public List<SelectListItem> ListCountryCode { get; set; }

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

        #region Contract
        public string BankAccount { get; set; }

        public string TaxCode { get; set; }

        public string SocialInsuranceNo { get; set; }

        public string RegisteredHospital { get; set; }

        [Display(Name = "Contract 1")]
        public byte Contract1 { get; set; }

        public DateTime? FromDate1 { get; set; }

        public DateTime? ToDate1 { get; set; }

        [Display(Name = "Contract 2")]
        public byte Contract2 { get; set; }

        public DateTime? FromDate2 { get; set; }

        public DateTime? ToDate2 { get; set; }

        [Display(Name = "Contract 3")]
        public byte Contract3 { get; set; }

        public DateTime? FromDate3 { get; set; }

        public DateTime? ToDate3 { get; set; }

        [Display(Name = "Contract 4")]
        public byte Contract4 { get; set; }

        public DateTime? FromDate4 { get; set; }

        public DateTime? ToDate4 { get; set; }

        [Display(Name = "Contract 5")]
        public byte Contract5 { get; set; }

        public DateTime? FromDate5 { get; set; }

        public DateTime? ToDate5 { get; set; }

        public string History { get; set; }

        public string ExtensionNo { get; set; }

        #endregion

        public IList<short> RoleList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    return null;
                }

                var roles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var result = new List<short>();

                foreach (var item in roles)
                {
                    result.Add(short.Parse(item));
                }

                return result;
            }

            set
            {
            }
        }

        public string[] RoleArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    return null;
                }

                return Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            set
            {
                Roles = Utilities.StringArrayToString(value);
            }
        }

        public string[] LeaderGroupsArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LeaderGroups))
                {
                    return null;
                }

                return LeaderGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            set
            {
                LeaderGroups = Utilities.StringArrayToString(value);
            }
        }

        public string DisplayName
        {
            get
            {
                return (string.IsNullOrEmpty(NickName) || string.IsNullOrEmpty(FullName))
                ? string.Empty
                : NickName + " (" + FullName + ")";
            }
        }

        public string MemberRoleAbbreviation
        {
            get
            {
                string roles = string.Empty;
                if (Roles == null || Roles == string.Empty || (RoleList.Count == 1 && RoleList[0] == (short)RolesEnum.Member))
                {
                    return roles = "Member";
                }
                else
                {
                    var lastItem = RoleList.Last();
                    foreach (var item in RoleList)
                    {
                        switch (item)
                        {
                            case 2:
                                roles += ", Verifier";
                                break;
                            case 3:
                                roles += ", Approver";
                                break;
                            case 4:
                                roles += ", SM";
                                break;
                            case 5:
                                roles += ", AS";
                                break;
                            case 6:
                                roles += ", HR";
                                break;
                            case 7:
                                roles += ", SA";
                                break;
                        }
                    }

                    roles = roles.Remove(0, roles.IndexOf(' ') + 1);
                }

                return roles;
            }
        }

        /// <summary>
        /// Check user is in specific roles.
        /// </summary>
        /// <param name="roles">Role of user.</param>
        /// <returns>Return Boolean.</returns>
        public bool IsInRoles(params RolesEnum[] roles)
        {
            if (RoleList == null)
            {
                return false;
            }

            var userRoles = System.Web.Security.Roles.GetRolesForUser();
            return roles.Any(role => userRoles.Contains(role.EnumToString()));
        }

        /// <summary>
        /// Check user is in specific roles.
        /// </summary>
        /// <param name="roles">Role of user.</param>
        /// <returns>Return Boolean.</returns>
        public bool IsInRoles(IList<string> roles)
        {
            if (roles == null || roles.Count == 0)
            {
                return false;
            }

            var userRoles = System.Web.Security.Roles.GetRolesForUser();
            return roles.Any(role => userRoles.Contains(role));
        }

        /// <summary>
        /// Display role for Employee.
        /// </summary>
        /// <returns>String role of member.</returns>
        public string MemberRole()
        {
            string roles = string.Empty;
            if (Roles == null || Roles == string.Empty || (RoleList.Count == 1 && RoleList[0] == (short)RolesEnum.Member))
            {
                return roles = "Member";
            }
            else
            {
                var lastItem = RoleList.Last();
                foreach (var item in RoleList)
                {
                    switch (item)
                    {
                        case 2:
                            roles += ", Verifier";
                            break;
                        case 3:
                            roles += ", Approver";
                            break;
                        case 4:
                            roles += ", Shift Manager";
                            break;
                        case 5:
                            roles += ", Acting Supervisor";
                            break;
                        case 6:
                            roles += ", Human Resource";
                            break;
                        case 7:
                            roles += ", System Admin";
                            break;
                    }
                }

                roles = roles.Remove(0, roles.IndexOf(' ') + 1);
            }

            return roles;
        }

        public bool IsMember()
        {
            return Roles == null || Roles == string.Empty || (RoleList.Count == 1 && RoleList[0] == (short)RolesEnum.Member);
        }
    }
}
