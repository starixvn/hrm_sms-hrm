namespace HRM.UserControl.Client.Core.Services.Interfaces
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models.Auth;

    public interface ISecurityService
    {
        Task<string> RefreshTokenAsync();

        string RefreshToken();

        Task<LoginResult> LoginAsync(LoginParam loginParam);

        LoginResult Login(LoginParam loginParam);
    }
}