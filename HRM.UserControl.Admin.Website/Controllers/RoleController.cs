namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Constants;
    using HRM.UserControl.Admin.Website.Criteria.Role;
    using HRM.UserControl.Admin.Website.Filters;
    using HRM.UserControl.Admin.Website.Models;
    using HRM.UserControl.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [UCPermission(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Read)]
    public class RoleController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IRoleService roleService;
        private readonly IFunctionService functionService;
        private readonly IBaseService baseService;

        public RoleController(
            IUserControlSession userControlSession,
            IRoleService roleService,
            IFunctionService functionService,
            IBaseService baseService)
        {
            this.userControlSession = userControlSession;
            this.roleService = roleService;
            this.functionService = functionService;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.RoleManagement, Policy.Host);
            return View(isHost);
        }

        public async Task<IActionResult> GetBySiteId(short siteId, bool? isActive = null)
        {
            IList<Role> roles = await roleService.GetBySiteIdAsync(siteId, isActive);
            return Json(roles);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody] UpdateRoleCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateRoleStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        public IActionResult Permission(short? siteId)
        {
            ViewBag.isHost = userControlSession.HasPermissionOnFunction(FunctionKey.RoleManagement, Policy.Host);
            return View(siteId);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.RoleManagement, Policy = Policy.Host)]
        public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionToRole model)
        {
            CommandResult commandResult = await roleService.AssignPermissionAsync(model.SiteId, model.RoleId, model.Permissions);
            return Json(commandResult);
        }

        public async Task<IActionResult> GetRolePermission(short siteId, short roleId)
        {
            var functions = await functionService.GetBySiteIdAsync(siteId);
            var hierarchyFunctions = await functionService.GetHierarchyBySiteIdAsync(siteId);
            var permissions = await functionService.GetPermissionBySiteAndRoleIdAsync(siteId, roleId);
            return Json(new { functions, hierarchyFunctions, permissions });
        }
    }
}