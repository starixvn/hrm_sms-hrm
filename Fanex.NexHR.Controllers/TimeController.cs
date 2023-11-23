namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.ViewModels.Time;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;

    public class TimeController : BaseController
    {
        private readonly IBaseService baseService;

        public TimeController(
            IUserControlSession userControlSession,
            IBaseService baseService) : base(userControlSession)
        {
            this.baseService = baseService;
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.TimeRecord)]
        public JsonResult ProcessInOut(UpdateTypeTimeRecordCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = EmployeeSession.EmpId;
                criteria.ModifiedDate = DateTimeOffset.Now.DateTime;
            }

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddTimeRecord()
        {
            var model = new TimeRecordViewModel
            {
                ForDate = DateTimeOffset.Now.Date,
                Hour = DateTimeOffset.Now.Hour,
                Minute = DateTimeOffset.Now.Minute
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

        [HttpPost]
        public JsonResult AddChekInTime()
        {
            var criteria = new InsertCheckInOutRecordCriteria
            {
                EmpId = EmployeeSession.EmpId,
                RecordTime = DateTimeOffset.Now.DateTime,
                InOutType = 0,
                IPAddress = GetClientIpAddress(),
                Platform = Request.Headers["User-Agent"]
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddChekOutTime()
        {
            var criteria = new InsertCheckInOutRecordCriteria
            {
                EmpId = EmployeeSession.EmpId,
                RecordTime = DateTimeOffset.Now.DateTime,
                InOutType = 1,
                IPAddress = GetClientIpAddress(),
                Platform = Request.Headers["User-Agent"]
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);
            return Json(result);
        }

        public string GetClientIpAddress()
        {
            var clientIp = Request.ServerVariables["X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = Request.UserHostAddress;
            }
            else
            {
                var forwardedIps = clientIp.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                clientIp = forwardedIps[forwardedIps.Length - 1];
            }

            return clientIp;
        }
    }
}