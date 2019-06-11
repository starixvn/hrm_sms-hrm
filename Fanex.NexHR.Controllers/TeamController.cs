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
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Section;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.EmployeeGroup;

    public class TeamController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IDepartmentService departmentService;
        private readonly IBaseService baseService;

        public TeamController(
            ISessionService sessionService,
            ITeamService teamService,
            IDepartmentService departmentService,
            IBaseService baseService) : base(sessionService)
        {
            this.teamService = teamService;
            this.departmentService = departmentService;
            this.baseService = baseService;
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
                Departments = GetAllDepartment(isActive: true)
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Create(TeamViewModel model)
        {
            model.Departments = GetAllDepartment(isActive: true);

            if (ModelState.IsValid)
            {
                var criteria = new InsertNewGroupCriteria
                {
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    DepartmentId = model.DepartmentId,
                    MailGroup = model.MailGroup,
                    LeaderId = model.LeaderId,
                    AssistantLeaderId = model.AssistantLeaderId,
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
                Leader = team.Leader != null ? team.Leader.DisplayName : string.Empty,
                LeaderId = team.Leader != null ? team.Leader.Id : 0,
                AssistantLeader = team.AssistantLeader != null ? team.AssistantLeader.DisplayName : string.Empty,
                AssistantLeaderId = team.AssistantLeader != null ? team.AssistantLeader.Id : 0,
                ModifiedDate = DateTime.Now,
                ModifiedBy = sessionService.Employee.Id,
                DepartmentId = team.Department != null ? team.Department.Id : 0,
                Departments = GetAllDepartment(isActive: true)
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

                if (ModelState.IsValid)
                {
                    var criteria = new UpdateGroupCriteria
                    {
                        GroupId = model.Id,
                        Code = model.TeamCode,
                        Name = model.TeamName,
                        DepartmentId = model.DepartmentId,
                        MailGroup = model.MailGroup,
                        LeaderId = model.LeaderId,
                        AssistantLeaderId = model.AssistantLeaderId,
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

        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult ManageSupervisor(int teamId, short supervisorRole)
        {
            ViewBag.Groups = GetAllTeam();
            var criteria = new GetTeamSupervisorCriteria
            {
                GroupId = teamId,
                Role = supervisorRole
            };
            IList<SupervisorModel> supervisors = teamService.GetTeamSupervisor(criteria);

            var model = new SupervisorViewModel
            {
                SupervisorList = supervisors,
                TeamId = teamId,
                SupervisorRole = supervisorRole,
                ForView = "Team"
            };

            return View("~/Views/Employee/Supervisor.cshtml", model);
        }

        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult ShiftManager(int teamId)
        {
            ViewBag.Groups = GetAllTeam();

            var criteria = new GetTeamSupervisorCriteria
            {
                GroupId = teamId,
                Role = (short)RoleSystem.ShiftManager
            };
            IList<SupervisorModel> supervisorModel = teamService.GetTeamSupervisor(criteria);

            var model = new ShiftManagerViewModel
            {
                ShiftManagerList = supervisorModel,
                TeamId = teamId,
                SupervisorRole = (short)RoleSystem.ShiftManager,
                ForView = "Team"
            };

            return View("~/Views/Employee/ShiftManager.cshtml", model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Supervisor(SupervisorViewModel model)
        {
            var criteria = new InsertOrUpdateTeamSupervisorCriteria
            {
                EmpIdChangeShifts = model.RoleChangeShifts,
                EmpIdCheckInOuts = model.RoleCheckInOuts,
                EmpIdKeepInViews = model.RoleKeepInViews,
                EmpIdLateInEarlyOuts = model.RoleLateInEarlyOuts,
                EmpIdLeaves = model.RoleLeaves,
                EmpIdOvertimes = model.RoleOvertimes,
                GroupId = model.TeamId,
                ModifiedBy = sessionService.Employee.Id,
                Role = model.SupervisorRole
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult ShiftManager(ShiftManagerViewModel model)
        {
            var criteria = new InsertTeamShiftManagerCriteria
            {
                GroupId = model.TeamId,
                ShiftArrangers = model.RoleShiftArrange,
                ModifiedBy = sessionService.Employee.Id
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult DisciplineManager(int teamId)
        {
            ViewBag.TeamId = teamId;
            ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
            return View();
        }

        [HttpGet]
        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public CustomJsonResult GetDisciplineManagerByTeamId(int teamId)
        {
            IEnumerable<DisciplineManager> supervisors = teamService.GetDisciplineManagerByTeamId(teamId);
            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult EditDisciplineManager(int teamId, string supervisorIds = "")
        {
            var criteria = new InsertOrUpdateTeamSupervisorCriteria
            {
                EmpIdDisciplines = supervisorIds,
                GroupId = teamId,
                ModifiedBy = sessionService.Employee.Id,
                Role = (short)RoleSystem.DisciplineManager
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

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

        private IList<Team> GetAllTeam()
        {
            var criteria = new GetAllGroupCriteria();
            IList<Team> teams = teamService.GetAll(criteria);

            return teams;
        }
    }
}