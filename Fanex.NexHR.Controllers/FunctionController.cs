namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class FunctionController : BaseController
    {
        private readonly IFunctionService functionService;
        private readonly IRoleService roleService;

        public FunctionController()
            : this(null, null)
        {
        }

        public FunctionController(IFunctionService functionService, IRoleService roleService)
        {
            this.functionService = functionService ?? new FunctionService();
            this.roleService = roleService ?? new RoleService();
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
            IEnumerable<FunctionViewModel> functions = functionService.GetFunctionsByRoleId(roleId);
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

            Result commandResult = functionService.AssignFunctionsToRole(criteria);

            return Json(new { success = commandResult.IsSuccessful });
        }
    }
}