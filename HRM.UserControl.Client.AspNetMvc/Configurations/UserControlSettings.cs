namespace HRM.UserControl.Client.AspNetMvc.Configurations
{
    using System.ComponentModel;
    using System.Configuration;
    using HRM.UserControl.Client.Core.Configurations;

    public class UserControlSettings : IUserControlSettings
    {
        public UserControlSettings()
        {
            SiteId = GetValue<short>("HRM.UserControl.Client.Configurations:SiteId");
            AccessToken = GetValue<string>("HRM.UserControl.Client.Configurations:AccessToken");
            ServiceUrl = GetValue<string>("HRM.UserControl.Client.Configurations:ServiceUrl");
        }

        public UserControlSettings(short siteId, string accessToken, string serviceUrl)
        {
            SiteId = siteId;
            AccessToken = accessToken;
            ServiceUrl = serviceUrl;
        }

        public short SiteId { get; set; }

        public string AccessToken { get; set; }

        public string ServiceUrl { get; set; }

        private static T GetValue<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}