namespace Fanex.NexHR.Site
{
    using System;
    using Fanex.NexHR.Services;
    using Hangfire;

    public static class HangfireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            RecurringJob.AddOrUpdate<BackgroundJobService>("InsertScheduleByDefaultShift",
                job => job.InsertScheduleByDefaultShift(), "05 00 * * FRI", TimeZoneInfo.Local); // every friday at 00:05AM

            RecurringJob.AddOrUpdate<BackgroundJobService>("InsertScheduleByWeeklyDefaultShift",
                job => job.InsertScheduleByWeeklyDefaultShift(), "10 00 01 1-12/4 *", TimeZoneInfo.Local); // every four months at 00:10AM
        }
    }
}