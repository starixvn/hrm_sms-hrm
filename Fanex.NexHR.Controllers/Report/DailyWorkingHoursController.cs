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

    [HRMPermission(Policy.Read, FunctionKey.ReportHome)]
    public class DailyWorkingHoursController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IEmployeeWorkingHoursService workingHoursService;

        public DailyWorkingHoursController(
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
            var model = new WeeklyFilterViewModel
            {
                FromDate = DateTime.Today,
                EmployeeStatusId = (byte)EmployeeStatus.Enabled,
                Sections = sections.Select(x => new EntityBase<int>
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
            };

            return View("~/Views/Report/DailyWorkingHours.cshtml", model);
        }

        [HttpGet]
        public ActionResult ExportExcel(WeeklyFilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                FromDate = model.FromDate,
                ToDate = model.FromDate,
                SectionIds = model.SectionIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            IEnumerable<MonthlyWorking> monthlyWorkings = workingHoursService.GetMonthlyWorkingHours(param);
            byte[] bytes = workingHoursService.ExportDailyWorkingHours(param.FromDate, monthlyWorkings);

            var fileName = $"Daily working hours {param.FromDate.ToString("dd MMM yyyy")}.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }
    }
}