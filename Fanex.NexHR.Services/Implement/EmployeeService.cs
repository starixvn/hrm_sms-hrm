namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;
    using Fanex.NexHR.Models.Section;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IRoleService roleService;
        private readonly IBaseService baseService;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IRoleService roleService,
            IBaseService baseService)
        {
            this.employeeRepository = employeeRepository;
            this.roleService = roleService;
            this.baseService = baseService;
        }

        public string GenerateEmployeeCode()
        {
            var criteria = new GenerateEmployeeCodeCriteria();
            return baseService.GetDataEntity<string>(criteria);
        }

        public bool CheckResetPassword(int employeeId)
        {
            var criteria = new CheckResetPasswordCriteria { EmployeeId = employeeId };
            return baseService.GetDataEntity<bool>(criteria);
        }

        public Employee GetEmployeeById(int id)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmpId = id
            };
            Employee employee = employeeRepository.GetEmployeeById(criteria);
            if (employee != null)
            {
                employee.Roles = roleService.GetRolesByUserId(employee.Id).ToList();
            }

            return employee;
        }

        public IList<Employee> GetEmployees(GetAllEmployeeCriteria criteria)
        {
            IList<Employee> employees = null;
            if (criteria != null && criteria.IsValid())
            {
                employees = employeeRepository.GetEmployees(criteria);
            }

            return employees ?? new List<Employee>();
        }

        public IList<Employee> GetMemberByManager(GetMemberByManagerCriteria criteria)
        {
            return baseService.GetData<Employee>(criteria).ToList();
        }

        public IList<Employee> GetEmployeesByRole(GetEmployeeByRoleCriteria criteria)
        {
            IList<Employee> employees = null;
            if (criteria != null && criteria.IsValid())
            {
                employees = employeeRepository.GetEmployeesByRole(criteria);
            }

            return employees ?? new List<Employee>();
        }

        public EmployeeShift GetShiftByEmployeeId(int id)
        {
            var criteria = new GetShiftByEmployeeIdCriteria
            {
                EmpId = id
            };
            return baseService.GetDataEntity<EmployeeShift>(criteria);
        }

        public IList<DateTime> GetOffDateAndHolidayDate(GetOffDateAndHolidayDateCriteria criteria)
        {
            return baseService.GetData<DateTime>(criteria).ToList();
        }

        public IList<DateTime> GetNotOffOrHolidayDate(GetOffDateAndHolidayDateCriteria criteria)
        {
            IList<DateTime> availableDates = GetOffDateAndHolidayDate(criteria);
            IList<DateTime> unavailableDates = new List<DateTime>();

            if (availableDates == null)
            {
                return unavailableDates;
            }

            for (DateTime dateInRange = criteria.FromDate; dateInRange <= criteria.ToDate; dateInRange = dateInRange.AddDays(1))
            {
                bool isIncludedToUnvailableDates = true;

                foreach (DateTime availableDate in availableDates)
                {
                    if (dateInRange.IsSameDateWith(availableDate) && isIncludedToUnvailableDates)
                    {
                        isIncludedToUnvailableDates = false;
                    }
                }

                if (isIncludedToUnvailableDates)
                {
                    unavailableDates.Add(dateInRange);
                }
            }

            return unavailableDates;
        }

        public IList<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            return baseService.GetData<AnnualLeaveTransactionViewModel>(criteria).ToList();
        }

        public AnnualLeaveTransactionViewModel GetAnnualLeaveTransaction(int leaveTransId)
        {
            var criteria = new GetAnnualLeaveTransactionByIdCriteria
            {
                LeaveTransId = leaveTransId
            };
            return baseService.GetDataEntity<AnnualLeaveTransactionViewModel>(criteria);
        }

        public IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria)
        {
            return baseService.GetData<UserChangedShift>(criteria).ToList();
        }

        public IList<HalfDayOff> GetHalfDayOffs(GetEmployeeHalfDayOffCriteria criteria)
        {
            return baseService.GetData<HalfDayOff>(criteria).ToList();
        }

        public int GetDateRange(GetDateRangeCriteria criteria)
        {
            return baseService.GetDataEntity<int>(criteria);
        }

        public IList<DateTime> GetUnavailableDates(GetUnavailableRequestDateCriteria criteria)
        {
            return baseService.GetData<DateTime>(criteria).ToList();
        }

        public IList<Country> GetAllCountry()
        {
            var criteria = new GetAllCountryCriteria();
            return baseService.GetData<Country>(criteria).ToList();
        }

        public IList<SupervisorModel> GetEmployeeSupervisorByRoleId(int employeeId, RoleSystem roleId)
        {
            var criteria = new GetEmployeeSupervisorCriteria
            {
                EmpId = employeeId,
                Role = (short)roleId
            };
            return baseService.GetData<SupervisorModel>(criteria).ToList();
        }

        public IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria)
        {
            return baseService.GetData<SupervisorNameModel>(criteria).ToList();
        }

        public IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria)
        {
            return baseService.GetData<SupervisorNameModel>(criteria).ToList();
        }

        public IList<SupervisorNameModel> GetSupervisorByQuery(GetEmployeeSupervisorByQueryCriteria criteria)
        {
            return baseService.GetData<SupervisorNameModel>(criteria).ToList();
        }

        public IEnumerable<Supervisor> GetDisciplineSettingByEmployeeId(int employeeId)
        {
            var criteria = new GetDisciplineSettingByEmployeeId
            {
                EmpId = employeeId
            };
            return baseService.GetData<Supervisor>(criteria).ToList();
        }

        public CommandResult EditEmployeeSupervisor(int actorId, int employeeId, short roleId, IEnumerable<EditSectionSupervisor> supervisors)
        {
            var criteria = new EditEmployeeSupervisorCriteria
            {
                EmployeeId = employeeId,
                ActorId = actorId,
                RoleId = roleId,
                SupervisorXml = supervisors.ToXML().ToString()
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public IList<Employee> GetEmployeeByGender(string query, int gender)
        {
            var criteria = new GetMemberByGenderCriteria
            {
                Query = query,
                Gender = gender
            };

            return baseService.GetData<Employee>(criteria).ToList();
        }

        public IList<EmployeeFullProfile> GetEmployeeFullProfile(GetEmployeeFullProfileCriteria criteria)
        {
            return employeeRepository.GetEmployeeFullInformation(criteria);
        }

        public CommandResult EditPasswordResetStatus(int employeeId, bool status)
        {
            var criteria = new EditPasswordResetStatusCriteria
            {
                EmpId = employeeId,
                PasswordReset = status
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult EditPassword(int actorId, EditPassword model)
        {
            var criteria = new EditPasswordCriteria
            {
                EmpId = model.EmployeeId,
                Password = model.Password,
                PasswordReset = model.IsReset,
                ModifiedBy = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}