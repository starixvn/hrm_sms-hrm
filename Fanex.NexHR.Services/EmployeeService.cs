namespace Fanex.NexHR.Services
{
    using Fanex.Common.Helper;
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
using Fanex.NexHR.Models.Calendar;
using Fanex.NexHR.Models.Supervisor;
using Fanex.NexHR.Repositories;
using Fanex.NexHR.Repositories.Interfaces;
using Fanex.NexHR.Services.Interfaces;
using Fanex.NexHR.ViewModels.AnnualLeave;
using Nexus.RoleManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;

    public class EmployeeService : BaseService, IEmployeeService
    {
        private IEmployeeRepository employeeRepository;
        private IFunctionService functionService;
        private IRoleService roleService;
        private IMenuService menuService;

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
            }

            return employee;
        }

        public UserLoginModel GetUserLoginByDomainUserName(GetUserLoginByDomainUserNameCriteria criteria)
        {
            UserLoginModel user = GetDataEntity<UserLoginModel>(criteria);
            return user;
        }

        public UserLoginModel GetUserLoginByCode(GetEmployeeByCodeCriteria criteria)
        {
            UserLoginModel userLogin = GetDataEntity<UserLoginModel>(criteria);
            if (userLogin != null)
            {
                userLogin.NE_Functions = functionService.GetFunctionsByUserId(userLogin.Id);
                userLogin.NE_Menus = menuService.GetMenusByUserId(userLogin.Id);
                userLogin.NE_Roles = roleService.GetRolesByUserId(userLogin.Id);
            }

            return userLogin;
        }

        public UserLoginModel GetUserLoginByEmployeeId(GetEmployeeByIdCriteria criteria)
        {
            UserLoginModel userLogin = GetDataEntity<UserLoginModel>(criteria);
            return userLogin;
        }

        public Employee GetEmployeeByCode(GetEmployeeByCodeCriteria criteria)
        {
            Employee employee = null;
            if (criteria != null && criteria.IsValid())
            {
                employee = employeeRepository.GetEmployeeByCode(criteria);
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

            return employees;
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

            return employees;
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

        public MySpaceModel GetMySpace(GetMySpaceCriteria criteria)
        {
            MySpaceModel space = null;
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                space = this.GetDataEntity<MySpaceModel>(criteria);
            }

            return space;
        }

        public IList<DateTime> GetOffDateAndHolidayDate(GetOffDateAndHolidayDateCriteria criteria)
        {
            IList<DateTime> offAndHoliday = null;
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                offAndHoliday = this.GetData<DateTime>(criteria).ToList();
            }

            return offAndHoliday;
        }

        public IList<DateTime> GetNotOffOrHolidayDate(GetOffDateAndHolidayDateCriteria criteria)
        {
            IList<DateTime> availableDates = GetOffDateAndHolidayDate(criteria);
            IList<DateTime> unavailableDates = new List<DateTime>();

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

        public IList<HRLeaderDashboardModel> GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<HRLeaderDashboardModel> dashboard = null;
            if (criteria.IsValid())
            {
                dashboard = this.GetData<HRLeaderDashboardModel>(criteria).ToList();
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

        public IList<HalfDayOff> GetHalfDayOffs(GetEmployeeHalfDayOffCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<HalfDayOff> halfDayOffs = null;
            if (criteria.IsValid())
            {
                halfDayOffs = this.GetData<HalfDayOff>(criteria).ToList();
            }

            return halfDayOffs;
        }

        public int GetDateRange(GetDateRangeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int dateRange = 0;
            if (criteria.IsValid())
            {
                dateRange = this.GetDataEntity<int>(criteria);
            }

            return dateRange;
        }

        public IList<DateTime> GetUnavailableDates(GetUnavailableRequestDateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<DateTime> unavailableDates = null;
            if (criteria.IsValid())
            {
                unavailableDates = this.GetData<DateTime>(criteria).ToList();
            }

            return unavailableDates;
        }

        public int ValidateUser(string code, string password)
        {
            var criteria = new GetEmployeeByCodeCriteria() { Code = code };
            Employee employee = GetEmployeeByCode(criteria);
            int retCode = ValidateUser(employee, password);

            return retCode;
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

        public IList<SupervisorNameModel> GetSupervisorByQuery(GetEmployeeSupervisorByQueryCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                return this.GetData<SupervisorNameModel>(criteria).ToList();
            }

            return null;
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
                    if (Encryptor.CompareStringToHash(HashProvider, password, employee.Password))
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