namespace HRM.UserControl.Client.Core.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models.Auth;

    public interface ISecurityService
    {
        Task<LoginResult> LoginAsync(LoginParam loginParam);

        LoginResult Login(LoginParam loginParam);
    }
}