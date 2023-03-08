namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class HrppUnitHeadInfoWorker : BaseWorker
    {
        private readonly IHrppUnitHeadService unitHeadService;

        public HrppUnitHeadInfoWorker(
            IHrppUnitHeadService unitHeadService,
            List<JobSettings> jobSettings,
            ILogger<HrppUnitHeadInfoWorker> logger) : base(logger)
        {
            this.unitHeadService = unitHeadService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncUnitHeadInfoFromHRPP);
        }

        public override async Task DoWorkAsync()
        {
            await unitHeadService.SyncUniHeadInfoAsync();
        }
    }
}