namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Criteria;
    using Criteria.Report;
    using Models;
    using Services;

    public class ExportManpowerController : BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelManpowerCategoryService exportExcelManpowerCategoryService;
        private readonly IExportExcelManpowerService exportExcelManpowerService;

        public ExportManpowerController(
            IReportService reportService,
            IExportExcelManpowerCategoryService exportExcelManpowerCategoryService,
            IExportExcelManpowerService exportExcelManpowerService)
        {
            this.reportService = reportService;
            this.exportExcelManpowerCategoryService = exportExcelManpowerCategoryService;
            this.exportExcelManpowerService = exportExcelManpowerService;
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