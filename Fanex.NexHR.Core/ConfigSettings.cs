namespace Fanex.NexHR.Core
{
    using Fanex.Common;
    using System.Configuration;

    public static class ConfigSettings
    {
        public static string EmailSetting => ConfigurationManager.AppSettings["Email"];
        public static bool IsShowCloseBalance => Converter.ToBool(ConfigurationManager.AppSettings["IsCloseBalance"]);
        public static bool AllowAdminLogin => Converter.ToBool(ConfigurationManager.AppSettings["AllowAdminLogin"]);
        public static bool IsDisabledSendMail => Converter.ToBool(ConfigurationManager.AppSettings["IsDisabledSendMail"]);
        public static int SiteId => Converter.ToInt(ConfigurationManager.AppSettings["SiteId"]);
        public static string ApplicationDomain => ConfigurationManager.AppSettings["ApplicationDomain"];
        public static bool EnableUpdateWorkingHourPerDay => Converter.ToBool(ConfigurationManager.AppSettings["EnableUpdateWorkingHourPerDay"]);
        public static bool EnablePublishSchedule => Converter.ToBool(ConfigurationManager.AppSettings["EnablePublishSchedule"]);
        public static bool EnableInsertScheduleByDefaultShift
            => Converter.ToBool(ConfigurationManager.AppSettings["EnableInsertScheduleByDefaultShift"]);
        public static bool EnableInsertScheduleByWeeklyDefaultShift
            => Converter.ToBool(ConfigurationManager.AppSettings["EnableInsertScheduleByWeeklyDefaultShift"]);
        public static bool EnableInsertHolidayAutomatically
            => Converter.ToBool(ConfigurationManager.AppSettings["EnableInsertHolidayAutomatically"]);
        public static bool EnableUpdateAnnualLeaveBalanceForNewEmployee
            => Converter.ToBool(ConfigurationManager.AppSettings["EnableUpdateAnnualLeaveBalanceForNewEmployee"]);
        public static bool EnableUpdateAnnualLeaveBalance
            => Converter.ToBool(ConfigurationManager.AppSettings["EnableUpdateAnnualLeaveBalance"]);
    }
}