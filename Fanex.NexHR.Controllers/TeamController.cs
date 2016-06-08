namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.EmployeeGroup;
    using Fanex.NexHR.ViewModels.Supervisor;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class TeamController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IDepartmentService departmentService;
        private readonly ICommandService commandService;

        public TeamController()
            : this(null, null, null)
        {
        }

        public TeamController(ITeamService teamService, IDepartmentService departmentService, ICommandService commandService)
        {
            this.teamService = teamService ?? new TeamService();
            this.departmentService = departmentService ?? new DepartmentService();
            this.commandService = commandService ?? new CommandService();
        }

        [Authorize(Roles = "HR")]
        public ActionResult Index()
        {
            var criteria = new GetAllGroupCriteria();
            IList<Team> teams = teamService.GetAll(criteria);

            return View(teams);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var model = new TeamViewModel
            {
                Departments = GetAllDepartment(isActive: true)
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Create(TeamViewModel model)
        {
            model.Departments = GetAllDepartment(isActive: true);
            if (model != null && ModelState.IsValid)
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
                    ViewBag.Inserted = true;
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

            return View(model);
        }

        [Authorize(Roles = "HR")]
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
        [Authorize(Roles = "HR")]
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
                        AssistantLeaderId = string.IsNullOrEmpty(model.AssistantLeader) ? (int?)null : int.Parse(model.AssistantLeader),
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

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteGroupCriteria
            {
                GroupID = id,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = commandService.Delete(criteria);
            return Json(result);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TeamSupervisor(int id)
        {
            var criteria = new GetSupervisorByTeamIdCriteria() 
            { 
                GroupId = id 
            };
            var model = teamService.GetSupervisorByTeamId(criteria);
            ViewBag.TeamId = id;

            return View(model);
        }

        public ActionResult ManageSupervisor(int teamId, short supervisorRole)
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = teamService.GetAll(getAllGroupCriteria);
            var criteria = new GetTeamSupervisorCriteria()
            {
                GroupId = teamId,
                Role = supervisorRole
            };
            var supervisorModel = teamService.GetTeamSupervisor(criteria);

            var model = new SupervisorViewModel
            {
                SupervisorList = supervisorModel,
                TeamId = teamId,
                SupervisorRole = supervisorRole,
                ForView = "Team"
            };

            return View("~/Views/Employee/Supervisor.cshtml", model);
        }

        public ActionResult ShiftManager(int teamId)
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = teamService.GetAll(getAllGroupCriteria);
            var criteria = new GetTeamSupervisorCriteria()
            {
                GroupId = teamId,
                Role = (short)RoleSystem.ShiftManager
            };
            var supervisorModel = teamService.GetTeamSupervisor(criteria);

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

        private IList<Department> GetAllDepartment(bool? isActive)
        {
            var criteria = new GetDepartmentByParentIdCriteria()
            {
                IsActive = isActive
            };
            IList<Department> departments = departmentService.GetDepartment(criteria);

            return departments;
        }
    }
}
