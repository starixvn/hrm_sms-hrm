namespace Fanex.NexHR.Controllers.ApiControllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Http;

    public class EmployeeController : ApiController
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController()
            : this(null)
        {
        }

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
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

            var getEmployeeCriteria = new GetAllEmployeeCriteria()
            {
                Query = query,
                GroupIds = groupIds,
                EnableUser = enable,
                IsDeleteUser = isDelete,
                TitleId = titleId,
                IsLocalUser = isLocal
            };
            IList<Employee> employees = employeeService.GetEmployees(getEmployeeCriteria);

            return employees.AsQueryable();
        }

        public IList<Employee> GetSupervisors(string roleIds, string query, string excludeIds)
        {
            var criteria = new GetEmployeeByRoleCriteria
            {
                ExcludeIds = excludeIds,
                Query = query.Trim(),
                RoleIds = roleIds
            };
            IList<Employee> employees = employeeService.GetEmployeesByRole(criteria);

            return employees;
        }

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
                    IEnumerable<DateTime> dates = employeeService.GetOffDateAndHolidayDate(criteria);
                    numberDays = (toDate - fromDate).Days + 1 - (dates != null ? dates.Count() : 0);
                    break;
                default:
                    var getDateRangeCriteria = new GetDateRangeCriteria()
                    {
                        EmpId = employeeId,
                        FromDate = fromDate,
                        ToDate = toDate
                    };
                    numberDays = employeeService.GetDateRange(getDateRangeCriteria);
                    break;
            }

            return numberDays;
        }

        public IEnumerable<DateTime> GetUnavailableDates(int employeeId, DateTime date, int? leaveTypeId = null)
        {
            IEnumerable<DateTime> dates = null;
            var fromDate = new DateTime(date.AddMonths(-5).Year, date.AddMonths(-5).Month, 1);
            var tempDate = date.AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest);
            var toDate = new DateTime(tempDate.Year, tempDate.Month, DateTime.DaysInMonth(tempDate.Year, tempDate.Month));

            if (leaveTypeId.HasValue && (leaveTypeId.Value == Constants.UnpaidLeave || leaveTypeId.Value == Constants.BusinessTrip))
            {
                var criteria = new GetOffDateAndHolidayDateCriteria()
                {
                    EmpId = employeeId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                dates = employeeService.GetOffDateAndHolidayDate(criteria);
            }
            else
            {
                var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
                {
                    EmpId = employeeId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                dates = employeeService.GetUnavailableDates(getUnavailableDateCriteria);
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
            IList<SupervisorModel> supervisors = employeeService.GetEmployeeSupervisor(getEmployeeSupervisorCriteria);

            return supervisors;
        }

        public Employee GetEmployee(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { EmpId = id };
            Employee employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            return employee;
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed. Suppression is OK here.")]
        public string GetNewEmployeeCode()
        {
            var getNewEmployeeCodeCriteria = new GetNewEmployeeCodeCriteria();
            string newCode = employeeService.NewEmployeeCode(getNewEmployeeCodeCriteria);

            return newCode;
        }
    }
}
