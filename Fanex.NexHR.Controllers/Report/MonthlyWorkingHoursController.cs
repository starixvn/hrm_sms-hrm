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

    [HRMPermission(Policy.Read, FunctionKey.AttendanceMonthlyReport)]
    public class MonthlyWorkingHoursController : BaseController
    {
        private readonly IEmployeeWorkingHoursService workingHoursService;

        public MonthlyWorkingHoursController(
            ISessionService sessionService,
            IEmployeeWorkingHoursService workingHoursService) : base(sessionService)
        {
            this.workingHoursService = workingHoursService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var today = DateTime.Today;
            var model = new MonthlyFilterViewModel
            {
                FromDate = today.FirstDayOfMonth(),
                ToDate = today.LastDayOfMonth(),
                EmployeeStatusId = (byte)EmployeeStatus.Enabled
            };

            return View("~/Views/Report/MonthlyWorkingHours/Index.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetMonthlyWorkingHours(MonthlyFilterViewModel model)
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

            return PartialView("~/Views/Report/MonthlyWorkingHours/_MonthlyWorkingHoursDetails.cshtml", monthlyWorkings);
        }

        [HttpGet]
        public ActionResult ExportExcel(MonthlyFilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                ActorId = sessionService.Employee.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                SectionIds = model.SectionIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal
            };
            byte[] bytes = workingHoursService.ExportMonthlyWorkingHours(param);

            var fileName = $"{Labels.AttendanceMonthlyDetail} {param.FromDate.ToString("dd MMM yyyy")} - {param.ToDate.ToString("dd MMM yyyy")}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}