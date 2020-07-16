namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Model;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Group;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Group;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.EmployeeGroup;
    using Fanex.NexHR.ViewModels.Team;
    using Newtonsoft.Json;

    [HRMPermission(Policy.Read, FunctionKey.TeamManagement)]
    public class TeamController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly ISectionService sectionService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;

        public TeamController(
            ISessionService sessionService,
            ITeamService teamService,
            ISectionService sectionService,
            IBaseService baseService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.teamService = teamService;
            this.sectionService = sectionService;
            this.baseService = baseService;
            this.employeeService = employeeService;
        }

        public ActionResult Index(string keyword = null)
        {
            IList<Team> teams = teamService.GetAll(keyword: keyword);
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.TeamManagement, Policy.Host);
            ViewBag.HasPermissionToViewEmployee = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Read);
            return View(teams);
        }

        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult Create()
        {
            var model = new TeamViewModel
            {
                Sections = sectionService.GetSection(),
                Employees = GetAllEmployees()
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult Create(TeamViewModel model)
        {
            model.Sections = sectionService.GetSection();
            model.Employees = GetAllEmployees();
            if (ModelState.IsValid)
            {
                var criteria = new CreateTeamCriteria
                {
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    SectionId = model.SectionId ?? 0,
                    HeadId = model.HeadId ?? 0,
                    ManagerIds = model.ManagerIds,
                    LeaderIds = model.LeaderIds,
                    SupervisorIds = model.SupervisorIds,
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

        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult Edit(int id)
        {
            Team team = teamService.GetById(id);
            if (team == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var unitHead = team.Managers.FirstOrDefault(x => x.RoleId == (byte)TeamManagerRole.Head);
            var model = new TeamViewModel
            {
                TeamCode = team.Code,
                TeamName = team.Name,
                MailGroup = team.MailGroup,
                BasicAnnualLeaveBalance = team.BasicAnnualLeaveBalance,
                HeadId = unitHead?.EmpId,
                UnitHead = unitHead?.DisplayName,
                ManagerIds = string.Join(",", team.Managers.Where(x => x.RoleId == (byte)TeamManagerRole.Manager).Select(e => e.EmpId)),
                SupervisorIds = string.Join(",", team.Managers.Where(x => x.RoleId == (byte)TeamManagerRole.Supervisor).Select(e => e.EmpId)),
                LeaderIds = string.Join(",", team.Managers.Where(x => x.RoleId == (byte)TeamManagerRole.Leader).Select(e => e.EmpId)),
                SectionId = team.Section != null ? team.Section.Id : 0,
                Sections = sectionService.GetSection(),
                Employees = GetAllEmployees()
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult Edit(TeamViewModel model)
        {
            if (model != null)
            {
                model.Sections = sectionService.GetSection();
                model.Employees = GetAllEmployees();
                if (ModelState.IsValid)
                {
                    var criteria = new EditTeamCriteria
                    {
                        TeamId = model.Id,
                        Code = model.TeamCode,
                        Name = model.TeamName,
                        SectionId = model.SectionId ?? 0,
                        MailGroup = model.MailGroup,
                        HeadId = model.HeadId ?? 0,
                        ManagerIds = model.ManagerIds,
                        LeaderIds = model.LeaderIds,
                        SupervisorIds = model.SupervisorIds,
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
        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
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

        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult ManageSupervisor(int id, short roleId)
        {
            ViewBag.TeamId = id;
            ViewBag.RoleId = roleId;

            return View();
        }

        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public CustomJsonResult GetTeamSupervisorByRoleId(int teamId, RoleSystem roleId)
        {
            IEnumerable<SupervisorModel> supervisors = teamService.GetTeamSupervisorByRoleId(teamId, roleId);

            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult ShiftManager(int id)
        {
            ViewBag.TeamId = id;
            ViewBag.RoleId = (short)RoleSystem.ShiftManager;

            return View();
        }

        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public CustomJsonResult GetShiftManagerByTeamId(int teamId)
        {
            IEnumerable<SupervisorModel> supervisors = teamService.GetTeamSupervisorByRoleId(teamId, RoleSystem.ShiftManager);

            return new CustomJsonResult
            {
                Data = supervisors.Select(x => new Supervisor { OwnerId = x.OwnerId, Nickname = x.NickName, FullName = x.FullName }),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult DisciplineManager(int id)
        {
            ViewBag.TeamId = id;
            ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
            return View();
        }

        [HttpGet]
        public CustomJsonResult GetDisciplineManagerByTeamId(int teamId)
        {
            IEnumerable<Supervisor> supervisors = teamService.GetDisciplineManagerByTeamId(teamId);
            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public CustomJsonResult EditTeamSupervisor(int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
        {
            CommandResult commandResult = teamService.EditTeamSupervisor(sessionService.Employee.Id, teamId, roleId, supervisors);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult DisciplineRule(int id)
        {
            Team team = teamService.GetById(id);
            if (team == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var model = new DisciplineRuleViewModel
            {
                TeamId = team.Id,
                Team = team.Name,
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.TeamManagement, Policy.Host),
                DisciplineRules = teamService.GetDisciplineRuleByTeam(id)
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.TeamManagement)]
        public CustomJsonResult EditDisciplineRule(int teamId, IEnumerable<DisciplineRule> disciplineRules)
        {
            CommandResult commandResult = teamService.EditDisciplineRule(sessionService.Employee.Id, teamId, disciplineRules);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IList<Employee> GetAllEmployees()
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = null,
                TeamIds = null,
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