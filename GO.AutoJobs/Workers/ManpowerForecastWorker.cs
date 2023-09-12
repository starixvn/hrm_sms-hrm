namespace GO.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Constants;
    using GO.AutoJobs.Helpers;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class ManpowerForecastWorker : BaseWorker
    {
        private readonly IManpowerForecastService manpowerForecastService;

        public ManpowerForecastWorker(
            IManpowerForecastService manpowerForecastService,
            List<JobSettings> jobSettings,
            ILogger<ManpowerForecastWorker> serilog,
            Fanex.Logging.ILogger logger) : base(serilog, logger)
        {
            this.manpowerForecastService = manpowerForecastService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.ManpowerForecast);
        }

        public override async Task DoWorkAsync()
        {
            await manpowerForecastService.SendNotificationAsync();
        }
    }
}