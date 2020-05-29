namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;
    using Fanex.NexHR.Models.Group;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IRoleService roleService;
        private readonly IBaseService baseService;
        private readonly IAuthService authService;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IRoleService roleService,
            IBaseService baseService,
            IAuthService authService)
        {
            this.employeeRepository = employeeRepository;
            this.roleService = roleService;
            this.baseService = baseService;
            this.authService = authService;
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

        public int GetDateRange(GetDateRangeCriteria criteria)
        {
            return baseService.GetDataEntity<int>(criteria);
        }

        public IList<DateTime> GetUnavailableDates(GetUnavailableRequestDateCriteria criteria)
        {
            return baseService.GetData<DateTime>(criteria).ToList();
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

        public CommandResult EditEmployeeSupervisor(int actorId, int employeeId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
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

        public CommandResult Create(int actorId, CreateEmployeeParam param)
        {
            var criteria = new InsertEmployeeCriteria
            {
                Code = param.Code,
                CreateBy = actorId,
                DomainUsername = param.DomainUsername,
                FullName = param.FullName,
                Nickname = param.Nickname,
                GroupId = param.TeamId,
                Password = Encryptor.ComputeHash(authService.HashProvider, param.Code)
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            commandResult.OutputId = criteria.EmpId;

            return commandResult;
        }

        public CommandResult Edit(int actorId, EditEmployeeParam param)
        {
            var criteria = new UpdateEmployeeCriteria
            {
                EmpId = param.EmployeeId,
                Code = param.Code,
                DomainUsername = param.DomainUsername,
                FullName = param.FullName,
                Nickname = param.Nickname,
                EmployeeCard = param.EmployeeCard,
                DateOfBirth = param.DateOfBirth,
                Email = param.Email,
                JobTitleId = param.JobTitleId,
                Enabled = param.Enabled,
                Phone = param.Phone,
                Gender = param.Gender,
                Roles = param.Roles,
                JoinedDate = param.JoinedDate,
                CompanyDate = param.CompanyDate,
                ConfirmedDate = param.ConfirmedDate,
                ResignedDate = param.ResignedDate,
                Skype = param.Skype,
                LetsTalk = param.LetsTalk,
                Marriage = param.Marriage,
                PermanentAddress = param.PermanentAddress,
                CurrentAddress = param.CurrentAddress,
                IdCardNumber = param.IdCardNumber,
                PassportNo = param.PassportNo,
                PassportExp = param.PassportExp,
                ModifiedBy = actorId,
                GroupId = param.GroupId,
                IsLocal = param.IsLocal,
                CountryCode = param.CountryCode,
                CanOT = param.CanOT,
                SpecialOT = param.SpecialOT,
                WorkingDays = param.WorkingDays,
                WorkingHours = param.WorkingHours,
                IsTopManager = param.IsTopManager,
                OffDaysbyWeek = param.OffDaysbyWeek,
                Unit = param.Unit,
                PlaceOfbirth = param.PlaceOfBirth,
                Ethnic = param.Ethnic,
                Native = param.Native,
                Religion = param.Religion,
                DateOfIdCard = param.DateOfIdCard,
                PlaceOfIdCard = param.PlaceOfIdCard,
                HomePhone = param.HomePhone,
                Qualification = param.Qualification,
                Major = param.Major,
                IsExpatUseHoliday = !param.IsLocal && param.IsExpatUseHoliday,
                PickupRemark = param.PickupRemark,
                PromotionDate = param.PromotionDate,
                SectionDate = param.SectionDate,
                MilitaryFromDate = param.MilitaryFromDate,
                MilitaryToDate = param.MilitaryToDate
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int actorId, DeleteEmployeeParam param)
        {
            var criteria = new DeleteEmployeeCriteria
            {
                ActorId = actorId,
                EmpId = param.EmployeeId,
                ResignedDate = param.ResignedDate
            };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}