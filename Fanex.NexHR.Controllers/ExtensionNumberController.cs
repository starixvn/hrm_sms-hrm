// --------------------------------------------------------------------------
// <copyright file="ExtensionNumberController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.28">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.ExtensionNumber;

    public class ExtensionNumberController : BaseController
    {
        private readonly IExtensionNumberService _extensionNumberService;
        private readonly ITeamService _teamService;

        public ExtensionNumberController()
            : this(null, null)
        {
        }

        public ExtensionNumberController(IExtensionNumberService extensionNumberService = null, ITeamService teamService = null)
        {
            this._extensionNumberService = extensionNumberService ?? new ExtensionNumberService();
            this._teamService = teamService ?? new TeamService();
        }

        public ActionResult Index()
        {
            var authorize = false;
            var listRole = this.CurrentUser.RoleList;
            if (listRole != null)
            {
                authorize = listRole.Any(x => x == (int)RoleSystem.HR);
            }

            ViewBag.Authorize = authorize;
            IEnumerable<ExtensionNoModel> lstEntity = _extensionNumberService.GetAllExtensionNo(new GetAllExtensionNumberCriteria());
            var model = (from item in lstEntity
                         group item by item.TeamName into g
                         select new ExtensionNumberViewModel() { Name = g.Key, ListExt = g.ToList() }).ToList();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult AddNewMember()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult AddNewMember(ExtensionNoModel model)
        {
            CheckExternalEmployee(model);
            if (ModelState.IsValid)
            {
                var criteria = new InsertExtensionNumberCriteria
                {
                    FullName = model.FullName,
                    NickName = model.NickName,
                    ExtensionNo = model.ExtensionNo,
                    IsInternal = model.IsInternal,
                    EmpID = model.EmpId,
                    GroupID = model.TeamId,
                };

                CommandResult commandResult = _extensionNumberService.AddNewExtensionNoMember(criteria);
                ViewBag.Requested = commandResult.Code == 0;
            }

            return View();
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(bool isInternal, int empExtID)
        {
            var getExtensionNoByIdcriteria = new GetExtensionNumberMemberByIdCriteria
            {
                IsInternal = isInternal,
                EmployeeExtensionNoId = empExtID
            };
            ExtensionNoModel model = _extensionNumberService.GetEmployeeExtensionNoById(getExtensionNoByIdcriteria);
            var getAllExtensionNoGroupInfo = new GetExtensionNumberGroupInfoCriteria
            {
                IsInternal = isInternal,
                GroupID = null
            };
            IEnumerable<ExtensionNoModel> groups = _extensionNumberService.GetAllExtensionGroupInfo(getAllExtensionNoGroupInfo);

            if (groups != null)
            {
                ViewBag.Group = groups.Select(x => new SelectListItem
                {
                    Text = x.TeamName,
                    Value = x.TeamId.ToString()
                }).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Edit(ExtensionNoModel model)
        {
            CheckExternalEmployee(model);
            var getAllExtensionNoGroupInfo = new GetExtensionNumberGroupInfoCriteria
            {
                IsInternal = model.IsInternal,
                GroupID = null
            };
            IEnumerable<ExtensionNoModel> group = _extensionNumberService.GetAllExtensionGroupInfo(getAllExtensionNoGroupInfo);
            ViewBag.Group = group.Select(x => new SelectListItem
            {
                Text = x.TeamName,
                Value = x.TeamId.ToString()
            }).ToList();
            if (ModelState.IsValid)
            {
                var updateExtensionNoCriteria = new UpdateExtensionNumberMemberCriteria
                {
                    FullName = model.FullName,
                    NickName = model.NickName,
                    ExtensionNo = model.ExtensionNo,
                    IsInternal = model.IsInternal,
                    ExtID = model.EmpExtID,
                    EmpID = model.EmpId,
                    GroupID = model.TeamId
                };
                CommandResult commandResult = _extensionNumberService.UpdateExtensionNoMember(updateExtensionNoCriteria);
                ViewBag.Updated = commandResult.Code == 0;
                return RedirectToAction("ExtensionNo");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ManageExternalGroup()
        {
            var getAllExtensionNoGroupInfo = new GetExtensionNumberGroupInfoCriteria
            {
                IsInternal = false,
                GroupID = null
            };
            IEnumerable<ExtensionNoModel> group = _extensionNumberService.GetAllExtensionGroupInfo(getAllExtensionNoGroupInfo);
            return View(group);
        }

        [HttpGet]
        [HttpPost]
        public IList<System.Web.Mvc.SelectListItem> GetGroupExtension(bool isInternal, int? groupId)
        {
            if (isInternal)
            {
                var getAllGroupCriteria = new GetAllGroupCriteria();
                IList<Team> data = _teamService.GetAll(getAllGroupCriteria);
                var selectList = data.Select(x => new System.Web.Mvc.SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                return selectList;
            }
            else
            {
                var getGroupInfoCriteria = new GetExtensionNumberGroupInfoCriteria
                {
                    IsInternal = isInternal,
                    GroupID = groupId
                };
                IEnumerable<ExtensionNoModel> data = _extensionNumberService.GetAllExtensionGroupInfo(getGroupInfoCriteria);
                var selectList = data.Select(x => new System.Web.Mvc.SelectListItem
                {
                    Text = x.TeamName,
                    Value = x.TeamId.ToString()
                }).ToList();
                return selectList;
            }
        }

        private void CheckExternalEmployee(ExtensionNoModel model)
        {
            if (!model.IsInternal)
            {
                foreach (var modelError in ModelState)
                {
                    string propertyName = modelError.Key;

                    if (propertyName.Contains("EmpId"))
                    {
                        ModelState[propertyName].Errors.Clear();
                    }
                }
            }
        }
    }
}
