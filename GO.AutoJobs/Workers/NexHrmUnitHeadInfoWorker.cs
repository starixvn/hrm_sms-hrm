namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class NexHrmUnitHeadInfoWorker : BaseWorker
    {
        private readonly INexHrmService unitHeadService;

        public NexHrmUnitHeadInfoWorker(
            INexHrmService unitHeadService,
            List<JobSettings> jobSettings,
            ILogger<NexHrmUnitHeadInfoWorker> logger) : base(logger)
        {
            this.unitHeadService = unitHeadService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncUnitHeadInfoFromNexHRM);
        }

        public override async Task DoWorkAsync()
        {
            await unitHeadService.SyncUniHeadInfoAsync();
        }
    }
}