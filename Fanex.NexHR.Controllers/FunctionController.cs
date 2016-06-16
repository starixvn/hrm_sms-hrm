namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class FunctionController : BaseController
    {
        private IFunctionService _functionService;
        private IRoleService _roleService;

        public FunctionController()
            : this(null, null)
        {
        }

        public FunctionController(IFunctionService functionService, IRoleService roleService)
        {
            _functionService = functionService ?? new FunctionService();
            _roleService = roleService ?? new RoleService();
        }

        [HRAuthorize(Policy.Read, FunctionKey.FunctionManagement)]
        public ActionResult Index()
        {
            ViewBag.roles = _roleService.GetRoles();

            return View();
        }

        [HRAuthorize(Policy.Read, FunctionKey.FunctionManagement)]
        public ActionResult GetFunctionByRoleId(int roleId)
        {
            IEnumerable<FunctionModel> functions = _functionService.GetFunctionsByRoleId(roleId);

            return PartialView("_GetFunctionByRoleId", functions);
        }

        [HRAuthorize(Policy.Edit, FunctionKey.FunctionManagement)]
        public JsonResult AssignFunctionToRole(RoleFunctionViewModel roleFunction)
        {
            var criteria = new AssignFunctionToRoleCriteria
            {
                Functions = roleFunction.Functions.ToList(),
                RoleId = roleFunction.RoleId,
                ModifiedBy = CurrentUser.Id
            };

            Result commandResult = _functionService.AssignFunctionsToRole(criteria);

            return Json(new { success = commandResult.IsSuccessful });
        }
    }
}
