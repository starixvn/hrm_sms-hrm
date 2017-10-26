namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;

    public class MenuController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly IFunctionService functionService;

        public MenuController()
            : this(null, null)
        {
        }

        public MenuController(IMenuService menuService, IFunctionService functionService)
        {
            this.menuService = menuService ?? new MenuService();
            this.functionService = functionService ?? new FunctionService();
        }

        [HttpGet]
        [HRAuthorize(Policy.Read, FunctionKey.MenuManagement)]
        public ActionResult Index()
        {
            IEnumerable<MenuViewModel> menus = menuService.GetMenus();
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.MenuManagement, Policy.Host);

            return View(menus);
        }

        [HttpGet]
        [HRAuthorize(Policy.Host, FunctionKey.MenuManagement)]
        public ActionResult Create()
        {
            ViewBag.Menus = menuService.GetMenus();
            ViewBag.Functions = functionService.GetFunctions();

            return View();
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.MenuManagement)]
        public ActionResult Create(MenuModel menu)
        {
            if (ModelState.IsValid)
            {
                var criteria = new InsertMenuCriteria
                {
                    DisplayOrder = menu.DisplayOrder,
                    FunctionId = menu.FunctionId == 0 ? null : menu.FunctionId,
                    Link = menu.Link,
                    Name = menu.Name,
                    ParentId = menu.ParentId == 0 ? null : menu.ParentId,
                    Text = menu.Text
                };
                var result = menuService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(menu);
        }

        [HttpGet]
        [HRAuthorize(Policy.Host, FunctionKey.MenuManagement)]
        public ActionResult Edit(int id)
        {
            MenuModel menu = menuService.GetMenusById(id);
            ViewBag.Menus = menuService.GetMenus();
            ViewBag.Functions = functionService.GetFunctions();

            return View(menu);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.MenuManagement)]
        public ActionResult Edit(MenuModel menu)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateMenuCriteria
                {
                    DisplayOrder = menu.DisplayOrder,
                    FunctionId = menu.FunctionId == 0 ? null : menu.FunctionId,
                    Link = menu.Link,
                    Name = menu.Name,
                    ParentId = menu.ParentId == 0 ? null : menu.ParentId,
                    Text = menu.Text,
                    MenuId = menu.MenuId,
                    IsActive = menu.IsActive
                };
                var result = menuService.Update(criteria);
                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(menu);
        }

        [HRAuthorize(Policy.Host, FunctionKey.MenuManagement)]
        public JsonResult Delete(int id)
        {
            var result = menuService.Delete(id);

            return Json(result);
        }
    }
}