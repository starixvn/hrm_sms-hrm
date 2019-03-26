namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Function;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
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
        public ActionResult Index()
        {
            ViewBag.roles = roleService.GetRoles();
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.FunctionManagement, Policy.Host);

            return View();
        }

        [HRAuthorize(Policy.Read, FunctionKey.FunctionManagement)]
        public ActionResult GetFunctionByRoleId(int roleId)
        {
            IEnumerable<FunctionModel> functions = functionService.GetFunctionsByRoleId(roleId);
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.FunctionManagement, Policy.Host);

            return PartialView("_GetFunctionByRoleId", functions);
        }

        [HRAuthorize(Policy.Host, FunctionKey.FunctionManagement)]
        public JsonResult AssignFunctionToRole(RoleFunctionViewModel roleFunction)
        {
            var criteria = new AssignFunctionToRoleCriteria
            {
                Functions = roleFunction.Functions.ToList(),
                RoleId = roleFunction.RoleId,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult commandResult = functionService.AssignFunctionsToRole(criteria);

            return Json(new { success = commandResult.IsSuccessful });
        }
    }
}