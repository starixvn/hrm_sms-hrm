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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.Caching;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// UI Controller.
    /// </summary>
    [Authorize]
    public class UIController : BaseController
    {
        private readonly ICache _cacheManager;

        public UIController()
            : this(null)
        {
        }

        public UIController(ICache cacheManager)
        {
            _cacheManager = cacheManager ?? CacheService.CreateInstance();
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
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
                var userRole = user.RoleList;
                //// section level
                if (sectionNode.Roles.Count == 0
                    || this.CurrentUser.IsInRoles(sectionNode.Roles.Cast<string>().ToList<string>()))
                {
                    var s = sectionNode.Roles.Cast<string>().ToList<string>();
                    var sectionItem = new NavigationViewModel(sectionNode);
                    navigationItemList.Add(sectionItem);

                    // populate navigation item by section item
                    sectionItem.Childs = new List<NavigationViewModel>();
                    foreach (SiteMapNode navNode in sectionNode.ChildNodes)
                    {
                        var navItem = new NavigationViewModel(navNode);
                        if (navNode.Roles == null || navNode.Roles.Count == 0 ||
                            roles.Any(i => navNode.Roles.Contains(i)))
                        {
                            sectionItem.Childs.Add(navItem);
                        }
                    }

                    CheckRoleForDisplayNavigationItem(sectionNode, user, userRole, sectionItem);
                }
            }

            return navigationItemList;
        }

        /// <summary>
        /// Method check role for display navigation item.
        /// </summary>
        /// <param name="sectionNode">SiteMap Node.</param>
        /// <param name="user">UserLoginModel entity.</param>
        /// <param name="userRole">User's role.</param>
        /// <param name="sectionItem">NavigationViewModel entity.</param>
        private static void CheckRoleForDisplayNavigationItem(
            SiteMapNode sectionNode,
            Models.UserLoginModel user,
            IList<short> userRole,
            NavigationViewModel sectionItem)
        {
            string personal = "Personal";
            string humanResourceSpace = "HR space";
            string companySpace = "Company space";

            string overtime = "My overtime";
            string annualLeaveBalance = "Annual leave balance";
            string createAccount = "Create account";
            string dailyRequest = "Daily requests";

            if (sectionNode.Title.ToUpper() == personal.ToUpper())
            {
                if (user.CanOT == false)
                {
                    sectionItem.Childs.RemoveAll(row => row.Title.ToUpper() == overtime.ToUpper());
                }
            }

            if (sectionNode.Title.ToUpper() == humanResourceSpace.ToUpper())
            {
                var canCreateAccount = userRole.Any(x => x == (int)RolesEnum.SystemAdmin);
                if (!canCreateAccount)
                {
                    sectionItem.Childs.RemoveAll(row => row.Title.ToUpper() == createAccount.ToUpper());
                }
            }

            if (sectionNode.Title.ToUpper() == companySpace.ToUpper())
            {
                var canViewDailyRequest = userRole.Any(
                    x => x == (int)RolesEnum.Verifier
                    || x == (int)RolesEnum.Approver
                    || x == (int)RolesEnum.ShiftManager
                    || x == (int)RolesEnum.ActingSupervisor
                    || x == (int)RolesEnum.HR);
                var canViewAnnualLeaveBalance = userRole.Any(x => x == (int)RolesEnum.Verifier
                    || x == (int)RolesEnum.Approver
                    || x == (int)RolesEnum.HR);
                if (!canViewDailyRequest)
                {
                    sectionItem.Childs.RemoveAll(row => row.Title.ToUpper() == dailyRequest.ToUpper());
                }

                if (!canViewAnnualLeaveBalance)
                {
                    sectionItem.Childs.RemoveAll(row => row.Title.ToUpper() == annualLeaveBalance.ToUpper());
                }
            }
        }
    }
}