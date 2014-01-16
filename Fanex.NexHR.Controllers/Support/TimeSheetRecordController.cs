// -----------------------------------------------------------------------
// <copyright file="TimeSheetRecordController.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Controllers.Support
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TimeSheetRecordController : BaseController
    {
        private readonly ISupportService _supportService;

        public TimeSheetRecordController()
            : this(null)
        {
        }

        public TimeSheetRecordController(
            ISupportService supportService = null)
        {
            _supportService = supportService ?? new SupportService();
        }

        public ActionResult Index()
        {
            var model = new TimeSheetRecordViewModel()
            {
                ForDate = DateTime.Today,
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute
            };
            return View(model);
        }

        /// <summary>
        /// ActionResult Post TimeSheetRecordViewModel.
        /// </summary>
        /// <param name="model">TimeSheetRecord ViewModel.</param>
        /// <returns>Boolean to check.</returns>
        [HttpPost]
        public ActionResult Index(TimeSheetRecordViewModel model)
        {
            if (model.Type == 1)
            {
                model.InOutType = false;
            }

            if (model.Type == 2)
            {
                model.InOutType = true;
            }

            model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);

            model.RecordTime = model.ForDate;
            model.InsertDate = model.ForDate;

            ViewBag.Requested = _supportService.RecordTimeSheet(model);

            return View(model);
        }
    }
}
