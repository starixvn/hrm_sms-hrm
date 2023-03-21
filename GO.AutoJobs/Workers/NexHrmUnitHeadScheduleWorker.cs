namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class NexHrmUnitHeadScheduleWorker : BaseWorker
    {
        private readonly INexHrmService unitHeadService;

        public NexHrmUnitHeadScheduleWorker(
            INexHrmService unitHeadService,
            List<JobSettings> jobSettings,
            ILogger<NexHrmUnitHeadScheduleWorker> logger) : base(logger)
        {
            this.unitHeadService = unitHeadService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncUnitHeadScheduleFromNexHRM);
        }

        public override async Task DoWorkAsync()
        {
            await unitHeadService.SyncUniHeadScheduleAsync();
        }
    }
}