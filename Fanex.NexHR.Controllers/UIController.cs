namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.UI;

    public class UIController : BaseController
    {
        private readonly IMessageService messageService;

        public UIController(
            ISessionService sessionService,
            IMessageService messageService) : base(sessionService)
        {
            this.messageService = messageService;
        }

        [ChildActionOnly]
        public ActionResult GlobalScript()
        {
            ViewBag.Employee = sessionService.Employee;
            ViewBag.IsShowLeftNavigation = sessionService.IsShowLeftNavigation;
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            var viewModel = new HeaderViewModel
            {
                SiteId = ConfigSettings.SiteId,
                Employee = sessionService.Employee,
                IsShowSearchBox = sessionService.HasPermissionOnFunction(FunctionKey.ContactList, Policy.Read)
            };

            return PartialView("_Header", viewModel);
        }

        [ChildActionOnly]
        public PartialViewResult NavBar()
        {
            var menus = GetNavBar();
            return PartialView("_NavBar", menus);
        }

        private IEnumerable<MenuItemSession> GetNavBar()
        {
            var routeData = ControllerContext.ParentActionViewContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            var link = string.Format("/{0}/{1}", controller, action);
            var selectedMenu = sessionService.MenuItems.FirstOrDefault(i => !string.IsNullOrEmpty(i.Link) && i.Link.Equals(link, StringComparison.OrdinalIgnoreCase));

            IEnumerable<MenuItemSession> menus = sessionService.MenuItems;
            var unreadMessageCount = messageService.GetUnreadMessageOfEmployee(sessionService.Employee.Id).Count;

            ViewBag.SelectedMenu = selectedMenu;
            ViewBag.UnreadMessage = unreadMessageCount;

            return menus;
        }
    }
}