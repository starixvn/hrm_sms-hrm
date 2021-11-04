namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models;

    public interface IFunctionService
    {
        IList<Function> Get(bool? isActive = null);

        Task<IList<Function>> GetAsync(bool? isActive = null);

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

        public async Task<IList<Function>> GetAsync(bool? isActive = null)
             => await functionClient.GetAsync(isActive);

        public IList<Function> Get(bool? isActive = null)
            => functionClient.Get(isActive);

        public async Task<IList<FunctionPermission>> GetPermissionByRoleIdAsync(short roleId)
            => await functionClient.GetPermissionByRoleIdAsync(roleId);

        public IList<FunctionPermission> GetPermissionByRoleId(short roleId)
            => functionClient.GetPermissionByRoleId(roleId);
    }
}