namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Role;
    using Fanex.NexHR.Models;

    public class RoleService : IRoleService
    {
        private readonly IBaseService baseService;

        public RoleService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<SystemRoleModel> GetRolesByUserId(int userId)
        {
            var criteria = new GetRolesByUserIdCriteria { UserId = userId };
            return baseService.GetData<SystemRoleModel>(criteria);
        }

        public IEnumerable<SystemRoleModel> GetRoles()
        {
            var criteria = new GetRolesCriteria();
            return baseService.GetData<SystemRoleModel>(criteria);
        }
    }
}