namespace Fanex.NexHR.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Services.Interfaces;

    public class DisciplineService : BaseService, IDisciplineService
    {
        public IList<OtherViolationModel> GetOtherViolation(GetOtherViolationOfEmployeeCriteria criteria)
        {
            IList<OtherViolationModel> violations = new List<OtherViolationModel>();

            if (criteria != null && criteria.IsValid())
            {
                violations = GetData<OtherViolationModel>(criteria).ToList();
            }

            return violations;
        }

        public IList<DisciplineTypeModel> GetOtherViolationType(GetOtherViolationTypeCriteria criteria)
        {
            IList<DisciplineTypeModel> otherViolationTypes = new List<DisciplineTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                otherViolationTypes = GetData<DisciplineTypeModel>(criteria).ToList();
            }

            return otherViolationTypes;
        }

        public CommandResult InsertOtherViolation(InsertOtherViolationCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }
    }
}