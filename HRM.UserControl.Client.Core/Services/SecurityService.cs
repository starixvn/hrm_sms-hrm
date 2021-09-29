namespace HRM.UserControl.Client.Core.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Services.Interfaces;

    public class SecurityService : ISecurityService
    {
        private readonly SecurityClient securityClient;

        public SecurityService()
        {
            securityClient = new SecurityClient();
        }

        public async Task<string> RefreshTokenAsync()
            => await securityClient.GetRefreshedTokenAsync();

        public async Task<LoginResult> LoginAsync(LoginParam loginParam)
            => await securityClient.LoginAsync(loginParam);

        public LoginResult Login(LoginParam loginParam)
        => securityClient.Login(loginParam);

        public string RefreshToken()
            => securityClient.GetRefreshedToken();
    }
}