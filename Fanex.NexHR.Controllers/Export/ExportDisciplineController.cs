namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Discipline;

    public class ExportDisciplineController : BaseController
    {
        private readonly IDisciplineService disciplineService;
        private readonly IExportDisciplineSummaryService exportDisciplineSummaryService;
        private readonly IExportDisciplineDetailService exportDisciplineDetailService;

        public ExportDisciplineController(
            ISessionService sessionService,
            IDisciplineService disciplineService,
            IExportDisciplineSummaryService exportDisciplineSummaryService,
            IExportDisciplineDetailService exportDisciplineDetailService) : base(sessionService)
        {
            this.disciplineService = disciplineService;
            this.exportDisciplineSummaryService = exportDisciplineSummaryService;
            this.exportDisciplineDetailService = exportDisciplineDetailService;
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
                ApproverId = sessionService.Employee.Id,
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
                ApproverId = sessionService.Employee.Id,
                EmployeeName = model.Employee,
                GroupName = model.TeamName
            };

            IList<DisciplineDetailModel> disciplines = disciplineService.GetDisciplineDetail(criteria);
            exportDisciplineDetailService.Export(criteria, disciplines);

            return new EmptyResult();
        }
    }
}