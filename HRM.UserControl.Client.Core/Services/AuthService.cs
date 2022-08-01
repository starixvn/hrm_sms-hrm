namespace HRM.UserControl.Client.Core.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.Auth;

    public class AuthService : IAuthService
    {
        private readonly AuthClient authClient;
        private readonly SSOClient ssoClient;

        public AuthService()
        {
            authClient = new AuthClient();
            ssoClient = new SSOClient();
        }

        public LoginResult Login(LoginParam loginParam)
            => authClient.Login(loginParam);

        public async Task<LoginResult> LoginAsync(LoginParam loginParam)
            => await authClient.LoginAsync(loginParam);

        public LoginResult SSOLogin(SSOLoginParam loginParam)
            => ssoClient.Login(loginParam);

        public async Task<LoginResult> SSOLoginAsync(SSOLoginParam loginParam)
            => await ssoClient.LoginAsync(loginParam);

        public string SSOGetTime()
            => ssoClient.GetTime();

        public async Task<string> SSOGetTimeAsync()
            => await ssoClient.GetTimeAsync();

        public ReCheckInResult ReCheckIn(long onUserId)
            => authClient.ReCheckIn(onUserId);

        public async Task<ReCheckInResult> ReCheckInAsync(long onUserId)
            => await authClient.ReCheckInAsync(onUserId);

        public bool Logout(long onUserId)
            => authClient.Logout(onUserId);

        public async Task<bool> LogoutAsync(long onUserId)
            => await authClient.LogoutAsync(onUserId);
    }
}