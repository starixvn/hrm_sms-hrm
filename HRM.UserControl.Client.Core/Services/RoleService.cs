namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;

    public interface IRoleService
    {
        IList<Role> GetBySiteId(short siteId, bool? isActive = true);

        Task<IList<Role>> GetBySiteIdAsync(short siteId, bool? isActive = true);

        IList<Role> GetByUserId(int userId);

        Task<IList<Role>> GetByUserIdAsync(int userId);

        CommandResult AssignFunction(short roleId, IList<FunctionPermission> permissions);

        Task<CommandResult> AssignFunctionAsync(short roleId, IList<FunctionPermission> permissions);
    }

    public class RoleService : IRoleService
    {
        private readonly RoleClient roleClient;

        public RoleService()
        {
            roleClient = new RoleClient();
        }

        public IList<Role> GetBySiteId(short siteId, bool? isActive = true)
            => roleClient.GetBySiteId(siteId, isActive);

        public async Task<IList<Role>> GetBySiteIdAsync(short siteId, bool? isActive = true)
             => await roleClient.GetBySiteIdAsync(siteId, isActive);

        public IList<Role> GetByUserId(int userId)
            => roleClient.GetByUserId(userId);

        public async Task<IList<Role>> GetByUserIdAsync(int userId)
            => await roleClient.GetByUserIdAsync(userId);

        public CommandResult AssignFunction(short roleId, IList<FunctionPermission> permissions)
            => roleClient.AssignFunction(roleId, permissions);

        public async Task<CommandResult> AssignFunctionAsync(short roleId, IList<FunctionPermission> permissions)
            => await roleClient.AssignFunctionAsync(roleId, permissions);
    }
}