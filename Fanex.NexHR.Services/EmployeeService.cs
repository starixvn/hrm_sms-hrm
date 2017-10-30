namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core;
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
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.AnnualLeave;
    using Nexus.RoleManagement.Services;

    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IFunctionService functionService;
        private readonly IRoleService roleService;
        private readonly IMenuService menuService;

        public EmployeeService()
            : this(null, null, null, null)
        {
        }

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IFunctionService functionService,
            IRoleService roleService,
            IMenuService menuService)
        {
            this.employeeRepository = employeeRepository ?? new EmployeeRepository();
            this.functionService = functionService ?? new FunctionService();
            this.roleService = roleService ?? new RoleService();
            this.menuService = menuService ?? new MenuService();
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
            string newCode = GetDataEntity<string>(criteria);
            return newCode;
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
            UserLoginModel userLogin = GetDataEntity<UserLoginModel>(criteria);
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
            UserLoginModel userLogin = GetDataEntity<UserLoginModel>(criteria);
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
            IList<Employee> employees = GetData<Employee>(criteria).ToList();
            return employees;
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
            EmployeeShift employeeShift = GetDataEntity<EmployeeShift>(criteria);
            return employeeShift;
        }

        public MySpaceModel GetMySpace(GetMySpaceCriteria criteria)
        {
            MySpaceModel space = GetDataEntity<MySpaceModel>(criteria);
            return space;
        }

        public IList<DateTime> GetOffDateAndHolidayDate(GetOffDateAndHolidayDateCriteria criteria)
        {
            IList<DateTime> offAndHoliday = GetData<DateTime>(criteria).ToList();
            return offAndHoliday;
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
            IList<HRLeaderDashboardModel> dashboard = GetData<HRLeaderDashboardModel>(criteria).ToList();
            return dashboard;
        }

        public IList<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            IList<AnnualLeaveTransactionViewModel> annualLeaves = GetData<AnnualLeaveTransactionViewModel>(criteria).ToList();
            return annualLeaves;
        }

        public AnnualLeaveTransactionViewModel GetAnnualLeaveTransaction(GetAnnualLeaveTransactionByIdCriteria criteria)
        {
            AnnualLeaveTransactionViewModel annualLeave = GetDataEntity<AnnualLeaveTransactionViewModel>(criteria);
            return annualLeave;
        }

        public IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria)
        {
            IList<UserChangedShift> users = GetData<UserChangedShift>(criteria).ToList();
            return users;
        }

        public IList<HalfDayOff> GetHalfDayOffs(GetEmployeeHalfDayOffCriteria criteria)
        {
            IList<HalfDayOff> halfDayOffs = GetData<HalfDayOff>(criteria).ToList();
            return halfDayOffs;
        }

        public int GetDateRange(GetDateRangeCriteria criteria)
        {
            int dateRange = GetDataEntity<int>(criteria);
            return dateRange;
        }

        public IList<DateTime> GetUnavailableDates(GetUnavailableRequestDateCriteria criteria)
        {
            IList<DateTime> unavailableDates = GetData<DateTime>(criteria).ToList();
            return unavailableDates;
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
            IList<Country> countries = GetData<Country>(criteria).ToList();
            return countries;
        }

        public Country GetCountryByCode(GetCountryByCodeCriteria criteria)
        {
            Country country = GetDataEntity<Country>(criteria);
            return country;
        }

        public IList<SupervisorModel> GetEmployeeSupervisor(GetEmployeeSupervisorCriteria criteria)
        {
            IList<SupervisorModel> supervisors = GetData<SupervisorModel>(criteria).ToList();
            return supervisors;
        }

        public IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria)
        {
            IList<SupervisorNameModel> supervisors = GetData<SupervisorNameModel>(criteria).ToList();
            return supervisors;
        }

        public IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria)
        {
            IList<SupervisorNameModel> supervisors = GetData<SupervisorNameModel>(criteria).ToList();
            return supervisors;
        }

        public IList<SupervisorNameModel> GetSupervisorByQuery(GetEmployeeSupervisorByQueryCriteria criteria)
        {
            return GetData<SupervisorNameModel>(criteria).ToList();
        }

        public IList<SupervisorModel> GetDisciplineSettingByEmployeeId(GetDisciplineSettingByEmployeeId criteria)
        {
            IList<SupervisorModel> supervisors = GetData<SupervisorModel>(criteria).ToList();
            return supervisors;
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
    }
}