namespace HRM.UserControl.API.Core.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Models;

    public interface IUserService
    {
        Task<User> GetByUsernameAsync(string username);

        Task<User> GetByIdAsync(int userId);

        Task<IEnumerable<SiteModel>> GetUserSiteCompanyAsync(int userId);

        Task<IEnumerable<Company>> GetUserCompanyBySiteAsync(int userId, int siteId);

        Task<CommandResult> UserLoginWriteLogAsync(InsertUserLoginLogCommand command);
    }
}