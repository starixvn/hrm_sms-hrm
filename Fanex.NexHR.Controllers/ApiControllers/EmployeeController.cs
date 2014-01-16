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
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

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

        public IQueryable<Employee> GetEmployees(string query, string groupIds = "", bool? enable = null, bool? isDelete = null, int? titleId = null)
        {
            if (groupIds == null)
            {
                groupIds = string.Empty;
            }

            var entityList = _employeeService.GetEmployees(query, groupIds, enable, isDelete, titleId).AsQueryable();

            return entityList;
        }

        public IQueryable<Employee> GetSupervisors(string roleIds, string query, string excludeIds)
        {
            var employees = _employeeService.GetEmployeesByRole(roleIds, query.Trim(), excludeIds).AsQueryable();

            return employees;
        }

        public int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return _employeeService.GetDateRange(employeeId, fromDate, toDate);
        }

        public IEnumerable<DateTime> GetUnavailableDates(int employeeId, DateTime date)
        {
            return _employeeService.GetUnavailableDates(
                employeeId, 
                new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1), 
                new DateTime(date.AddMonths(2).Year, date.AddMonths(2).Month, 1).AddDays(-1));
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
    }
}
