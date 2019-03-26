namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.ViewModels.Time;
    using Fanex.NexHR.Web.Providers;

    public class TimeController : BaseController
    {
        private readonly ICommandService commandService;

        public TimeController(ICommandService commandService)
        {
            this.commandService = commandService;
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.TimeRecord)]
        public JsonResult ProcessInOut(UpdateTypeTimeRecordCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.Id;
                criteria.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = commandService.Update(criteria);
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

                CommandResult result = commandService.Insert(criteria);

                ViewBag.Requested = result.IsSuccessful;
            }

            return View(model);
        }
    }
}