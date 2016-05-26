namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;

    public interface IDisciplineService
    {
        IList<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria);

        IList<DisciplineDetailModel> GetDisciplineDetail(GetDisciplineDetailCriteria criteria);

        IList<OtherDisciplineModel> GetOtherDiscipline(GetOtherDisciplineOfEmployeeCriteria criteria);

        IList<DisciplineTypeModel> GetDisciplineType(GetDisciplineTypeCriteria criteria);

        IList<DisciplineTypeModel> GetOtherDisciplineType(GetOtherDisciplineTypeCriteria criteria);

        CommandResult InsertOtherDiscipline(InsertOtherDisciplineCriteria criteria);

        CommandResult DeleteOtherDiscipline(DeleteOtherDisciplineCriteria criteria);

        CommandResult UpdateIgnoredDiscipline(UpdateIgnoredDisciplineCriteria criteria);

        void ExportDisciplineSummaryReport(GetDisciplineSummaryCriteria criteria, IList<DisciplineSummaryModel> disciplines, string templatePath);

        void ExportDisciplineDetailReport(GetDisciplineDetailCriteria criteria, IList<DisciplineDetailModel> disciplineDetails, string templatePath);

        IList<Employee> GetMemberToAddOtherDiscipline(GetMemberToAddOtherDiscipineBySupervisorIdCriteria criteria);
    }
}