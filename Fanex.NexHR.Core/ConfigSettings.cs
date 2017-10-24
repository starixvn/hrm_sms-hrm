namespace Fanex.NexHR.Core
{
    using Fanex.Common;
    using System.Configuration;

    public static class ConfigSettings
    {
        public static string EmailSetting
        {
            get
            {
                return ConfigurationManager.AppSettings["Email"];
            }
        }

        public static bool IsShowCloseBalance
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["IsCloseBalance"]);
            }
        }

        public static bool AllowEditPersonalInformation
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["AllowEditPersonalInformation"]);
            }
        }

        public static bool AllowAdminLogin
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["AllowAdminLogin"]);
            }
        }

        public static bool IsDisabledSendMail
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["IsDisabledSendMail"]);
            }
        }

        public static int SiteId
        {
            get
            {
                return Converter.ToInt(ConfigurationManager.AppSettings["SiteId"]);
            }
        }

        public static string ApplicationDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationDomain"];
            }
        }
    }
}