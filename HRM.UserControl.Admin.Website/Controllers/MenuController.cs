namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Constants;
    using HRM.UserControl.Admin.Website.Criteria.Menu;
    using HRM.UserControl.Admin.Website.Filters;
    using HRM.UserControl.Admin.Website.Models.Menu;
    using HRM.UserControl.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [UCPermission(FunctionKey = FunctionKey.MenuManagement, Policy = Policy.Read)]
    public class MenuController : Controller
    {
        private readonly IBaseService baseService;
        private readonly IUserControlSession userControlSession;

        public MenuController(IBaseService baseService, IUserControlSession userControlSession)
        {
            this.baseService = baseService;
            this.userControlSession = userControlSession;
        }

        public IActionResult Index()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.MenuManagement, Policy.Host);
            return View(isHost);
        }

        public async Task<IActionResult> GetBySiteId(short id)
        {
            var criteria = new GetMenuBySiteIdCriteria { SiteId = id };
            var menus = await baseService.GetDataAsync<Menu>(criteria);
            var hierarchyMenus = GetHierarchyMenu(menus.ToList());
            return Json(hierarchyMenus);
        }

        public async Task<IActionResult> GetParentBySiteId(short id)
        {
            var criteria = new GetMenuBySiteIdCriteria { SiteId = id };
            var menus = await baseService.GetDataAsync<Menu>(criteria);
            return Json(menus);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.MenuManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreateMenuCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.MenuManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody] EditMenuCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.MenuManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateMenuStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        private IList<Menu> GetHierarchyMenu(IList<Menu> menus)
        {
            IList<Menu> menuTrees = menus.Where(menu => !menu.ParentId.HasValue)
                .OrderBy(x => x.DisplayOrder).ToList();
            foreach (Menu menu in menuTrees)
            {
                menu.Level = 0;
                GetChildMenus(menu, menus);
            }

            return menuTrees;
        }

        private void GetChildMenus(Menu menu, IList<Menu> menus)
        {
            IList<Menu> menuItems = menus.Where(x => x.ParentId == menu.MenuItemId)
                .OrderBy(x => x.DisplayOrder).ToList();
            if (menuItems.Any())
            {
                menu.Childs ??= new List<Menu>();

                foreach (Menu item in menuItems)
                {
                    item.Level = (byte)(item.Level + 1);
                    menu.Childs.Add(item);
                    GetChildMenus(item, menus);
                }
            }
        }
    }
}
