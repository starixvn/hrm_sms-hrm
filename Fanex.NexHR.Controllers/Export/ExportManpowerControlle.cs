namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
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