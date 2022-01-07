namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;

    public interface IRoleService
    {
        IList<Role> GetBySiteId(short siteId, bool? isActive = true);

        Task<IList<Role>> GetBySiteIdAsync(short siteId, bool? isActive = true);

        IList<Role> GetByUserId(int userId);

        Task<IList<Role>> GetByUserIdAsync(int userId);

        CommandResult AssignPermission(short siteId, short roleId, IList<FunctionPermission> permissions);

        Task<CommandResult> AssignPermissionAsync(short siteId, short roleId, IList<FunctionPermission> permissions);
    }
}