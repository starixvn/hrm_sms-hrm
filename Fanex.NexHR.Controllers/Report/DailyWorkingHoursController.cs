namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report.WorkingHours;

    [HRMPermission(Policy.Read, FunctionKey.AttendanceDailyReport)]
    public class DailyWorkingHoursController : BaseController
    {
        private readonly IEmployeeWorkingHoursService workingHoursService;

        public DailyWorkingHoursController(
            ISessionService sessionService,
            IEmployeeWorkingHoursService workingHoursService) : base(sessionService)
        {
            this.workingHoursService = workingHoursService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = new WeeklyFilterViewModel
            {
                FromDate = DateTime.Today,
                EmployeeStatusId = (byte)EmployeeStatus.Enabled
            };

            return View("~/Views/Report/DailyWorkingHours/Index.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetDailyWorkingHours(WeeklyFilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                ActorId = sessionService.Employee.Id,
                FromDate = model.FromDate,
                ToDate = model.FromDate,
                TeamIds = model.TeamIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            ViewBag.FromDate = param.FromDate;
            IEnumerable<MonthlyWorking> monthlyWorkings = workingHoursService.GetMonthlyWorkingHours(param);

            return PartialView("~/Views/Report/DailyWorkingHours/_DailyWorkingHoursDetails.cshtml", monthlyWorkings);
        }

        [HttpGet]
        public ActionResult ExportExcel(WeeklyFilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                ActorId = sessionService.Employee.Id,
                FromDate = model.FromDate,
                ToDate = model.FromDate,
                TeamIds = model.TeamIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            IEnumerable<MonthlyWorking> monthlyWorkings = workingHoursService.GetMonthlyWorkingHours(param);
            byte[] bytes = workingHoursService.ExportDailyWorkingHours(param.FromDate, monthlyWorkings);

            var fileName = $"{Labels.AttendanceDailyDetail} {param.FromDate.ToString("dd MMM yyyy")}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}