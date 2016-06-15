namespace Nexus.RoleManagement.Services
{
    using System.Collections.Generic;

    public interface IRoleService
    {
        IEnumerable<SystemRoleModel> GetRolesByUserId(int userId);

        IEnumerable<SystemRoleModel> GetRoles();
    }
}
