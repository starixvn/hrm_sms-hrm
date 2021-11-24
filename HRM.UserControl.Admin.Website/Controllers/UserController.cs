namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Criteria.User;
    using HRM.UserControl.Admin.Website.Models.Site;
    using HRM.UserControl.Admin.Website.Models.User;
    using HRM.UserControl.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IUserService userService;
        private readonly IRoleService roleService;

        public UserController(
            IUserControlSession userControlSession,
            IBaseService baseService,
            ICryptoService cryptoService,
            IUserService userService,
            IRoleService roleService)
        {
            this.userControlSession = userControlSession;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.userService = userService;
            this.roleService = roleService;
        }

        public IActionResult Index()
        {
            var token = cryptoService.GenerateRijndaelKeyIV();
            return View("Index", token);
        }

        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] GetAllUserCriteria criteria)
        {
            var users = await baseService.GetDataAsync<Models.User.User>(criteria);
            return Json(new { totalItems = criteria.TotalRows, data = users });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserParam param)
        {
            CommandResult commandResult = await userService.CreateAsync(param);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] UpdatePasswordParam param)
        {
            CommandResult commandResult = await userService.ResetPasswordAsync(param);
            return Json(commandResult);
        }

        public IActionResult AssignSite(int id)
        {
            return View(id);
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

        public async Task<IActionResult> GetRoleBySiteId(short siteId)
        {
            var roles = await roleService.GetBySiteIdAsync(siteId);
            return Json(roles);
        }

        public async Task<IActionResult> GetAvailableSiteByUserId(int id)
        {
            var criteria = new GetAvailableSiteByUserIdCriteria { UserId = id };
            var sites = await baseService.GetDataAsync<Site>(criteria);
            return Json(sites);
        }

        [HttpPost]
        public async Task<IActionResult> AssignSite([FromBody] AssignSiteToUserCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserSiteStatus([FromBody] UpdateUserSiteStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserSiteRole([FromBody] UpdateUserSiteRoleCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}