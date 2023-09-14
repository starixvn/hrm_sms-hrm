using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.Website.Constants;
using GO.Website.Criteria.Report.Manpower;
using GO.Website.Filters;
using GO.Website.Services;
using HRM.UserControl.Client.Core.Enumerables;
using HRM.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;

namespace GO.Website.Controllers.Report
{
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
        public async Task<IActionResult> GetRequests([FromBody] GetManpowerForecastReportCriteria criteria)
        {
            criteria.UserId = userControlSession.UserId;
            criteria.SiteId = userControlSession.SiteId;
            var requests = await manpowerForecastService.GetManpowerForecastReportAsync(criteria);
            return Json(requests);
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel([FromBody] GetManpowerForecastReportCriteria criteria)
        {
            criteria.UserId = userControlSession.UserId;
            criteria.SiteId = userControlSession.SiteId;
            var requests = await manpowerForecastService.GetManpowerForecastReportAsync(criteria);

            var bytes = excelService.ExportManpowerForecastReport(requests);
            var reportName = $"ManpowerForecast_{DateTime.Today:yyyyMMdd}";
            return File(bytes, "application/xlsx", $"{reportName}.xlsx");
        }
    }
}
