namespace HRM.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;

    public interface IFunctionClient
    {
        IList<Function> Get(bool? isActive = true);

        Task<IList<Function>> GetAsync(bool? isActive = null);

        IList<FunctionPermission> GetPermissionByRoleId(short roleId);

        Task<IList<FunctionPermission>> GetPermissionByRoleIdAsync(short roleId);
    }

    public class FunctionClient : BaseHttpClient, IFunctionClient
    {
        public IList<Function> Get(bool? isActive = true)
            => Get<IList<Function>>(string.Format(RelativePaths.GetFunctionBySiteIdPath, isActive));

        public async Task<IList<Function>> GetAsync(bool? isActive = null)
            => await GetAsync<IList<Function>>(string.Format(RelativePaths.GetFunctionBySiteIdPath, isActive));

        public IList<FunctionPermission> GetPermissionByRoleId(short roleId)
            => Get<IList<FunctionPermission>>(string.Format(RelativePaths.GetFunctionPermissionByRoleIdPath, roleId));

        public async Task<IList<FunctionPermission>> GetPermissionByRoleIdAsync(short roleId)
            => await GetAsync<IList<FunctionPermission>>(string.Format(RelativePaths.GetFunctionPermissionByRoleIdPath, roleId));

        private static class RelativePaths
        {
            public const string GetFunctionBySiteIdPath = "api/function?isActive={0}";
            public const string GetFunctionPermissionByRoleIdPath = "api/function/getPermissionByRoleId?roleId={0}";
        }
    }
}