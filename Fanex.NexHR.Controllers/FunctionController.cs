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
            IList<Function> functions = ucfunctionService.GetBySiteId(userControlSession.SiteId);
            var functionTrees = new TreeFunctionViewModel
            {
                RoleId = roleId ?? systemRoles.First().RoleId,
                IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.FunctionManagement, Policy.Host),
                Functions = GetFunctionTrees(functions),
                FunctionPermissions = ucfunctionService.GetPermissionBySiteAndRoleId(userControlSession.SiteId, roleId ?? systemRoles.First().RoleId)
            };

            ViewBag.SystemRoles = systemRoles;

            return View(functionTrees);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.FunctionManagement)]
        public CustomJsonResult AssignPermission(short roleId, IList<FunctionPermission> permissions)
        {
            CommandResult commandResult = roleService.AssignPermission(userControlSession.SiteId, roleId, permissions);
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IList<Function> GetFunctionTrees(IList<Function> functions)
        {
            IList<Function> functionTrees = functions.Where(func => !func.ParentId.HasValue).ToList();
            foreach (Function function in functionTrees)
            {
                function.Childs = new List<Function>();
                function.Level = 0;
                GetChildFunctions(function, functions);
            }

            return functionTrees;
        }

        private void GetChildFunctions(Function function, IList<Function> functions)
        {
            IList<Function> functionItems = functions.Where(x => x.ParentId == function.FunctionId).ToList();
            if (functionItems.Any())
            {
                foreach (Function item in functionItems)
                {
                    item.Level = (byte)(function.Level + 1);
                    function.Childs.Add(item);
                    GetChildFunctions(item, functions);
                }
            }
        }
    }
}