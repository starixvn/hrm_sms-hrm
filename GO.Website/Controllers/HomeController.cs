namespace GO.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using GO.Website.Criteria;
    using GO.Website.Models;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IUserControlSettings userControlSettings;
        private readonly IUserControlSession userControlSession;
        private readonly IAuthService authService;
        private readonly IBaseService baseService;

        public HomeController(
            IUserControlSettings userControlSettings,
            IUserControlSession userControlSession,
            IAuthService authService,
            IBaseService baseService)
        {
            this.userControlSettings = userControlSettings;
            this.userControlSession = userControlSession;
            this.authService = authService;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetSiteMenus()
        {
            var time = Cryptography.EncryptPassword(await authService.SSOGetTimeAsync());
            var userId = Cryptography.EncryptPassword(userControlSession.UserId.ToString());
            var onUserId = Cryptography.EncryptPassword(userControlSession.OnUserId.ToString());
            var criteria = new GetSiteAndMenuItemByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            (var sites, var menus) = await baseService.FetchMultipleAsync<SiteMenu, MenuItem>(criteria);
            sites = sites.Where(x => x.SiteId != userControlSettings.SiteId).ToList();
            foreach(var site in sites)
            {
                site.MenuItems = menus.Where(x => x.SiteId == site.SiteId).ToList();
                foreach(var menu in site.MenuItems)
                {
                    if (!string.IsNullOrWhiteSpace(menu.URL))
                    {
                        menu.URL = string.Format("{0}/auth/sso?userId={1}&onUserId={2}&time={3}&redirectUrl={4}",
                            site.SiteURL, userId, onUserId, time, menu.URL);
                    }
                }

                site.MenuItems = GetHierarchyMenu(site.MenuItems);
            }

            return Json(sites);
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