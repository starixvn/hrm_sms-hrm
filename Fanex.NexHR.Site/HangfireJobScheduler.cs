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
                RecurringJob.AddOrUpdate("Import time record",
                    () => backgroundJob.ImportTimeRecord(), "*/5 * * * *", TimeZoneInfo.Local); // every 5 minutes
            }

            if (ConfigSettings.EnableUpdateWorkingHourPerDay)
            {
                RecurringJob.AddOrUpdate("Update working hour per day",
                    () => backgroundJob.UpdateWorkingHourPerDay(), "01 00 * * *", TimeZoneInfo.Local); // everyday at 00:01AM
            }

            if (ConfigSettings.EnablePublishSchedule)
            {
                RecurringJob.AddOrUpdate("Public schedule",
                    () => backgroundJob.PublishSchedule(), "05 00 * * *", TimeZoneInfo.Local); // everyday at 00:05AM
            }

            if (ConfigSettings.EnableInsertScheduleByDefaultShift)
            {
                RecurringJob.AddOrUpdate("Insert schedule by default shift",
                    () => backgroundJob.InsertScheduleByDefaultShift(), "10 00 * * FRI", TimeZoneInfo.Local); // every friday at 00:10AM
            }

            if (ConfigSettings.EnableInsertScheduleByWeeklyDefaultShift)
            {
                RecurringJob.AddOrUpdate("Insert schedule by weekly default shift",
                    () => backgroundJob.InsertScheduleByWeeklyDefaultShift(), "15 00 * * FRI", TimeZoneInfo.Local); // every friday at 00:15AM
            }

            if (ConfigSettings.EnableInsertHolidayAutomatically)
            {
                RecurringJob.AddOrUpdate("Add holiday for next year",
                    () => backgroundJob.InsertHolidayAutomatically(), "01 00 1 9 *", TimeZoneInfo.Local); // 01 Sep at 00:01AM
            }

            if (ConfigSettings.EnableUpdateAnnualLeaveBalanceForNewEmployee)
            {
                RecurringJob.AddOrUpdate("Add annual leave balance for new employee",
                    () => backgroundJob.UpdateAnnualLeaveBalanceForNewEmployee(), "02 00 * * *", TimeZoneInfo.Local); // everyday at 00:02AM
            }

            if (ConfigSettings.EnableUpdateAnnualLeaveBalance)
            {
                RecurringJob.AddOrUpdate("Add annual leave balance for next year",
                    () => backgroundJob.UpdateAnnualLeaveBalance(), "05 00 1 9 *", TimeZoneInfo.Local); // run once a year at 00:05AM of 1 September

                RecurringJob.AddOrUpdate("New employee - Add annual leave balance for next year",
                    () => backgroundJob.UpdateAnnualLeaveBalance(), "05 00 L 12 *", TimeZoneInfo.Local); // run once a year at 00:05AM of 31 December
            }

            if (ConfigSettings.EnableSyncWorkingShift)
            {
                RecurringJob.AddOrUpdate("Sync working shift from OWP",
                    () => backgroundJob.SyncWorkingShiftAsync(), "*/5 * * * *", TimeZoneInfo.Local); // every 5 minutes
            }

            if (ConfigSettings.EnableRemindAnnualLeaveBalance)
            {
                RecurringJob.AddOrUpdate("Reminder to use AL",
                    () => backgroundJob.SendMailRemindUserToUseAnnualLeave(), "20 00 1 3,6,9,12 *", TimeZoneInfo.Local); // first day of month, only in Mar, Jun, Sep, Dec
            }
        }
    }
}