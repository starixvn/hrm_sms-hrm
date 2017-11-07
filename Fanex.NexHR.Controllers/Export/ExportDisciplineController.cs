namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Criteria.Discipline;
    using Models.Discipline;
    using Services.ExportExcel;
    using Services.Implement;
    using Services.Interfaces;
    using ViewModels.Discipline;

    public class ExportDisciplineController : BaseController
    {
        private readonly IDisciplineService disciplineService;
        private readonly IExportDisciplineSummaryService exportDisciplineSummaryService;
        private readonly IExportDisciplineDetailService exportDisciplineDetailService;

        public ExportDisciplineController()
            : this(null, null, null)
        {
        }

        public ExportDisciplineController(
            IDisciplineService disciplineService,
            IExportDisciplineSummaryService exportDisciplineSummaryService,
            IExportDisciplineDetailService exportDisciplineDetailService)
        {
            this.disciplineService = disciplineService ?? new DisciplineService();
            this.exportDisciplineSummaryService = exportDisciplineSummaryService ?? new ExportDisciplineSummaryService();
            this.exportDisciplineDetailService = exportDisciplineDetailService ?? new ExportDisciplineDetailService();
        }

        public ActionResult ExportDisciplineSummaryReport(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                ApproverId = CurrentUser.Id,
                EmployeeName = model.Employee,
                GroupName = model.TeamName
            };

            IList<DisciplineSummaryModel> disciplines = disciplineService.GetDisciplineSummary(criteria);
            exportDisciplineSummaryService.Export(criteria, disciplines);

            return new EmptyResult();
        }

        public ActionResult ExportDisciplineDetailReport(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                ApproverId = CurrentUser.Id,
                EmployeeName = model.Employee,
                GroupName = model.TeamName
            };

            IList<DisciplineDetailModel> disciplines = disciplineService.GetDisciplineDetail(criteria);
            exportDisciplineDetailService.Export(criteria, disciplines);

            return new EmptyResult();
        }
    }
}