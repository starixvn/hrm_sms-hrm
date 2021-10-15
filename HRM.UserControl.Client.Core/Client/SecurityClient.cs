namespace HRM.UserControl.Client.Core.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models.Auth;

    internal interface ISecurityClient
    {
        Task<string> GetRefreshedTokenAsync();

        string GetRefreshedToken();

        Task<LoginResult> LoginAsync(LoginParam loginParam);

        LoginResult Login(LoginParam loginParam);
    }

    internal class SecurityClient : BaseHttpClient, ISecurityClient
    {
        public string GetRefreshedToken()
            => Post<string>(RelativePaths.RefreshTokenPath, null);

        public async Task<string> GetRefreshedTokenAsync()
            => await PostAsync<string>(RelativePaths.RefreshTokenPath, null);

        public LoginResult Login(LoginParam loginParam)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", loginParam.UserId },
                {"ip", loginParam.Ip },
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
                {"userId", loginParam.UserId },
                {"ip", loginParam.Ip },
                {"browserName", loginParam.BrowserName },
                {"domain", loginParam.Domain },
                {"platform", Environment.OSVersion.Platform.ToString() }
            };

            return await PostAsync<LoginResult>(RelativePaths.LoginPath, data);
        }

        private static class RelativePaths
        {
            public const string LoginPath = "api/auth/signin";
            public const string RefreshTokenPath = "api/refreshToken";
        }
    }
}