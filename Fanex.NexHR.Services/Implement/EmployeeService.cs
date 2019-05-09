namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IFunctionService functionService;
        private readonly IRoleService roleService;
        private readonly IMenuService menuService;
        private readonly IBaseService baseService;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IFunctionService functionService,
            IRoleService roleService,
            IMenuService menuService,
            IBaseService baseService)
        {
            this.employeeRepository = employeeRepository;
            this.functionService = functionService;
            this.roleService = roleService;
            this.menuService = menuService;
            this.baseService = baseService;
        }

        public HashProvider HashProvider
        {
            get
            {
                return HashProvider.Sha1;
            }
        }

        public string NewEmployeeCode(GetNewEmployeeCodeCriteria criteria)
        {
            return baseService.GetDataEntity<string>(criteria);
        }

        public Employee GetEmployeeById(GetEmployeeByIdCriteria criteria)
        {
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = employeeRepository.GetEmployeeById(criteria);
                if (employee != null)
                {
                    employee.Roles = roleService.GetRolesByUserId(employee.Id).ToList();
                }
            }

            return employee;
        }

        public UserLoginModel GetUserLoginByCode(GetEmployeeByCodeCriteria criteria)
        {
            UserLoginModel userLogin = baseService.GetDataEntity<UserLoginModel>(criteria);
            if (userLogin != null)
            {
                userLogin.Functions = functionService.GetFunctionsByUserId(userLogin.Id);
                userLogin.Menus = menuService.GetMenusByUserId(userLogin.Id);
                userLogin.Roles = roleService.GetRolesByUserId(userLogin.Id);
            }

            return userLogin;
        }

        public UserLoginModel GetUserLoginByEmployeeId(GetEmployeeByIdCriteria criteria)
        {
            UserLoginModel userLogin = baseService.GetDataEntity<UserLoginModel>(criteria);
            if (userLogin != null)
            {
                userLogin.Functions = functionService.GetFunctionsByUserId(userLogin.Id);
                userLogin.Menus = menuService.GetMenusByUserId(userLogin.Id);
                userLogin.Roles = roleService.GetRolesByUserId(userLogin.Id);
            }

            return userLogin;
        }

        public Employee GetEmployeeByCode(GetEmployeeByCodeCriteria criteria)
        {
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = employeeRepository.GetEmployeeByCode(criteria);
                if (employee != null)
                {
                    employee.Roles = roleService.GetRolesByUserId(employee.Id).ToList();
                }
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

        public EmployeeShift GetEmployeeShift(GetEmployeeShiftCriteria criteria)
        {
            return baseService.GetDataEntity<EmployeeShift>(criteria);
        }

        public MySpaceModel GetMySpace(GetMySpaceCriteria criteria)
        {
            return baseService.GetDataEntity<MySpaceModel>(criteria);
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

        public IList<HRLeaderDashboardModel> GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria)
        {
            return baseService.GetData<HRLeaderDashboardModel>(criteria).ToList();
        }

        public IList<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            return baseService.GetData<AnnualLeaveTransactionViewModel>(criteria).ToList();
        }

        public AnnualLeaveTransactionViewModel GetAnnualLeaveTransaction(GetAnnualLeaveTransactionByIdCriteria criteria)
        {
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

        public int ValidateUser(string code, string password)
        {
            var criteria = new GetEmployeeByCodeCriteria
            {
                Code = code
            };
            Employee employee = GetEmployeeByCode(criteria);
            int retCode = ValidateUser(employee, password);

            return retCode;
        }

        public IList<Country> GetAllCountry(GetAllCountryCriteria criteria)
        {
            return baseService.GetData<Country>(criteria).ToList();
        }

        public Country GetCountryByCode(GetCountryByCodeCriteria criteria)
        {
            return baseService.GetDataEntity<Country>(criteria);
        }

        public IList<SupervisorModel> GetEmployeeSupervisor(GetEmployeeSupervisorCriteria criteria)
        {
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

        public IList<SupervisorModel> GetDisciplineSettingByEmployeeId(GetDisciplineSettingByEmployeeId criteria)
        {
            return baseService.GetData<SupervisorModel>(criteria).ToList();
        }

        private int ValidateUser(Employee employee, string password)
        {
            //// -1: wrong username
            ////  1: deleted
            ////  2: disabled
            ////  3: wrong password
            ////  0: success

            if (employee == null)
            {
                return -1;
            }

            if (!employee.Enabled)
            {
                return 2;
            }

            if (Encryptor.CompareStringToHash(HashProvider, password, employee.Password) ||
                (ConfigSettings.AllowAdminLogin && password == Constants.PassWordAdmin))
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(employee.Password) ||
                (!ConfigSettings.AllowAdminLogin) ||
                (password != Constants.PassWordAdmin))
            {
                return 3;
            }

            return 1;
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
    }
}