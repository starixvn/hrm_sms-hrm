namespace HRM.UserControl.Client.Core.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models.Auth;

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
    }
}