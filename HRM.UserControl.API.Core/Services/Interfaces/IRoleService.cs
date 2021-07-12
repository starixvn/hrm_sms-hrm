namespace HRM.UserControl.API.Core.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Models;

    public interface IRoleService
    {
        Task<IEnumerable<CompanyRole>> GetUserRolesOnSite(int userId, int siteId);
    }
}