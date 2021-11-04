namespace HRM.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;

    public interface IRoleClient
    {
        IList<Role> Get(bool? isActive = null);

        Task<IList<Role>> GetAsync(bool? isActive = null);

        CommandResult AssignFunction(short roleId, IList<FunctionPermission> permissions);

        Task<CommandResult> AssignFunctionAsync(short roleId, IList<FunctionPermission> permissions);
    }

    public class RoleClient : BaseHttpClient, IRoleClient
    {
        public IList<Role> Get(bool? isActive = null)
            => Get<IList<Role>>(string.Format(RelativePaths.GetRoleBySiteIdPath, isActive));

        public async Task<IList<Role>> GetAsync(bool? isActive = null)
            => await GetAsync<IList<Role>>(string.Format(RelativePaths.GetRoleBySiteIdPath, isActive));

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
            public const string GetRoleBySiteIdPath = "api/role?isActive={0}";
            public const string AssignPermissionToRolePath = "api/role/assignPermission";
        }
    }
}