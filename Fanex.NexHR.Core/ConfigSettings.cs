namespace Fanex.NexHR.Core
{
    using System;
    using System.Configuration;

    public static class ConfigSettings
    {
        public static string EmailSetting => ConfigurationManager.AppSettings["Email"];
        public static bool IsShowCloseBalance => Convert.ToBoolean(ConfigurationManager.AppSettings["IsCloseBalance"]);
        public static bool AllowAdminLogin => Convert.ToBoolean(ConfigurationManager.AppSettings["AllowAdminLogin"]);
        public static bool IsDisabledSendMail => Convert.ToBoolean(ConfigurationManager.AppSettings["IsDisabledSendMail"]);
        public static short SiteId => Convert.ToInt16(ConfigurationManager.AppSettings["SiteId"]);
        public static string ApplicationDomain => ConfigurationManager.AppSettings["ApplicationDomain"];
        public static string SyncWorkingShiftUrl => ConfigurationManager.AppSettings["SyncWorkingShiftUrl"];
        public static bool EnableImportTimeRecord => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableImportTimeRecord"]);
        public static bool EnableUpdateWorkingHourPerDay => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableUpdateWorkingHourPerDay"]);
        public static bool EnablePublishSchedule => Convert.ToBoolean(ConfigurationManager.AppSettings["EnablePublishSchedule"]);
        public static bool EnableInsertScheduleByDefaultShift
            => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableInsertScheduleByDefaultShift"]);
        public static bool EnableInsertScheduleByWeeklyDefaultShift
            => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableInsertScheduleByWeeklyDefaultShift"]);
        public static bool EnableInsertHolidayAutomatically
            => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableInsertHolidayAutomatically"]);
        public static bool EnableUpdateAnnualLeaveBalanceForNewEmployee
            => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableUpdateAnnualLeaveBalanceForNewEmployee"]);
        public static bool EnableUpdateAnnualLeaveBalance
            => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableUpdateAnnualLeaveBalance"]);
        public static bool EnableSyncWorkingShift
            => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSyncWorkingShift"]);
        public static bool EnableManualSyncWorkingShift
            => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableManualSyncWorkingShift"]);
        public static bool EnableRemindAnnualLeaveBalance
            => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableRemindAnnualLeaveBalance"]);
    }
}