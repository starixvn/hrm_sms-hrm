namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Constants;
    using HRM.UserControl.Admin.Website.Criteria.User;
    using HRM.UserControl.Admin.Website.Filters;
    using HRM.UserControl.Admin.Website.Models.Site;
    using HRM.UserControl.Admin.Website.Models.User;
    using HRM.UserControl.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Read)]
    public class UserController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IFunctionService functionService;
        private readonly IUserService userService;

        public UserController(
            IUserControlSession userControlSession,
            IBaseService baseService,
            ICryptoService cryptoService,
            IFunctionService functionService,
            IUserService userService)
        {
            this.userControlSession = userControlSession;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.functionService = functionService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            var token = cryptoService.GenerateRijndaelKeyIV();
            ViewBag.isHost = userControlSession.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host);
            return View("Index", token);
        }

        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] GetAllUserCriteria criteria)
        {
            var users = await baseService.GetDataAsync<Models.User.User>(criteria);
            return Json(new { totalItems = criteria.TotalRows, data = users });
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreateUserParam param)
        {
            CommandResult commandResult = await userService.CreateAsync(param);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public async Task<IActionResult> ResetPassword([FromBody] UpdatePasswordParam param)
        {
            CommandResult commandResult = await userService.ResetPasswordAsync(param);
            return Json(commandResult);
        }

        public async Task<IActionResult> AssignSite(int id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            user.Password = string.Empty;
            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host);
            return View(user);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public async Task<IActionResult> AssignSite([FromBody] AssignSiteToUserCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        public async Task<IActionResult> GetSiteByUserId(int id)
        {
            var criteria = new GetSiteByUserIdCriteria { UserId = id };
            (var sites, var roles) = await baseService.FetchMultipleAsync<UserSite, UserRole>(criteria);
            foreach (var site in sites)
            {
                site.Roles = roles.Where(x => x.SiteId == site.SiteId).ToList();
            }

            return Json(sites);
        }

        public async Task<IActionResult> GetAvailableSiteByUserId(int id)
        {
            var criteria = new GetAvailableSiteByUserIdCriteria { UserId = id };
            var sites = await baseService.GetDataAsync<Site>(criteria);
            return Json(sites);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateUserSiteStatus([FromBody] UpdateUserSiteStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateUserSiteRole([FromBody] UpdateUserSiteRoleCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        public async Task<IActionResult> Permission(int id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            user.Password = string.Empty;
            return View(user);
        }

        public async Task<IActionResult> GetUserPermission(short siteId, int userId)
        {
            IList<Function> functions = await functionService.GetHierarchyBySiteIdAsync(siteId, true);
            IList<FunctionPermission> permissions = await userService.GetFunctionPermissionAsync(siteId, userId);
            return Json(new { functions, permissions });
        }

        public async Task<IActionResult> UserMapping(int id)
        {
            var user = await userService.GetByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            user.Password = string.Empty;
            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host);
            return View("UserMapping", user);
        }

        public async Task<IActionResult> GetUserMappingByUserId(int id)
        {
            var criteria = new GetMappingByUserIdCriteria { UserId = id };
            var mappings = await baseService.GetDataAsync<UserMapping>(criteria);

            return Json(mappings);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateUserMapping([FromBody] UpdateUserMappingCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}