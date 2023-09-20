namespace GO.Website.Controllers.Report
{
    using System;
    using System.Threading.Tasks;
    using Constants;
    using GO.Website.Criteria.Report.Manpower;
    using Filters;
    using GO.Website.Models.Report.Manpower;
    using Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [GOPermission(FunctionKey = FunctionKey.ManpowerForecastReport, Policy = Policy.Read)]
    public class ManpowerForecastReportController : Controller
    {
        private readonly IUserControlSession userControlSession;
        private readonly IManpowerForecastService manpowerForecastService;
        private readonly IExportExcelService excelService;

        public ManpowerForecastReportController(
            IUserControlSession userControlSession,
            IManpowerForecastService manpowerForecastService,
            IExportExcelService excelService)
        {
            this.userControlSession = userControlSession;
            this.manpowerForecastService = manpowerForecastService;
            this.excelService = excelService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Report/ManpowerForecastReport.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GetRequests([FromBody] GetManpowerForecastReportParam param)
        {
            var criteria = new GetManpowerForecastReportCriteria
            {
                UserId = userControlSession.UserId,
                SiteId = userControlSession.SiteId,
                PeriodId = param.PeriodId,
                UnitType = param.UnitType
            };
            var requests = await manpowerForecastService.GetManpowerForecastReportAsync(criteria);

            return Json(requests);
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel([FromBody] GetManpowerForecastReportParam param)
        {
            var criteria = new GetManpowerForecastReportCriteria
            {
                UserId = userControlSession.UserId,
                SiteId = userControlSession.SiteId,
                PeriodId = param.PeriodId,
                UnitType = param.UnitType
            };
            var requests = await manpowerForecastService.GetManpowerForecastReportAsync(criteria);

            var bytes = excelService.ExportManpowerForecastReport(requests, param.Year, param.PeriodName);
            var reportName = $"ManpowerForecast_{DateTime.Today:yyyyMMdd}";
            return File(bytes, "application/xlsx", $"{reportName}.xlsx");
        }
    }
}