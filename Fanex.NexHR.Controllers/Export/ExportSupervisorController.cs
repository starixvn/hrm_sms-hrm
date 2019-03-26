namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Criteria.Supervisor;
    using Models.Supervisor;
    using Services;
    using ViewModels;

    public class ExportSupervisorController : BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelEmployeeSupervisor exportExcelEmployeeSupervisor;
        private readonly IExportExcelSectionSupervisorService exportExcelSectionSupervisorService;

        public ExportSupervisorController(
            IReportService reportService,
            IExportExcelEmployeeSupervisor exportExcelEmployeeSupervisor,
            IExportExcelSectionSupervisorService exportExcelSectionSupervisorService)
        {
            this.reportService = reportService;
            this.exportExcelEmployeeSupervisor = exportExcelEmployeeSupervisor;
            this.exportExcelSectionSupervisorService = exportExcelSectionSupervisorService;
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