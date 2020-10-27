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

            RecurringJob.AddOrUpdate("UpdateWorkingHourPerDay",
                () => backgroundJob.UpdateWorkingHourPerDay(), "01 00 * * *", TimeZoneInfo.Local); // everyday at 00:01AM

            RecurringJob.AddOrUpdate("PublishSchedule",
                () => backgroundJob.PublishSchedule(), "05 00 * * *", TimeZoneInfo.Local); // everyday at 00:05AM

            RecurringJob.AddOrUpdate("InsertScheduleByDefaultShift",
                () => backgroundJob.InsertScheduleByDefaultShift(), "10 00 * * FRI", TimeZoneInfo.Local); // every friday at 00:10AM

            RecurringJob.AddOrUpdate("InsertScheduleByWeeklyDefaultShift",
                () => backgroundJob.InsertScheduleByWeeklyDefaultShift(), "15 00 * * FRI", TimeZoneInfo.Local); // every friday at 00:15AM

            RecurringJob.AddOrUpdate("InsertHolidayAutomatically",
                () => backgroundJob.InsertHolidayAutomatically(), "01 00 1 9 *", TimeZoneInfo.Local); // 01 Sep at 00:01AM

            RecurringJob.AddOrUpdate("UpdateAnnualLeaveBalanceForNewEmployee",
                () => backgroundJob.UpdateAnnualLeaveBalanceForNewEmployee(), "02 00 * * *", TimeZoneInfo.Local); // everyday at 00:02AM

            RecurringJob.AddOrUpdate("UpdateAnnualLeaveBalance",
                () => backgroundJob.UpdateAnnualLeaveBalance(), "05 00 1 9 *", TimeZoneInfo.Local); // run once a year at 00:05AM of 1 September
        }
    }
}