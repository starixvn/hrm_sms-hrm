namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Menu;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;

    public class MenuController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly IFunctionService functionService;

        public MenuController(
            ISessionService sessionService,
            IMenuService menuService,
            IFunctionService functionService) : base(sessionService)
        {
            this.menuService = menuService;
            this.functionService = functionService;
        }

        [HttpGet]
        [HRMPermission(Policy.Read, FunctionKey.MenuManagement)]
        public ActionResult Index()
        {
            IEnumerable<MenuViewModel> menus = menuService.GetMenus();
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.MenuManagement, Policy.Host);

            return View(menus);
        }

        [HttpGet]
        [HRMPermission(Policy.Host, FunctionKey.MenuManagement)]
        public ActionResult Create()
        {
            ViewBag.Menus = menuService.GetMenus();
            ViewBag.Functions = functionService.GetFunctions();

            return View();
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.MenuManagement)]
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
        [HRMPermission(Policy.Host, FunctionKey.MenuManagement)]
        public ActionResult Edit(int id)
        {
            MenuModel menu = menuService.GetMenusById(id);
            ViewBag.Menus = menuService.GetMenus();
            ViewBag.Functions = functionService.GetFunctions();

            return View(menu);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.MenuManagement)]
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

        [HRMPermission(Policy.Host, FunctionKey.MenuManagement)]
        public JsonResult Delete(int id)
        {
            var result = menuService.Delete(id);

            return Json(result);
        }
    }
}