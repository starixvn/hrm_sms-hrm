namespace Fanex.NexHR.Services.Implement
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Criteria.JobLevel;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class JobLevelService : IJobLevelService
    {
        private readonly IBaseService baseService;

        public JobLevelService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<JobLevel> GetJobLevel(bool? isActive = true)
        {
            var criteria = new GetJobLevelCriteria { IsActive = isActive };

            return baseService.GetData<JobLevel>(criteria).ToList();
        }

        public CommandResult Insert(InsertJobLevelCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Update(UpdateJobLevelCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteJobLevelCriteria
            {
                jobLevelId = id
            };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}
