namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System.Web.Mvc;

    public class MenuController : BaseController
    {
        private IMenuService _menuService;
        private IFunctionService _functionService;

        public MenuController()
            : this(null, null)
        {
        }

        public MenuController(IMenuService menuService, IFunctionService functionService)
        {
            _menuService = menuService ?? new MenuService();
            _functionService = functionService ?? new FunctionService();
        }

        [HttpGet]
        [HRAuthorize(Policy.Read, FunctionKey.MenuManagement)]
        public ActionResult Index()
        {
            var menus = this._menuService.GetMenus();
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.MenuManagement, Policy.Delete)
                || CurrentUser.HasPermission(FunctionKey.MenuManagement, Policy.Edit);

            return View(menus);
        }

        [HttpGet]
        [HRAuthorize(Policy.Create, FunctionKey.MenuManagement)]
        public ActionResult Create()
        {
            ViewBag.Menus = this._menuService.GetMenus();
            ViewBag.Functions = this._functionService.GetFunctions();

            return View();
        }

        [HttpPost]
        [HRAuthorize(Policy.Create, FunctionKey.MenuManagement)]
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
                var result = this._menuService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(menu);
        }

        [HttpGet]
        [HRAuthorize(Policy.Edit, FunctionKey.MenuManagement)]
        public ActionResult Edit(int id)
        {
            var menu = this._menuService.GetMenusById(id);
            ViewBag.Menus = this._menuService.GetMenus();
            ViewBag.Functions = this._functionService.GetFunctions();

            return View(menu);
        }

        [HttpPost]
        [HRAuthorize(Policy.Edit, FunctionKey.MenuManagement)]
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
                var result = this._menuService.Update(criteria);
                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(menu);
        }

        [HRAuthorize(Policy.Delete, FunctionKey.MenuManagement)]
        public JsonResult Delete(int id)
        {
            var result = this._menuService.Delete(id);

            return Json(result);
        }
    }
}