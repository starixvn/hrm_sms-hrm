namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.User;

    public interface IUserService
    {
        IList<MenuItem> GetMenuItem();

        Task<IList<MenuItem>> GetMenuItemAsync();

        IList<FunctionPermission> GetFunctionPermission(short siteId, int userId);

        Task<IList<FunctionPermission>> GetFunctionPermissionAsync(short siteId, int userId);

        User GetById(int id);

        Task<User> GetByIdAsync(int id);

        User GetByCode(string code);

        Task<User> GetByCodeAsync(string code);

        CommandResult Create(CreateUserParam param);

        Task<CommandResult> CreateAsync(CreateUserParam param);

        CommandResult UpdateInfo(UpdateUserInfoParam param);

        Task<CommandResult> UpdateInfoAsync(UpdateUserInfoParam param);

        CommandResult UpdatePassword(UpdatePasswordParam param);

        Task<CommandResult> UpdatePasswordAsync(UpdatePasswordParam param);

        CommandResult ResetPassword(UpdatePasswordParam param);

        Task<CommandResult> ResetPasswordAsync(UpdatePasswordParam param);

        CommandResult Delete(int userId);

        Task<CommandResult> DeleteAsync(int userId);
    }
}