namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;

    public interface IDisciplineService
    {
        IList<OtherViolationModel> GetOtherViolation(GetOtherViolationOfEmployeeCriteria criteria);

        IList<DisciplineTypeModel> GetOtherViolationType(GetOtherViolationTypeCriteria criteria);

        CommandResult InsertOtherViolation(InsertOtherViolationCriteria criteria);
    }
}