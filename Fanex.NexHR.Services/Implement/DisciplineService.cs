namespace Fanex.NexHR.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria.Discipline;
    using Interfaces;
    using Models;
    using Models.Discipline;
    using Repositories;
    using Repositories.Interfaces;

    public class DisciplineService : BaseService, IDisciplineService
    {
        private readonly IDisciplineRepository disciplineRepository;

        public DisciplineService()
            : this(null)
        {
        }

        public DisciplineService(IDisciplineRepository disciplineRepository)
        {
            this.disciplineRepository = disciplineRepository ?? new DisciplineRepository();
        }

        public IList<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria)
        {
            IList<DisciplineSummaryModel> disciplines = new List<DisciplineSummaryModel>();

            if (criteria != null && criteria.IsValid())
            {
                disciplines = disciplineRepository.GetDisciplineSummary(criteria);
            }

            return disciplines;
        }

        public IList<DisciplineDetailModel> GetDisciplineDetail(GetDisciplineDetailCriteria criteria)
        {
            IList<DisciplineDetailModel> disciplines = new List<DisciplineDetailModel>();

            if (criteria != null && criteria.IsValid())
            {
                disciplines = disciplineRepository.GetDisciplineDetail(criteria);
            }

            return disciplines;
        }

        public IList<Employee> GetMemberToAddOtherDiscipline(GetMemberToAddOtherDiscipineBySupervisorIdCriteria criteria)
        {
            IList<Employee> employees = GetData<Employee>(criteria).ToList();
            return employees;
        }

        public IList<OtherDisciplineModel> GetOtherDiscipline(GetOtherDisciplineOfEmployeeCriteria criteria)
        {
            IList<OtherDisciplineModel> otherDisciplines = GetData<OtherDisciplineModel>(criteria).ToList();
            return otherDisciplines;
        }

        public IList<OtherDisciplineModel> GetWeekendLeave(GetWeekendLeaveOfEmployeeCriteria criteria)
        {
            IList<OtherDisciplineModel> weekendLeaves = GetData<OtherDisciplineModel>(criteria).ToList();
            return weekendLeaves;
        }

        public IList<DisciplineTypeModel> GetDisciplineType(GetDisciplineTypeCriteria criteria)
        {
            IList<DisciplineTypeModel> disciplineTypes = GetData<DisciplineTypeModel>(criteria).ToList();
            return disciplineTypes;
        }

        public IList<DisciplineTypeModel> GetOtherDisciplineType(GetOtherDisciplineTypeCriteria criteria)
        {
            IList<DisciplineTypeModel> otherDisciplineTypes = GetData<DisciplineTypeModel>(criteria).ToList();
            return otherDisciplineTypes;
        }
    }
}