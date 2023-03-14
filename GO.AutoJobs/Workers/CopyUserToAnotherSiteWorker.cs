namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class CopyUserToAnotherSiteWorker : BaseWorker
    {
        private readonly IUserSiteService userSiteService;

        public CopyUserToAnotherSiteWorker(
            IUserSiteService userSiteService,
            List<JobSettings> jobSettings,
            ILogger<CopyUserToAnotherSiteWorker> logger) : base(logger)
        {
            this.userSiteService = userSiteService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.CopyUserToAnotherSite);
        }

        public override async Task DoWorkAsync()
        {
            await userSiteService.CopyUserToAnotherSiteAsync();
        }
    }
}