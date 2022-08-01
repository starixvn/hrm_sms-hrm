namespace HRM.UserControl.Client.Core.Client
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models.Auth;

    public class SSOClient : BaseHttpClient, ISSOClient
    {
        public string GetTime()
            => Get<string>(RelativePaths.GetTimePath);

        public async Task<string> GetTimeAsync()
            => await GetAsync<string>(RelativePaths.GetTimePath);

        public LoginResult Login(SSOLoginParam loginParam)
            => Post<SSOLoginParam, LoginResult>(RelativePaths.LoginPath, loginParam);

        public async Task<LoginResult> LoginAsync(SSOLoginParam loginParam)
            => await PostAsync<SSOLoginParam, LoginResult>(RelativePaths.LoginPath, loginParam);

        private static class RelativePaths
        {
            public const string GetTimePath = "api/sso/gettime";
            public const string LoginPath = "api/sso/login";
        }
    }
}