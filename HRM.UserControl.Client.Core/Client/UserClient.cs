namespace HRM.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.User;

    public class UserClient : BaseHttpClient, IUserClient
    {
        public IList<MenuItem> GetMenuItem()
            => Get<IList<MenuItem>>(RelativePaths.GetMenuItemPath);

        public async Task<IList<MenuItem>> GetMenuItemAsync()
            => await GetAsync<IList<MenuItem>>(RelativePaths.GetMenuItemPath);

        public IList<FunctionPermission> GetFunctionPermission(short siteId, int userId)
            => Get<IList<FunctionPermission>>(string.Format(RelativePaths.GetFunctionPermissionPath, siteId, userId));

        public async Task<IList<FunctionPermission>> GetFunctionPermissionAsync(short siteId, int userId)
            => await GetAsync<IList<FunctionPermission>>(string.Format(RelativePaths.GetFunctionPermissionPath, siteId, userId));

        public IList<User> GetAll(string keyword = null, byte? statusId = null)
            => Get<IList<User>>(string.Format(RelativePaths.GetAllUserPath, keyword, statusId));

        public async Task<IList<User>> GetAllAsync(string keyword = null, byte? statusId = null)
            => await GetAsync<IList<User>>(string.Format(RelativePaths.GetAllUserPath, keyword, statusId));

        public User GetById(int id)
            => Get<User>(string.Format(RelativePaths.GetUserByIdPath, id));

        public async Task<User> GetByIdAsync(int id)
            => await GetAsync<User>(string.Format(RelativePaths.GetUserByIdPath, id));

        public CommandResult Create(CreateUserParam param)
            => Post<CreateUserParam, CommandResult>(RelativePaths.CreateUserPath, param);

        public async Task<CommandResult> CreateAsync(CreateUserParam param)
            => await PostAsync<CreateUserParam, CommandResult>(RelativePaths.CreateUserPath, param);

        public CommandResult UpdateInfo(UpdateUserInfoParam param)
            => Post<UpdateUserInfoParam, CommandResult>(RelativePaths.UpdateUserInfoPath, param);

        public async Task<CommandResult> UpdateInfoAsync(UpdateUserInfoParam param)
            => await PostAsync<UpdateUserInfoParam, CommandResult>(RelativePaths.UpdateUserInfoPath, param);

        public CommandResult AssignRole(AssignRoleParam param)
            => Post<AssignRoleParam, CommandResult>(RelativePaths.AssignRoleToUserPath, param);

        public async Task<CommandResult> AssignRoleAsync(AssignRoleParam param)
            => await PostAsync<AssignRoleParam, CommandResult>(RelativePaths.AssignRoleToUserPath, param);

        public CommandResult UpdatePassword(UpdatePasswordParam param)
            => Post<UpdatePasswordParam, CommandResult>(RelativePaths.UpdatePasswordPath, param);

        public async Task<CommandResult> UpdatePasswordAsync(UpdatePasswordParam param)
            => await PostAsync<UpdatePasswordParam, CommandResult>(RelativePaths.UpdatePasswordPath, param);

        public CommandResult ResetPassword(UpdatePasswordParam param)
            => Post<UpdatePasswordParam, CommandResult>(RelativePaths.ResetPasswordPath, param);

        public async Task<CommandResult> ResetPasswordAsync(UpdatePasswordParam param)
            => await PostAsync<UpdatePasswordParam, CommandResult>(RelativePaths.ResetPasswordPath, param);

        public CommandResult Delete(int userId)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", userId }
            };
            return Post<Dictionary<string, object>, CommandResult>(RelativePaths.DeleteUserPath, data);
        }

        public async Task<CommandResult> DeleteAsync(int userId)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", userId }
            };
            return await PostAsync<Dictionary<string, object>, CommandResult>(RelativePaths.DeleteUserPath, data);
        }

        private static class RelativePaths
        {
            public const string GetMenuItemPath = "api/user/getMenuItem";
            public const string GetFunctionPermissionPath = "api/user/getFunctionPermission?siteId={0}&userId={1}";
            public const string GetAllUserPath = "api/user/getAll?keyword={0}&statusId={1}";
            public const string GetUserByIdPath = "api/user/getById/{0}";
            public const string CreateUserPath = "api/user/create";
            public const string UpdateUserInfoPath = "api/user/update";
            public const string AssignRoleToUserPath = "api/user/assignRole";
            public const string UpdatePasswordPath = "api/user/updatePassword";
            public const string ResetPasswordPath = "api/user/resetPassword";
            public const string DeleteUserPath = "api/user/delete";
        }
    }
}