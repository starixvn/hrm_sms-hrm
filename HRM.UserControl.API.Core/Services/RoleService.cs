namespace HRM.UserControl.API.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria;
    using HRM.UserControl.API.Core.Services.Interfaces;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models;

    public class RoleService : IRoleService
    {
        private readonly IBaseService baseService;

        public RoleService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<CompanyRole>> GetUserRolesOnSite(int userId, int siteId)
        {
            var criteria = new GetRoleBySiteIdCriteria
            {
                UserId = userId,
                SiteId = siteId
            };

            var tuple = await baseService.FetchMultipleAsync<CompanyRole, Function>(criteria);
            var functions = MappingFunctions(tuple.Item2);
            MappingData(tuple.Item1, functions);

            return tuple.Item1;
        }

        private static IEnumerable<Function> MappingFunctions(IEnumerable<Function> functions)
        {
            var parents = functions.Where(f => f.ParentId == null);

            foreach (Function parent in parents)
            {
                parent.ChildFunctions = functions.Where(child => child.FunctionId == parent.FunctionId);
            }

            return parents;
        }

        private static void MappingData(IEnumerable<CompanyRole> roles, IEnumerable<Function> functions)
        {
            foreach (CompanyRole role in roles)
            {
                role.Functions = functions.Where(f => f.RoleId == role.RoleId);
            }
        }
    }
}