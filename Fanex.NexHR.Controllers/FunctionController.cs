namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.ViewModels.Function;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;

    public class FunctionController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IRoleService roleService;
        private readonly IFunctionService ucfunctionService;

        public FunctionController(
            IUserControlSession userControlSession,
            IRoleService roleService,
            IFunctionService ucfunctionService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.roleService = roleService;
            this.ucfunctionService = ucfunctionService;
        }

        [HRMPermission(Policy.Read, FunctionKey.FunctionManagement)]
        public ActionResult Index(short? roleId = null)
        {
            IList<Role> systemRoles = roleService.GetBySiteId(userControlSession.SiteId);
            var functionTrees = new TreeFunctionViewModel
            {
                RoleId = roleId ?? systemRoles.First().RoleId,
                IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.FunctionManagement, Policy.Host),
                Functions = ucfunctionService.GetHierarchyBySiteId(userControlSession.SiteId),
                FunctionPermissions = ucfunctionService.GetPermissionBySiteAndRoleId(userControlSession.SiteId, roleId ?? systemRoles.First().RoleId)
            };

            ViewBag.SystemRoles = systemRoles;

            return View();
        }

        [HRMPermission(Policy.Host, FunctionKey.FunctionManagement)]
        public ActionResult GetSystemRoles()
        {
            IList<Role> systemRoles = roleService.GetBySiteId(userControlSession.SiteId);
            return new CustomJsonResult { Data = systemRoles, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.FunctionManagement)]
        public CustomJsonResult AssignPermission(short roleId, IList<FunctionPermission> permissions)
        {
            CommandResult commandResult = roleService.AssignPermission(userControlSession.SiteId, roleId, permissions);
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}