// --------------------------------------------------------------------------
// <copyright file="TeamController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.GroupSupervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Team Controller.
    /// </summary>
    [Authorize]
    public class TeamController : BaseController
    {
        private readonly ITeamService _teamService;

        public TeamController()
            : this(null)
        {
        }

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService ?? new TeamService();
        }

        [Authorize(Roles = "HR")]
        public ActionResult Index()
        {
            var criteria = new GetAllGroupCriteria();
            IList<Team> teams = _teamService.GetAll(criteria);

            return View(teams);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var model = new TeamViewModel
            {
                CreatedDate = DateTime.Now,
                CreatedBy = this.CurrentUser.Id
            };
            return View(model);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">Team ViewModel.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Create(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new InsertNewGroupCriteria
                {
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    MailGroup = model.MailGroup,
                    LeaderId = model.LeaderId,
                    AssistantLeaderId = model.AssistantLeaderId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
                };
                CommandResult commandResult = _teamService.AddNewTeam(criteria);

                if (commandResult.Code != 0)
                {
                    if (commandResult.Code == 100)
                    {
                        ModelState.AddModelError(string.Empty, Resources.Messages.DuplicateTeamCode);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, commandResult.Message);
                    }
                }
                else
                {
                    ViewBag.Inserted = true;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="id">Team's id.</param>
        /// <returns>View Edit.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            var criteria = new GetGroupByIdCriteria
            {
                GroupId = id
            };

            Team team = _teamService.GetById(criteria);

            var model = new TeamViewModel
            {
                TeamCode = team.Code,
                TeamName = team.Name,
                MailGroup = team.MailGroup,
                Leader = team.Leader != null ? team.Leader.DisplayName : string.Empty,
                LeaderId = team.Leader != null ? team.Leader.EmpId : 0,
                AssistantLeader = team.AssistantLeader != null ? team.AssistantLeader.DisplayName : string.Empty,
                AssistantLeaderId = team.AssistantLeader != null ? team.AssistantLeader.EmpId : 0,
                ModifiedDate = DateTime.Now,
                ModifiedBy = this.CurrentUser.Id
            };
            return View(model);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="model">Team ViewModel.</param>
        /// <returns>View Edit.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Edit(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.AssistantLeader))
                {
                    model.AssistantLeaderId = null;
                }

                var criteria = new UpdateGroupCriteria
                {
                    GroupId = model.Id,
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    MailGroup = model.MailGroup,
                    LeaderId = model.LeaderId,
                    AssistantLeaderId = model.AssistantLeaderId,
                    ModifiedBy = this.CurrentUser.Id,
                    ModifiedDate = DateTime.Now
                };

                CommandResult result = _teamService.UpdateTeam(criteria);

                ViewBag.Updated = result.Code == 0;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TeamSupervisor(int id)
        {
            var criteria = new GetSupervisorByTeamIdCriteria() { GroupId = id };
            var model = _teamService.GetSupervisorByTeamId(criteria);
            ViewBag.TeamId = id;

            return View(model);
        }

        /// <summary>
        /// Get Supervisors of Team.
        /// </summary>
        /// <param name="teamId">Id of team.</param>
        /// <param name="supervisorRole">Supervisor Role.</param>
        /// <returns>Action Result.</returns>
        public ActionResult ManageSupervisor(int teamId, short supervisorRole)
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            var criteria = new GetTeamSupervisorCriteria()
            {
                GroupId = teamId,
                Role = supervisorRole
            };
            var supervisorModel = _teamService.GetTeamSupervisor(criteria);

            if (supervisorRole == (short)RoleSystem.ShiftManager)
            {
                var model = new ShiftManagerViewModel
                {
                    ShiftManagerList = supervisorModel,
                    TeamId = teamId,
                    SupervisorRole = supervisorRole,
                    ForView = "Team"
                };

                return View("~/Views/Employee/ShiftManager.cshtml", model);
            }
            else
            {
                var model = new SupervisorViewModel
                {
                    SupervisorList = supervisorModel,
                    TeamId = teamId,
                    SupervisorRole = supervisorRole,
                    ForView = "Team"
                };
                
                return View("~/Views/Employee/Supervisor.cshtml", model);
            }
        }

        /// <summary>
        /// Method Insert Or Update Team Supervisor.
        /// </summary>
        /// <param name="model">Supervisor ViewModel.</param>
        /// <returns>Action Result.</returns>
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
                ModifiedBy = this.CurrentUser.Id,
                Role = model.SupervisorRole
            };
            var result = _teamService.UpdateSupervisor(criteria);

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Insert Or Update Shift Manger.
        /// </summary>
        /// <param name="model">Shift Manager ViewModel.</param>
        /// <returns>Action Result.</returns>
        [HttpPost]
        public ActionResult ShiftManager(ShiftManagerViewModel model)
        {
            var criteria = new InsertTeamShiftManagerCriteria
            {
                GroupId = model.TeamId,
                ShiftArrangers = model.RoleShiftArrange,
                ModifiedBy = this.CurrentUser.Id
            };

            bool inserted = _teamService.InsertTeamShiftManager(criteria);

            return Json(new { result = inserted }, JsonRequestBehavior.AllowGet);
        }
    }
}
