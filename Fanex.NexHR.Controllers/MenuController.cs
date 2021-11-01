namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;

    public class MenuController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IMenuService menuService;
        private readonly IFunctionService functionService;

        public MenuController(
            ISessionService sessionService,
            IUserControlSession userControlSession,
            IMenuService menuService,
            IFunctionService functionService) : base(sessionService)
        {
            this.userControlSession = userControlSession;
            this.menuService = menuService;
            this.functionService = functionService;
        }

        [HttpGet]
        [HRMPermission(Policy.Read, FunctionKey.MenuManagement)]
        public ActionResult Index()
        {
            IEnumerable<MenuViewModel> menus = menuService.GetMenus();
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.MenuManagement, Policy.Host);

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
                CommandResult commandResult = menuService.Create(menu);
                if (commandResult.IsSuccessful)
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
                CommandResult commandResult = menuService.Edit(menu);
                if (commandResult.IsSuccessful)
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