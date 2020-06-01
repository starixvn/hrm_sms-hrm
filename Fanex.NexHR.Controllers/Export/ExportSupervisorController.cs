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

        public ActionResult EmployeeSupervisor(EmployeeAndTeamSupervisorViewModel model)
        {
            var criteria = new GetSupervisorReportCriteria
            {
                EmployeeId = model.EmployeeId,
                TeamIds = model.TeamIds,
                IsLocal = model.IsLocal,
                ActionType = model.ActionType
            };
            IList<SupervisorReportModel> supervisors = reportService.GetSupervisorReport(criteria);
            byte[] bytes = exportSupervisorService.ExportEmployeeSupervisor(supervisors);

            return File(bytes, MimeType.XLSX, $"EmployeeSupervisor_{DateTime.Now.ToString("yyyyMMdd")}.xlsx");
        }

        public ActionResult TeamSupervisor(EmployeeAndTeamSupervisorViewModel model)
        {
            var criteria = new GetTeamSupervisorCriteria
            {
                TeamIds = model.TeamIds,
            };
            IList<TeamSupervisorReportModel> supervisors = reportService.GetTeamSupervisor(criteria);
            byte[] bytes = exportSupervisorService.ExportTeamSupervisor(supervisors);

            return File(bytes, MimeType.XLSX, $"TeamSupervisor_{DateTime.Now.ToString("yyyyMMdd")}.xlsx");
        }
    }
}