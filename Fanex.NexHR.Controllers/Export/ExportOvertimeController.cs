namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;

    public class ExportOvertimeController : BaseExportController
    {
        private readonly IOvertimeRequestService overtimeService;
        private readonly IExportExcelOvertimeReportService exportExcelOvertimeReportService;

        public ExportOvertimeController(
            ISessionService sessionService,
            IOvertimeRequestService overtimeService,
            IExportExcelOvertimeReportService exportExcelOvertimeReportService) : base(sessionService)
        {
            this.overtimeService = overtimeService;
            this.exportExcelOvertimeReportService = exportExcelOvertimeReportService;
        }

        public ActionResult OvertimeReport(OvertimeDetailReportViewModel model)
        {
            var criteria = new GetOvertimeExcelReportDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeId = model.EmployeeId,
                JobTitleId = model.JobTitleIds,
                GroupIds = model.SectionIds
            };
            IList<OvertimeDaily> overtimeDetails = overtimeService.OTExcelReportDetails(criteria).OrderBy(x => x.OvertimeDate).ToList();
            exportExcelOvertimeReportService.Export(overtimeDetails, model.FromDate, model.ToDate);

            return new EmptyResult();
        }
    }
}