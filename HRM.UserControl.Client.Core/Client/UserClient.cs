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
        {
            var data = new Dictionary<string, object>
            {
                {"code", param.Code },
                {"nickname", param.Nickname },
                {"fullName", param.FullName },
                {"password", param.Password },
                {"token", param.Token },
                {"email", param.Email }
            };
            return Post<CommandResult>(RelativePaths.CreateUserPath, data);
        }

        public async Task<CommandResult> CreateAsync(CreateUserParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"code", param.Code },
                {"nickname", param.Nickname },
                {"fullName", param.FullName },
                {"password", param.Password },
                {"token", param.Token },
                {"email", param.Email }
            };
            return await PostAsync<CommandResult>(RelativePaths.CreateUserPath, data);
        }

        public CommandResult UpdateInfo(UpdateUserInfoParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", param.UserId },
                {"code", param.Code },
                {"nickname", param.Nickname },
                {"fullName", param.FullName },
                {"email", param.Email },
                {"isActive", param.IsActive }
            };
            return Post<CommandResult>(RelativePaths.UpdateUserInfoPath, data);
        }

        public async Task<CommandResult> UpdateInfoAsync(UpdateUserInfoParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", param.UserId },
                {"code", param.Code },
                {"nickname", param.Nickname },
                {"fullName", param.FullName },
                {"email", param.Email },
                {"isActive", param.IsActive }
            };
            return await PostAsync<CommandResult>(RelativePaths.UpdateUserInfoPath, data);
        }

        public CommandResult AssignRole(AssignRoleParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", param.UserId },
                {"roleIds", param.RoleIds }
            };
            return Post<CommandResult>(RelativePaths.AssignRoleToUserPath, data);
        }

        public async Task<CommandResult> AssignRoleAsync(AssignRoleParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", param.UserId },
                {"roleIds", param.RoleIds }
            };
            return await PostAsync<CommandResult>(RelativePaths.AssignRoleToUserPath, data);
        }

        public CommandResult UpdatePassword(UpdatePasswordParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"oldPassword", param.OldPassword },
                {"password", param.Password },
                {"token", param.Token }
            };
            return Post<CommandResult>(RelativePaths.UpdatePasswordPath, data);
        }

        public async Task<CommandResult> UpdatePasswordAsync(UpdatePasswordParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"oldPassword", param.OldPassword },
                {"password", param.Password },
                {"token", param.Token }
            };
            return await PostAsync<CommandResult>(RelativePaths.UpdatePasswordPath, data);
        }

        public CommandResult ResetPassword(UpdatePasswordParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", param.UserId },
                {"password", param.Password },
                {"token", param.Token }
            };
            return Post<CommandResult>(RelativePaths.ResetPasswordPath, data);
        }

        public async Task<CommandResult> ResetPasswordAsync(UpdatePasswordParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", param.UserId },
                {"password", param.Password },
                {"token", param.Token }
            };
            return await PostAsync<CommandResult>(RelativePaths.ResetPasswordPath, data);
        }

        public CommandResult Delete(int userId)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", userId }
            };
            return Post<CommandResult>(RelativePaths.DeleteUserPath, data);
        }

        public async Task<CommandResult> DeleteAsync(int userId)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", userId }
            };
            return await PostAsync<CommandResult>(RelativePaths.DeleteUserPath, data);
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