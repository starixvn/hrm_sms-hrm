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

        public static string API_Auth_Username => ConfigurationManager.AppSettings["API.Auth:Username"];
        public static string API_Auth_Password => ConfigurationManager.AppSettings["API.Auth:Password"];

        public static string GO_Token_Url => ConfigurationManager.AppSettings["GO.Token:Url"];
        public static string GO_Token_Username => ConfigurationManager.AppSettings["GO.Token:Username"];
        public static string GO_Token_Password => ConfigurationManager.AppSettings["GO.Token:Password"];

        public static bool GO_Todolist_EnableNotiRefreshHrm
            => Convert.ToBoolean(ConfigurationManager.AppSettings["GO.Todolist:EnableNotiRefreshHrm"]);
        public static string GO_Todolist_NotiRefreshHrmUrl => ConfigurationManager.AppSettings["GO.Todolist:NotiRefreshHrmUrl"];
        // Letstalk
        public static bool IsEnableSendLetstalk
            => Convert.ToBoolean(ConfigurationManager.AppSettings["Letstalk:Enable"]);
        public static string Letstalk_TestingAccountId => ConfigurationManager.AppSettings["Letstalk:TestingAccountId"];
        public static string Letstalk_API_BaseUrl => ConfigurationManager.AppSettings["Letstalk.API:BaseUrl"];
        public static string Letstalk_Token_Username => ConfigurationManager.AppSettings["Letstalk.API.Token:Username"];
        public static string Letstalk_Token_Password => ConfigurationManager.AppSettings["Letstalk.API.Token:Password"];
        public static int Letstalk_BotId => Convert.ToInt32(ConfigurationManager.AppSettings["Letstalk:BotId"]);

        // Proxy
        public static bool IsEnableProxy => Convert.ToBoolean(ConfigurationManager.AppSettings["Proxy:Enable"]);
        public static string Proxy_Address => ConfigurationManager.AppSettings["Proxy:Address"];
        public static bool Proxy_BypassOnLocal => Convert.ToBoolean(ConfigurationManager.AppSettings["Proxy:BypassOnLocal"]);
    }
}