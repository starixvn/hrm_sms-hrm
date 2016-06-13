namespace Fanex.NexHR.Controllers.Support
{
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.TimesheetRecord;
    using System;
    using System.Web.Mvc;

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

        [HttpPost]
        public ActionResult Index(TimeSheetRecordViewModel model)
        {
            if (model != null)
            {
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
                model.RecordTime = model.ForDate;
                model.InsertDate = model.ForDate;

                ViewBag.Requested = _supportService.RecordTimeSheet(model);
            }

            return View(model);
        }
    }
}
