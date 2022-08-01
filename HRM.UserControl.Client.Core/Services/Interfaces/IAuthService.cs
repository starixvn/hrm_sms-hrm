namespace HRM.UserControl.Client.Core.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.Auth;

    public interface IAuthService
    {
        LoginResult Login(LoginParam loginParam);

        Task<LoginResult> LoginAsync(LoginParam loginParam);

        LoginResult SSOLogin(SSOLoginParam loginParam);

        Task<LoginResult> SSOLoginAsync(SSOLoginParam loginParam);

        string SSOGetTime();

        Task<string> SSOGetTimeAsync();

        ReCheckInResult ReCheckIn(long onUserId);

        Task<ReCheckInResult> ReCheckInAsync(long onUserId);

        bool Logout(long onUserId);

        Task<bool> LogoutAsync(long onUserId);
    }
}