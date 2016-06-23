namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Nexus.RoleManagement.Services;

    [Authorize]
    public class UIController : BaseController
    {
        private readonly IMessageService _messageService;

        public UIController()
            : this(null)
        {
        }

        public UIController(IMessageService messageService)
        {
            _messageService = messageService ?? new MessageService();
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            var criteria = new GetMessageByEmployeeCriteria
            {
                EmployeeId = this.CurrentUser.Id,
                EffectedDate = DateTime.Today,
                IsDisabled = 0
            };

            ViewBag.Messages = _messageService.GetMessageByEmployee(criteria);
            return PartialView(this.CurrentUser);
        }

        [ChildActionOnly]
        public ActionResult GlobalScript()
        {
            ViewBag.Employee = this.CurrentUser;
            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult NavBar()
        {
            var routeData = ControllerContext.ParentActionViewContext.RouteData;
            ViewBag.Controller = routeData.Values["controller"];
            ViewBag.Action = routeData.Values["action"];
            var menus = GetNagivationData();

            return PartialView(menus);
        }

        [NonAction]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed. Suppression is OK here.")]
        public IEnumerable<MenuModel> GetNagivationData()
        {
            return CurrentUser.NE_Menus;
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
    }
}