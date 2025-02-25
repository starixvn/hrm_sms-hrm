namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.ManagementLevel;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IManagementLevelService
    {
        IList<ManagementLevel> GetManagementLevel();

        CommandResult Insert(InsertManagementLevelCriteria criteria);

        CommandResult Update(UpdateManagementLevelCriteria criteria);

        CommandResult Delete(int id, int actorId);
    }
}
