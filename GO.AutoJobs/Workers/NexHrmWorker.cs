namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class NexHrmWorker : BaseWorker
    {
        private readonly INexHrmService hrmService;

        public NexHrmWorker(
            INexHrmService hrmService,
            List<JobSettings> jobSettings,
            ILogger<NexHrmWorker> serilog,
            Fanex.Logging.ILogger logger) : base(serilog, logger)
        {
            this.hrmService = hrmService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncDataFromNexHRM);
        }

        public override async Task DoWorkAsync()
        {
            await hrmService.SyncSectionAsync();
            await hrmService.SyncTeamAsync();
            await hrmService.SyncJobTitleAsync();
            await hrmService.SyncEmployeeInfoAsync();
        }
    }
}