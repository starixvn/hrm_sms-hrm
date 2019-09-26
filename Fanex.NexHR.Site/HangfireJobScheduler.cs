namespace Fanex.NexHR.Site
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Services;
    using Hangfire;

    public static class HangfireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            IBackgroundJobService backgroundJob = DependencyResolver.Current.GetService<BackgroundJobService>();

            RecurringJob.AddOrUpdate("PublishSchedule",
                () => backgroundJob.PublishSchedule(), "05 00 * * *", TimeZoneInfo.Local); // everyday at 00:05AM

            RecurringJob.AddOrUpdate("InsertScheduleByDefaultShift",
                () => backgroundJob.InsertScheduleByDefaultShift(), "10 00 * * FRI", TimeZoneInfo.Local); // every friday at 00:10AM

            RecurringJob.AddOrUpdate("InsertScheduleByWeeklyDefaultShift",
                () => backgroundJob.InsertScheduleByWeeklyDefaultShift(), "15 00 01 1-12/4 *", TimeZoneInfo.Local); // every four months at 00:15AM
                
            RecurringJob.AddOrUpdate("InsertHolidayAutomatically",
                () => backgroundJob.InsertHolidayAutomatically(), "1 0 1 1 *", TimeZoneInfo.Local);
        }
    }
}