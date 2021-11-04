namespace HRM.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.User;

    public interface IUserClient
    {
        IList<MenuItem> GetMenuItem();

        Task<IList<MenuItem>> GetMenuItemAsync();

        IList<FunctionPermission> GetFunctionPermission();

        Task<IList<FunctionPermission>> GetFunctionPermissionAsync();
    }

    public class UserClient : BaseHttpClient, IUserClient
    {
        public IList<MenuItem> GetMenuItem()
            => Get<IList<MenuItem>>(RelativePaths.GetMenuItemPath);

        public async Task<IList<MenuItem>> GetMenuItemAsync()
            => await GetAsync<IList<MenuItem>>(RelativePaths.GetMenuItemPath);

        public IList<FunctionPermission> GetFunctionPermission()
            => Get<IList<FunctionPermission>>(RelativePaths.GetFunctionPermissionPath);

        public async Task<IList<FunctionPermission>> GetFunctionPermissionAsync()
            => await GetAsync<IList<FunctionPermission>>(RelativePaths.GetFunctionPermissionPath);

        public CommandResult UpdatePassword(UpdatePasswordParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", param.UserId },
                {"siteId", param.SiteId },
                {"password", param.Password },
                {"modifiedBy", param.ModifiedBy }
            };
            return Post<CommandResult>(RelativePaths.UpdatePasswordPath, data);
        }

        public async Task<CommandResult> UpdatePasswordAsync(UpdatePasswordParam param)
        {
            var data = new Dictionary<string, object>
            {
                {"userId", param.UserId },
                {"siteId", param.SiteId },
                {"password", param.Password },
                {"modifiedBy", param.ModifiedBy }
            };
            return await PostAsync<CommandResult>(RelativePaths.UpdatePasswordPath, data);
        }

        private static class RelativePaths
        {
            public const string GetMenuItemPath = "api/user/getMenuItem";
            public const string GetFunctionPermissionPath = "api/user/getFunctionPermission";
            public const string UpdatePasswordPath = "api/user/updatePassword";
        }
    }
}