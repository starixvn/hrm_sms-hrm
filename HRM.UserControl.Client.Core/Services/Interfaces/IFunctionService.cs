namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;

    public interface IFunctionService
    {
        IList<Function> GetBySiteId(short siteId, bool? isActive = true);

        Task<IList<Function>> GetBySiteIdAsync(short siteId, bool? isActive = true);

        IList<Function> GetHierarchyBySiteId(short siteId, bool? isActive = true);

        Task<IList<Function>> GetHierarchyBySiteIdAsync(short siteId, bool? isActive = true);

        IList<FunctionPermission> GetPermissionBySiteAndRoleId(short siteId, short roleId);

        Task<IList<FunctionPermission>> GetPermissionBySiteAndRoleIdAsync(short siteId, short roleId);
    }
}