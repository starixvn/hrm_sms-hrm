namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.ExtensionNumber;

    public class ExtensionNoController : BaseController
    {
        private readonly IExtensionNumberService extensionNumberService;
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;

        public ExtensionNoController(
            ISessionService sessionService,
            IExtensionNumberService extensionNumberService,
            ITeamService teamService,
            IBaseService baseService) : base(sessionService)
        {
            this.extensionNumberService = extensionNumberService;
            this.teamService = teamService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.ExtensionNumber)]
        public ActionResult Index()
        {
            IEnumerable<ExtensionNoModel> lstEntity = extensionNumberService.GetAllExtensionNo();

            var model = (from item in lstEntity
                         group item by item.TeamName into g
                         select new ExtensionNumberViewModel { Name = g.Key, Extensions = g.ToList() }).ToList();
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.ExtensionNumber, Policy.Host);

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
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
                        EmpId = model.EmpId,
                        GroupId = model.TeamId,
                    };

                    CommandResult result = baseService.ExecuteNonQuery(criteria);
                    if (result.IsSuccessful)
                    {
                        return RedirectToAction("Index", "ExtensionNo");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            return View();
        }

        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Edit(bool isInternal, int employeeExtensionId)
        {
            ExtensionNoModel model = extensionNumberService.GetEmployeeExtensionNoById(employeeExtensionId, isInternal);
            IEnumerable<ExtensionNoModel> groups = extensionNumberService.GetAllExtensionGroupInfo(isInternal: isInternal);

            if (groups != null)
            {
                ViewBag.Group = groups.Select(x => new SelectListItem { Text = x.TeamName, Value = x.TeamId.ToString() }).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Edit(ExtensionNoModel model)
        {
            CheckExternalEmployee(model);
            IEnumerable<ExtensionNoModel> group = extensionNumberService.GetAllExtensionGroupInfo(isInternal: model.IsInternal);
            ViewBag.Group = group.Select(x => new SelectListItem { Text = x.TeamName, Value = x.TeamId.ToString() }).ToList();
            if (ModelState.IsValid)
            {
                var criteria = new UpdateExtensionNumberMemberCriteria
                {
                    FullName = model.FullName,
                    NickName = model.NickName,
                    ExtensionNo = model.ExtensionNo,
                    IsInternal = model.IsInternal,
                    ExtId = model.EmployeeExtensionId,
                    EmpId = model.EmpId,
                    GroupId = model.TeamId
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);
                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index", "ExtensionNo");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public JsonResult DeleteExtensionNumber(int employeeExtensionId)
        {
            CommandResult commandResult = extensionNumberService.DeleteExtensionNumberById(employeeExtensionId);

            return Json(commandResult);
        }

        #region External Group

        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult ManageExternalGroup()
        {
            IEnumerable<ExtensionNoModel> group = extensionNumberService.GetAllExtensionGroupInfo(isInternal: false);

            return View(group);
        }

        [HttpGet]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult GetGroupExtension(bool isInternal, int? groupId)
        {
            IList<SelectListItem> groups = null;
            if (isInternal)
            {
                IList<Team> teams = teamService.GetAll();
                groups = teams.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            }
            else
            {
                IEnumerable<ExtensionNoModel> extensionGroups = extensionNumberService.GetAllExtensionGroupInfo(groupId, isInternal);
                groups = extensionGroups.Select(x => new SelectListItem
                {
                    Text = x.TeamName,
                    Value = x.TeamId.ToString()
                }).ToList();
            }

            return Json(groups, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public JsonResult AddNewExternalGroup(string groupName)
        {
            CommandResult commandResult = extensionNumberService.CreateExternalGroup(groupName);

            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public JsonResult DeleteExternalGroup(int groupId)
        {
            CommandResult commandResult = extensionNumberService.DeleteExternalGroup(groupId);

            return Json(commandResult);
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public JsonResult EditExternalGroup(int groupId, string groupName)
        {
            CommandResult commandResult = extensionNumberService.EditExternalGroup(groupId, groupName);

            return Json(commandResult);
        }

        #endregion External Group

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