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
                return ConfigurationManager.AppSettings["Email"].ToString();
            }
        }

        public static bool IsShowCloseBalance
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["IsCloseBalance"].ToString());
            }
        }

        public static bool AllowEditPersonalInformation
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["AllowEditPersonalInformation"].ToString());
            }
        }

        public static bool AllowAdminLogin
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["AllowAdminLogin"].ToString());
            }
        }

        public static bool IsDisabledSendMail
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["IsDisabledSendMail"].ToString());
            }
        }

        public static int SiteId
        {
            get
            {
                return Converter.ToInt(ConfigurationManager.AppSettings["SiteId"].ToString());
            }
        }
    }
}