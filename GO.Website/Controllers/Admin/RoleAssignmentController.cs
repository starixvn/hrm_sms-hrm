namespace GO.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Website.Criteria.RoleAssignment;
    using GO.Website.Models.RoleAssignment;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("Admin/RoleAssignment/[action]")]
    public class RoleAssignmentController : BaseController
    {
        private readonly IUserControlSettings userControlSettings;
        private readonly IBaseService baseService;
        private readonly IUserService userService;

        public RoleAssignmentController(
            IUserControlSettings userControlSettings,
            IBaseService baseService,
            IUserService userService)
        {
            this.userControlSettings = userControlSettings;
            this.baseService = baseService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return View();
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
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleParam param)
        {
            CommandResult commandResult = await userService.AssignRoleAsync(param);
            return Json(commandResult);
        }
    }
}