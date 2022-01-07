namespace HRM.UserControl.WebAPI.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Models;

    public interface ISiteService
    {
        Task<SiteModel> GetByIdAsync(short siteId);

        Task<IEnumerable<SiteModel>> GetByUserIdAsync(int userId, bool? isActive = null);
    }
}