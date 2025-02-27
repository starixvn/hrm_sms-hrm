namespace Fanex.NexHR.Services.Implement
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.ManagementLevel;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class ManagementLevelService : IManagementLevelService
    {
        private readonly IBaseService baseService;

        public ManagementLevelService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<ManagementLevel> GetManagementLevel()
        {
            var criteria = new GetManagementLevelCriteria {};

            return baseService.GetData<ManagementLevel>(criteria).ToList();
        }

        public CommandResult Insert(InsertManagementLevelCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Update(UpdateManagementLevelCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int id, int actorId)
        {
            var criteria = new DeleteManagementLevelCriteria
            {
                ManagementLevelId = id,
                ModifiedBy = actorId
            };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}
