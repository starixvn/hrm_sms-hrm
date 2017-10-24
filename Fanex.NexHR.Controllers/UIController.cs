namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    [Authorize]
    public class UIController : BaseController
    {
        private readonly IMessageService messageService;

        public UIController()
            : this(null)
        {
        }

        public UIController(IMessageService messageService)
        {
            this.messageService = messageService ?? new MessageService();
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            var criteria = new GetMessageByEmployeeCriteria
            {
                EmployeeId = CurrentUser.Id,
                FromDate = DateTime.Today,
                ToDate = DateTime.Today,
                IsDisabled = 0
            };

            ViewBag.Messages = messageService.GetMessageByEmployee(criteria);
            return PartialView(CurrentUser);
        }

        [ChildActionOnly]
        public ActionResult GlobalScript()
        {
            ViewBag.Employee = CurrentUser;
            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult NavBar()
        {
            var routeData = ControllerContext.ParentActionViewContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            var link = string.Format("/{0}/{1}", controller, action);
            var selectedMenu = CurrentUser.Menus.Where(i => !string.IsNullOrEmpty(i.Link) && i.Link.Equals(link, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var criteria = new GetUnreadMessageOfEmployeeCriteria { EmployeeId = CurrentUser.Id };
            var menus = GetNagivationData();
            var unreadMessageCount = messageService.GetUnreadMessageOfEmployee(criteria).Count;

            ViewBag.SelectedMenu = selectedMenu;
            ViewBag.UnreadMessage = unreadMessageCount;

            return PartialView(menus);
        }

        [NonAction]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed. Suppression is OK here.")]
        public IEnumerable<MenuModel> GetNagivationData()
        {
            return CurrentUser.Menus;
        }

        private static void CheckRoleForDisplayNavigationItem(
            SiteMapNode sectionNode,
            UserLoginModel user,
            NavigationViewModel sectionItem)
        {
            string personal = "Personal";
            string overtime = "My overtime";

            if (sectionNode.Title.ToUpper() == personal.ToUpper())
            {
                if (!user.CanOT)
                {
                    sectionItem.Childs.RemoveAll(row => string.Compare(row.Title, overtime, StringComparison.OrdinalIgnoreCase) == 0);
                }
            }
        }

        [ChildActionOnly]
        public ActionResult NewHeader()
        {
            var criteria = new GetMessageByEmployeeCriteria
            {
                EmployeeId = CurrentUser.Id,
                FromDate = DateTime.Today,
                ToDate = DateTime.Today,
                IsDisabled = 0
            };

            ViewBag.Messages = messageService.GetMessageByEmployee(criteria);
            return PartialView("_NewHeader", CurrentUser);
        }

        [ChildActionOnly]
        public PartialViewResult NewNavBar()
        {
            var routeData = ControllerContext.ParentActionViewContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            var link = string.Format("/{0}/{1}", controller, action);
            var selectedMenu = CurrentUser.Menus.Where(i => !string.IsNullOrEmpty(i.Link) && i.Link.Equals(link, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var criteria = new GetUnreadMessageOfEmployeeCriteria { EmployeeId = CurrentUser.Id };
            var menus = GetNagivationData();
            var unreadMessageCount = messageService.GetUnreadMessageOfEmployee(criteria).Count;

            ViewBag.SelectedMenu = selectedMenu;
            ViewBag.UnreadMessage = unreadMessageCount;

            return PartialView("_NewNavBar", menus);
        }
    }
}