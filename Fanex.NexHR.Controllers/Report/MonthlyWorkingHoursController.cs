namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report.MonthlyWorkingHours;

    [HRMPermission(Policy.Read, FunctionKey.ReportHome)]
    public class MonthlyWorkingHoursController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IMonthlyWorkingHoursService monthlyWorkingHoursService;

        public MonthlyWorkingHoursController(
            ISessionService sessionService,
            ITeamService teamService,
            IMonthlyWorkingHoursService monthlyWorkingHoursService) : base(sessionService)
        {
            this.teamService = teamService;
            this.monthlyWorkingHoursService = monthlyWorkingHoursService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            IEnumerable<Team> sections = teamService.GetAll();
            var today = DateTime.Today;

            var model = new FilterViewModel
            {
                Month = today.Month,
                Year = today.Year,
                Calendar = (ushort)CalendarType.Fiscal,
                Sections = sections.Select(x => new EntityBase<int>
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
            };

            return View("~/Views/Report/MonthlyWorkingHours/Index.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetMonthlyWorkingHours(FilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                SectionIds = model.SectionIds,
                EmployeeId = model.EmployeeId,
                IsLocal = model.IsLocal
            };
            IEnumerable<MonthlyWorking> monthlyWorkings = monthlyWorkingHoursService.GetMonthlyWorkingHours(param);
            ViewBag.FromDate = param.FromDate;
            ViewBag.ToDate = param.ToDate;

            return PartialView("~/Views/Report/MonthlyWorkingHours/_MonthlyWorkingHoursDetails.cshtml", monthlyWorkings);
        }

        [HttpGet]
        public ActionResult ExportExcel(FilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                SectionIds = model.SectionIds,
                EmployeeId = model.EmployeeId,
                IsLocal = model.IsLocal
            };
            IEnumerable<MonthlyWorking> monthlyWorkings = monthlyWorkingHoursService.GetMonthlyWorkingHours(param);
            byte[] bytes = monthlyWorkingHoursService.ExportExcel(param.FromDate, param.ToDate, monthlyWorkings);

            var fileName = $"Monthly working hours {param.FromDate.ToString("dd MMM yyyy")} - {param.ToDate.ToString("dd MMM yyyy")}.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }
    }
}