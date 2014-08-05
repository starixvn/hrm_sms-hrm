// --------------------------------------------------------------------------
// <copyright file="UIController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

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
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// UI Controller.
    /// </summary>
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

        /// <summary>
        /// The Header.
        /// </summary>
        /// <returns>The partial view of current user.</returns>
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
            ViewBag.NavigationItems = GetNagivationData();

            return PartialView();
        }

        /// <summary>
        /// Get navigation site map.
        /// </summary>
        /// <returns>Return list.</returns>
        [NonAction]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed. Suppression is OK here.")]
        public IList<NavigationViewModel> GetNagivationData()
        {
            var navigationItemList = new List<NavigationViewModel>();
            var roles = Roles.GetRolesForUser();
            navigationItemList = new List<NavigationViewModel>();

            // populate sitemap data
            SiteMapNode root = SiteMap.RootNode;

            foreach (SiteMapNode sectionNode in root.ChildNodes)
            {
                var user = this.CurrentUser;
                //// section level
                if (sectionNode.Roles.Count == 0 || this.CurrentUser.IsInRoles(sectionNode.Roles.Cast<string>().ToList<string>()))
                {
                    var sectionItem = new NavigationViewModel(sectionNode);
                    navigationItemList.Add(sectionItem);

                    // populate navigation item by section item
                    sectionItem.Childs = new List<NavigationViewModel>();
                    foreach (SiteMapNode navNode in sectionNode.ChildNodes)
                    {
                        var navItem = new NavigationViewModel(navNode);
                        if (navNode.Roles == null || navNode.Roles.Count == 0 || roles.Any(i => navNode.Roles.Contains(i)))
                        {
                            if (navItem.Title.Equals("My messages"))
                            {
                                var criteria = new GetUnreadMessageOfEmployeeCriteria
                                {
                                    EmployeeId = this.CurrentUser.Id
                                };
                                IEnumerable<Message> unreadMessages = _messageService.GetUnreadMessageOfEmployee(criteria);

                                if (unreadMessages != null && unreadMessages.Count() > 0)
                                {
                                    navItem.Title += " (" + unreadMessages.Count() + ")";
                                }
                            }

                            sectionItem.Childs.Add(navItem);
                        }
                    }

                    CheckRoleForDisplayNavigationItem(sectionNode, user, sectionItem);
                }
            }

            return navigationItemList;
        }

        /// <summary>
        /// Method check role for display navigation item.
        /// </summary>
        /// <param name="sectionNode">SiteMap Node.</param>
        /// <param name="user">UserLoginModel entity.</param>
        /// <param name="sectionItem">NavigationViewModel entity.</param>
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
                    sectionItem.Childs.RemoveAll(row => row.Title.ToUpper() == overtime.ToUpper());
                }
            }
        }
    }
}