namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;

    public class ExportSupervisorController : BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportSupervisorService exportSupervisorService;

        public ExportSupervisorController(
            ISessionService sessionService,
            IReportService reportService,
            IExportSupervisorService exportSupervisorService) : base(sessionService)
        {
            this.reportService = reportService;
            this.exportSupervisorService = exportSupervisorService;
        }

        public ActionResult EmployeeSupervisor(EmployeeAndSectionSupervisorViewModel model)
        {
            var criteria = new GetSupervisorReportCriteria
            {
                EmployeeId = model.EmployeeId,
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal,
                ActionType = model.ActionType
            };
            IList<SupervisorReportModel> supervisors = reportService.GetSupervisorReport(criteria);
            byte[] bytes = exportSupervisorService.ExportEmployeeSupervisor(supervisors);

            return File(bytes, MimeType.XLSX, $"EmployeeSupervisor_{DateTime.Now.ToString("yyyyMMdd")}.xlsx");
        }

        public ActionResult SectionSupervisor(EmployeeAndSectionSupervisorViewModel model)
        {
            var criteria = new GetSectionSupervisorCriteria
            {
                GroupIds = model.SectionIds,
            };
            IList<SectionSupervisorReportModel> supervisors = reportService.GetSectionSupervisor(criteria);
            byte[] bytes = exportSupervisorService.ExportSectionSupervisor(supervisors);

            return File(bytes, MimeType.XLSX, $"SectionSupervisor_{DateTime.Now.ToString("yyyyMMdd")}.xlsx");
        }
    }
}