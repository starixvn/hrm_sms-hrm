namespace HRM.UserControl.Client.Core.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models.Auth;

    internal interface ISecurityClient
    {
        Task<string> GetRefreshedToken();

        Task<LoginResult> LoginAsync(LoginParam loginParam);
    }

    internal class SecurityClient : BaseHttpClient, ISecurityClient
    {
        public async Task<string> GetRefreshedToken()
       => await PostAsync<string>(RelativePaths.RefreshTokenPath, null);

        public async Task<LoginResult> LoginAsync(LoginParam loginParam)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", loginParam.UserId },
                {"companyId", loginParam.CompanyId },
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