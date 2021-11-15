namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;

    public interface IRoleService
    {
        IList<Role> Get(bool? isActive = true);

        Task<IList<Role>> GetAsync(bool? isActive = true);

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

        public IList<Role> Get(bool? isActive = true)
            => roleClient.Get(isActive);

        public async Task<IList<Role>> GetAsync(bool? isActive = true)
             => await roleClient.GetAsync(isActive);

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