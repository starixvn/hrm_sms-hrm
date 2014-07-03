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
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Employee Service.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private const HashProvider _HashProvider = HashProvider.SHA1;

        private readonly IShiftRepository _shiftRepository;
        private IEmployeeRepository _employeeRepository;

        public EmployeeService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._shiftRepository = new ShiftRepository();
        }

        public EmployeeService(IEmployeeRepository employeeRepository, IShiftRepository shifRepository)
        {
            this._employeeRepository = employeeRepository;
            this._shiftRepository = shifRepository;
        }

        public string NewEmployeeCode
        {
            get
            {
                return _employeeRepository.NewEmployeeCode;
            }
        }

        public IList<Shift> GetAllShifts()
        {
            return this._shiftRepository.GetAll();
        }

        public Employee GetEmployeeById(int? employeeId = null)
        {
            return this._employeeRepository.GetEmployeeById(employeeId);
        }

        public UserLoginModel GetUserLoginByDomainUserName(string domainUsername = null)
        {
            return this._employeeRepository.GetUserLoginByDomainUserName(domainUsername);
        }

        public UserLoginModel GetUserLoginByCode(string code)
        {
            return this._employeeRepository.GetUserLoginByCode(code);
        }

        public UserLoginModel GetUserLoginByEmployeeId(int employeeId)
        {
            return this._employeeRepository.GetUserLoginByEmployeeId(employeeId);
        }

        public Employee GetEmployeeByCode(string code)
        {
            return this._employeeRepository.GetEmployeeByCode(code);
        }

        public IList<Employee> GetEmployees(string query, string groupIds, bool? enable = null, bool? isDelete = null, int? titleId = null, bool? isLocal = null)
        {
            IList<Employee> employees = this._employeeRepository.GetEmployees(query, groupIds, enable, isDelete, titleId, isLocal);
            return employees;
        }

        public IList<Employee> GetEmployeesForRelatedTeam(int employeeId)
        {
            return this._employeeRepository.GetEmployeesForRelatedTeam(employeeId);
        }

        public IList<Employee> GetMembersByManager(int managerId, short role, string keyword, int? groupId = null)
        {
            var members = this._employeeRepository.GetMembersByManager(managerId, role, keyword, groupId);

            return members;
        }

        public IList<Employee> GetEmployeesByRole(string roleIds, string query, string excludeIds)
        {
            var employees = this._employeeRepository.GetEmployeesByRole(roleIds, query, excludeIds);

            return employees;
        }

        /// <summary>
        /// Method adjust Annual leaves.
        /// </summary>
        /// <param name="modelState">AnnualLeave Model State.</param>
        /// <param name="empIds">Employees id.</param>
        /// <param name="description">Reason to adjust.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="deductDays">Deduct days.</param>
        /// <param name="creditDays">Credit days.</param>
        /// <param name="createdDate">Created date.</param>
        /// <param name="createdBy">Created by.</param>
        /// <returns>Boolean to check.</returns>
        public bool AdjustAnnualLeaves(
            ModelStateDictionary modelState,
            string empIds,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy)
        {
            int errorCode;
            string message;
            var result = this._employeeRepository.AdjustAnnualLeaves(
                empIds,
                description,
                fromDate,
                deductDays,
                creditDays,
                createdDate,
                createdBy,
                out errorCode,
                out message);
            if (!result)
            {
                modelState.AddModelError(string.Empty, message);
                return false;
            }

            return result;
        }

        public bool ResetAnnualLeaves(string empValue, string description, DateTime fromDate, int resetDays, DateTime createdDate, int createdBy)
        {
            return this._employeeRepository.ResetAnnualLeaves(
                empValue,
                description,
                fromDate,
                resetDays,
                createdDate,
                createdBy);
        }

        public IList<Employee> GetAll()
        {
            return this._employeeRepository.GetAll();
        }

        /// <summary>
        /// Insert new account.
        /// </summary>
        /// <param name="modelState">Model state.</param>
        /// <param name="model">CreateEmployee ViewModel.</param>
        /// <returns>Boolean result.</returns>
        public bool Insert(ModelStateDictionary modelState, CreateEmployeeViewModel model)
        {
            Check.Argument.IsNotNull(model, "model");

            if (modelState.IsValid)
            {
                int errorCode;
                string message;
                //// hash pasword
                model.Password = Encryptor.ComputeHash(_HashProvider, model.Password);
                this._employeeRepository.Insert(model, out errorCode, out message);
                if (errorCode == 0)
                {
                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, message);
                    return false;
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        public bool Delete(int id, DateTime resignedDate)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");
            return this._employeeRepository.Delete(id, resignedDate);
        }

        /// <summary>
        /// Update Employee.
        /// </summary>
        /// <param name="model">Employee Model.</param>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <returns>Error code.</returns>
        public int Update(Employee model, ModelStateDictionary modelState)
        {
            int errorCode;
            string message;
            Check.Argument.IsNotNull(model, "model");

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                model.Password = Encryptor.ComputeHash(_HashProvider, model.Password);
            }

            if (model.IsLocal == true)
            {
                model.IsExpatUseHoliday = false;
            }

            var updated = this._employeeRepository.Update(model, out errorCode, out message);
            if (errorCode != 0)
            {
                modelState.AddModelError(string.Empty, message);
            }

            return updated;
        }

        /// <summary>
        /// Update Employee.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="model">Employee Model.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// Error code.
        /// </returns>
        public bool UpdatePersonalInformation(ModelStateDictionary modelState, Employee model, out int errorCode, out string message)
        {
            bool isSuccessful = this._employeeRepository.UpdatePersonalInformation(model, out errorCode, out message);
            if (!isSuccessful)
            {
                modelState.AddModelError(errorCode.ToString(), message);
            }

            return isSuccessful;
        }

        public EmployeeShift GetEmployeeShift(int id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");
            return this._employeeRepository.GetEmployeeShift(id);
        }

        public bool UpdateEmployeeShift(EmployeeShift model)
        {
            Check.Argument.IsNotNull(model, "modmodelel");
            return this._employeeRepository.UpdateEmployeeShift(model);
        }

        public IList<WeekWorking> GetWeekWorking(int id, System.DateTime date)
        {
            Check.Argument.IsNotNull(id, "id");
            return this._employeeRepository.GetWeekWorking(id, date);
        }

        public bool UpdateWeekWorking(EmployeeShift model, System.DateTime date)
        {
            Check.Argument.IsNotNull(model, "model");
            return this._employeeRepository.UpdateWeekWorking(model, date);
        }

        public ViewModels.MySpaceViewModel GetMySpace(int id)
        {
            return this._employeeRepository.GetMySpace(id);
        }

        public IList<DateOfEmployeeForgotAndLateEarlyIOViewModel> GetDateOfEmployeeForgotAndLateEarlyIO(int id)
        {
            return this._employeeRepository.GetDateOfEmployeeForgotAndLateEarlyIO(id);
        }

        /// <summary>
        /// Employees the calendar_ get off date_ holiday date.
        /// </summary>
        /// <param name="empId">The employee identifier.</param>
        /// <param name="fromDate">Get unavailable date is greater than From date.</param>
        /// <param name="toDate">Get unavailable date is less than To date.</param>
        /// <returns>The list of unavailable dates for creating keep in view.</returns>
        public IList<DateTime> GetOffDateAndHolidayDate(int empId, DateTime fromDate, DateTime toDate)
        {
            IList<DateTime> availableDates = this._employeeRepository.GetOffDateAndHolidayDate(empId, fromDate, toDate);
            IList<DateTime> unavailableDates = new List<DateTime>();

            for (var dateInRange = fromDate; dateInRange <= toDate; dateInRange = dateInRange.AddDays(1))
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

            return unavailableDates;
        }

        public IList<DateTime> GetOffAndHoliday(int empId, DateTime fromDate, DateTime toDate)
        {
            IList<DateTime> dates = this._employeeRepository.GetOffDateAndHolidayDate(empId, fromDate, toDate);

            return dates;
        }

        public IList<HrLeaderDashboardModel> GetHrLeaderDashboard(DateTime date, int? employeeId = null)
        {
            return this._employeeRepository.GetHrLeaderDashboard(date, employeeId);
        }

        public bool UpdateShift(int id, System.DateTime date, int? shift, int modifiedBy, DateTime modifiedDate)
        {
            return this._employeeRepository.UpdateShift(id, date, shift, modifiedBy, modifiedDate);
        }

        public IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(int id, int year, bool excludedRequestTrans)
        {
            return this._employeeRepository.GetAnnualLeaveTransactions(id, year, excludedRequestTrans);
        }

        public AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int id)
        {
            return this._employeeRepository.GetAnnualLeaveTransaction(id);
        }

        public bool DeleteAnnualLeaveTransaction(long id, DateTime modifiedDate, int modifiedBy)
        {
            return this._employeeRepository.DeleteAnnualLeaveTransaction(id, modifiedDate, modifiedBy);
        }

        public bool UpdateAnnualLeaveTransaction(
            long id,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime modifiedDate,
            int modifiedBy)
        {
            return this._employeeRepository.UpdateAnnualLeaveTransaction(id, description, fromDate, deductDays, creditDays, modifiedDate, modifiedBy);
        }

        public bool InsertAnnualLeaveTransaction(
            int empId,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy)
        {
            return this._employeeRepository.InsertAnnualLeaveTransaction(empId, description, fromDate, deductDays, creditDays, createdDate, createdBy);
        }

        public IList<UserChangedShift> GetUserChangedShift(DateTime date)
        {
            return this._employeeRepository.GetUserChangedShift(date);
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

        public int ValidateUser(string code, string password)
        {
            Employee employee = this.GetEmployeeByCode(code);
            return this.ValidateUser(employee, password);
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
            Employee employee = this.GetEmployeeById(empId);
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

        public IList<Country> GetAllCountry()
        {
            return _employeeRepository.GetAllCountry();
        }

        public Country GetCountryByCode(string countryCode)
        {
            return _employeeRepository.GetCountryByCode(countryCode);
        }

        public bool UpdateSupervisor(SupervisorViewModel model, int modifiedBy)
        {
            return this._employeeRepository.UpdateSupervisor(model, modifiedBy);
        }

        public IList<SupervisorModel> GetEmployeeSupervisor(int empId, int role)
        {
            return this._employeeRepository.GetEmployeeSupervisor(empId, role);
        }

        public IList<SupervisorNameModel> GetListSupervisorName(int empId, int role, int? requestType)
        {
            return this._employeeRepository.GetListSupervisorName(empId, role, requestType);
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