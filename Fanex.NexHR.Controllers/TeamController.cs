namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Section;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Section;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.EmployeeGroup;
    using Fanex.NexHR.ViewModels.Section;

    [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
    public class TeamController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IDepartmentService departmentService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;

        public TeamController(
            ISessionService sessionService,
            ITeamService teamService,
            IDepartmentService departmentService,
            IBaseService baseService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.teamService = teamService;
            this.departmentService = departmentService;
            this.baseService = baseService;
            this.employeeService = employeeService;
        }

        public ActionResult Index()
        {
            IList<Team> teams = teamService.GetAll();
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host);
            ViewBag.HasPermissionToViewEmployee = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Read);

            return View(teams);
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Create()
        {
            var model = new TeamViewModel
            {
                Departments = departmentService.GetDepartment(),
                Employees = GetAllEmployees()
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Create(TeamViewModel model)
        {
            model.Departments = departmentService.GetDepartment();
            model.Employees = GetAllEmployees();
            if (ModelState.IsValid)
            {
                var criteria = new CreateSectionCriteria
                {
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    DepartmentId = model.DepartmentId,
                    LeaderIds = model.LeaderIds,
                    AssistantLeaderIds = model.AssistantLeaderIds,
                    MailGroup = model.MailGroup,
                    CreatedBy = sessionService.Employee.Id,
                    BasicAnnualLeaveBalance = model.BasicAnnualLeaveBalance ?? 0
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index", "Team");
                }

                ModelState.AddModelError(string.Empty, result.Message);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Edit(int id)
        {
            Team team = teamService.GetById(id);
            if (team == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var model = new TeamViewModel
            {
                TeamCode = team.Code,
                TeamName = team.Name,
                MailGroup = team.MailGroup,
                BasicAnnualLeaveBalance = team.BasicAnnualLeaveBalance,
                Leaders = team.Leaders,
                LeaderIds = string.Join(",", team.Leaders.Select(e => e.Id)),
                AssistantLeaders = team.AssistantLeaders,
                AssistantLeaderIds = string.Join(",", team.AssistantLeaders.Select(e => e.Id)),
                ModifiedDate = DateTime.Now,
                ModifiedBy = sessionService.Employee.Id,
                DepartmentId = team.Department != null ? team.Department.Id : 0,
                Departments = departmentService.GetDepartment(),
                Employees = GetAllEmployees()
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Edit(TeamViewModel model)
        {
            if (model != null)
            {
                model.Departments = departmentService.GetDepartment();
                model.Employees = GetAllEmployees();
                if (ModelState.IsValid)
                {
                    var criteria = new EditSectionCriteria
                    {
                        GroupId = model.Id,
                        Code = model.TeamCode,
                        Name = model.TeamName,
                        DepartmentId = model.DepartmentId,
                        MailGroup = model.MailGroup,
                        LeaderIds = model.LeaderIds,
                        AssistantLeaderIds = model.AssistantLeaderIds,
                        ModifiedBy = sessionService.Employee.Id,
                        BasicAnnualLeaveBalance = model.BasicAnnualLeaveBalance ?? 0
                    };
                    CommandResult result = baseService.ExecuteNonQuery(criteria);

                    if (result.IsSuccessful)
                    {
                        ViewBag.Updated = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
                }
            }

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public CustomJsonResult Delete(int id)
        {
            CommandResult commandResult = teamService.Delete(sessionService.Employee.Id, id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult TeamSupervisor(int id)
        {
            IEnumerable<TeamSupervisorModel> supervisors = teamService.GetSupervisorByTeamId(id);
            ViewBag.TeamId = id;
            ViewBag.TeamCode = supervisors.Any() ? supervisors.First().Code : string.Empty;

            return View(supervisors);
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult ManageSupervisor(int id, short roleId)
        {
            ViewBag.SectionId = id;
            ViewBag.RoleId = roleId;

            return View();
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public CustomJsonResult GetSectionSupervisorByRoleId(int sectionId, RoleSystem roleId)
        {
            IEnumerable<SupervisorModel> supervisors = teamService.GetSectionSupervisorByRoleId(sectionId, roleId);

            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult ShiftManager(int id)
        {
            ViewBag.SectionId = id;
            ViewBag.RoleId = (short)RoleSystem.ShiftManager;

            return View();
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public CustomJsonResult GetShiftManagerBySectionId(int sectionId)
        {
            IEnumerable<SupervisorModel> supervisors = teamService.GetSectionSupervisorByRoleId(sectionId, RoleSystem.ShiftManager);

            return new CustomJsonResult
            {
                Data = supervisors.Select(x => new Supervisor { OwnerId = x.OwnerId, Nickname = x.NickName, FullName = x.FullName }),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult DisciplineManager(int id)
        {
            ViewBag.SectionId = id;
            ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
            return View();
        }

        [HttpGet]
        public CustomJsonResult GetDisciplineManagerBySectionId(int sectionId)
        {
            IEnumerable<Supervisor> supervisors = teamService.GetDisciplineManagerBySectionId(sectionId);
            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public CustomJsonResult EditSectionSupervisor(int sectionId, short roleId, IEnumerable<EditSectionSupervisor> supervisors)
        {
            CommandResult commandResult = teamService.EditSectionSupervisor(sessionService.Employee.Id, sectionId, roleId, supervisors);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult DisciplineRule(int id)
        {
            Team section = teamService.GetById(id);
            if (section == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var model = new DisciplineRuleViewModel
            {
                SectionId = section.Id,
                Section = section.Name,
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host),
                DisciplineRules = teamService.GetDisciplineRuleBySection(id)
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public CustomJsonResult EditDisciplineRule(int sectionId, IEnumerable<DisciplineRule> disciplineRules)
        {
            CommandResult commandResult = teamService.EditDisciplineRule(sessionService.Employee.Id, sectionId, disciplineRules);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IList<Employee> GetAllEmployees()
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = null,
                GroupIds = null,
                EnableUser = true,
                IsDeleteUser = null,
                TitleId = null,
                IsLocalUser = null
            };
            IList<Employee> employees = employeeService.GetEmployees(criteria).ToList();

            return employees;
        }
    }
}