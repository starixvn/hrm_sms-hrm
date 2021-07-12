namespace HRM.UserControl.Client.AspNetMvc.Configurations
{
    using System.ComponentModel;
    using System.Configuration;
    using HRM.UserControl.Client.Core.Configurations;

    public class UserControlSettings : IUserControlSettings
    {
        public UserControlSettings()
        {
            SiteId = GetValue<int>("HRM.UserControl.Client.Configurations:SiteId");
            SiteName = GetValue<string>("HRM.UserControl.Client.Configurations:SiteName");
            AccessToken = GetValue<string>("HRM.UserControl.Client.Configurations:AccessToken");
            ServiceUrl = GetValue<string>("HRM.UserControl.Client.Configurations:ServiceUrl");
        }

        public UserControlSettings(int siteId, string siteName, string accessToken, string serviceUrl)
        {
            SiteId = siteId;
            SiteName = siteName;
            AccessToken = accessToken;
            ServiceUrl = serviceUrl;
        }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

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