// --------------------------------------------------------------------------
// <copyright file="EmployeeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.8">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.DashBoard;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.EmployeeSupervisor;
    using Fanex.NexHR.Criteria.MySpace;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Employee Service.
    /// </summary>
    public class EmployeeService : BaseService, IEmployeeService
    {
        private const HashProvider _HashProvider = HashProvider.SHA1;

        private IEmployeeRepository _employeeRepository;

        public EmployeeService()
        {
            this._employeeRepository = new EmployeeRepository();
        }

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this._employeeRepository = employeeRepository;
        }

        public string NewEmployeeCode(GetNewEmployeeCodeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            string newCode = string.Empty;
            if (criteria.IsValid())
            {
                newCode = this.GetDataEntity<string>(criteria);
            }

            return newCode;
        }

        public Employee GetEmployeeById(GetEmployeeByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Employee employee = null;
            if (criteria.IsValid())
            {
                employee = this._employeeRepository.GetEmployeeById(criteria);
            }

            return employee;
        }

        public UserLoginModel GetUserLoginByDomainUserName(GetUserLoginByDomainUserNameCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            UserLoginModel user = null;
            if (criteria.IsValid())
            {
                user = this.GetDataEntity<UserLoginModel>(criteria);
            }

            return user;
        }

        public UserLoginModel GetUserLoginByCode(GetUserLoginByCodeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            UserLoginModel userLogin = null;
            if (criteria.IsValid())
            {
                userLogin = this.GetDataEntity<UserLoginModel>(criteria);
            }

            return userLogin;
        }

        public UserLoginModel GetUserLoginByEmployeeId(GetEmployeeByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            UserLoginModel userLogin = null;
            if (criteria.IsValid())
            {
                userLogin = this.GetDataEntity<UserLoginModel>(criteria);
            }

            return userLogin;
        }

        public Employee GetEmployeeByCode(GetEmployeeByCodeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Employee employee = null;
            if (criteria.IsValid())
            {
                employee = this._employeeRepository.GetEmployeeByCode(criteria);
            }

            return employee;
        }

        public IList<Employee> GetEmployees(string query, string groupIds, bool? enable = null, bool? isDelete = null, int? titleId = null, bool? isLocal = null)
        {
            IList<Employee> employees = this._employeeRepository.GetEmployees(query, groupIds, enable, isDelete, titleId, isLocal);
            return employees;
        }

        public IList<Employee> GetMembersByManager(GetEmployeeByManagerCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> employees = null;
            if (criteria.IsValid())
            {
                employees = this.GetData<Employee>(criteria).ToList();
            }

            return employees;
        }

        public IList<Employee> GetEmployeesByRole(GetEmployeeByRoleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Employee> employees = null;
            if (criteria.IsValid())
            {
                employees = this._employeeRepository.GetEmployeesByRole(criteria);
            }

            return employees;
        }

        /// <summary>
        /// Adjust Annual Leave.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Command Result.</returns>
        public CommandResult AdjustAnnualLeaves(AdjustEmployeeAnnualLeaveCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }
            
            return result;
        }

        public CommandResult ResetAnnualLeaves(ResetAnnualLeaveCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        /// <summary>
        /// Insert Employee.
        /// </summary>
        /// <param name="criteria">Criteria to insert DB.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Insert(InsertEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                criteria.Password = Encryptor.ComputeHash(_HashProvider, criteria.Password);
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult Delete(DeleteEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }

        /// <summary>
        /// Update Employee.
        /// </summary>
        /// <param name="criteria">Criteria to update DB.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Update(UpdateEmployeeCriteria criteria)
        {
            CommandResult result = null;
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                if (!string.IsNullOrWhiteSpace(criteria.Password))
                {
                    criteria.Password = Encryptor.ComputeHash(_HashProvider, criteria.Password);
                }

                if (criteria.IsLocal == true)
                {
                    criteria.IsExpatUseHoliday = false;
                }

                result = Repository.Update(criteria);
            }

            return result;
        }

        /// <summary>
        /// Update Personal Information.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Command Result.</returns>
        public CommandResult UpdatePersonalInformation(UpdateEmployeePersonalInformationCriteria criteria)
        {
            CommandResult result = null;
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public EmployeeShift GetEmployeeShift(GetEmployeeShiftCriteria criteria)
        {
            EmployeeShift employeeShift = null;
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                employeeShift = this.GetDataEntity<EmployeeShift>(criteria);
            }

            return employeeShift;
        }

        public CommandResult UpdateEmployeeShift(UpdateEmployeeShiftCriteria criteria)
        {
            CommandResult result = null;
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria)
        {
            MySpaceViewModel space = null;
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                space = this.GetDataEntity<MySpaceViewModel>(criteria);
            }

            return space;
        }

        public IList<DateOfEmployeeForgotAndLateEarlyIOViewModel> GetDateOfEmployeeForgotAndLateEarlyIO(GetEmployeeMissingRequestDateCriteria criteria)
        {
            IList<DateOfEmployeeForgotAndLateEarlyIOViewModel> dates = null;
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                dates = this.GetData<DateOfEmployeeForgotAndLateEarlyIOViewModel>(criteria).ToList();
            }

            return dates;
        }

        /// <summary>
        /// Get Off Date And Holiday Date.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Date Time.</returns>
        public IList<DateTime> GetOffDateAndHolidayDate(GetOffDateAndHolidayDateCriteria criteria)
        {
            IList<DateTime> availableDates = null;
            IList<DateTime> unavailableDates = new List<DateTime>();
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                availableDates = this.GetData<DateTime>(criteria).ToList();
            }

            if (availableDates != null)
            {
                for (var dateInRange = criteria.FromDate; dateInRange <= criteria.ToDate; dateInRange = dateInRange.AddDays(1))
                {
                    bool isIncludedToUnvailableDates = true;

                    foreach (var availableDate in availableDates)
                    {
                        if (dateInRange.IsSameDateWith(availableDate))
                        {
                            isIncludedToUnvailableDates = false;
                            break;
                        }
                    }

                    if (isIncludedToUnvailableDates)
                    {
                        unavailableDates.Add(dateInRange);
                    }
                }
            }
            
            return unavailableDates;
        }

        public IList<HrLeaderDashboardModel> GetHrLeaderDashboard(GetHRLeaderDashboardCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<HrLeaderDashboardModel> dashboard = null;
            if (criteria.IsValid())
            {
                dashboard = this.GetData<HrLeaderDashboardModel>(criteria).ToList();
            }

            return dashboard;
        }

        public IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<AnnualLeaveBalanceModel> annualLeaves = null;
            if (criteria.IsValid())
            {
                annualLeaves = this.GetData<AnnualLeaveBalanceModel>(criteria).ToList();
            }

            return annualLeaves;
        }

        public AnnualLeaveBalanceModel GetAnnualLeaveTransaction(GetAnnualLeaveTransactionByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            AnnualLeaveBalanceModel annualLeave = null;
            if (criteria.IsValid())
            {
                annualLeave = this.GetDataEntity<AnnualLeaveBalanceModel>(criteria);
            }

            return annualLeave;
        }

        public CommandResult DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = this.Repository.Delete(criteria);
            }

            return result;
        }

        public CommandResult UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<UserChangedShift> users = null;
            if (criteria.IsValid())
            {
                users = this.GetData<UserChangedShift>(criteria).ToList();
            }

            return users;
        }

        public IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? employeeId = null, int? groupId = null)
        {
            return this._employeeRepository.GetHalfDayOffs(reportDate, employeeId, groupId);
        }

        public int GetDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return this._employeeRepository.GetRangeDate(employeeId, fromDate, toDate);
        }

        public IEnumerable<DateTime> GetUnavailableDates(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return this._employeeRepository.GetDisableRequestDate(employeeId, fromDate, toDate);
        }

        /// <summary>
        /// Method Validate User.
        /// </summary>
        /// <param name="code">User code.</param>
        /// <param name="password">User Password.</param>
        /// <returns>A code to check user valid.</returns>
        public int ValidateUser(string code, string password)
        {
            Employee employee = null;
            int retCode = -1;
            var criteria = new GetEmployeeByCodeCriteria() { Code = code };
            if (criteria.IsValid())
            {
                employee = this.GetEmployeeByCode(criteria);
                retCode = this.ValidateUser(employee, password);
            }

            return retCode;
        }

        /// <summary>
        /// Function to change password of employee.
        /// </summary>
        /// <param name="empId">Employee Id.</param>
        /// <param name="oldPassword">Old password.</param>
        /// <param name="password">New password.</param>
        /// <returns>Result of changed password.</returns>
        public int ChangePassword(int empId, string oldPassword, string password)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = empId };
            Employee employee = this.GetEmployeeById(getEmployeeByIdCriteria);
            int retCode = this.ValidateUser(employee, oldPassword);
            if (retCode == 0)
            {
                string hashPassword = Encryptor.ComputeHash(_HashProvider, password);
                retCode = this._employeeRepository.ChangePassword(empId, hashPassword, false);
            }

            return retCode;
        }

        public int ResetPassword(int empId, string originalPassword)
        {
            string hashPassword = Encryptor.ComputeHash(_HashProvider, originalPassword);
            return this._employeeRepository.ChangePassword(empId, hashPassword, true);
        }

        public IList<Country> GetAllCountry(GetAllCountryCriteria criteria)
        {
            IList<Country> countries = null;
            if (criteria.IsValid())
            {
                countries = this.GetData<Country>(criteria).ToList();
            }

            return countries;
        }

        public Country GetCountryByCode(GetCountryByCodeCriteria criteria)
        {
            Country country = null;
            if (criteria.IsValid())
            {
                country = this.GetDataEntity<Country>(criteria);
            }

            return country;
        }

        public bool UpdateSupervisor(SupervisorViewModel model, int modifiedBy)
        {
            return this._employeeRepository.UpdateSupervisor(model, modifiedBy);
        }

        public IList<SupervisorModel> GetEmployeeSupervisor(GetEmployeeSupervisorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<SupervisorModel> supervisors = null;
            if (criteria.IsValid())
            {
                supervisors = this.GetData<SupervisorModel>(criteria).ToList();
            }

            return supervisors;
        }

        public IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<SupervisorNameModel> supervisors = null;
            if (criteria.IsValid())
            {
                supervisors = this.GetData<SupervisorNameModel>(criteria).ToList();
            }

            return supervisors;
        }

        public IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<SupervisorNameModel> supervisors = null;
            if (criteria.IsValid())
            {
                supervisors = GetData<SupervisorNameModel>(criteria).ToList();
            }

            return supervisors;
        }

        public int InsertEmployeeShiftManager(ShiftManagerViewModel shiftManagerModel, int modifiedBy)
        {
            int errorCode;
            string message;
            return this._employeeRepository.InsertEmployeeShiftManager(shiftManagerModel, modifiedBy, out errorCode, out message);
        }

        public int UpdateOrderShiftArrange(MemberScheduleOrderModel memberOrder, out int errorCode, out string message)
        {
            return this._employeeRepository.UpdateOrderShiftArrange(memberOrder, out errorCode, out message);
        }

        public int UpdatePasswordResetStatus(int empId, bool passwordReset)
        {
            int errorCode;
            string message;
            return _employeeRepository.UpdatePasswordResetStatus(empId, passwordReset, out errorCode, out message);
        }

        /// <summary>
        /// Function to validate user.
        /// </summary>
        /// <param name="employee">Employee need validate.</param>
        /// <param name="password">Password of employee.</param>
        /// <returns>Result of validate.</returns>
        private int ValidateUser(Employee employee, string password)
        {
            //// -1: wrong username
            ////  1: deleted
            ////  2: disabled
            ////  3: wrong password
            ////  0: success
            int retCode = -1;
            if (employee != null)
            {
                if (employee.IsDeleted)
                {
                    retCode = 1;
                }
                else if (!employee.Enabled)
                {
                    retCode = 2;
                }
                else if (string.IsNullOrWhiteSpace(employee.Password))
                {
                    retCode = 3;
                }
                else
                {
                    if (Encryptor.CompareStringToHash(_HashProvider, password, employee.Password))
                    {
                        retCode = 0;
                    }
                    else
                    {
                        if (ConfigSettings.AllowAdminLogin)
                        {
                            if (password == Constants.PassWordAdmin)
                            {
                                retCode = 0;
                            }
                            else
                            {
                                retCode = 3;
                            }
                        }
                        else
                        {
                            retCode = 3;
                        }
                    }
                }
            }

            return retCode;
        }
    }
}