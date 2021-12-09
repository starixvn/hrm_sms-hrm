namespace HRM.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;

    public interface IFunctionClient
    {
        IList<Function> GetBySiteId(short siteId, bool? isActive = true);

        Task<IList<Function>> GetBySiteIdAsync(short siteId, bool? isActive = true);

        IList<FunctionPermission> GetPermissionBySiteAndRoleId(short siteId, short roleId);

        Task<IList<FunctionPermission>> GetPermissionBySiteAndRoleIdAsync(short siteId, short roleId);
    }

    public class FunctionClient : BaseHttpClient, IFunctionClient
    {
        public IList<Function> GetBySiteId(short siteId, bool? isActive = true)
         => Get<IList<Function>>(string.Format(RelativePaths.GetFunctionBySiteIdPath, siteId, isActive));

        public async Task<IList<Function>> GetBySiteIdAsync(short siteId, bool? isActive = true)
            => await GetAsync<IList<Function>>(string.Format(RelativePaths.GetFunctionBySiteIdPath, siteId, isActive));

        public IList<FunctionPermission> GetPermissionBySiteAndRoleId(short siteId, short roleId)
            => Get<IList<FunctionPermission>>(string.Format(RelativePaths.GetFunctionPermissionByRoleIdPath, siteId, roleId));

        public async Task<IList<FunctionPermission>> GetPermissionBySiteAndRoleIdAsync(short siteId, short roleId)
            => await GetAsync<IList<FunctionPermission>>(string.Format(RelativePaths.GetFunctionPermissionByRoleIdPath, siteId, roleId));

        private static class RelativePaths
        {
            public const string GetFunctionBySiteIdPath = "api/function/getBySiteId?siteId={0}&isActive={1}";
            public const string GetFunctionPermissionByRoleIdPath = "api/function/getPermissionBySiteAndRoleId?siteId={0}&roleId={1}";
        }
    }
}