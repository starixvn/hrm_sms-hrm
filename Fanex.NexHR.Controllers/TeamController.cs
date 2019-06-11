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
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Section;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.EmployeeGroup;

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

        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult Index()
        {
            IList<Team> teams = GetAllTeam();
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host);

            ViewBag.HasPermissionToViewEmployee = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Read);

            return View(teams);
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Create()
        {
            var model = new TeamViewModel
            {
                Departments = GetAllDepartment(isActive: true),
                Employees = GetAllEmployees()
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Create(TeamViewModel model)
        {
            model.Departments = GetAllDepartment(isActive: true);
            model.Employees = GetAllEmployees();
            if (ModelState.IsValid)
            {
                var criteria = new InsertNewGroupCriteria
                {
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    DepartmentId = model.DepartmentId,
                    LeaderIds = model.LeaderIds,
                    AssistantLeaderIds = model.AssistantLeaderIds,
                    MailGroup = model.MailGroup,
                    CreatedBy = sessionService.Employee.Id
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
            var criteria = new GetGroupByIdCriteria
            {
                GroupId = id
            };

            Team team = teamService.GetById(criteria);

            var model = new TeamViewModel
            {
                TeamCode = team.Code,
                TeamName = team.Name,
                MailGroup = team.MailGroup,
                Leaders = team.Leaders,
                LeaderIds = string.Join(",", team.Leaders.Select(e => e.Id)),
                AssistantLeaders = team.AssistantLeaders,
                AssistantLeaderIds = string.Join(",", team.AssistantLeaders.Select(e => e.Id)),
                ModifiedDate = DateTime.Now,
                ModifiedBy = sessionService.Employee.Id,
                DepartmentId = team.Department != null ? team.Department.Id : 0,
                Departments = GetAllDepartment(isActive: true),
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
                model.Departments = GetAllDepartment(isActive: true);
                model.Employees = GetAllEmployees();
                if (ModelState.IsValid)
                {
                    var criteria = new UpdateGroupCriteria
                    {
                        GroupId = model.Id,
                        Code = model.TeamCode,
                        Name = model.TeamName,
                        DepartmentId = model.DepartmentId,
                        MailGroup = model.MailGroup,
                        LeaderIds = model.LeaderIds,
                        AssistantLeaderIds = model.AssistantLeaderIds,
                        ModifiedBy = sessionService.Employee.Id
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
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteGroupCriteria
            {
                GroupId = id,
                ModifiedBy = sessionService.Employee.Id
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
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
        public CustomJsonResult GetSectionSupervisorByRoleId(int sectionId, short roleId)
        {
            var criteria = new GetTeamSupervisorCriteria
            {
                GroupId = sectionId,
                Role = roleId
            };
            IEnumerable<SupervisorModel> supervisors = teamService.GetTeamSupervisor(criteria);

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
            var criteria = new GetTeamSupervisorCriteria
            {
                GroupId = sectionId,
                Role = (short)RoleSystem.ShiftManager
            };
            IEnumerable<SupervisorModel> supervisors = teamService.GetTeamSupervisor(criteria);

            return new CustomJsonResult
            {
                Data = supervisors.Select(x => new DisciplineManager { OwnerId = x.OwnerId, Nickname = x.NickName, FullName = x.FullName }),
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
        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public CustomJsonResult GetDisciplineManagerBySectionId(int sectionId)
        {
            IEnumerable<DisciplineManager> supervisors = teamService.GetDisciplineManagerBySectionId(sectionId);
            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult EditSectionSupervisor(int sectionId, short roleId, IEnumerable<EditSectionSupervisor> supervisors)
        {
            CommandResult commandResult = teamService.EditSectionSupervisor(sessionService.Employee.Id, sectionId, roleId, supervisors);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IList<Department> GetAllDepartment(bool? isActive)
        {
            var criteria = new GetDepartmentByParentIdCriteria
            {
                IsActive = isActive
            };
            IList<Department> departments = departmentService.GetDepartment(criteria);

            return departments;
        }

        private IList<Employee> GetAllEmployees()
        {
            var getEmployeeCriteria = new GetAllEmployeeCriteria
            {
                Query = null,
                GroupIds = null,
                EnableUser = true,
                IsDeleteUser = null,
                TitleId = null,
                IsLocalUser = null
            };

            IList<Employee> employees = employeeService.GetEmployees(getEmployeeCriteria).ToList();

            return employees;
        }

        private IList<Team> GetAllTeam()
        {
            var criteria = new GetAllGroupCriteria();
            IList<Team> teams = teamService.GetAll(criteria);

            return teams;
        }
    }
}