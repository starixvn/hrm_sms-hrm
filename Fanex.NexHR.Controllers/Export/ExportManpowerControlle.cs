namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;

    public class ExportManpowerController : BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelManpowerCategoryService exportExcelManpowerCategoryService;
        private readonly IExportExcelManpowerService exportExcelManpowerService;

        public ExportManpowerController(
            ISessionService sessionService,
            IReportService reportService,
            IExportExcelManpowerCategoryService exportExcelManpowerCategoryService,
            IExportExcelManpowerService exportExcelManpowerService) : base(sessionService)
        {
            this.reportService = reportService;
            this.exportExcelManpowerCategoryService = exportExcelManpowerCategoryService;
            this.exportExcelManpowerService = exportExcelManpowerService;
        }

        public ActionResult Manpower(int year)
        {
            IEnumerable<ManpowerModel> manpowers = reportService.GetManpower(year);
            exportExcelManpowerService.Export(manpowers, year);

            return new EmptyResult();
        }

        public ActionResult ManpowerCategory(int year)
        {
            IEnumerable<ManpowerCategoryModel> manpowers = reportService.GetManpowerCategory(year);
            exportExcelManpowerCategoryService.Export(manpowers, year);

            return new EmptyResult();
        }
    }
}