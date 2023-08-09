namespace GO.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Criteria.RoleAssignment;
    using GO.Website.Filters;
    using GO.Website.Models.RoleAssignment;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [Route("Admin/RoleAssignment/[action]")]
    [GOPermission(FunctionKey = FunctionKey.RoleAssignment, Policy = Policy.Read)]
    public class RoleAssignmentController : BaseController
    {
        private readonly IUserControlSettings userControlSettings;
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;
        private readonly IUserService userService;

        public RoleAssignmentController(
            IUserControlSettings userControlSettings,
            IUserControlSession userControlSession,
            IBaseService baseService,
            IUserService userService)
        {
            this.userControlSettings = userControlSettings;
            this.userControlSession = userControlSession;
            this.baseService = baseService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.RoleAssignment, Policy.Host);
            return View("~/Views/Admin/RoleAssignment/Index.cshtml", isHost);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserRoles([FromBody] GetUserRoleCriteria criteria)
        {
            criteria.SiteId = userControlSettings.SiteId;

            (var users, var roles) = await baseService.FetchMultipleAsync<UserInfo, UserRole>(criteria);
            foreach(var user in users)
            {
                user.Roles = roles.Where(x => x.UserId == user.UserId).ToList();
            }

            return Json(new { totalItems = criteria.TotalRows, data = users });
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.RoleAssignment, Policy = Policy.Host)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleParam param)
        {
            CommandResult commandResult = await userService.AssignRoleAsync(param);
            return Json(commandResult);
        }
    }
}