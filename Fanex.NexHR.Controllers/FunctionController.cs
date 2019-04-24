namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Function;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Function;
    using Fanex.NexHR.Web.Providers;

    public class FunctionController : BaseController
    {
        private readonly IFunctionService functionService;
        private readonly IRoleService roleService;

        public FunctionController(
            IFunctionService functionService,
            IRoleService roleService)
        {
            this.functionService = functionService;
            this.roleService = roleService;
        }

        [HRAuthorize(Policy.Read, FunctionKey.FunctionManagement)]
        public ActionResult Index(int? roleId = null)
        {
            IEnumerable<SystemRoleModel> systemRoles = roleService.GetRoles();
            IEnumerable<FunctionModel> functions = functionService.GetFunctionsByRoleId(roleId ?? systemRoles.First().RoleId);
            var functionTrees = new TreeFunctionViewModel
            {
                RoleId = roleId ?? systemRoles.First().RoleId,
                IsHost = CurrentUser.HasPermission(FunctionKey.FunctionManagement, Policy.Host),
                Functions = GetFunctionTrees(functions)
            };

            ViewBag.SystemRoles = systemRoles;

            return View(functionTrees);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.FunctionManagement)]
        public CustomJsonResult AssignPermission(int roleId, IEnumerable<FunctionPermission> permissions)
        {
            CommandResult commandResult = functionService.AssignFunctionsToRole(CurrentUser.Id, roleId, permissions);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IEnumerable<FunctionModel> GetFunctionTrees(IEnumerable<FunctionModel> functions)
        {
            IEnumerable<FunctionModel> functionTrees = functions.Where(func => !func.ParentFunctionId.HasValue);

            foreach (FunctionModel function in functionTrees)
            {
                function.Level = 0;
                GetChildFunctions(function, functions);
            }

            return functionTrees;
        }

        private void GetChildFunctions(FunctionModel function, IEnumerable<FunctionModel> functions)
        {
            IEnumerable<FunctionModel> functionItems = functions.Where(x => x.ParentFunctionId == function.FunctionId);

            if (functionItems.Any())
            {
                foreach (FunctionModel item in functionItems)
                {
                    item.Level = function.Level + 1;
                    function.Childs.Add(item);
                    GetChildFunctions(item, functions);
                }
            }
        }
    }
}