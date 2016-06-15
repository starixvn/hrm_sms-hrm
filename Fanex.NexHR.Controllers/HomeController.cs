// --------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System.Web.Mvc;

    /// <summary>
    /// Home Controller.
    /// </summary>
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Employee = this.CurrentUser;
            return RedirectToAction("myspace", "employee");
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult AccountSuspended()
        {
            return View();
        }
    }
}
