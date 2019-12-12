namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models.Message;
    using Fanex.NexHR.Services;

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
            var param = new GetMessageByEmployeeParam
            {
                EmployeeId = sessionService.Employee.Id,
                FromDate = DateTime.Today,
                ToDate = DateTime.Today,
                IsDisabled = 0
            };

            ViewBag.Messages = messageService.GetMessageByEmployee(param);
            return PartialView("_Header", sessionService.Employee);
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