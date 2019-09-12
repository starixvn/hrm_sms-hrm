namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report.WorkingHours;

    [HRMPermission(Policy.Read, FunctionKey.AttendanceWeeklyReport)]
    public class WeeklyWorkingHoursController : BaseController
    {
        private readonly IEmployeeWorkingHoursService workingHoursService;

        public WeeklyWorkingHoursController(
            ISessionService sessionService,
            IEmployeeWorkingHoursService workingHoursService) : base(sessionService)
        {
            this.workingHoursService = workingHoursService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var today = DateTime.Today;
            var model = new WeeklyFilterViewModel
            {
                FromDate = today.FirstDayOfWeek(),
                ToDate = today.LastDayOfWeek(),
                EmployeeStatusId = (byte)EmployeeStatus.Enabled
            };

            return View("~/Views/Report/WeeklyWorkingHours/Index.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetWeeklyWorkingHours(WeeklyFilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                ActorId = sessionService.Employee.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                SectionIds = model.SectionIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            IEnumerable<MonthlyWorking> monthlyWorkings = workingHoursService.GetMonthlyWorkingHours(param);
            ViewBag.FromDate = param.FromDate;
            ViewBag.ToDate = param.ToDate;

            return PartialView("~/Views/Report/WeeklyWorkingHours/_WeeklyWorkingHoursDetails.cshtml", monthlyWorkings);
        }

        [HttpGet]
        public ActionResult ExportExcel(WeeklyFilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                ActorId = sessionService.Employee.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                SectionIds = model.SectionIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            IEnumerable<MonthlyWorking> monthlyWorkings = workingHoursService.GetMonthlyWorkingHours(param);
            byte[] bytes = workingHoursService.ExportWeeklyWorkingHours(param.FromDate, param.ToDate, monthlyWorkings);

            var fileName = $"{Labels.AttendanceWeeklyDetail} {param.FromDate.ToString("dd MMM yyyy")} - {param.ToDate.ToString("dd MMM yyyy")}.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }
    }
}