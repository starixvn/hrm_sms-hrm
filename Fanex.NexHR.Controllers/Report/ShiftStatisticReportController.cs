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
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;

    [HRMPermission(Policy.Read, FunctionKey.ScheduleArrange)]
    public class ShiftStatisticReportController : BaseController
    {
        private readonly IShiftStatisticReportService statisticReportService;
        private readonly IShiftService shiftService;

        public ShiftStatisticReportController(
            ISessionService sessionService,
            IShiftStatisticReportService statisticReportService,
            IShiftService shiftService) : base(sessionService)
        {
            this.statisticReportService = statisticReportService;
            this.shiftService = shiftService;
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

        public ActionResult Details(ShiftStatisticDetailViewModel model)
        {
            model.Shifts = shiftService.GetAll();
            return View("~/Views/Report/ShiftStatistic/Details.cshtml", model);
        }

        public ActionResult ShiftStatisticDetails(ShiftStatisticDetailViewModel model)
        {
            var param = new GetShiftStatisticDetailParam
            {
                SupervisorId = sessionService.Employee.Id,
                WorkDate = model.WorkDate,
                ShiftId = model.ShiftId,
                EmployeeId = model.EmployeeId,
                SectionIds = model.SectionIds,
                IsShowWholeCompany = model.IsShowWholeCompany,
                IsLocal = model.IsLocal
            };
            IEnumerable<ShiftStatisticDetail> details = statisticReportService.GetShiftStatisticDetail(param);
            return PartialView("~/Views/Report/ShiftStatistic/_ShiftStatisticDetails.cshtml", details);
        }
    }
}