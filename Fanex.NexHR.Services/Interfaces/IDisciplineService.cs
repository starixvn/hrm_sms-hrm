namespace Fanex.NexHR.Services.Interfaces
{
    using Criteria.Discipline;
    using Models;
    using Models.Discipline;
    using System.Collections.Generic;

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