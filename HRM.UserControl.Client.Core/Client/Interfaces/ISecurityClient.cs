namespace HRM.UserControl.Client.Core.Client
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models.Auth;

    public interface ISecurityClient
    {
        Task<LoginResult> LoginAsync(LoginParam loginParam);

        LoginResult Login(LoginParam loginParam);
    }
}