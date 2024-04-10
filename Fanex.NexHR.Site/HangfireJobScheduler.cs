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
        public static void ScheduleRecurringJobs()
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
                    () => backgroundJob.ImportTimeRecord(), "*/5 * * * *", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // every 5 minutes
            }

            if (ConfigSettings.EnableUpdateWorkingHourPerDay)
            {
                RecurringJob.AddOrUpdate("Update working hour per day",
                    () => backgroundJob.UpdateWorkingHourPerDay(), "00 08 * * *", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // everyday at 08:00AM
            }

            if (ConfigSettings.EnablePublishSchedule)
            {
                RecurringJob.AddOrUpdate("Public schedule",
                    () => backgroundJob.PublishSchedule(), "05 08 * * *", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // everyday at 08:05AM
            }

            if (ConfigSettings.EnableInsertScheduleByDefaultShift)
            {
                RecurringJob.AddOrUpdate("Insert schedule by default shift",
                    () => backgroundJob.InsertScheduleByDefaultShift(), "10 08 * * FRI", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // every friday at 08:10AM
            }

            if (ConfigSettings.EnableInsertScheduleByWeeklyDefaultShift)
            {
                RecurringJob.AddOrUpdate("Insert schedule by weekly default shift",
                    () => backgroundJob.InsertScheduleByWeeklyDefaultShift(), "15 08 * * FRI", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // every friday at 08:15AM
            }

            if (ConfigSettings.EnableInsertHolidayAutomatically)
            {
                RecurringJob.AddOrUpdate("Add holiday for next year",
                    () => backgroundJob.InsertHolidayAutomatically(), "00 08 1 9 *", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // 01 Sep at 08:00AM
            }

            if (ConfigSettings.EnableSyncWorkingShift)
            {
                RecurringJob.AddOrUpdate("Sync working shift from OWP",
                    () => backgroundJob.SyncWorkingShiftAsync(), "*/5 * * * *", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // every 5 minutes
            }

            AnnualLeaveBalanceJobs(backgroundJob);
        }

        private static void AnnualLeaveBalanceJobs(IBackgroundJobService backgroundJob)
        {
            if (ConfigSettings.EnableUpdateAnnualLeaveBalanceForNewEmployee)
            {
                RecurringJob.AddOrUpdate("Add annual leave balance for new employee",
                    () => backgroundJob.UpdateAnnualLeaveBalanceForNewEmployee(), "02 08 * * *", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // everyday at 08:02AM
            }

            if (ConfigSettings.EnableUpdateAnnualLeaveBalance)
            {
                RecurringJob.AddOrUpdate("Add annual leave balance for next year",
                    () => backgroundJob.UpdateAnnualLeaveBalance(), "05 08 1 9 *", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // run once a year at 08:05AM of 1 September

                RecurringJob.AddOrUpdate("New employee - Add annual leave balance for next year",
                    () => backgroundJob.UpdateAnnualLeaveBalance(), "05 08 L 12 *", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // run once a year at 08:05AM of 31 December
            }

            if (ConfigSettings.EnableRemindAnnualLeaveBalance)
            {
                RecurringJob.AddOrUpdate("Reminder to use AL",
                    () => backgroundJob.SendMailRemindUserToUseAnnualLeave(), "00 08 1 3,6,9,12 *", new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }); // first day of month, only in Mar, Jun, Sep, Dec at 08:00AM
            }
        }
    }
}