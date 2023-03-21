namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class NexHrmTeamWorker : BaseWorker
    {
        private readonly INexHrmService hrmService;

        public NexHrmTeamWorker(
            INexHrmService hrmService,
            List<JobSettings> jobSettings,
            ILogger<NexHrmTeamWorker> logger) : base(logger)
        {
            this.hrmService = hrmService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncTeamFromNexHRM);
        }

        public override async Task DoWorkAsync()
        {
            await hrmService.SyncTeamAsync();
        }
    }
}