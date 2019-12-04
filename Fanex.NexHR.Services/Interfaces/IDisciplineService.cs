namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.Discipline;
    using Models;
    using Models.Discipline;

    public interface IDisciplineService
    {
        IList<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria);

        IList<DisciplineDetailModel> GetDisciplineDetail(GetDisciplineDetailCriteria criteria);

        IList<OtherDisciplineModel> GetOtherDiscipline(GetOtherDisciplineOfEmployeeCriteria criteria);

        IList<DisciplineTypeModel> GetDisciplineType(GetDisciplineTypeCriteria criteria);

        IList<DisciplineTypeModel> GetOtherDisciplineType(GetOtherDisciplineTypeCriteria criteria);

        IList<Employee> GetMemberToAddOtherDiscipline(GetMemberToAddOtherDiscipineBySupervisorIdCriteria criteria);
    }
}