namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Criteria.Role;
    using HRM.UserControl.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    public class RoleController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IRoleService roleService;
        private readonly IBaseService baseService;

        public RoleController(
            IUserControlSession userControlSession,
            IRoleService roleService,
            IBaseService baseService)
        {
            this.userControlSession = userControlSession;
            this.roleService = roleService;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAll(short siteId, bool? isActive = null)
        {
            IList<Role> roles = await roleService.GetBySiteIdAsync(siteId, isActive);
            return Json(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateRoleCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateRoleStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}