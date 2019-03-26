namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IRoleService
    {
        IEnumerable<SystemRoleModel> GetRolesByUserId(int userId);

        IEnumerable<SystemRoleModel> GetRoles();
    }
}