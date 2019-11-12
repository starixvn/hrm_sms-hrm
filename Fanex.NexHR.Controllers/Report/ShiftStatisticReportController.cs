namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;

    public class ShiftStatisticReportController : BaseController
    {
        private readonly IShiftStatisticReportService statisticReportService;

        public ShiftStatisticReportController(
            ISessionService sessionService,
            IShiftStatisticReportService statisticReportService) : base(sessionService)
        {
            this.statisticReportService = statisticReportService;
        }

        public ActionResult Index()
        {
            var model = new ShiftStatisticViewModel
            {
                FromDate = DateTime.Now.FirstDayOfWeek(),
                ToDate = DateTime.Now.LastDayOfWeek()
            };

            return View("~/Views/Report/ShiftStatistic/Index.cshtml", model);
        }

        public ActionResult ShiftStatistic(ShiftStatisticViewModel model)
        {
            var param = new GetShiftStatisticParam
            {
                SupervisorId = sessionService.Employee.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                SectionIds = model.SectionIds,
                IsShowWholeCompany = model.IsShowWholeCompany,
                IsLocal = model.IsLocal
            };
            model.ShiftStatistics = statisticReportService.GetShiftStatistic(param);
            return PartialView("~/Views/Report/ShiftStatistic/_ShiftStatistic.cshtml", model);
        }
    }
}