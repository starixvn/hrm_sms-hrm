namespace Nexus.RoleManagement.Services
{
    using System.Collections.Generic;
    using System.Linq;

    public class RoleService : BaseService, IRoleService
    {
        public IEnumerable<SystemRoleModel> GetRolesByUserId(int userId)
        {
            var criteria = new GetRolesByUserIdCriteria { UserId = userId };
            IEnumerable<SystemRoleModel> roles = new List<SystemRoleModel>();
            if (criteria.IsValid())
            {
                roles = GetData<SystemRoleModel>(criteria);
            }

            return roles;
        }

        public IEnumerable<SystemRoleModel> GetRoles()
        {
            var criteria = new GetRolesCriteria();
            IEnumerable<SystemRoleModel> roles = GetData<SystemRoleModel>(criteria);

            return roles;
        }
    }
}
