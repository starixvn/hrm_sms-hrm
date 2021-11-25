namespace HRM.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;

    public interface IFunctionClient
    {
        IList<Function> GetBySiteId(short siteId, bool? isActive = true);

        Task<IList<Function>> GetBySiteIdAsync(short siteId, bool? isActive = null);

        IList<FunctionPermission> GetPermissionByRoleId(short roleId);

        Task<IList<FunctionPermission>> GetPermissionByRoleIdAsync(short roleId);
    }

    public class FunctionClient : BaseHttpClient, IFunctionClient
    {
        public IList<Function> GetBySiteId(short siteId, bool? isActive = true)
        {
            var data = new Dictionary<string, object>
            {
                { "siteId", siteId },
                { "isActive", isActive }
            };
            return Get<IList<Function>>(RelativePaths.GetFunctionBySiteIdPath, data);
        }

        public async Task<IList<Function>> GetBySiteIdAsync(short siteId, bool? isActive = null)
        {
            var data = new Dictionary<string, object>
            {
                { "siteId", siteId },
                { "isActive", isActive }
            };
            return await GetAsync<IList<Function>>(RelativePaths.GetFunctionBySiteIdPath, data);
        }

        public IList<FunctionPermission> GetPermissionByRoleId(short roleId)
            => Get<IList<FunctionPermission>>(string.Format(RelativePaths.GetFunctionPermissionByRoleIdPath, roleId));

        public async Task<IList<FunctionPermission>> GetPermissionByRoleIdAsync(short roleId)
            => await GetAsync<IList<FunctionPermission>>(string.Format(RelativePaths.GetFunctionPermissionByRoleIdPath, roleId));

        private static class RelativePaths
        {
            public const string GetFunctionBySiteIdPath = "api/function/getBySiteId";
            public const string GetFunctionPermissionByRoleIdPath = "api/function/getPermissionByRoleId?roleId={0}";
        }
    }
}