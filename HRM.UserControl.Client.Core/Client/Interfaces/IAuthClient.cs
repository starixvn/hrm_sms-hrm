namespace HRM.UserControl.Client.Core.Client
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.Auth;

    public interface IAuthClient
    {
        LoginResult Login(LoginParam loginParam);

        Task<LoginResult> LoginAsync(LoginParam loginParam);

        ReCheckInResult ReCheckIn(long onUserId);

        Task<ReCheckInResult> ReCheckInAsync(long onUserId);

        bool Logout(long onUserId);

        Task<bool> LogoutAsync(long onUserId);
    }
}