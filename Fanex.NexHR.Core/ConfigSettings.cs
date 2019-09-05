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
    }
}