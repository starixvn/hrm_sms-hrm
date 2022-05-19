namespace Fanex.NexHR.Site
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services;
    using Hangfire;
    using Hangfire.Storage;

    public static class HangfireJobScheduler
    {
#pragma warning disable S1541 // Methods and properties should not be too complex
        public static void ScheduleRecurringJobs()
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            IBackgroundJobService backgroundJob = DependencyResolver.Current.GetService<BackgroundJobService>();

            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            if (ConfigSettings.EnableImportTimeRecord)
            {
                RecurringJob.AddOrUpdate("ImportTimeRecord",
                    () => backgroundJob.ImportTimeRecord(), "*/5 * * * *", TimeZoneInfo.Local); // every 5 minutes
            }

            if (ConfigSettings.EnableUpdateWorkingHourPerDay)
            {
                RecurringJob.AddOrUpdate("UpdateWorkingHourPerDay",
                    () => backgroundJob.UpdateWorkingHourPerDay(), "01 00 * * *", TimeZoneInfo.Local); // everyday at 00:01AM
            }

            if (ConfigSettings.EnablePublishSchedule)
            {
                RecurringJob.AddOrUpdate("PublishSchedule",
                    () => backgroundJob.PublishSchedule(), "05 00 * * *", TimeZoneInfo.Local); // everyday at 00:05AM
            }

            if (ConfigSettings.EnableInsertScheduleByDefaultShift)
            {
                RecurringJob.AddOrUpdate("InsertScheduleByDefaultShift",
                    () => backgroundJob.InsertScheduleByDefaultShift(), "10 00 * * FRI", TimeZoneInfo.Local); // every friday at 00:10AM
            }

            if (ConfigSettings.EnableInsertScheduleByWeeklyDefaultShift)
            {
                RecurringJob.AddOrUpdate("InsertScheduleByWeeklyDefaultShift",
                    () => backgroundJob.InsertScheduleByWeeklyDefaultShift(), "15 00 * * FRI", TimeZoneInfo.Local); // every friday at 00:15AM
            }

            if (ConfigSettings.EnableInsertHolidayAutomatically)
            {
                RecurringJob.AddOrUpdate("InsertHolidayAutomatically",
                    () => backgroundJob.InsertHolidayAutomatically(), "01 00 1 9 *", TimeZoneInfo.Local); // 01 Sep at 00:01AM
            }

            if (ConfigSettings.EnableUpdateAnnualLeaveBalanceForNewEmployee)
            {
                RecurringJob.AddOrUpdate("UpdateAnnualLeaveBalanceForNewEmployee",
                    () => backgroundJob.UpdateAnnualLeaveBalanceForNewEmployee(), "02 00 * * *", TimeZoneInfo.Local); // everyday at 00:02AM
            }

            if (ConfigSettings.EnableUpdateAnnualLeaveBalance)
            {
                RecurringJob.AddOrUpdate("UpdateAnnualLeaveBalance",
                    () => backgroundJob.UpdateAnnualLeaveBalance(), "05 00 1 9 *", TimeZoneInfo.Local); // run once a year at 00:05AM of 1 September
            }

            if (ConfigSettings.EnableSyncWorkingShift)
            {
                RecurringJob.AddOrUpdate("SyncWorkingShift",
                    () => backgroundJob.SyncWorkingShiftAsync(), "*/5 * * * *", TimeZoneInfo.Local); // every 5 minutes
            }

            if (ConfigSettings.EnableRemindAnnualLeaveBalance)
            {
                RecurringJob.AddOrUpdate("SendMailRemindUserToUseAnnualLeave",
                    () => backgroundJob.SendMailRemindUserToUseAnnualLeave(), "20 00 1 3,6,9,12 *", TimeZoneInfo.Local); // first day of month, only in Mar, Jun, Sep, Dec
            }
        }
    }
}