namespace HRM.UserControl.Client.Core.Client.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Client.Core;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Auth;

    public interface IUserClient
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<LoginResult> LoginAsync(LoginParam loginParam);

        Task<User> GetUserByIdAsync(int userId);
    }

    internal class UserClient : BaseHttpClient, IUserClient
    {
        public async Task<User> GetUserByEmailAsync(string email)
        {
            string requestUrl = string.Format(RelativePaths.GetUserByEmailPath, email);

            return await GetAsync<User>(requestUrl);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            string requestUrl = string.Format(RelativePaths.GetUserByIdPath, userId);

            return await GetAsync<User>(requestUrl);
        }

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
            public const string GetUserByEmailPath = "api/user/getByEmail?email={0}";
            public const string GetUserByIdPath = "api/user/{0}";
            public const string LoginPath = "api/auth/signin";
        }
    }
}