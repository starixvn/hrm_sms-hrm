namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class HongMaHrmWorker : BaseWorker
    {
        private readonly ISyncDataFromDBService syncDataService;

        public HongMaHrmWorker(
            ISyncDataFromDBService syncDataService,
            List<JobSettings> jobSettings,
            ILogger<HongMaHrmWorker> logger) : base(logger)
        {
            this.syncDataService = syncDataService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncDataFromHongMa);
        }

        public override async Task DoWorkAsync()
        {
            await syncDataService.SyncUnitAsync(jobSetting.SiteId, jobSetting.DbName);
            await syncDataService.SyncDepartmentAsync(jobSetting.SiteId, jobSetting.DbName);
            await syncDataService.SyncSectionAsync(jobSetting.SiteId, jobSetting.DbName);
            await syncDataService.SyncTeamAsync(jobSetting.SiteId, jobSetting.DbName);
            await syncDataService.SyncJobTitleAsync(jobSetting.SiteId, jobSetting.DbName);
            await syncDataService.SyncJobLevelAsync(jobSetting.SiteId, jobSetting.DbName);
            await syncDataService.SyncEmployeeInfoAsync(jobSetting.SiteId, jobSetting.DbName);
        }
    }
}