namespace HRM.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.Auth;

    public class AuthClient : BaseHttpClient, IAuthClient
    {
        public LoginResult Login(LoginParam loginParam)
            => Post<LoginParam, LoginResult>(RelativePaths.LoginPath, loginParam);

        public async Task<LoginResult> LoginAsync(LoginParam loginParam)
            => await PostAsync<LoginParam, LoginResult>(RelativePaths.LoginPath, loginParam);

        public ReCheckInResult ReCheckIn(long onUserId)
        {
            var data = new Dictionary<string, object>
            {
                { "onUserId", onUserId }
            };
            return Post<Dictionary<string, object>, ReCheckInResult>(RelativePaths.ReCheckInPath, data);
        }

        public async Task<ReCheckInResult> ReCheckInAsync(long onUserId)
        {
            var data = new Dictionary<string, object>
            {
                { "onUserId", onUserId }
            };
            return await PostAsync<Dictionary<string, object>, ReCheckInResult>(RelativePaths.ReCheckInPath, data);
        }

        public bool Logout(long onUserId)
        {
            var data = new Dictionary<string, object>
            {
                { "onUserId", onUserId }
            };
            return Post<Dictionary<string, object>, bool>(RelativePaths.LogoutPath, data);
        }

        public async Task<bool> LogoutAsync(long onUserId)
        {
            var data = new Dictionary<string, object>
            {
                { "onUserId", onUserId }
            };
            return await PostAsync<Dictionary<string, object>, bool>(RelativePaths.LogoutPath, data);
        }

        private static class RelativePaths
        {
            public const string LoginPath = "api/v1/auth/login";
            public const string ReCheckInPath = "api/auth/recheckin";
            public const string LogoutPath = "api/auth/logout";
        }
    }
}