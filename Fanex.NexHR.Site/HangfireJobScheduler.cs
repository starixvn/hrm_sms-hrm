namespace Fanex.NexHR.Site
{
    using System;
    using Fanex.NexHR.Services;
    using Hangfire;

    public static class HangfireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            RecurringJob.AddOrUpdate<IBackgroundJobService>("InsertScheduleByDefaultShift",
                job => job.InsertScheduleByDefaultShift(), "05 00 * * FRI", TimeZoneInfo.Local); // every friday at 00:05AM

            RecurringJob.AddOrUpdate<IBackgroundJobService>("InsertScheduleByWeeklyDefaultShift",
                job => job.InsertScheduleByWeeklyDefaultShift(), "10 00 01 1-12/4 *", TimeZoneInfo.Local); // every four months at 00:10AM
        }
    }
}