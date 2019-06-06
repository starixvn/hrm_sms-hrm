namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Message;
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
        public ActionResult Header()
        {
            var criteria = new GetMessageByEmployeeCriteria
            {
                EmployeeId = sessionService.Employee.Id,
                FromDate = DateTime.Today,
                ToDate = DateTime.Today,
                IsDisabled = 0
            };

            ViewBag.Messages = messageService.GetMessageByEmployee(criteria);
            return PartialView(sessionService.Employee);
        }

        [ChildActionOnly]
        public ActionResult GlobalScript()
        {
            ViewBag.Employee = sessionService.Employee;
            ViewBag.IsShowLeftNavigation = sessionService.IsShowLeftNavigation;
            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult NavBar()
        {
            var menus = GetNavBar();
            return PartialView(menus);
        }

        [NonAction]
        public IEnumerable<MenuItemSession> GetNagivationData()
        {
            return sessionService.MenuItems;
        }

        [ChildActionOnly]
        public ActionResult NewHeader()
        {
            var criteria = new GetMessageByEmployeeCriteria
            {
                EmployeeId = sessionService.Employee.Id,
                FromDate = DateTime.Today,
                ToDate = DateTime.Today,
                IsDisabled = 0
            };

            ViewBag.Messages = messageService.GetMessageByEmployee(criteria);
            return PartialView("_NewHeader", sessionService.Employee);
        }

        [ChildActionOnly]
        public PartialViewResult NewNavBar()
        {
            var menus = GetNavBar();
            return PartialView("_NewNavBar", menus);
        }

        private IEnumerable<MenuItemSession> GetNavBar()
        {
            var routeData = ControllerContext.ParentActionViewContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            var link = string.Format("/{0}/{1}", controller, action);
            var selectedMenu = sessionService.MenuItems.FirstOrDefault(i => !string.IsNullOrEmpty(i.Link) && i.Link.Equals(link, StringComparison.OrdinalIgnoreCase));
            var criteria = new GetUnreadMessageOfEmployeeCriteria { EmployeeId = sessionService.Employee.Id };
            var menus = GetNagivationData();
            var unreadMessageCount = messageService.GetUnreadMessageOfEmployee(criteria).Count;

            ViewBag.SelectedMenu = selectedMenu;
            ViewBag.UnreadMessage = unreadMessageCount;

            return menus;
        }
    }
}