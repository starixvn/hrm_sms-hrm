namespace Fanex.NexHR.Controllers
{
    using Criteria;
    using Criteria.Report;
    using Models;
    using Services;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class ExportManpowerController: BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelManpowerCategoryService exportExcelManpowerCategoryService;
        private readonly IExportExcelManpowerService exportExcelManpowerService;

        public ExportManpowerController(): this(null, null, null)
        {
        }

        public ExportManpowerController(
            IReportService reportService,
            IExportExcelManpowerCategoryService exportExcelManpowerCategoryService,
            IExportExcelManpowerService exportExcelManpowerService)
        {
            this.reportService = reportService ?? new ReportService();
            this.exportExcelManpowerCategoryService = exportExcelManpowerCategoryService ?? new ExportExcelManpowerCategoryService();
            this.exportExcelManpowerService = exportExcelManpowerService ?? new ExportExcelManpowerService();
        }

        public ActionResult Manpower(int year)
        {
            var criteria = new GetManpowerCriteria
            {
                Year = year
            };

            IList<ManpowerModel> manpowers = reportService.GetManpower(criteria);
            exportExcelManpowerService.Export(manpowers, year);

            return new EmptyResult();
        }

        public ActionResult ManpowerCategory(int year)
        {
            var criteria = new GetManpowerCategoryCriteria
            {
                Year = year
            };

            IList<ManpowerCategoryModel> manpowers = reportService.GetManpowerCategory(criteria);
            exportExcelManpowerCategoryService.Export(manpowers, year);

            return new EmptyResult();
        }
    }
}