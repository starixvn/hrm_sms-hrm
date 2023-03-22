namespace GO.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Data;
    using GO.Website.Criteria;
    using GO.Website.Models;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IUserControlSettings userControlSettings;
        private readonly IUserControlSession userControlSession;
        private readonly IAuthService authService;
        private readonly IUserService userService;
        private readonly IBaseService baseService;

        public HomeController(
            IUserControlSettings userControlSettings,
            IUserControlSession userControlSession,
            IAuthService authService,
            IUserService userService,
            IBaseService baseService)
        {
            this.userControlSettings = userControlSettings;
            this.userControlSession = userControlSession;
            this.authService = authService;
            this.baseService = baseService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetSiteMenus()
        {
            var userId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.UserId.ToString()));
            var onUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.OnUserId.ToString()));
            var criteria = new GetSiteAndMenuItemByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            (var usersites, var menus) = await baseService.FetchMultipleAsync<UserSiteMenu, MenuItem>(criteria);
            usersites = usersites.Where(x => x.SiteId != userControlSettings.SiteId).ToList();
            foreach(var usersite in usersites)
            {
                var username = !string.IsNullOrWhiteSpace(usersite.Username) ? usersite.Username : usersite.Code;
                username = !string.IsNullOrWhiteSpace(username) ? HttpUtility.UrlEncode(Cryptography.EncryptPassword(username)) : string.Empty;
                usersite.MenuItems = menus.Where(x => x.SiteId == usersite.SiteId).ToList();

                usersite.SiteURL = $"{usersite.SSOUrl}?userId={userId}&onUserId={onUserId}&username={username}";

                foreach (var menu in usersite.MenuItems)
                {
                    if (!string.IsNullOrWhiteSpace(menu.URL))
                    {
                        menu.URL = $"{usersite.SSOUrl}?userId={userId}&onUserId={onUserId}&username={username}&redirectUrl={menu.URL}";
                    }
                }

                usersite.MenuItems = GetHierarchyMenu(usersite.MenuItems);
            }

            return Json(usersites);
        }

        [Route("sso/gettime")]
        public async Task<IActionResult> GetTime()
        {
            var time = HttpUtility.UrlEncode(Cryptography.EncryptPassword(await authService.SSOGetTimeAsync()));
            return Json(time);
        }

        public IActionResult GetUserInfo()
        {
            User user = userService.GetById(userControlSession.UserId);
            user.UserId = 0;
            user.Password = string.Empty;
            return Json(user);
        }

        private static IList<MenuItem> GetHierarchyMenu(IList<MenuItem> menus)
        {
            IList<MenuItem> menuTrees = menus.Where(menu => !menu.ParentId.HasValue)
                .OrderBy(x => x.DisplayOrder).ToList();
            foreach (var menu in menuTrees)
            {
                menu.Level = 0;
                GetChildMenus(menu, menus);
            }

            return menuTrees;
        }

        private static void GetChildMenus(MenuItem menu, IList<MenuItem> menus)
        {
            IList<MenuItem> menuItems = menus.Where(x => x.ParentId == menu.MenuItemId)
                .OrderBy(x => x.DisplayOrder).ToList();
            if (menuItems.Any())
            {
                menu.Childs ??= new List<MenuItem>();
                foreach (var item in menuItems)
                {
                    item.Level = (byte)(item.Level + 1);
                    menu.Childs.Add(item);
                    GetChildMenus(item, menus);
                }
            }
        }
    }
}