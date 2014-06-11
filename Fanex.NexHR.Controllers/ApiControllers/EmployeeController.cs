// --------------------------------------------------------------------------
// <copyright file="EmployeeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Employee Controller API.
    /// </summary>
    public class EmployeeController : ApiController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController()
            : this(null)
        {
        }

        public EmployeeController(IEmployeeService employeeService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        public IQueryable<Employee> GetEmployees(
            string query,
            string groupIds = "", 
            bool? enable = null,
            bool? isDelete = null,
            int? titleId = null,
            bool? isLocal = null)
        {
            if (groupIds == null)
            {
                groupIds = string.Empty;
            }

            var entityList = _employeeService.GetEmployees(query, groupIds, enable, isDelete, titleId, isLocal);

            return entityList.AsQueryable();
        }

        public IQueryable<Employee> GetSupervisors(string roleIds, string query, string excludeIds)
        {
            var employees = _employeeService.GetEmployeesByRole(roleIds, query.Trim(), excludeIds).AsQueryable();

            return employees;
        }

        /// <summary>
        /// Function to get number work date between from and to.
        /// </summary>
        /// <param name="leaveTypeId">LeaveType Id.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="fromDate">The From date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <returns>Numbers day.</returns>
        public int GetRangeDate(int leaveTypeId, int employeeId, DateTime fromDate, DateTime toDate)
        {
            var numberDays = 0;
            switch (leaveTypeId)
            {
                case Constants.MaternityLeave:
                    numberDays = (toDate - fromDate).Days + 1;
                    break;
                case Constants.UnpaidLeave:
                case Constants.BusinessTrip:
                    IEnumerable<DateTime> dates = _employeeService.GetOffAndHoliday(employeeId, fromDate, toDate);
                    numberDays = (toDate - fromDate).Days + 1 - (dates != null ? dates.Count() : 0);
                    break;
                default:
                    numberDays = _employeeService.GetDateRange(employeeId, fromDate, toDate);
                    break;
            }

            return numberDays;
        }

        /// <summary>
        /// Function to get all unavailable date.
        /// </summary>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="date">The current date.</param>
        /// <param name="leaveTypeId">LeaveType Id.</param>
        /// <returns>List date time.</returns>
        public IEnumerable<DateTime> GetUnavailableDates(int employeeId, DateTime date, int? leaveTypeId = null)
        {
            IEnumerable<DateTime> dates = null;
            var fromDate = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
            var toDate = new DateTime(date.AddMonths(3).Year, date.AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest).Month, 1).AddDays(-1);

            if (leaveTypeId.HasValue && (leaveTypeId.Value == Constants.UnpaidLeave || leaveTypeId.Value == Constants.BusinessTrip))
            {
                dates = _employeeService.GetOffAndHoliday(employeeId, fromDate, toDate);
            }
            else
            {
                dates = _employeeService.GetUnavailableDates(employeeId, fromDate, toDate);
            }

            return dates;
        }

        public IList<SupervisorModel> GetEmployeeSupervisor(int empId, int role)
        {
            return _employeeService.GetEmployeeSupervisor(empId, role);
        }

        public IList<SupervisorNameModel> GetVerifiers(int employeeId, int requestType)
        {
            var verifierList = _employeeService.GetListSupervisorName(employeeId, (int)RolesEnum.Verifier, requestType);
            return verifierList;
        }

        public IList<SupervisorNameModel> GetApprovers(int employeeId, int requestType)
        {
            var approverList = _employeeService.GetListSupervisorName(employeeId, (int)RolesEnum.Approver, requestType);
            return approverList;
        }

        public Employee GetEmployee(int id)
        {
            return _employeeService.GetEmployeeById(id);
        }

        public string GetNewEmployeeCode()
        {
            return _employeeService.NewEmployeeCode;
        }
    }
}
