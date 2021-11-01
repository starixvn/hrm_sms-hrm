namespace HRM.UserControl.Client.Core.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models.Auth;

    public interface ISecurityService
    {
        Task<string> RefreshTokenAsync();

        string RefreshToken();

        Task<LoginResult> LoginAsync(LoginParam loginParam);

        LoginResult Login(LoginParam loginParam);
    }

    public class SecurityService : ISecurityService
    {
        private readonly SecurityClient securityClient;

        public SecurityService()
        {
            securityClient = new SecurityClient();
        }

        public async Task<LoginResult> LoginAsync(LoginParam loginParam)
            => await securityClient.LoginAsync(loginParam);

        public LoginResult Login(LoginParam loginParam)
        => securityClient.Login(loginParam);

        public async Task<string> RefreshTokenAsync()
            => await securityClient.GetRefreshedTokenAsync();

        public string RefreshToken()
            => securityClient.GetRefreshedToken();
    }
}