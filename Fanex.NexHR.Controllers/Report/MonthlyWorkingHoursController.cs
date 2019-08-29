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
    using Fanex.NexHR.ViewModels.Report.WorkingHours;

    [HRMPermission(Policy.Read, FunctionKey.AttendanceMonthlyReport)]
    public class MonthlyWorkingHoursController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IEmployeeWorkingHoursService workingHoursService;

        public MonthlyWorkingHoursController(
            ISessionService sessionService,
            ITeamService teamService,
            IEmployeeWorkingHoursService workingHoursService) : base(sessionService)
        {
            this.teamService = teamService;
            this.workingHoursService = workingHoursService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            IEnumerable<Team> sections = teamService.GetAll();
            var today = DateTime.Today;

            var model = new MonthlyFilterViewModel
            {
                FromDate = new DateTime(today.Year, today.Month, 1),
                ToDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)),
                EmployeeStatusId = (byte)EmployeeStatus.Enabled,
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

            var fileName = $"Attendance Monthly Detail {param.FromDate.ToString("dd MMM yyyy")} - {param.ToDate.ToString("dd MMM yyyy")}.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }
    }
}