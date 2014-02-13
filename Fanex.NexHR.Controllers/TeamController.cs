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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
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

        public TeamController(ITeamService teamService = null)
        {
            _teamService = teamService ?? new TeamService();
        }

        [Authorize(Roles = "HR")]
        public ActionResult Index()
        {
            IList<Team> teams = _teamService.GetAll();

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
                var errorCode = 0;
                var message = string.Empty;
                _teamService.AddNewTeam(model, out errorCode, out message);
                if (errorCode != 0) 
                {
                    if (errorCode == 100)
                    {
                        ModelState.AddModelError(string.Empty, Resources.Messages.DuplicateTeamCode);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, message);
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
            var team = _teamService.Get(id);

            var model = new TeamViewModel
            {
                TeamCode = team.Code,
                TeamName = team.Name,
                MailGroup = team.MailGroup,
                Leader = team.Leader.DisplayName,
                LeaderId = team.Leader.Id,
                AssistantLeader = team.AssistantLeader.DisplayName,
                AssistantLeaderId = team.AssistantLeader.Id,
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
                var errorCode = 0;
                var message = string.Empty;

                if (string.IsNullOrEmpty(model.AssistantLeader))
                {
                    model.AssistantLeaderId = null;
                }

                var result = _teamService.UpdateTeam(model, out errorCode, out message);

                ViewBag.Updated = result;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }
    }
}
