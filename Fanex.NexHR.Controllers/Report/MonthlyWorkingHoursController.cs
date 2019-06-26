namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report.MonthlyWorkingHours;

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

        public ActionResult GetMonthlyWorkingHours(FilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                SectionIds = model.SectionIds,
                EmployeeId = model.EmployeeId,
                IsLocal = model.IsLocal
            };
            if (model.Calendar == (ushort)CalendarType.Normal)
            {
                var dayInMonth = DateTime.DaysInMonth(model.Year, model.Month);
                param.FromDate = new DateTime(model.Year, model.Month, 1);
                param.ToDate = new DateTime(model.Year, model.Month, dayInMonth);
            }
            else
            {
                DateTime previousMonth = new DateTime(model.Year, model.Month, 1).AddMonths(-1);
                param.FromDate = new DateTime(previousMonth.Year, previousMonth.Month, 26);
                param.ToDate = new DateTime(model.Year, model.Month, 25);
            }
            IEnumerable<MonthlyWorking> monthlyWorkings = monthlyWorkingHoursService.GetMonthlyWorkingHours(param);
            ViewBag.FromDate = param.FromDate;
            ViewBag.ToDate = param.ToDate;

            return PartialView("~/Views/Report/MonthlyWorkingHours/_MonthlyWorkingHoursDetails.cshtml", monthlyWorkings);
        }
    }
}