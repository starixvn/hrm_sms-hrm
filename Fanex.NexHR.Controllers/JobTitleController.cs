// --------------------------------------------------------------------------
// <copyright file="JobTitleController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// JobTitle Controller.
    /// </summary>
    [Authorize]
    public class JobTitleController : BaseController
    {
        private readonly ILookupDataService _lookupDataService;

        public JobTitleController()
            : this(null)
        {
        }

        public JobTitleController(ILookupDataService lookupDataService)
        {
            _lookupDataService = lookupDataService ?? new LookupDataService();
        }

        [Authorize(Roles = "HR")]
        public ActionResult JobTitleManagement()
        {
            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.JobTitle
            };
            ViewBag.JobTitles = _lookupDataService.GetAll(getAllJobTitleCriteria);
            return View();
        }
    }
}
