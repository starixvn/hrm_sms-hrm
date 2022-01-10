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

        IList<User> GetAll(string keyword = null, byte? statusId = null);

        Task<IList<User>> GetAllAsync(string keyword = null, byte? statusId = null);

        User GetById(int id);

        Task<User> GetByIdAsync(int id);

        CommandResult Create(CreateUserParam param);

        Task<CommandResult> CreateAsync(CreateUserParam param);

        CommandResult UpdateInfo(UpdateUserInfoParam param);

        Task<CommandResult> UpdateInfoAsync(UpdateUserInfoParam param);

        CommandResult AssignRole(AssignRoleParam param);

        Task<CommandResult> AssignRoleAsync(AssignRoleParam param);

        CommandResult UpdatePassword(UpdatePasswordParam param);

        Task<CommandResult> UpdatePasswordAsync(UpdatePasswordParam param);

        CommandResult ResetPassword(UpdatePasswordParam param);

        Task<CommandResult> ResetPasswordAsync(UpdatePasswordParam param);

        CommandResult Delete(int userId);

        Task<CommandResult> DeleteAsync(int userId);
    }
}