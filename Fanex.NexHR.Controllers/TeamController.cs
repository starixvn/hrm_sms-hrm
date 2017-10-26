namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.EmployeeGroup;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;

    public class TeamController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IDepartmentService departmentService;
        private readonly ICommandService commandService;

        public TeamController()
            : this(null, null, null)
        {
        }

        public TeamController(
            ITeamService teamService,
            IDepartmentService departmentService,
            ICommandService commandService)
        {
            this.teamService = teamService ?? new TeamService();
            this.departmentService = departmentService ?? new DepartmentService();
            this.commandService = commandService ?? new CommandService();
        }

        [HRAuthorize(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult Index()
        {
            IList<Team> teams = GetAllTeam();
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.SectionManagement, Policy.Host);

            ViewBag.HasPermissionToViewEmployee = CurrentUser.HasPermission(FunctionKey.EmployeeManagement, Policy.Read);

            return View(teams);
        }

        [HRAuthorize(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Create()
        {
            var model = new TeamViewModel
            {
                Departments = GetAllDepartment(isActive: true)
            };
            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.SectionManagement)]
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
                    CreatedBy = CurrentUser.Id
                };
                CommandResult result = commandService.Insert(criteria);

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

        [HRAuthorize(Policy.Host, FunctionKey.SectionManagement)]
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
                ModifiedBy = this.CurrentUser.Id,
                DepartmentId = team.Department != null ? team.Department.Id : 0,
                Departments = GetAllDepartment(isActive: true)
            };
            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.SectionManagement)]
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
                        ModifiedBy = CurrentUser.Id
                    };
                    CommandResult result = commandService.Update(criteria);

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
        [HRAuthorize(Policy.Host, FunctionKey.SectionManagement)]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteGroupCriteria
            {
                GroupId = id,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = commandService.Delete(criteria);

            return Json(result);
        }

        [HRAuthorize(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult TeamSupervisor(int id)
        {
            var criteria = new GetSupervisorByTeamIdCriteria
            {
                GroupId = id
            };

            IList<TeamSupervisorModel> model = teamService.GetSupervisorByTeamId(criteria);
            ViewBag.TeamId = id;

            return View(model);
        }

        [HRAuthorize(Policy.Read, FunctionKey.SectionManagement)]
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

        [HRAuthorize(Policy.Read, FunctionKey.SectionManagement)]
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

        [HRAuthorize(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult DisciplineSetting(int teamId)
        {
            ViewBag.Groups = GetAllTeam();

            var criteria = new GetDisciplineSettingByGroupIdCriteria
            {
                GroupId = teamId
            };
            IList<SupervisorModel> supervisors = teamService.GetDisciplineSettingByGroupId(criteria);

            var model = new SupervisorViewModel
            {
                SupervisorList = supervisors,
                TeamId = teamId,
                SupervisorRole = (short)RoleSystem.DisciplineManager
            };

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.SectionManagement)]
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
                ModifiedBy = CurrentUser.Id,
                Role = model.SupervisorRole
            };
            CommandResult result = commandService.Update(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult ShiftManager(ShiftManagerViewModel model)
        {
            var criteria = new InsertTeamShiftManagerCriteria
            {
                GroupId = model.TeamId,
                ShiftArrangers = model.RoleShiftArrange,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = commandService.Update(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult UpdateDisciplineSetting(SupervisorViewModel model)
        {
            var criteria = new InsertOrUpdateTeamSupervisorCriteria
            {
                EmpIdDisciplines = model.RoleDisciplines,
                GroupId = model.TeamId,
                ModifiedBy = CurrentUser.Id,
                Role = (short)RoleSystem.DisciplineManager
            };
            CommandResult result = commandService.Update(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
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