// --------------------------------------------------------------------------
// <copyright file="ErrorController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.12.19">Created.</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.Caching;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Error Controller.
    /// </summary>
    [Authorize]
    public class ErrorController : Controller
    {
        public ActionResult Index(int errorCode)
        {
            return View();
        }
    }
}