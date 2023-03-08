namespace GO.AutoJobs.Workers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cronos;
    using GO.AutoJobs.Models;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class BaseWorker : BackgroundService
    {
        private readonly ILogger logger;
        protected JobSettings jobSetting;

        public BaseWorker(ILogger<BaseWorker> logger)
        {
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{jobSetting.JobName}: Start");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var cronExpression = CronExpression.Parse(jobSetting.CronExpression);
                    var currentUtcTime = DateTimeOffset.UtcNow.UtcDateTime;
                    var occurenceTime = cronExpression.GetNextOccurrence(currentUtcTime);

                    var delay = occurenceTime.GetValueOrDefault() - currentUtcTime;
                    await Task.Delay(delay, stoppingToken);

                    await RunJobAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"{jobSetting.JobName}: Error - {ex}");
            }
        }

        private async Task RunJobAsync()
        {
            try
            {
                if (jobSetting.IsEnabled)
                {
                    logger.LogInformation($"{jobSetting.JobName}: Doing...");
                    await DoWorkAsync();
                    logger.LogInformation($"{jobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"{jobSetting.JobName}: Error - {ex}");
            }
        }

        public virtual async Task DoWorkAsync()
        {
            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{jobSetting.JobName}: Stop");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logger.LogInformation($"{jobSetting.JobName}: Disposed");
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}