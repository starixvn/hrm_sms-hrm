namespace HRM.UserControl.Client.AspNetCore.Configurations
{
    using System;
    using System.ComponentModel;
    using HRM.UserControl.Client.Core.Configurations;
    using Microsoft.Extensions.Configuration;

    public class UserControlSettings : IUserControlSettings
    {
        private readonly IConfiguration configuration;

        public UserControlSettings(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            SiteId = GetValue<int>("HRM.UserControl.Client.Configurations:SiteId");
            SiteName = GetValue<string>("HRM.UserControl.Client.Configurations:SiteName");
            AccessToken = GetValue<string>("HRM.UserControl.Client.Configurations:AccessToken");
            ServiceUrl = GetValue<string>("HRM.UserControl.Client.Configurations:ServiceUrl");
        }

        public int SiteId { get; }

        public string SiteName { get; }

        public string AccessToken { get; }

        public string ServiceUrl { get; }

        private T GetValue<T>(string key)
        {
            var value = configuration.GetSection(key).Value;

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}