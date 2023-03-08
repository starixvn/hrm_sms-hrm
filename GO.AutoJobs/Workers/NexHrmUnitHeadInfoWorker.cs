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
        private readonly INexHrmUnitHeadService unitHeadService;

        public NexHrmUnitHeadInfoWorker(
            INexHrmUnitHeadService unitHeadService,
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