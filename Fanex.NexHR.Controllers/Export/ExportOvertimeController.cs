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
        private readonly IExportExcelYearlyOvertimeReportService exportExcelYearlyOvertimeReportService;

        public ExportOvertimeController(
            ISessionService sessionService,
            IOvertimeRequestService overtimeService,
            IExportExcelOvertimeReportService exportExcelOvertimeReportService,
            IExportExcelYearlyOvertimeReportService exportExcelYearlyOvertimeReportService) : base(sessionService)
        {
            this.overtimeService = overtimeService;
            this.exportExcelOvertimeReportService = exportExcelOvertimeReportService;
            this.exportExcelYearlyOvertimeReportService = exportExcelYearlyOvertimeReportService;
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

        public ActionResult YearlyOvertimeReport(YearlyOvertimeReportViewModel model)
        {
            IList<OvertimeAnnualReportViewModel> entity = null;
            if (model.Calendar == 1)
            {
                var getYearlyOvertimeReportCriteria = new GetYearlyOvertimeReportCriteria
                {
                    SupervisorId = this.sessionService.Employee.Id,
                    Year = model.Year,
                    GroupIds = model.SectionIds,
                    EmployeeId = model.EmployeeId,
                    Status = null
                };
                entity = overtimeService.GetYearlyOvertimeReport(getYearlyOvertimeReportCriteria);
            }
            else
            {
                var criteria = new GetYearlyOvertimeReportByNormalCalendarCriteria
                {
                    Year = model.Year,
                    GroupIds = model.SectionIds,
                    EmployeeId = model.EmployeeId,
                    Status = null
                };
                entity = overtimeService.GetYearlyOvertimeReportByNormalCalendar(criteria);
            }

            exportExcelYearlyOvertimeReportService.Export(entity, model.Year, model.Calendar);

            return new EmptyResult();
        }
    }
}