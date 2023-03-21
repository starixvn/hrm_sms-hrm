namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class NexHrmEmployeeInfoWorker : BaseWorker
    {
        private readonly INexHrmService hrmService;

        public NexHrmEmployeeInfoWorker(
            INexHrmService hrmService,
            List<JobSettings> jobSettings,
            ILogger<NexHrmEmployeeInfoWorker> logger) : base(logger)
        {
            this.hrmService = hrmService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncEmployeeInfoFromNexHRM);
        }

        public override async Task DoWorkAsync()
        {
            await hrmService.SyncEmployeeInfoAsync();
        }
    }
}