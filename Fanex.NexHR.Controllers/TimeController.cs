namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.ViewModels.Time;

    public class TimeController : BaseController
    {
        private readonly IBaseService baseService;

        public TimeController(
            ISessionService sessionService,
            IBaseService baseService) : base(sessionService)
        {
            this.baseService = baseService;
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.TimeRecord)]
        public JsonResult ProcessInOut(UpdateTypeTimeRecordCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = sessionService.Employee.Id;
                criteria.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddTimeRecord()
        {
            var model = new TimeRecordViewModel
            {
                ForDate = DateTime.Today,
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult AddTimeRecord(TimeRecordViewModel model)
        {
            if (model != null)
            {
                var criteria = new InsertTimeRecordCriteria
                {
                    EmpId = model.EmpId,
                    RecordTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                    InOutType = model.InOutType != 1
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);

                ViewBag.Requested = result.IsSuccessful;
            }

            return View(model);
        }
    }
}