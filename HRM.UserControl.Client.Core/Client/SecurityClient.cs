namespace HRM.UserControl.Client.Core.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models.Auth;

    public class SecurityClient : BaseHttpClient, ISecurityClient
    {
        public LoginResult Login(LoginParam loginParam)
        {
            var data = new Dictionary<string, object>
            {
                {"username", loginParam.Username },
                {"password", loginParam.Password },
                {"token", loginParam.Token },
                {"browserName", loginParam.BrowserName },
                {"domain", loginParam.Domain },
                {"platform", Environment.OSVersion.Platform.ToString() }
            };

            return Post<LoginResult>(RelativePaths.LoginPath, data);
        }

        public async Task<LoginResult> LoginAsync(LoginParam loginParam)
        {
            var data = new Dictionary<string, object>
            {
                {"username", loginParam.Username },
                {"password", loginParam.Password },
                {"token", loginParam.Token },
                {"ip", loginParam.IP },
                {"browserName", loginParam.BrowserName },
                {"domain", loginParam.Domain },
                {"platform", Environment.OSVersion.Platform.ToString() }
            };

            return await PostAsync<LoginResult>(RelativePaths.LoginPath, data);
        }

        private static class RelativePaths
        {
            public const string LoginPath = "api/auth/login";
        }
    }
}