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
using Fanex.NexHR.Criteria.Calendar;
using Fanex.NexHR.Criteria.Employee;
using Fanex.NexHR.Criteria.EmployeeSupervisor;
using Fanex.NexHR.Criteria.MySpace;
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

        /// <summary>
        /// Query All Employees.
        /// </summary>
        /// <param name="query">Query to DB.</param>
        /// <param name="groupIds">Employee Group Id.</param>
        /// <param name="enable">Is Enabled.</param>
        /// <param name="isDelete">Id Deleted.</param>
        /// <param name="titleId">Employee Job Title.</param>
        /// <param name="isLocal">Expat Or Local.</param>
        /// <returns>List Of Employee.</returns>
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

            var getEmployeeCriteria = new GetAllEmployeeCriteria()
            {
                Query = query,
                GroupIds = groupIds,
                EnableUser = enable,
                IsDeleteUser = isDelete,
                TitleId = titleId,
                IsLocalUser = isLocal
            };
            var entityList = _employeeService.GetEmployees(getEmployeeCriteria);

            return entityList.AsQueryable();
        }

        /// <summary>
        /// Get Employee By Role.
        /// </summary>
        /// <param name="roleIds">Role of employee to get.</param>
        /// <param name="query">A string to query data.</param>
        /// <param name="excludeIds">A string of Excluded id.</param>
        /// <returns>List of employees.</returns>
        public IList<Employee> GetSupervisors(string roleIds, string query, string excludeIds)
        {
            var criteria = new GetEmployeeByRoleCriteria
            {
                ExcludeIds = excludeIds,
                Query = query.Trim(),
                RoleIds = roleIds
            };
            IList<Employee> employees = _employeeService.GetEmployeesByRole(criteria);

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
                    var criteria = new GetOffDateAndHolidayDateCriteria()
                    {
                        EmpId = employeeId,
                        FromDate = fromDate,
                        ToDate = toDate
                    };
                    IEnumerable<DateTime> dates = _employeeService.GetOffDateAndHolidayDate(criteria);
                    numberDays = (toDate - fromDate).Days + 1 - (dates != null ? dates.Count() : 0);
                    break;
                default:
                    var getDateRangeCriteria = new GetDateRangeCriteria()
                    {
                        EmpId = employeeId,
                        FromDate = fromDate,
                        ToDate = toDate
                    };
                    numberDays = _employeeService.GetDateRange(getDateRangeCriteria);
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
                var criteria = new GetOffDateAndHolidayDateCriteria()
                {
                    EmpId = employeeId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                dates = _employeeService.GetOffDateAndHolidayDate(criteria);
            }
            else
            {
                var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
                {
                    EmpId = employeeId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                dates = _employeeService.GetUnavailableDates(getUnavailableDateCriteria);
            }

            return dates;
        }

        public IList<SupervisorModel> GetEmployeeSupervisor(int empId, int role)
        {
            var getEmployeeSupervisorCriteria = new GetEmployeeSupervisorCriteria()
            {
                EmpId = empId,
                Role = role
            };
            IList<SupervisorModel> supervisors = _employeeService.GetEmployeeSupervisor(getEmployeeSupervisorCriteria);

            return supervisors;
        }

        public Employee GetEmployee(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            Employee employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            return employee;
        }

        public string GetNewEmployeeCode()
        {
            var getNewEmployeeCodeCriteria = new GetNewEmployeeCodeCriteria();
            string newCode = _employeeService.NewEmployeeCode(getNewEmployeeCodeCriteria);

            return newCode;
        }
    }
}
