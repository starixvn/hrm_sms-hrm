namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class LumiHrmTeamWorker : BaseWorker
    {
        private readonly INexHrmService hrmService;

        public LumiHrmTeamWorker(
            INexHrmService hrmService,
            List<JobSettings> jobSettings,
            ILogger<LumiHrmTeamWorker> logger) : base(logger)
        {
            this.hrmService = hrmService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncTeamFromLumiHRM);
        }

        public override async Task DoWorkAsync()
        {
            await hrmService.SyncTeamAsync(false);
        }
    }
}