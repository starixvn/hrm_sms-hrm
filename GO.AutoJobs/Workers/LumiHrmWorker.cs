namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class LumiHrmWorker : BaseWorker
    {
        private readonly INexHrmService hrmService;

        public LumiHrmWorker(
            INexHrmService hrmService,
            List<JobSettings> jobSettings,
            ILogger<LumiHrmWorker> logger) : base(logger)
        {
            this.hrmService = hrmService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncDataFromLumiHRM);
        }

        public override async Task DoWorkAsync()
        {
            await hrmService.SyncSectionAsync(false);
            await hrmService.SyncTeamAsync(false);
            await hrmService.SyncJobTitleAsync(false);
            await hrmService.SyncEmployeeInfoAsync(false);
        }
    }
}