namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class HrppUnitHeadScheduleWorker : BaseWorker
    {
        private readonly IHrppUnitHeadService unitHeadService;

        public HrppUnitHeadScheduleWorker(
            IHrppUnitHeadService unitHeadService,
            List<JobSettings> jobSettings,
            ILogger<HrppUnitHeadScheduleWorker> serilog,
            Fanex.Logging.ILogger logger) : base(serilog, logger)
        {
            this.unitHeadService = unitHeadService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncUnitHeadScheduleFromHRPP);
        }

        public override async Task DoWorkAsync()
        {
            await unitHeadService.SyncUniHeadScheduleAsync();
        }
    }
}