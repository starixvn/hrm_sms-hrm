namespace HRM.UserControl.WebAPI.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Models.User;
    using HRM.UserControl.WebAPI.Criteria.User;

    public interface IUserService
    {
        Task<User> GetByUsernameAsync(string username);

        Task<User> GetByIdAsync(int userId);

        Task<CommandResult> UpdatePasswordAsync(UpdateUserPasswordCommand command);

        Task<CommandResult> UserCheckInAsync(UserCheckInCommand command);

        Task<CommandResult> UserReCheckInAsync(UserReCheckInCommand command);

        Task<CommandResult> UserLogoutAsync(UserLogoutCommand command);

        Task<CommandResult> SSOCheckInAsync(SSOCheckInCommand command);

        Task<CommandResult> LogUserLoginAsync(InsertLogUserLoginCommand command);
    }
}