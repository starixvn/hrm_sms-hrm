namespace HRM.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;

    public interface IRoleClient
    {
        IList<Role> GetBySiteId(short siteId, bool? isActive = true);

        Task<IList<Role>> GetBySiteIdAsync(short siteId, bool? isActive = true);

        IList<Role> GetByUserId(int userId);

        Task<IList<Role>> GetByUserIdAsync(int userId);

        CommandResult AssignFunction(short roleId, IList<FunctionPermission> permissions);

        Task<CommandResult> AssignFunctionAsync(short roleId, IList<FunctionPermission> permissions);
    }

    public class RoleClient : BaseHttpClient, IRoleClient
    {
        public IList<Role> GetBySiteId(short siteId, bool? isActive = true)
            => Get<IList<Role>>(string.Format(RelativePaths.GetRoleBySiteIdPath, siteId, isActive));

        public async Task<IList<Role>> GetBySiteIdAsync(short siteId, bool? isActive = true)
            => await GetAsync<IList<Role>>(string.Format(RelativePaths.GetRoleBySiteIdPath, siteId, isActive));

        public IList<Role> GetByUserId(int userId)
            => Get<IList<Role>>(string.Format(RelativePaths.GetRoleByUserIdPath, userId));

        public async Task<IList<Role>> GetByUserIdAsync(int userId)
            => await GetAsync<IList<Role>>(string.Format(RelativePaths.GetRoleByUserIdPath, userId));

        public CommandResult AssignFunction(short roleId, IList<FunctionPermission> permissions)
        {
            var data = new Dictionary<string, object>
            {
                {"roleId", roleId },
                {"permissions", permissions }
            };
            return Post<CommandResult>(RelativePaths.AssignPermissionToRolePath, data);
        }

        public async Task<CommandResult> AssignFunctionAsync(short roleId, IList<FunctionPermission> permissions)
        {
            var data = new Dictionary<string, object>
            {
                {"roleId", roleId },
                {"permissions", permissions }
            };
            return await PostAsync<CommandResult>(RelativePaths.AssignPermissionToRolePath, data);
        }

        private static class RelativePaths
        {
            public const string GetRoleBySiteIdPath = "api/role/getBySiteId?siteId={0}&isActive={1}";
            public const string GetRoleByUserIdPath = "api/role/getByUserId?userId={0}";
            public const string AssignPermissionToRolePath = "api/role/assignPermission";
        }
    }
}