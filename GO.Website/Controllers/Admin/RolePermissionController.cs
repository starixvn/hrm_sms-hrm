namespace GO.Website.Controllers.Admin
{
    using System.Threading.Tasks;
    using GO.Website.Constants;
    using GO.Website.Filters;
    using GO.Website.Models.Admin.RolePermission;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [Route("Admin/RolePermission/[action]")]
    [GOPermission(FunctionKey = FunctionKey.AdminPageRolePermission, Policy = Policy.Read)]
    public class RolePermissionController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IRoleService roleService;
        private readonly IFunctionService functionService;

        public RolePermissionController(
            IUserControlSession userControlSession,
            IRoleService roleService,
            IFunctionService functionService)
        {
            this.userControlSession = userControlSession;
            this.roleService = roleService;
            this.functionService = functionService;
        }

        public IActionResult Index()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.AdminPageRolePermission, Policy.Host);
            return View("~/Views/Admin/RolePermission/Index.cshtml", isHost);
        }

        public async Task<IActionResult> GetRolePermission(short roleId)
        {
            var functions = await functionService.GetBySiteIdAsync(userControlSession.SiteId);
            var hierarchyFunctions = await functionService.GetHierarchyBySiteIdAsync(userControlSession.SiteId);
            var permissions = await functionService.GetPermissionBySiteAndRoleIdAsync(userControlSession.SiteId, roleId);

            return Json(new { functions, permissions, hierarchyFunctions });
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.AdminPageRolePermission, Policy = Policy.Host)]
        public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionToRole model)
        {
            CommandResult commandResult = await roleService.AssignPermissionAsync(userControlSession.SiteId, model.RoleId, model.Permissions);
            return Json(commandResult);
        }
    }
}