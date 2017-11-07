namespace Fanex.NexHR.Controllers
{
    using Criteria.Supervisor;
    using Models.Supervisor;
    using Services;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using ViewModels;

    public class ExportSupervisorController : BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelEmployeeSupervisor exportExcelEmployeeSupervisor;
        private readonly IExportExcelSectionSupervisorService exportExcelSectionSupervisorService;

        public ExportSupervisorController()
            : this(null, null, null)
        {
        }

        public ExportSupervisorController(
            IReportService reportService,
            IExportExcelEmployeeSupervisor exportExcelEmployeeSupervisor,
            IExportExcelSectionSupervisorService exportExcelSectionSupervisorService)
        {
            this.reportService = reportService ?? new ReportService();
            this.exportExcelEmployeeSupervisor = exportExcelEmployeeSupervisor ?? new ExportExcelEmployeeSupervisor();
            this.exportExcelSectionSupervisorService = exportExcelSectionSupervisorService ?? new ExportExcelSectionSupervisorService();
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
            exportExcelEmployeeSupervisor.Export(supervisors);

            return new EmptyResult();
        }

        public ActionResult SectionSupervisor(EmployeeAndSectionSupervisorViewModel model)
        {
            var criteria = new GetSectionSupervisorCriteria
            {
                GroupIds = model.SectionIds,
            };
            IList<SectionSupervisorReportModel> supervisors = reportService.GetSectionSupervisor(criteria);
            exportExcelSectionSupervisorService.Export(supervisors);

            return new EmptyResult();
        }
    }
}