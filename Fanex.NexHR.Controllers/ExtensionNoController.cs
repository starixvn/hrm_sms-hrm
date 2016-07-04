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
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;

    public class ExtensionNoController : BaseController
    {
        private readonly IExtensionNumberService extensionNumberService;
        private readonly ITeamService teamService;
        private readonly ICommandService commandService;

        public ExtensionNoController()
            : this(null, null, null)
        {
        }

        public ExtensionNoController(IExtensionNumberService extensionNumberService, ITeamService teamService, ICommandService commandService)
        {
            this.extensionNumberService = extensionNumberService ?? new ExtensionNumberService();
            this.teamService = teamService ?? new TeamService();
            this.commandService = commandService ?? new CommandService();
        }

        [HRAuthorize(Policy.Read, FunctionKey.ExtensionNumber)]
        public ActionResult Index()
        {
            IEnumerable<ExtensionNoModel> lstEntity = extensionNumberService.GetAllExtensionNo(new GetAllExtensionNumberCriteria());
            var model = (from item in lstEntity
                         group item by item.TeamName into g
                         select new ExtensionNumberViewModel() { Name = g.Key, Extensions = g.ToList() }).ToList();
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.ExtensionNumber, Policy.Host);

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Create(ExtensionNoModel model)
        {
            if (model != null)
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

                    CommandResult result = commandService.Insert(criteria);
                    if (result.IsSuccessful)
                    {
                        ViewBag.Requested = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            return View();
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Edit(bool isInternal, int extensionNoId)
        {
            var getExtensionNoByIdcriteria = new GetExtensionNumberMemberByIdCriteria
            {
                IsInternal = isInternal,
                EmployeeExtensionNoId = extensionNoId
            };
            ExtensionNoModel model = extensionNumberService.GetEmployeeExtensionNoById(getExtensionNoByIdcriteria);
            var getAllExtensionNoGroupInfo = new GetExtensionNumberGroupInfoCriteria
            {
                IsInternal = isInternal,
                GroupID = null
            };
            IEnumerable<ExtensionNoModel> groups = extensionNumberService.GetAllExtensionGroupInfo(getAllExtensionNoGroupInfo);

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

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Edit(ExtensionNoModel model)
        {
            CheckExternalEmployee(model);
            var getAllExtensionNoGroupInfo = new GetExtensionNumberGroupInfoCriteria
            {
                IsInternal = model.IsInternal,
                GroupID = null
            };
            IEnumerable<ExtensionNoModel> group = extensionNumberService.GetAllExtensionGroupInfo(getAllExtensionNoGroupInfo);
            ViewBag.Group = group.Select(x => new SelectListItem
            {
                Text = x.TeamName,
                Value = x.TeamId.ToString()
            }).ToList();
            if (ModelState.IsValid)
            {
                var criteria = new UpdateExtensionNumberMemberCriteria
                {
                    FullName = model.FullName,
                    NickName = model.NickName,
                    ExtensionNo = model.ExtensionNo,
                    IsInternal = model.IsInternal,
                    ExtID = model.ExtensionNoId,
                    EmpID = model.EmpId,
                    GroupID = model.TeamId
                };
                CommandResult result = commandService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        public ActionResult ManageExternalGroup()
        {
            var getAllExtensionNoGroupInfo = new GetExtensionNumberGroupInfoCriteria
            {
                IsInternal = false,
                GroupID = null
            };
            IEnumerable<ExtensionNoModel> group = extensionNumberService.GetAllExtensionGroupInfo(getAllExtensionNoGroupInfo);
            return View(group);
        }

        [HttpGet]
        public ActionResult GetGroupExtension(bool isInternal, int? groupId)
        {
            IList<SelectListItem> groups = null;
            if (isInternal)
            {
                var getAllGroupCriteria = new GetAllGroupCriteria();
                IList<Team> teams = teamService.GetAll(getAllGroupCriteria);
                groups = teams.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            }
            else
            {
                var getGroupInfoCriteria = new GetExtensionNumberGroupInfoCriteria
                {
                    IsInternal = isInternal,
                    GroupID = groupId
                };
                IEnumerable<ExtensionNoModel> extensionGroups = extensionNumberService.GetAllExtensionGroupInfo(getGroupInfoCriteria);
                groups = extensionGroups.Select(x => new SelectListItem
                {
                    Text = x.TeamName,
                    Value = x.TeamId.ToString()
                }).ToList();
            }

            return Json(groups, JsonRequestBehavior.AllowGet);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ExtensionNumber)]
        public JsonResult DeleteExtensionNumber(int extId)
        {
            var criteria = new DeleteExtensionNumberMemberCriteria
            {
                ExtensionNumberId = extId
            };
            CommandResult result = commandService.Delete(criteria);
            return Json(result);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ExtensionNumber)]
        public JsonResult AddNewExternalGroup(string groupName)
        {
            var criteria = new InsertExternalGroupCriteria
            {
                FullName = groupName
            };
            CommandResult result = commandService.Insert(criteria);
            return Json(result);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ExtensionNumber)]
        public JsonResult DeleteExternalGroup(int extId)
        {
            var criteria = new DeleteExternalGroupCriteria
            {
                ExternalId = extId
            };
            CommandResult result = commandService.Delete(criteria);
            return Json(result);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ExtensionNumber)]
        public JsonResult EditExternalGroup(int extId, string groupName)
        {
            var criteria = new UpdateExternalNumberCriteria
            {
                ExternalId = extId,
                FullName = groupName
            };
            CommandResult result = commandService.Update(criteria);
            return Json(result);
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
