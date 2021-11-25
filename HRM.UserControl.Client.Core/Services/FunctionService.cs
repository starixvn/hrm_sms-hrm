namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models;

    public interface IFunctionService
    {
        IList<Function> GetBySiteId(short siteId, bool? isActive = true);

        Task<IList<Function>> GetBySiteIdAsync(short siteId, bool? isActive = true);

        IList<FunctionPermission> GetPermissionByRoleId(short roleId);

        Task<IList<FunctionPermission>> GetPermissionByRoleIdAsync(short roleId);
    }

    public class FunctionService : IFunctionService
    {
        private readonly FunctionClient functionClient;

        public FunctionService()
        {
            functionClient = new FunctionClient();
        }

        public async Task<IList<Function>> GetBySiteIdAsync(short siteId, bool? isActive = true)
             => await functionClient.GetBySiteIdAsync(siteId, isActive);

        public IList<Function> GetBySiteId(short siteId, bool? isActive = true)
            => functionClient.GetBySiteId(siteId, isActive);

        public async Task<IList<FunctionPermission>> GetPermissionByRoleIdAsync(short roleId)
            => await functionClient.GetPermissionByRoleIdAsync(roleId);

        public IList<FunctionPermission> GetPermissionByRoleId(short roleId)
            => functionClient.GetPermissionByRoleId(roleId);
    }
}